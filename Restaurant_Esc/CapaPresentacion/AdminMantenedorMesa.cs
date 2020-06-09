using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.OracleClient;
namespace CapaPresentacion
{
    public partial class AdminMantenedorMesa : Form
    {
        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorMesa()
        {
            InitializeComponent();
            ListarTabla();
            LlenarCbo();
            LlenarCboTipo();
        }

        private void LlenarCbo()
        {
            cboDisponibilidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDisponibilidad.Items.Add("Seleccione Disponibilidad");
            cboDisponibilidad.Items.Add("Libre");
            cboDisponibilidad.Items.Add("Ocupada");
            cboDisponibilidad.SelectedIndex = 0;
        }

        private void LlenarCboTipo()
        {
            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipo.Items.Add("Seleccione Tipo Mesa");
            cboTipo.Items.Add("Pequeña");
            cboTipo.Items.Add("Mediana");
            cboTipo.Items.Add("Familiar");
            cboTipo.SelectedIndex = 0;
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarMesa", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvMesa.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }

     
        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (cboDisponibilidad.SelectedItem.ToString() == "Seleccione Disponibilidad")
            {
                MessageBox.Show("Debe definir disponibilidad de la mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboDisponibilidad.Select();
                return;
            }
            else if (cboTipo.SelectedItem.ToString() == "Seleccione Tipo Mesa")
            {
                MessageBox.Show("Debe seleccionar tipo de mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboTipo.Select();
                return;
            }
            else
            {
                try
                {
                    ora.Open();
                    OracleCommand comando = new OracleCommand("insertarMesa", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("cant", OracleType.VarChar).Value = txtCant.Text;

                    if (cboDisponibilidad.SelectedItem.ToString() == "Ocupada")
                    {
                        comando.Parameters.Add("disp", OracleType.Char).Value = 'n';
                    }
                    else
                    {
                        comando.Parameters.Add("disp", OracleType.Char).Value = 's';
                    }

                    comando.Parameters.Add("tip", OracleType.VarChar).Value = cboTipo.SelectedItem.ToString();

                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Mesa ha sido ingresada correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al Ingresar Mesa.");
                }
                ora.Close();
                ListarTabla();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Mesa de la tabla para editar.");
                return;
            }
            else if (cboDisponibilidad.SelectedItem.ToString() == "Seleccione Disponibilidad")
            {
                MessageBox.Show("Debe definir disponibilidad de la mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboDisponibilidad.Select();
                return;
            }
            else if (cboTipo.SelectedItem.ToString() == "Seleccione Tipo Mesa")
            {
                MessageBox.Show("Debe seleccionar tipo de mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboTipo.Select();
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarMesa", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idm", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("cant", OracleType.VarChar).Value = txtCant.Text;

                if (cboDisponibilidad.SelectedItem.ToString() == "Ocupada")
                {
                    comando.Parameters.Add("disp", OracleType.Char).Value = 'n';
                }
                else
                {
                    comando.Parameters.Add("disp", OracleType.Char).Value = 's';
                }

                comando.Parameters.Add("tip", OracleType.VarChar).Value = cboTipo.SelectedItem.ToString();

                comando.ExecuteNonQuery();
                MessageBox.Show("Mesa actualizada correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Mesa de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar esta Mesa?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que mesa no es parte de una reserva
                    bool existeReserva = false;
                    OracleCommand oraCmdReserva = new OracleCommand("SELECT * FROM RESERVA WHERE ID_MESA='" + txtID.Text + "'", ora);
                    OracleDataReader oraReaderReserva = oraCmdReserva.ExecuteReader();

                    while (oraReaderReserva.Read())
                    {
                        existeReserva = true;
                    }

                    //Verificar que mesa no es parte de un pedido
                    bool existePedido = false;
                    OracleCommand oraCmdPedido = new OracleCommand("SELECT * FROM PEDIDO WHERE ID_MESA='" + txtID.Text + "'", ora);
                    OracleDataReader oraReaderPedido = oraCmdPedido.ExecuteReader();

                    while (oraReaderPedido.Read())
                    {
                        existePedido = true;
                    }

                    ora.Close();

                    if (existeReserva == true || existePedido == true)
                    {
                        MessageBox.Show("No se puede eliminar la Mesa seleccionada, porque es parte de una Reserva y/o Pedido. \n \n" +
                        "Primero modifique el elemento en que la mesa está presente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarMesa", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idm", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Mesa eliminado correctamente.");
                        ora.Close();
                    }

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
            txtCant.Select();
            txtID.Clear();
            txtCant.Clear();
            //cbo
            cboDisponibilidad.SelectedItem = "Seleccione Disponibilidad";
            cboTipo.SelectedItem = "Seleccione Tipo Mesa";
        }

        private void LimpiarTextBox()
        {
            txtCant.Select();
            txtID.Clear();
            txtCant.Clear();
            //cbo
            cboDisponibilidad.SelectedItem = "Seleccione Disponibilidad";
            cboTipo.SelectedItem = "Seleccione Tipo Mesa";
        }

        private void AdminMantenedorMesa_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorMesa frm = new AdminMantenedorMesa();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
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
            AdminCrud frm2 = new AdminCrud();
            frm2.Show();
        }

        private void dgvMesa_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvMesa.CurrentRow.Cells[0].Value.ToString();
            txtCant.Text = dgvMesa.CurrentRow.Cells[1].Value.ToString();

            //cbo
            char valor = Convert.ToChar(dgvMesa.CurrentRow.Cells[2].Value.ToString());

            if (valor == 's')
            {
                cboDisponibilidad.SelectedItem = "Libre";
            }
            else
            {
                cboDisponibilidad.SelectedItem = "Ocupada";
            }

            string valor1 = dgvMesa.CurrentRow.Cells[3].Value.ToString();
            cboTipo.SelectedItem = valor1;
        }

        //VALIDACIONES

        private void txtCant_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitudTexto = txtCant.Text.Trim().Length;

            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else if (e.KeyChar == Convert.ToChar(Keys.D0) && longitudTexto == 0 && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Tiene que ser Mayor que 0", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }

        }

        private void txtCant_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvMesa.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudCant = txtCant.Text.Trim().Length;

                if (longitudCant == 0)
                {
                    MessageBox.Show("Debe ingresar una cantidad de 2 sillas como mínimo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCant.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valorCant = Convert.ToInt32(txtCant.Text);

                if (valorCant < 2)
                {
                    MessageBox.Show("Debe ingresar una cantidad de 2 sillas como mínimo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCant.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
