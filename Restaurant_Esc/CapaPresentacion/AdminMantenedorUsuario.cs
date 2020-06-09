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
    public partial class AdminMantenedorUsuario : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorUsuario()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
            LlenarCbo(); //llena cbo con Perfil
        }



        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarUsuario", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvUsuario.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }



        private void LlenarCbo()
        {
            ora.Open();
            cboPerfil.Refresh();//refrescamos el ComboBox1
            cboPerfil.Text = "Seleccione un Perfil";
            OracleCommand oraCmd = new OracleCommand("SELECT * FROM PERFIL ", ora);
            OracleDataReader oraReader = oraCmd.ExecuteReader();

            while (oraReader.Read())
            {
                cboPerfil.Refresh();
                cboPerfil.Items.Add(oraReader.GetValue(1).ToString());//se llena cbo   
            }
            ora.Close();
            cboPerfil.Items.Insert(0, "Seleccione un Perfil");
            cboPerfil.SelectedIndex = 0;
        }



        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (cboPerfil.Text == "Seleccione un Perfil")
            {
                MessageBox.Show("Debe seleccionar perfil para el Usuario.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboPerfil.Select();
                return;
            }
            else
            {
                try
                {
                    int ide2 = 0;
                    ora.Open();
                    OracleCommand comando = new OracleCommand("insertarUsuario", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("nom", OracleType.VarChar).Value = txtNombreU.Text;
                    comando.Parameters.Add("clv", OracleType.VarChar).Value = txtClave.Text;

                    //cbo
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PERFIL WHERE NOMBRE_PERFIL='" + cboPerfil.SelectedItem.ToString() + "'", ora);
                    OracleDataReader oraReader = oraCmd2.ExecuteReader();

                    while (oraReader.Read())
                    {
                        ide2 = Convert.ToInt32(oraReader.GetValue(0).ToString());
                    }

                    comando.Parameters.Add("idperf", OracleType.Number).Value = ide2;

                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Usuario ha sido insertado correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al insertar Usuario.");
                }
                ora.Close();
                ListarTabla();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Producto de la tabla para editarlo.");
                return;
            }
            else if (cboPerfil.Text == "Seleccione un Perfil")
            {
                MessageBox.Show("Debe seleccionar perfil para el Usuario.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboPerfil.Select();
                return;
            }
            else
            {
                int ide2 = 0;
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarUsuario", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idu", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("nom", OracleType.VarChar).Value = txtNombreU.Text;
                comando.Parameters.Add("clv", OracleType.VarChar).Value = txtClave.Text;

                //cbo
                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PERFIL WHERE NOMBRE_PERFIL='" + cboPerfil.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReader = oraCmd2.ExecuteReader();

                while (oraReader.Read())
                {
                    ide2 = Convert.ToInt32(oraReader.GetValue(0).ToString());
                }

                comando.Parameters.Add("idperf", OracleType.Number).Value = ide2;

                comando.ExecuteNonQuery();
                MessageBox.Show("Usuario ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Usario de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Usuario?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();
                    OracleCommand comando = new OracleCommand("eliminarUsuario", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("idu", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Usuario ha sido eliminado correctamente.");
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
            txtNombreU.Select();
            txtID.Clear();
            txtNombreU.Clear();
            txtClave.Clear();

            //cbo
            cboPerfil.SelectedItem = "Seleccione un Perfil";
        }

        private void LimpiarTextBox()
        {
            txtNombreU.Select();
            txtID.Clear();
            txtNombreU.Clear();
            txtClave.Clear();

            //cbo
            cboPerfil.SelectedItem = "Seleccione un Perfil";
        }

        private void AdminMantenedorUsario_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorUsuario frm = new AdminMantenedorUsuario();
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

        private void dgvUsuario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvUsuario.CurrentRow.Cells[0].Value.ToString();
            txtNombreU.Text = dgvUsuario.CurrentRow.Cells[1].Value.ToString();
            txtClave.Text = dgvUsuario.CurrentRow.Cells[2].Value.ToString();

            //cbo
            string valor = dgvUsuario.CurrentRow.Cells[3].Value.ToString();
            cboPerfil.SelectedItem = valor;
        }

        //VALIDACIONES

        private void txtNombreU_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtNombreU_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvUsuario.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudNom = txtNombreU.Text.Trim().Length;

                if (longitudNom == 0)
                {
                    MessageBox.Show("Debe ingresar un nombre.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNombreU.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtClave_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvUsuario.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudClave = txtClave.Text.Trim().Length;

                if (longitudClave == 0)
                {
                    MessageBox.Show("Debe ingresar una clave.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtClave.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
