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
    public partial class AdminMantenedorPedido : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorPedido()
        {
            InitializeComponent();
            ListarTabla();
        }

        //Listar elementos de la tabla
        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarPedido", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvPedidos.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            //LimpiarTextBox();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminCrud frm2 = new AdminCrud();
            frm2.Show();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Dispose();
            IniciarSesion frm2 = new IniciarSesion();
            frm2.Show();
        }

        private void AdminMantenedorPedido_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorPedido frm = new AdminMantenedorPedido();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ListarTabla();
        }
    }
}
