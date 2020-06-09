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
    public partial class AdminMantenedorVenta : Form
    {
        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorVenta()
        {
            InitializeComponent();
            ListarTabla();
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarVenta", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvVentas.DataSource = tabla;
            ora.Close();

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

        private void AdminMantenedorVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorVenta frm = new AdminMantenedorVenta();
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
