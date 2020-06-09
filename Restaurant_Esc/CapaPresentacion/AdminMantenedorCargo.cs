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
    public partial class AdminMantenedorCargo : Form
    {
        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorCargo()
        {
            InitializeComponent();
            ListarTabla();
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarCargo", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvCargo.DataSource = tabla;
            ora.Close();

            LimpiarTextBox();
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("insertarCargo", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescrip.Text;


                comando.ExecuteNonQuery(); //ejecuta proc almacenado
                MessageBox.Show("Cargo agregado correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Fallo al hacer ingreso de Cargo.");
            }
            ora.Close();
            ListarTabla();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Cargo de la tabla para editarlo.");
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarCargo", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idct", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescrip.Text;


                comando.ExecuteNonQuery();
                MessageBox.Show("Cargo ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Cargo de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Cargo?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que categoría no es parte de un plato
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM TRABAJADOR WHERE ID_CARGO='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Cargo seleccionado, porque es parte de uno o más Trabajadores. \n \n" +
                        "Primero modifique el Trabajador para poder eliminar el Cargo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarCargo", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idct", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Cargo ha sido eliminado correctamente.");
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

        private void dgvCargo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvCargo.CurrentRow.Cells[0].Value.ToString();
            txtDescrip.Text = dgvCargo.CurrentRow.Cells[1].Value.ToString();
        }

        private void AdminMantenedorCargo_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorCargo frm = new AdminMantenedorCargo();
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

        private void txtDescrip_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCargo.ContainsFocus)
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

        private void txtDescrip_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && !(char.IsSeparator(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }
    }
}
