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

namespace CapaPresentacion
{
    public partial class BodegaGenerarSolicitud : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public BodegaGenerarSolicitud()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
        }

        //Listar elementos de la tabla
        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarProducto", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvProductosBodega.DataSource = tabla;
            ora.Close();


            LimpiarTextBox();
        }

        private void dgvProductosBodega_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvProductosBodega.CurrentRow.Cells[0].Value.ToString();
            string descripProducto = dgvProductosBodega.CurrentRow.Cells[1].Value.ToString();
            string detalleStock = dgvProductosBodega.CurrentRow.Cells[3].Value.ToString();
            txtCantidad.Enabled = true; //se activa para que ingrese la cantidad de stock a solicitar

            if (detalleStock == "unidades")
            {
                lblConsulta.Text = "¿Cuántas " + detalleStock + " de " + descripProducto + " necesita?";
            }
            else
            {
                lblConsulta.Text = "¿Cuántos " + detalleStock + " de " + descripProducto + " necesita?";
            }
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
            BodegaSolicitud frm2 = new BodegaSolicitud();
            frm2.Show();
        }

        private void BodegaSolicitud_FormClosing(object sender, FormClosingEventArgs e)
        {
            BodegaGenerarSolicitud frm = new BodegaGenerarSolicitud();
            BodegaSolicitud frm2 = new BodegaSolicitud();
            frm.Dispose();
            frm2.Show();
        }

        private void btnSolicitar_Click_1(object sender, EventArgs e)
        {
            int longitudStock = txtCantidad.Text.Trim().Length;

            if (lblConsulta.Text == "Pulse sobre la tabla del producto al que va a solicitar más stock.")
            {
                MessageBox.Show("Debe seleccionar algún producto de la tabla para solicitar más stock.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (longitudStock == 0)
            {
                MessageBox.Show("Debe ingresar una cantidad de stock a solicitar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                try
                {
                    string detalleProducto = "";
                    string medida = "";
                    string detalleSolicitud = "";

                    ora.Open();

                    //buscar detalle del producto seleccionado
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PRODUCTO WHERE ID_PRODUCTO='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader = oraCmd2.ExecuteReader();

                    while (oraReader.Read())
                    {
                        detalleProducto = oraReader.GetValue(1).ToString();
                        medida = oraReader.GetValue(3).ToString();
                    }

                    OracleCommand comando = new OracleCommand("insertarSolicitud", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    detalleSolicitud = "Se requieren " + txtCantidad.Text + " " + medida + " de " + detalleProducto + ".";

                    comando.Parameters.Add("descrip", OracleType.VarChar).Value = detalleSolicitud;
                    comando.Parameters.Add("estad", OracleType.VarChar).Value = "Pendiente";

                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Solicitud de Stock ha sido insertada correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al insertar Solicitud de Stock.");
                }
                ora.Close();
                ListarTabla();
            }
        }

        private void LimpiarTextBox()
        {
            txtID.Clear();
            lblConsulta.Text = "Pulse sobre la tabla del producto al que va a solicitar más stock.";
            txtCantidad.Clear();
            txtCantidad.Enabled = false;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtID.Clear();
            lblConsulta.Text = "Pulse sobre la tabla del producto al que va a solicitar más stock.";
            txtCantidad.Clear();
            txtCantidad.Enabled = false;
        }

        //VALIDACIONES
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitudTexto = txtCantidad.Text.Trim().Length;
            //validación de solo números
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else if (e.KeyChar == Convert.ToChar(Keys.D0) && longitudTexto == 0 && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Tiene que ser distinto que 0", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || dgvProductosBodega.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudStock = txtCantidad.Text.Trim().Length;

                if (longitudStock == 0)
                {
                    MessageBox.Show("Debe ingresar una cantidad mayor o igual a 1.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCantidad.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valorStock = Convert.ToInt32(txtCantidad.Text);

                if (valorStock < 1)
                {
                    MessageBox.Show("Debe ingresar una cantidad mayor o igual a 1.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCantidad.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
