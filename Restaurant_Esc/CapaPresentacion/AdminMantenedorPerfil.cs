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
    public partial class AdminMantenedorPerfil : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorPerfil()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
        }

        //Listar elementos de la tabla
        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarPerfil", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvPerfiles.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }

        //Agregar Elementos
        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("insertarPerfil", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("nom", OracleType.VarChar).Value = txtNombre.Text;
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescripcion.Text;
                comando.ExecuteNonQuery(); //ejecuta proc almacenado
                MessageBox.Show("Perfil ha sido insertado correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Falló al insertar Perfil.");
            }
            ora.Close();
            ListarTabla();
        }

        //Modificar elementos
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Perfi de la tabla para editarlo.");
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarPerfil", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("nom", OracleType.VarChar).Value = txtNombre.Text;
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescripcion.Text;
                comando.ExecuteNonQuery();
                MessageBox.Show("Perfil ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        //Eliminar elemento
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Perfil de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Perfil?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que perfil no es parte de un usuario
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM USUARIO WHERE ID_PERFIL='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Perfil seleccionado, porque es parte de uno o más Usuarios. \n \n" +
                        "Primero modifique el Usuario para poder eliminar el Perfil.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarPerfil", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Perfil ha sido eliminado correctamente.");
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

        //Limpiar textbox
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Select();
            txtID.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
        }

        private void LimpiarTextBox()
        {
            txtNombre.Select();
            txtID.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
        }

        private void MantenedorPerfil_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorPerfil frm = new AdminMantenedorPerfil();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        //Al pulsar sobre un elemento de la tabla, se muestran sus datos en los textbox
        private void dgvPerfiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvPerfiles.CurrentRow.Cells[0].Value.ToString();
            txtNombre.Text = dgvPerfiles.CurrentRow.Cells[1].Value.ToString();
            txtDescripcion.Text = dgvPerfiles.CurrentRow.Cells[2].Value.ToString();
        }


        //VALIDACIONES

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsSeparator(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
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
            AdminCrud frm2 = new AdminCrud();
            frm2.Show();
        }

        private void txtNombre_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvPerfiles.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudNom = txtNombre.Text.Trim().Length;

                if (longitudNom == 0)
                {
                    MessageBox.Show("Debe ingresar un nombre.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNombre.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtDescripcion_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvPerfiles.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudDescp = txtDescripcion.Text.Trim().Length;

                if (longitudDescp == 0)
                {
                    MessageBox.Show("Debe ingresar una descripción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtDescripcion.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
