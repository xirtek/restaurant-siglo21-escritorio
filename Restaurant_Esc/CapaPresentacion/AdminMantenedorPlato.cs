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
    public partial class AdminMantenedorPlato : Form
    {
        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorPlato()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
            LlenarCbo(); //llena cbo con categorías
        }

        //Listar elementos de la tabla
        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarPlato", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvPlatos.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }

        private void LlenarCbo()
        {
            ora.Open();
            cboCategoria.Refresh();//refrescamos el ComboBox1
            cboCategoria.Text = "Seleccione una Categoría";
            OracleCommand oraCmd = new OracleCommand("SELECT * FROM CATEGORIA ", ora); 
            OracleDataReader oraReader = oraCmd.ExecuteReader();

            while (oraReader.Read())
            {
                cboCategoria.Refresh();
                cboCategoria.Items.Add(oraReader.GetValue(1).ToString());//se llena cbo   
            }
            ora.Close();
            cboCategoria.Items.Insert(0, "Seleccione una Categoría");
            cboCategoria.SelectedIndex = 0;
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (cboCategoria.Text == "Seleccione una Categoría")
            {
                MessageBox.Show("Debe seleccionar una Categoría.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboCategoria.Select();
                return;
            }
            else
            {
                try
                {
                    int ide2 = 0;
                    ora.Open();
                    OracleCommand comando = new OracleCommand("insertarPlato", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("descp", OracleType.VarChar).Value = txtDescripcion.Text;
                    comando.Parameters.Add("precio", OracleType.Number).Value = Convert.ToInt32(txtPrecio.Text);
                    //cbo
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM CATEGORIA WHERE DESCRIPCION_CATEGORIA='" + cboCategoria.SelectedItem.ToString() + "'", ora);
                    OracleDataReader oraReader = oraCmd2.ExecuteReader();

                    while (oraReader.Read())
                    {
                        ide2 = Convert.ToInt32(oraReader.GetValue(0).ToString());
                    }

                    comando.Parameters.Add("idc", OracleType.Number).Value = ide2;
                    comando.Parameters.Add("tiempo", OracleType.Number).Value = Convert.ToInt32(txtTiempo.Text);

                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Plato ha sido insertado correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al insertar Plato.");
                }
                ora.Close();
                ListarTabla();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Plato de la tabla para editarlo.");
                return;
            }
            else if (cboCategoria.Text == "Seleccione una Categoría")
            {
                MessageBox.Show("Debe seleccionar una Categoría.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboCategoria.Select();
                return;
            }
            else
            {
                int ide2 = 0;
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarPlato", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("descp", OracleType.VarChar).Value = txtDescripcion.Text;
                comando.Parameters.Add("precio", OracleType.Number).Value = Convert.ToInt32(txtPrecio.Text);
                //cbo
                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM CATEGORIA WHERE DESCRIPCION_CATEGORIA='"+ cboCategoria.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReader = oraCmd2.ExecuteReader();

                while (oraReader.Read())
                {
                    ide2 = Convert.ToInt32(oraReader.GetValue(0).ToString());
                }

                comando.Parameters.Add("idc", OracleType.Number).Value = ide2;
                comando.Parameters.Add("tiempo", OracleType.Number).Value = Convert.ToInt32(txtTiempo.Text);

                comando.ExecuteNonQuery();
                MessageBox.Show("Plato ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Plato de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Plato?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que plato no es parte de una orden
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM ORDEN WHERE ID_PLATO='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Plato seleccionado, porque es parte de una o más Órdenes realizadas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarPlato", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Plato ha sido eliminado correctamente.");
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
            txtDescripcion.Select();
            txtID.Clear();
            txtDescripcion.Clear();
            txtTiempo.Clear();
            txtPrecio.Clear();
            //cbo
            //cboCategoria.SelectedIndex = 0;
            cboCategoria.SelectedItem = "Seleccione una Categoría";
        }

        private void LimpiarTextBox()
        {
            txtDescripcion.Select();
            txtID.Clear();
            txtDescripcion.Clear();
            txtTiempo.Clear();
            txtPrecio.Clear();
            //cbo
            cboCategoria.SelectedItem = "Seleccione una Categoría";

        }

        private void MantenedorPlato_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorPlato frm = new AdminMantenedorPlato();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        private void dgvPlatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvPlatos.CurrentRow.Cells[0].Value.ToString();
            txtDescripcion.Text = dgvPlatos.CurrentRow.Cells[1].Value.ToString();
            txtPrecio.Text = dgvPlatos.CurrentRow.Cells[2].Value.ToString();
            //cbo
            string valor = dgvPlatos.CurrentRow.Cells[3].Value.ToString();
            cboCategoria.SelectedItem = valor;
            txtTiempo.Text = dgvPlatos.CurrentRow.Cells[4].Value.ToString();
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


        //VALIDACIONES

        private void txtTiempo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitudTexto = txtPrecio.Text.Trim().Length;

            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else if (e.KeyChar == Convert.ToChar(Keys.D0) && longitudTexto == 0 && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Tiene que ser Mayor que 0", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void txtDescripcion_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvPlatos.ContainsFocus)
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

        private void txtPrecio_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvPlatos.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudPrecio = txtPrecio.Text.Trim().Length;

                if (longitudPrecio == 0)
                {
                    MessageBox.Show("Debe ingresar precio mayor a $1000 pesos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPrecio.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valor = Convert.ToInt32(txtPrecio.Text);

                if (valor < 1000)
                {
                    MessageBox.Show("Debe ingresar precio mayor a $1000 pesos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPrecio.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtTiempo_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvPlatos.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudTiempo = txtTiempo.Text.Trim().Length;

                if (longitudTiempo == 0)
                {
                    MessageBox.Show("Debe ingresar un tiempo mínimo de 120 segundos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTiempo.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valorTiem = Convert.ToInt32(txtTiempo.Text);

                if (valorTiem < 120)
                {
                    MessageBox.Show("Debe ingresar un tiempo mínimo de 120 segundos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTiempo.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }                                 
        }
    }
}
