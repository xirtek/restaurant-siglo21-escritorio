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
using System.Collections;

namespace CapaPresentacion
{
    public partial class AdminEstadisticas : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminEstadisticas()
        {
            InitializeComponent();
            LlenarCboVentasxFecha();
         
        }

        private void AdminReporte_Load(object sender, EventArgs e)
        {
            //Gráficos
            GrafVentasxDias();
            GrafGananciasxDias();

            //Cuadro de texto
            TotalGananciasVentas();
            CantidadVentas();
            CantidadTrabajadores();
            CantidadReservas();
        }

        private void LlenarCboVentasxFecha()
        {
            cboVentasxFecha.Items.Add("Gráficos por Día");
            cboVentasxFecha.Items.Add("Gráficos por Mes");
            cboVentasxFecha.Items.Add("Gráficos por Año");
            cboVentasxFecha.SelectedIndex = 0;
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

        private void AdminReporte_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminEstadisticas frm = new AdminEstadisticas();
            AdminPrincipal frm2 = new AdminPrincipal();
            frm.Dispose();
            frm2.Show();
        }

        //Arreglo VentasxDía
        ArrayList Ventas1 = new ArrayList();
        ArrayList Fechas1 = new ArrayList();

        //Arreglo VentasxMes
        ArrayList Ventas11 = new ArrayList();
        ArrayList Fechas11 = new ArrayList();

        //Arreglo VentasxAño
        ArrayList Ventas111 = new ArrayList();
        ArrayList Fechas111 = new ArrayList();

        private void GrafVentasxDias()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("grafVentasxDia", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando.ExecuteReader();
            oraReader1 = comando.ExecuteReader();

            while (oraReader1.Read())
            {
                Fechas1.Add(oraReader1.GetDateTime(0));
                Ventas1.Add(oraReader1.GetInt32(1));
            }
            chartVentasxDia.Series[0].Points.DataBindXY(Fechas1, Ventas1);
            ora.Close();
        }

        private void GrafVentasxMes()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("grafVentasxMes", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando.ExecuteReader();
            oraReader1 = comando.ExecuteReader();

            while (oraReader1.Read())
            {
                Fechas11.Add(oraReader1.GetString(0));
                Ventas11.Add(oraReader1.GetInt32(1));
            }
            chartVentasxDia.Series[0].Points.DataBindXY(Fechas11, Ventas11);
            ora.Close();
        }

        private void GrafVentasxAnio()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("grafVentasxAnio", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando.ExecuteReader();
            oraReader1 = comando.ExecuteReader();

            while (oraReader1.Read())
            {
                Fechas111.Add(oraReader1.GetString(0));
                Ventas111.Add(oraReader1.GetInt32(1));
            }
            chartVentasxDia.Series[0].Points.DataBindXY(Fechas111, Ventas111);
            ora.Close();
        }

        //Arreglo GananciasxDía
        ArrayList Ganancia2 = new ArrayList();
        ArrayList Fechas2 = new ArrayList();

        //Arreglo GananciasxMes
        ArrayList Ganancia22 = new ArrayList();
        ArrayList Fechas22 = new ArrayList();

        //Arreglo GananciasxAño
        ArrayList Ganancia222 = new ArrayList();
        ArrayList Fechas222 = new ArrayList();

        private void GrafGananciasxDias()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("grafGananciasxDia", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando.ExecuteReader();
            oraReader1 = comando.ExecuteReader();

            while (oraReader1.Read())
            {
                Fechas2.Add(oraReader1.GetDateTime(0));
                Ganancia2.Add(oraReader1.GetInt32(1));
            }
            chartGananciasxDia.Series[0].Points.DataBindXY(Fechas2, Ganancia2);
            ora.Close();
        }

        private void GrafGananciasxMes()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("grafGananciasxMes", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando.ExecuteReader();
            oraReader1 = comando.ExecuteReader();

            while (oraReader1.Read())
            {
                Fechas22.Add(oraReader1.GetString(0));
                Ganancia22.Add(oraReader1.GetInt32(1));
            }
            chartGananciasxDia.Series[0].Points.DataBindXY(Fechas22, Ganancia22);
            ora.Close();
        }

        private void GrafGananciasxAnio()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("grafGananciasxAnio", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando.ExecuteReader();
            oraReader1 = comando.ExecuteReader();

            while (oraReader1.Read())
            {
                Fechas222.Add(oraReader1.GetString(0));
                Ganancia222.Add(oraReader1.GetInt32(1));
            }
            chartGananciasxDia.Series[0].Points.DataBindXY(Fechas222, Ganancia222);
            ora.Close();
        }

