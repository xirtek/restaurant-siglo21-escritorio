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
    public partial class AdminMantenedorCategoria : Form
    {
        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorCategoria()
        {
            InitializeComponent();
            ListarTabla();
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarCategoria", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvCategoria.DataSource = tabla;
            ora.Close();

            LimpiarTextBox();
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("insertarCategoria", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescrip.Text;


                comando.ExecuteNonQuery(); //ejecuta proc almacenado
                MessageBox.Show("Categoria agregada correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Fallo al hacer ingreso de Categoria.");
            }
            ora.Close();
            ListarTabla();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Categoría de la tabla para editarla.");
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarCategoria", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idct", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescrip.Text;


                comando.ExecuteNonQuery();
                MessageBox.Show("Categoría ha sido actualizada correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero una Categoría de la tabla para eliminarla.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar esta Categoria?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que categoría no es parte de un plato
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PLATO WHERE ID_CATEGORIA='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if(existe == true)
                    {
                        MessageBox.Show("No se puede eliminar la Categoría seleccionada, porque es parte de uno o más Platos. \n \n" +
                        "Primero modifique el Plato para poder eliminar la Categoría.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarCategoria", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idct", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Categoria ha sido eliminada correctamente.");
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
            txtDescrip.Select();
            txtID.Clear();
            txtDescrip.Clear();
        }

        private void LimpiarTextBox()
        {
            txtDescrip.Select();
            txtID.Clear();
            txtDescrip.Clear();
        }

        private void dgvCategoria_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvCategoria.CurrentRow.Cells[0].Value.ToString();
            txtDescrip.Text = dgvCategoria.CurrentRow.Cells[1].Value.ToString();
        }

        private void MantenedorCategoria_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorCategoria frm = new AdminMantenedorCategoria();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        private void txtDescrip_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtDescrip_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCategoria.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudDescp = txtDescrip.Text.Trim().Length;

                if (longitudDescp == 0)
                {
                    MessageBox.Show("Debe ingresar una descripción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtDescrip.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
