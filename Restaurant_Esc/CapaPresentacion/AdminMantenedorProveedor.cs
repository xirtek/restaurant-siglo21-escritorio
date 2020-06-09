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
using System.Text.RegularExpressions;

namespace CapaPresentacion
{
    public partial class AdminMantenedorProveedor : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorProveedor()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
        }

        //Al pulsar sobre un elemento de la tabla, se muestran sus datos en los textbox
        private void dgvProveedores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvProveedores.CurrentRow.Cells[0].Value.ToString();
            txtRut.Text = dgvProveedores.CurrentRow.Cells[1].Value.ToString();
            txtRazon.Text = dgvProveedores.CurrentRow.Cells[2].Value.ToString();
            txtDescripcion.Text = dgvProveedores.CurrentRow.Cells[3].Value.ToString();
            txtDireccion.Text = dgvProveedores.CurrentRow.Cells[4].Value.ToString();
            txtCiudad.Text = dgvProveedores.CurrentRow.Cells[5].Value.ToString();
            txtNumero.Text = dgvProveedores.CurrentRow.Cells[6].Value.ToString();
            txtFono.Text = dgvProveedores.CurrentRow.Cells[7].Value.ToString();
            txtCorreo.Text = dgvProveedores.CurrentRow.Cells[8].Value.ToString();

            txtRut.ForeColor = Color.Black;
        }

        //Listar elementos de la tabla
        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarProveedor", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvProveedores.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("insertarProveedor", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("rut", OracleType.VarChar).Value = txtRut.Text;
                comando.Parameters.Add("razon", OracleType.VarChar).Value = txtRazon.Text;
                comando.Parameters.Add("descp", OracleType.VarChar).Value = txtDescripcion.Text;
                comando.Parameters.Add("direcc", OracleType.VarChar).Value = txtDireccion.Text;
                comando.Parameters.Add("ciud", OracleType.VarChar).Value = txtCiudad.Text;
                comando.Parameters.Add("nro", OracleType.Number).Value = Convert.ToInt32(txtNumero.Text);
                comando.Parameters.Add("fono", OracleType.Number).Value = Convert.ToInt64(txtFono.Text);
                comando.Parameters.Add("correo", OracleType.VarChar).Value = txtCorreo.Text;
                comando.ExecuteNonQuery(); //ejecuta proc almacenado
                MessageBox.Show("Proveedor ha sido insertado correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Falló al insertar Proveedor.");
            }
            ora.Close();
            ListarTabla();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Proveedor de la tabla para editarlo.");
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarProveedor", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("rut", OracleType.VarChar).Value = txtRut.Text;
                comando.Parameters.Add("razon", OracleType.VarChar).Value = txtRazon.Text;
                comando.Parameters.Add("descp", OracleType.VarChar).Value = txtDescripcion.Text;
                comando.Parameters.Add("direcc", OracleType.VarChar).Value = txtDireccion.Text;
                comando.Parameters.Add("ciud", OracleType.VarChar).Value = txtCiudad.Text;
                comando.Parameters.Add("nro", OracleType.Number).Value = Convert.ToInt32(txtNumero.Text);
                comando.Parameters.Add("fono", OracleType.Number).Value = Convert.ToInt64(txtFono.Text);
                comando.Parameters.Add("correo", OracleType.VarChar).Value = txtCorreo.Text;
                comando.ExecuteNonQuery();
                MessageBox.Show("Proveedor ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Proveedor de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Proveedor?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que perfil no es parte de un usuario
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PRODUCTO WHERE ID_PROVEEDOR='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Proveedor seleccionado, porque es parte de uno o más Productos. \n \n" +
                        "Primero modifique el Producto para poder eliminar el Proveedor.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarProveedor", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Proveedor ha sido eliminado correctamente.");
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
            txtRut.ForeColor = Color.Black;
            txtRut.Select();
            txtID.Clear();
            txtRut.Clear();
            txtRazon.Clear();
            txtDescripcion.Clear();
            txtDireccion.Clear();
            txtCiudad.Clear();
            txtNumero.Clear();
            txtFono.Clear();
            txtCorreo.Clear();
        }

        private void LimpiarTextBox()
        {
            txtRut.ForeColor = Color.Black;
            txtRut.Select();
            txtID.Clear();
            txtRut.Clear();
            txtRazon.Clear();
            txtDescripcion.Clear();
            txtDireccion.Clear();
            txtCiudad.Clear();
            txtNumero.Clear();
            txtFono.Clear();
            txtCorreo.Clear();
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

        private void MantenedorProveedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorProveedor frm = new AdminMantenedorProveedor();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        //VALIDACIONES

        private void txtCiudad_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtNumero_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo números
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        //método para verificar formato de correo
        private Boolean email_bien_escrito(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void txtFono_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo números
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private string formatoRut(string rut)
        {
            int cont = 0;
            string format;
            rut = rut.Replace(".", "");
            rut = rut.Replace("-", "");
            format = "-" + rut.Substring(rut.Length - 1);
            for (int i = rut.Length - 2; i >= 0; i--)
            {
                format = rut.Substring(i, 1) + format;
                cont++;
                if (cont == 3 && i != 0)
                {
                    format = "." + format;
                    cont = 0;
                }
            }
            return format;
        }

        private bool validarRut(string rut)
        {
            bool validacion = false;
            rut = rut.ToUpper();
            rut = rut.Replace(".", "");
            rut = rut.Replace("-", "");
            int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));
            char dv = char.Parse(rut.Substring(rut.Length - 1, 1));
            int m = 0, s = 1;
            for (; rutAux != 0; rutAux /= 10)
            {
                s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
            }
            if (dv == (char)(s != 0 ? s + 47 : 75))
            {
                validacion = true;
            }
            return validacion;
        }

        private void txtRut_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                int n = txtRut.TextLength;
                if (n < 7)
                {
                    txtRut.ForeColor = Color.Red;
                    MessageBox.Show("El Rut no es válido. Debe ingresar rut con dígito verificador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRut.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
                else
                {
                    bool respuesta = false;
                    string rut = txtRut.Text;
                    txtRut.Text = formatoRut(rut);
                    rut = txtRut.Text;
                    respuesta = validarRut(rut);

                    if (respuesta == false)
                    {
                        txtRut.Focus();
                        txtRut.ForeColor = Color.Red;
                        MessageBox.Show("El Rut no es válido. Debe ingresar rut con dígito verificador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }
                    else
                    {
                        txtRut.ForeColor = Color.Black;
                        //TxtRut.BackColor = Color.GreenYellow;
                        // MessageBox.Show("Rut OK");
                    }
                }
            }
        }

        private void txtCorreo_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                //validación de campo vacío
                if (string.IsNullOrEmpty(txtCorreo.Text))
                {
                    MessageBox.Show("Debe ingresar un correo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCorreo.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                    //validación de formato de correo
                }
                else if (email_bien_escrito(txtCorreo.Text) == false)
                {
                    MessageBox.Show("Formato de correo no válido [ej: aaaa@gmail.com]", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //e.Handled = true;
                    txtCorreo.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtRazon_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudRaz = txtRazon.Text.Trim().Length;

                if (longitudRaz == 0)
                {
                    MessageBox.Show("Debe ingresar la razón social.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRazon.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtDescripcion_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
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

        private void txtDireccion_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudDire = txtDireccion.Text.Trim().Length;

                if (longitudDire == 0)
                {
                    MessageBox.Show("Debe ingresar una dirección.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtDireccion.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtCiudad_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudCiud = txtCiudad.Text.Trim().Length;

                if (longitudCiud == 0)
                {
                    MessageBox.Show("Debe ingresar la ciudad.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCiudad.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtNumero_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudNu = txtNumero.Text.Trim().Length;

                if (longitudNu == 0)
                {
                    MessageBox.Show("Debe ingresar el número asociado a la dirección.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNumero.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtFono_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProveedores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudFono = txtFono.Text.Trim().Length;

                if (longitudFono == 0)
                {
                    MessageBox.Show("Debe ingresar un número de teléfono.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtFono.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