        private void TotalGananciasVentas()
        {
            string totalv = "";

            ora.Open();
            OracleCommand comando1 = new OracleCommand("totalGananciasVentas", ora);
            comando1.CommandType = System.Data.CommandType.StoredProcedure;
            comando1.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador1 = new OracleDataAdapter();
            OracleDataReader oraReader1 = comando1.ExecuteReader();
            oraReader1 = comando1.ExecuteReader();

            while (oraReader1.Read())
            {
                totalv = oraReader1.GetInt32(0).ToString();
            }

            lblCuadro1.Text = "$" + totalv + " pesos";
            ora.Close();

        }

        private void CantidadVentas()
        {
            string cantidadVentas = "";

            ora.Open();
            OracleCommand comando2 = new OracleCommand("cantidadVentas", ora);
            comando2.CommandType = System.Data.CommandType.StoredProcedure;
            comando2.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador2 = new OracleDataAdapter();
            OracleDataReader oraReader2 = comando2.ExecuteReader();
            oraReader2 = comando2.ExecuteReader();

            while (oraReader2.Read())
            {
                cantidadVentas = oraReader2.GetInt32(0).ToString();
            }

            lblCuadro2.Text = cantidadVentas;
            ora.Close();

        }

        private void CantidadTrabajadores()
        {
            string cantidadTrabajadores = "";

            ora.Open();
            OracleCommand comando3 = new OracleCommand("cantidadTrabajadores", ora);
            comando3.CommandType = System.Data.CommandType.StoredProcedure;
            comando3.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador3 = new OracleDataAdapter();
            OracleDataReader oraReader3 = comando3.ExecuteReader();
            oraReader3 = comando3.ExecuteReader();

            while (oraReader3.Read())
            {
                cantidadTrabajadores = oraReader3.GetInt32(0).ToString();
            }

            lblCuadro3.Text = cantidadTrabajadores;
            ora.Close();

        }

        private void CantidadReservas()
        {
            string cantidadReservas = "";

            ora.Open();
            OracleCommand comando4 = new OracleCommand("cantidadReservas", ora);
            comando4.CommandType = System.Data.CommandType.StoredProcedure;
            comando4.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador4 = new OracleDataAdapter();
            OracleDataReader oraReader4 = comando4.ExecuteReader();
            oraReader4 = comando4.ExecuteReader();

            while (oraReader4.Read())
            {
                cantidadReservas = oraReader4.GetInt32(0).ToString();
            }

            lblCuadro4.Text = cantidadReservas;
            ora.Close();

        }

        private void cboVentasxFecha_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Arreglos para Ventas
            Ventas1.Clear();
            Ventas11.Clear();
            Ventas111.Clear();
            Fechas1.Clear();
            Fechas11.Clear();
            Fechas111.Clear();

            //Arreglos para Ganancias
            Ganancia2.Clear();
            Fechas2.Clear();
            Ganancia22.Clear();
            Fechas22.Clear();
            Ganancia222.Clear();
            Fechas222.Clear();

            if (cboVentasxFecha.SelectedItem.ToString() == "Gráficos por Día")
            {
                chartVentasxDia.Series[0].Name = "Cantidad de Ventas x Día";
                chartVentasxDia.Series[0].Points.Clear();
                GrafVentasxDias();
                chartGananciasxDia.Series[0].Name = "Ganancia x Día";
                chartGananciasxDia.Series[0].Points.Clear();
                GrafGananciasxDias();
            }
            else if (cboVentasxFecha.SelectedItem.ToString() == "Gráficos por Mes")
            {
                chartVentasxDia.Series[0].Name = "Cantidad de Ventas x Mes";
                chartVentasxDia.Series[0].Points.Clear();
                GrafVentasxMes();
                chartGananciasxDia.Series[0].Name = "Ganancia x Mes";
                chartGananciasxDia.Series[0].Points.Clear();
                GrafGananciasxMes();
            }
            else if (cboVentasxFecha.SelectedItem.ToString() == "Gráficos por Año")
            {
                chartVentasxDia.Series[0].Name = "Cantidad de Ventas x Año";
                chartVentasxDia.Series[0].Points.Clear();
                GrafVentasxAnio();
                chartGananciasxDia.Series[0].Name = "Ganancia x Año";
                chartGananciasxDia.Series[0].Points.Clear();
                GrafGananciasxAnio();
            }


        }

    }
}
