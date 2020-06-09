using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.Configuration;

namespace CapaPresentacion
{
    public partial class OperadorMantenedorReserva : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public OperadorMantenedorReserva()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
            LlenarCbo(); //llena cbo con proveedores
        }

        private void OperadorMantenedorReserva_Load(object sender, EventArgs e)
        {
            int HoraDesde = Convert.ToInt32(ConfigurationManager.AppSettings["HoraDesde"].ToString());
            int MinutoDesde = Convert.ToInt32(ConfigurationManager.AppSettings["MinutoDesde"].ToString());

            int HoraHasta = Convert.ToInt32(ConfigurationManager.AppSettings["HoraHasta"].ToString());
            int MinutoHasta = Convert.ToInt32(ConfigurationManager.AppSettings["MinutoHasta"].ToString());

            int IntervaloDeMinutos = Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloDeMinutos"].ToString());


            LlenarCombo(HoraDesde, MinutoDesde, HoraHasta, MinutoHasta, IntervaloDeMinutos);
        }

        private void OperadorMantenedorReserva_FormClosing(object sender, FormClosingEventArgs e)
        {
            OperadorMantenedorReserva frm = new OperadorMantenedorReserva();
            OperadorPrincipal frm2 = new OperadorPrincipal();
            frm.Dispose();
            frm2.Show();
        }


        private void AdminMantenedorReserva_Load(object sender, EventArgs e)
        {
            int HoraDesde = Convert.ToInt32(ConfigurationManager.AppSettings["HoraDesde"].ToString());
            int MinutoDesde = Convert.ToInt32(ConfigurationManager.AppSettings["MinutoDesde"].ToString());

            int HoraHasta = Convert.ToInt32(ConfigurationManager.AppSettings["HoraHasta"].ToString());
            int MinutoHasta = Convert.ToInt32(ConfigurationManager.AppSettings["MinutoHasta"].ToString());

            int IntervaloDeMinutos = Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloDeMinutos"].ToString());


            LlenarCombo(HoraDesde, MinutoDesde, HoraHasta, MinutoHasta, IntervaloDeMinutos);
        }

        private void LlenarCombo(int _HoraDesde, int _MinutoDesde, int _HoraHasta, int _MinutoHasta, int MinutesRange)
        {
            try
            {

                DateTime HorasYMinutosDesde = new DateTime();

                //setea primera hora
                TimeSpan ts = new TimeSpan(_HoraDesde, _MinutoDesde, 0);
                HorasYMinutosDesde = HorasYMinutosDesde.Date + ts;
                cboHora.Items.Add(HorasYMinutosDesde.ToString("HH:mm"));

                //Si hora 'desde' es mayor a la 'hasta' o es la misma hora pero mayores minutos
                //Agrega 24 hs para calcular que es en un día distinto
                if (_HoraDesde > _HoraHasta || (_HoraDesde == _HoraHasta && _MinutoDesde > _MinutoHasta))
                {
                    _HoraHasta += 24;

                }

                DateTime HorasYMinutosHasta = new DateTime();
                ts = new TimeSpan(_HoraHasta, _MinutoHasta, 0);
                HorasYMinutosHasta = HorasYMinutosHasta.Date + ts;

                DateTime HorasYMinutosIntervalo = new DateTime();

                HorasYMinutosIntervalo = HorasYMinutosDesde;

                //Acá se podría usar DateTime.Compare pero me parecio más simple así
                while (HorasYMinutosIntervalo.Hour <= HorasYMinutosHasta.Hour ||
                        HorasYMinutosIntervalo.Date <= HorasYMinutosHasta.Date)
                {
                    HorasYMinutosIntervalo = HorasYMinutosIntervalo.AddMinutes(MinutesRange);

                    //Acá chequea que la nueva hora agregar no haya igualado o superado a la máxima
                    if (HorasYMinutosIntervalo.Hour == HorasYMinutosHasta.Hour &&
                        HorasYMinutosIntervalo.Minute >= HorasYMinutosHasta.Minute
                        && HorasYMinutosIntervalo.Date == HorasYMinutosHasta.Date)
                    {

                        break;
                    }
                    cboHora.Items.Add(HorasYMinutosIntervalo.ToString("HH:mm"));
                }

                //Setea hora hasta   
                cboHora.Items.Add(HorasYMinutosHasta.ToString("HH:mm"));
                cboHora.Items.Insert(0, "Seleccione hora de uso"); //etiqueta de selección de hora
                cboHora.SelectedIndex = 0;
            }
            catch (Exception e)
            {

                MessageBox.Show("Se ha producido un error, contacte al administrador del sistema \n" + e.Message.ToString());
            }
        }


        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarReserva", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvReserva.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }



        private void LlenarCbo()
        {
            ora.Open();
            cboRut.Refresh();//refrescamos el ComboBox1
            OracleCommand oraCmd = new OracleCommand("SELECT * FROM CLIENTE", ora);
            OracleDataReader oraReader = oraCmd.ExecuteReader();

            cboMesa.Refresh();//refrescamos el ComboBox2
            OracleCommand oraCmdd = new OracleCommand("SELECT * FROM MESA", ora);
            OracleDataReader oraReaderr = oraCmdd.ExecuteReader();

            while (oraReader.Read())
            {
                cboRut.Refresh();
                cboRut.Items.Add(oraReader.GetValue(0).ToString());//se llena cbo1  
            }

            while (oraReaderr.Read())
            {
                cboMesa.Refresh();
                cboMesa.Items.Add(oraReaderr.GetValue(0).ToString());//se llena cbo2 
            }

            ora.Close();
            cboRut.Items.Insert(0, "Seleccione Rut del Cliente");
            cboRut.SelectedIndex = 0;
            cboMesa.Items.Insert(0, "Seleccione Número de Mesa");
            cboMesa.SelectedIndex = 0;
        }



        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (cboHora.Text == "Seleccione hora de uso")
            {
                MessageBox.Show("Debe seleccionar una hora.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboHora.Select();
                return;
            }
            else if (cboRut.Text == "Seleccione Rut del Cliente")
            {
                MessageBox.Show("Debe seleccionar un Cliente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboRut.Select();
                return;
            }
            else if (cboMesa.Text == "Seleccione Número de Mesa")
            {
                MessageBox.Show("Debe seleccionar una Mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboMesa.Select();
                return;
            }
            else
            {
                try
                {
                    string ide2 = "";
                    int ide3 = 0;
                    ora.Open();
                    OracleCommand comando = new OracleCommand("insertarReserva", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("fecha", OracleType.DateTime).Value = dpFecha.Value;

                    //cbo
                    comando.Parameters.Add("hor", OracleType.VarChar).Value = cboHora.SelectedItem.ToString();

                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM CLIENTE WHERE RUT='" + cboRut.SelectedItem.ToString() + "'", ora);
                    OracleDataReader oraReader = oraCmd2.ExecuteReader();

                    OracleCommand oraCmd3 = new OracleCommand("SELECT * FROM MESA WHERE ID_MESA='" + cboMesa.SelectedItem.ToString() + "'", ora);
                    OracleDataReader oraReaderr = oraCmd3.ExecuteReader();

                    while (oraReader.Read())
                    {
                        ide2 = oraReader.GetValue(0).ToString();
                    }

                    while (oraReaderr.Read())
                    {
                        ide3 = Convert.ToInt32(oraReaderr.GetValue(0).ToString());
                    }
                    comando.Parameters.Add("rt", OracleType.VarChar).Value = ide2;
                    comando.Parameters.Add("idmes", OracleType.Number).Value = ide3;

                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Reserva ha sido insertada correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al insertar Reserva.");
                }
                ora.Close();
                ListarTabla();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Reserva de la tabla para Editarla.");
                return;
            }
            else if (cboHora.Text == "Seleccione hora de uso")
            {
                MessageBox.Show("Debe seleccionar una hora.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboHora.Select();
                return;
            }
            else if (cboRut.Text == "Seleccione Rut del Cliente")
            {
                MessageBox.Show("Debe seleccionar un Cliente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboRut.Select();
                return;
            }
            else if (cboMesa.Text == "Seleccione Número de Mesa")
            {
                MessageBox.Show("Debe seleccionar una Mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboMesa.Select();
                return;
            }
            else
            {
                string ide2 = "";
                int ide3 = 0;
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarReserva", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idr", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("fecha", OracleType.DateTime).Value = dpFecha.Value;

                //cbo
                comando.Parameters.Add("hor", OracleType.VarChar).Value = cboHora.SelectedItem.ToString();

                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM CLIENTE WHERE RUT='" + cboRut.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReader = oraCmd2.ExecuteReader();

                OracleCommand oraCmd3 = new OracleCommand("SELECT * FROM MESA WHERE ID_MESA='" + cboMesa.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReaderr = oraCmd3.ExecuteReader();

                while (oraReader.Read())
                {
                    ide2 = oraReader.GetValue(0).ToString();
                }

                while (oraReaderr.Read())
                {
                    ide3 = Convert.ToInt32(oraReaderr.GetValue(0).ToString());
                }

                comando.Parameters.Add("rt", OracleType.VarChar).Value = ide2;
                comando.Parameters.Add("idmes", OracleType.Number).Value = ide3;


                comando.ExecuteNonQuery();
                MessageBox.Show("Reserva ha sido actualizada correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Reserva de la tabla para eliminarla.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar esta Reserva?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();
                    OracleCommand comando = new OracleCommand("eliminarReserva", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("idr", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Reserva ha sido eliminada correctamente.");
                    ora.Close();
                    ListarTabla();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtID.Clear();
            dpFecha.Value = DateTime.Today.AddDays(1);
            //cbo
            cboHora.SelectedItem = "Seleccione hora de uso";
            cboRut.SelectedItem = "Seleccione Rut del Cliente";
            cboMesa.SelectedItem = "Seleccione Número de Mesa";
        }

        private void LimpiarTextBox()
        {
            txtID.Clear();
            dpFecha.Value = DateTime.Today.AddDays(1);
            //cbo
            cboHora.SelectedItem = "Seleccione hora de uso";
            cboRut.SelectedItem = "Seleccione Rut del Cliente";
            cboMesa.SelectedItem = "Seleccione Número de Mesa";
        }

        private void dgvReserva_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvReserva.CurrentRow.Cells[0].Value.ToString();
            DateTime fechau = Convert.ToDateTime(dgvReserva.Rows[e.RowIndex].Cells[1].Value.ToString());
            dpFecha.Value = fechau;
            //cbo
            string valorhor = dgvReserva.CurrentRow.Cells[2].Value.ToString();
            cboHora.SelectedItem = valorhor;
            string valor = dgvReserva.CurrentRow.Cells[3].Value.ToString();
            cboRut.SelectedItem = valor;
            string valorr = dgvReserva.CurrentRow.Cells[4].Value.ToString();
            cboMesa.SelectedItem = valorr;
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Dispose();
            IniciarSesion frm2 = new IniciarSesion();
            frm2.Show();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Dispose();
            OperadorPrincipal frm2 = new OperadorPrincipal();
            frm2.Show();
        }

    }
}
