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
    public partial class IniciarSesion : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public IniciarSesion()
        {
            InitializeComponent();
            LlenarCbo();
            OcultarPass();
            cboTipoUsuario.SelectedIndex = 0;
        }

        private void LlenarCbo()
        {
            //cboTipoUsuario.Text = "Seleccione Tipo Usuario";
            cboTipoUsuario.Items.Add("Administrador");
            cboTipoUsuario.Items.Add("Bodeguero");
            cboTipoUsuario.Items.Add("Operador");
            cboTipoUsuario.Items.Insert(0, "Seleccione un Perfil de Usuario");
            cboTipoUsuario.SelectedIndex = 0;
        }

        private void OcultarPass()
        {
            txtPassword.Text = "";
            txtPassword.PasswordChar = '*';
            
        }

        private void btnLoguear_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrWhiteSpace(txtUsuario.Text) || String.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Debe ingresar un Usuario y Contraseña.");
                LimpiarTextos();
                return;
            }
            else if (cboTipoUsuario.Text == "Seleccione un Perfil de Usuario")
            {
                MessageBox.Show("Debe seleccionar una Perfil de Usuario.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                LimpiarTextos();
                return;
            }
            else
            {
                int tipo = 0;
                ora.Open();

                //obtener id tipo usuario
                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PERFIL WHERE NOMBRE_PERFIL='" + cboTipoUsuario.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReader = oraCmd2.ExecuteReader();

                while (oraReader.Read())
                {
                    tipo = Convert.ToInt32(oraReader.GetValue(0).ToString());
                }

                //comparar según datos
                OracleCommand cmd = new OracleCommand("SELECT * FROM USUARIO WHERE NOMBRE_USUARIO=:NOMBRE_USUARIO AND CLAVE=:CLAVE AND ID_PERFIL=:ID_PERFIL", ora);
                cmd.Parameters.Add(new OracleParameter(":NOMBRE_USUARIO", txtUsuario.Text.Trim()));
                cmd.Parameters.Add(new OracleParameter(":CLAVE", txtPassword.Text.Trim()));
                cmd.Parameters.Add(new OracleParameter(":ID_PERFIL", tipo));

                OracleDataReader lector = cmd.ExecuteReader();
                if (lector.Read())
                {
                    if (cboTipoUsuario.SelectedItem.ToString() == "Administrador")
                    {
                        IniciarSesion form = new IniciarSesion();
                        AdminPrincipal form2 = new AdminPrincipal();
                        form.Dispose();
                        form2.Show();
                    }
                    else if (cboTipoUsuario.SelectedItem.ToString() == "Bodeguero")
                    {
                        IniciarSesion form = new IniciarSesion();
                        BodegaPrincipal form2 = new BodegaPrincipal();
                        form.Dispose();
                        form2.Show();
                    }
                    else if (cboTipoUsuario.SelectedItem.ToString() == "Operador")
                    {
                        IniciarSesion form = new IniciarSesion();
                        OperadorPrincipal form2 = new OperadorPrincipal();
                        form.Dispose();
                        form2.Show();
                    }
                    this.Hide();
                }                
                else
                {
                    MessageBox.Show("Datos de usuario erróneos.");
                    LimpiarTextos();
                }
                ora.Close();

            }
        }

        private void IniciarSesion_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void LimpiarTextos()
        {
            txtUsuario.Clear();
            txtPassword.Clear();
            cboTipoUsuario.Text = "Seleccione un Perfil de Usuario";
        }
    }
}
