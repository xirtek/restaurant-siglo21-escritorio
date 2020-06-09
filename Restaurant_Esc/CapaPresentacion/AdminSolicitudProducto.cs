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
    public partial class AdminSolicitudProducto : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminSolicitudProducto()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarSolicitud", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvSolicitudesStock.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            txtID.Clear();
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
            AdminPrincipal frm2 = new AdminPrincipal();
            frm2.Show();
        }

        private void AdminSolicitudProducto_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminSolicitudProducto frm = new AdminSolicitudProducto();
            AdminPrincipal frm2 = new AdminPrincipal();
            frm.Dispose();
            frm2.Show();
        }

        private void dgvSolicitudesStock_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvSolicitudesStock.CurrentRow.Cells[0].Value.ToString();
        }

        private void btnSolicitudPedida_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Solicitud de la tabla.");
                return;
            }
            else
            {
                string descripc = "";

                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarSolicitud", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("ids", OracleType.Number).Value = Convert.ToInt32(txtID.Text);

                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM SOLICITUD_STOCK WHERE ID_SOLICITUD='" + txtID.Text + "'", ora);
                OracleDataReader oraReader = oraCmd2.ExecuteReader();

                while (oraReader.Read())
                {
                    descripc = oraReader.GetValue(1).ToString();
                }
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = descripc;
                comando.Parameters.Add("estad", OracleType.VarChar).Value = "Pedida";

                comando.ExecuteNonQuery();
                MessageBox.Show("Solicitud cambió su estado a Pedida.");
                ora.Close();
                ListarTabla();
            }
        }
    }
}
