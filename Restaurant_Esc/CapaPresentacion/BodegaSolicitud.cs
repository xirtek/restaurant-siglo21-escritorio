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
    public partial class BodegaSolicitud : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public BodegaSolicitud()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarSolicitudTotal", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvSolicitudes.DataSource = tabla;
            ora.Close();
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
            BodegaPrincipal frm2 = new BodegaPrincipal();
            frm2.Show();
        }

        private void btnSolicitar_Click(object sender, EventArgs e)
        {
            this.Dispose();
            BodegaGenerarSolicitud frm = new BodegaGenerarSolicitud();
            frm.Show();
        }

        private void BodegaSolicitud_FormClosing(object sender, FormClosingEventArgs e)
        {
            BodegaSolicitud frm = new BodegaSolicitud();
            BodegaPrincipal frm2 = new BodegaPrincipal();
            frm.Dispose();
            frm2.Show();
        }
    }
}
