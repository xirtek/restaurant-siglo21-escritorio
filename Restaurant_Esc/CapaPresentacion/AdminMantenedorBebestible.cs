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
    public partial class AdminMantenedorBebestible : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorBebestible()
        {
            InitializeComponent();
            ListarTabla();
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarBebestible", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvBebestible.DataSource = tabla;
            ora.Close();

            LimpiarTextBox();
        }



        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("insertarBebestible", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescrip.Text;
                comando.Parameters.Add("stck", OracleType.Number).Value = Convert.ToInt32(txtStock.Text);
                comando.Parameters.Add("prec_unit", OracleType.Number).Value = Convert.ToInt32(txtPrecio.Text);


                comando.ExecuteNonQuery(); //ejecuta proc almacenado
                MessageBox.Show("Bebestible ha sido ingresado correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Falló al agregar Bebestible.");
            }
            ora.Close();
            ListarTabla();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Bebestible de la tabla para editarlo.");
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarBebestible", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idb", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("descrip", OracleType.VarChar).Value = txtDescrip.Text;
                comando.Parameters.Add("stck", OracleType.Number).Value = Convert.ToInt32(txtStock.Text);
                comando.Parameters.Add("prec_unit", OracleType.Number).Value = Convert.ToInt32(txtPrecio.Text);

                comando.ExecuteNonQuery();
                MessageBox.Show("Actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Bebestible de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿ Desea eliminar ?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que bebestible no es parte de una orden
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM ORDEN WHERE ID_BEBESTIBLE='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Bebestible seleccionado, porque es parte de una o más Órdenes realizadas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarBebestible", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idb", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Bebestible ha sido eliminado correctamente.");
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
            txtStock.Clear();
            txtPrecio.Clear();
        }

        private void LimpiarTextBox()
        {
            txtDescrip.Select();
            txtID.Clear();
            txtDescrip.Clear();
            txtStock.Clear();
            txtPrecio.Clear();
        }

        private void MantenedorBebestible_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorBebestible frm = new AdminMantenedorBebestible();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        private void dgvBebestible_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvBebestible.CurrentRow.Cells[0].Value.ToString();
            txtDescrip.Text = dgvBebestible.CurrentRow.Cells[1].Value.ToString();
            txtStock.Text = dgvBebestible.CurrentRow.Cells[2].Value.ToString();
            txtPrecio.Text = dgvBebestible.CurrentRow.Cells[3].Value.ToString();
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

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitudTexto = txtStock.Text.Trim().Length;
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

        private void txtDescrip_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvBebestible.ContainsFocus)
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

        private void txtStock_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvBebestible.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudStock = txtStock.Text.Trim().Length;

                if (longitudStock == 0)
                {
                    MessageBox.Show("Debe ingresar un stock mayor o igual a 1.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtStock.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valorStock = Convert.ToInt32(txtStock.Text);

                if (valorStock < 1)
                {
                    MessageBox.Show("Debe ingresar un stock mayor o igual a 1.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtStock.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtPrecio_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvBebestible.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudPrecio = txtPrecio.Text.Trim().Length;

                if (longitudPrecio == 0)
                {
                    MessageBox.Show("Debe ingresar precio mayoro igual a $500 pesos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPrecio.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valor = Convert.ToInt32(txtPrecio.Text);

                if (valor < 500)
                {
                    MessageBox.Show("Debe ingresar precio mayoro igual a $500 pesos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPrecio.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        
    }
}
