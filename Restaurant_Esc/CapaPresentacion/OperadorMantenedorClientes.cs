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
using System.Text.RegularExpressions;

namespace CapaPresentacion
{
    public partial class OperadorMantenedorClientes : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public OperadorMantenedorClientes()
        {
            InitializeComponent();
            ListarTabla();
            txtPNombre.Select(); //para evitar focus en rut al abrir form y que se active el msgbox de error
        }

        private void OperadorMantenedorClientes_FormClosing(object sender, FormClosingEventArgs e)
        {
            OperadorMantenedorClientes frm = new OperadorMantenedorClientes();
            OperadorPrincipal frm2 = new OperadorPrincipal();
            frm.Dispose();
            frm2.Show();
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarCliente", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvCliente.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }



        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("insertarCliente", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("rt", OracleType.VarChar).Value = txtRut.Text;
                comando.Parameters.Add("pnombre", OracleType.VarChar).Value = txtPNombre.Text;
                comando.Parameters.Add("snombre", OracleType.VarChar).Value = txtSNombre.Text;
                comando.Parameters.Add("appaterno", OracleType.VarChar).Value = txtPaterno.Text;
                comando.Parameters.Add("apmaterno", OracleType.VarChar).Value = txtMaterno.Text;
                comando.Parameters.Add("fono", OracleType.Number).Value = Convert.ToInt64(txtFono.Text);
                comando.Parameters.Add("mail", OracleType.VarChar).Value = txtEmail.Text;
                comando.ExecuteNonQuery(); //ejecuta proc almacenado
                MessageBox.Show("Cliente ha sido ingresado correctamente.");
            }
            catch (Exception)
            {
                MessageBox.Show("Falló al Ingresar Cliente.");
            }
            ora.Close();
            ListarTabla();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRut.Text))
            {
                MessageBox.Show("Debe elegir primero un Cliente de la tabla para editarlo.");
                return;
            }
            else
            {
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarCliente", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("rt", OracleType.VarChar).Value = txtRut.Text;
                comando.Parameters.Add("pnombre", OracleType.VarChar).Value = txtPNombre.Text;
                comando.Parameters.Add("snombre", OracleType.VarChar).Value = txtSNombre.Text;
                comando.Parameters.Add("appaterno", OracleType.VarChar).Value = txtPaterno.Text;
                comando.Parameters.Add("apmaterno", OracleType.VarChar).Value = txtMaterno.Text;
                comando.Parameters.Add("fono", OracleType.Number).Value = Convert.ToInt64(txtFono.Text);
                comando.Parameters.Add("mail", OracleType.VarChar).Value = txtEmail.Text;

                comando.ExecuteNonQuery();
                MessageBox.Show("Datos del Cliente han sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRut.Text))
            {
                MessageBox.Show("Debe elegir primero un Cliente de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar cliente?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que mesa no es parte de una reserva
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM RESERVA WHERE RUT='" + txtRut.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Cliente seleccionado, porque es parte de una o más Reservas. \n \n" +
                        "Primero modifique la Reserva para poder eliminar el Cliente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarCliente", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("rt", OracleType.VarChar).Value = txtRut.Text;
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Cliente ha sido eliminado correctamente.");
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
            txtPNombre.Select();
            txtRut.Clear();
            txtPNombre.Clear();
            txtSNombre.Clear();
            txtPaterno.Clear();
            txtMaterno.Clear();
            txtFono.Clear();
            txtEmail.Clear();

            txtRut.Enabled = true; //activar txtbox al limpiar datos de todos los campos txtbox
        }

        private void LimpiarTextBox()
        {
            txtRut.ForeColor = Color.Black;
            txtPNombre.Select();
            txtRut.Clear();
            txtPNombre.Clear();
            txtSNombre.Clear();
            txtPaterno.Clear();
            txtMaterno.Clear();
            txtFono.Clear();
            txtEmail.Clear();

            txtRut.Enabled = true; //activar txtbox al limpiar datos de todos los campos txtbox
        }

        private void dgvCliente_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtRut.Text = dgvCliente.CurrentRow.Cells[0].Value.ToString();
            txtPNombre.Text = dgvCliente.CurrentRow.Cells[1].Value.ToString();
            txtSNombre.Text = dgvCliente.CurrentRow.Cells[2].Value.ToString();
            txtPaterno.Text = dgvCliente.CurrentRow.Cells[3].Value.ToString();
            txtMaterno.Text = dgvCliente.CurrentRow.Cells[4].Value.ToString();
            txtFono.Text = dgvCliente.CurrentRow.Cells[5].Value.ToString();
            txtEmail.Text = dgvCliente.CurrentRow.Cells[6].Value.ToString();

            txtRut.ForeColor = Color.Black;
            txtRut.Enabled = false; //rut es pk, entonces se desactiva para editar los demás datos del cliente
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Dispose();
            IniciarSesion frm2 = new IniciarSesion();
            frm2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            OperadorPrincipal frm2 = new OperadorPrincipal();
            frm2.Show();
        }

        //VALIDACIONES

        private void txtPNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtSNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
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

        private void txtPaterno_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtMaterno_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private bool email_bien_escrito(String email)
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

        private void txtRut_Leave(object sender, EventArgs e)
        {
            if (button1.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCliente.ContainsFocus)
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

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (button1.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCliente.ContainsFocus)
            {
                return;
            }
            //validación de campo vacío
            else
            {
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    MessageBox.Show("Debe ingresar un correo", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEmail.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                    //validación de formato de correo
                }
                else if (email_bien_escrito(txtEmail.Text) == false)
                {
                    MessageBox.Show("Formato de correo no válido [ej: aaaa@gmail.com]", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //e.Handled = true;
                    txtEmail.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtPNombre_Leave(object sender, EventArgs e)
        {
            if (button1.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCliente.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudPNom = txtPNombre.Text.Trim().Length;

                if (longitudPNom == 0)
                {
                    MessageBox.Show("Debe ingresar el primer nombre.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPNombre.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtFono_Leave(object sender, EventArgs e)
        {
            if (button1.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCliente.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudFono = txtFono.Text.Trim().Length;

                if (longitudFono == 0)
                {
                    MessageBox.Show("Debe ingresar el número de teléfono.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtFono.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtPaterno_Leave(object sender, EventArgs e)
        {
            if (button1.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCliente.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudPater = txtPaterno.Text.Trim().Length;

                if (longitudPater == 0)
                {
                    MessageBox.Show("Debe ingresar el apellido paterno.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPaterno.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtMaterno_Leave(object sender, EventArgs e)
        {
            if (button1.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvCliente.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudMater = txtMaterno.Text.Trim().Length;

                if (longitudMater == 0)
                {
                    MessageBox.Show("Debe ingresar el apellido materno.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtMaterno.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

    }
}
