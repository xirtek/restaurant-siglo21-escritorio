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
    public partial class AdminMantenedorTrabajador : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminMantenedorTrabajador()
        {
            InitializeComponent();
            ListarTabla();
            LlenarCboSexo(); //llena cbo con opciones: Masculino y Femenino
            LlenarCboCargo();
            cboSexo.SelectedItem = "Seleccione el Sexo";
        }

        private void dgvTrabajadores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvTrabajadores.CurrentRow.Cells[0].Value.ToString();
            txtRutTrabajador.Text = dgvTrabajadores.CurrentRow.Cells[1].Value.ToString();
            txtPrimerN.Text = dgvTrabajadores.CurrentRow.Cells[2].Value.ToString();
            txtSegundoN.Text = dgvTrabajadores.CurrentRow.Cells[3].Value.ToString();
            txtApellidoP.Text = dgvTrabajadores.CurrentRow.Cells[4].Value.ToString();
            txtApellidoM.Text = dgvTrabajadores.CurrentRow.Cells[5].Value.ToString();

            //fecha
            DateTime fechan = Convert.ToDateTime(dgvTrabajadores.Rows[e.RowIndex].Cells[6].Value.ToString());
            dpNac.Value = fechan;

            //cbo
            char valor = Convert.ToChar(dgvTrabajadores.CurrentRow.Cells[7].Value.ToString());

            if(valor == 'M')
            {
                cboSexo.SelectedItem = "Masculino";
            }
            else
            {
                cboSexo.SelectedItem = "Femenino";
            }
            
            txtDireccion.Text = dgvTrabajadores.CurrentRow.Cells[8].Value.ToString();
            txtNroDir.Text = dgvTrabajadores.CurrentRow.Cells[9].Value.ToString();
            txtCiudad.Text = dgvTrabajadores.CurrentRow.Cells[10].Value.ToString();

            string valorCargo = dgvTrabajadores.CurrentRow.Cells[11].Value.ToString();
            cboCargo.SelectedItem = valorCargo;

            txtFono.Text = dgvTrabajadores.CurrentRow.Cells[12].Value.ToString();
            txtMail.Text = dgvTrabajadores.CurrentRow.Cells[13].Value.ToString();
            txtSueldo.Text = dgvTrabajadores.CurrentRow.Cells[14].Value.ToString();

            txtRutTrabajador.ForeColor = Color.Black;
        }

        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarTrabajador", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvTrabajadores.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }

        private void LlenarCboSexo()
        {
            //cboSexo.Text = "Seleccione Sexo";
            cboSexo.Items.Add("Seleccione el Sexo");
            cboSexo.Items.Add("Masculino");
            cboSexo.Items.Add("Femenino");
            cboSexo.SelectedIndex = 0;
        }

        private void LlenarCboCargo()
        {
            ora.Open();
            cboCargo.Refresh();//refrescamos el ComboBox1
            cboCargo.Text = "Seleccione un Cargo";
            OracleCommand oraCmd = new OracleCommand("SELECT * FROM CARGO ", ora);
            OracleDataReader oraReader = oraCmd.ExecuteReader();

            while (oraReader.Read())
            {
                cboCargo.Refresh();
                cboCargo.Items.Add(oraReader.GetValue(1).ToString());//se llena cbo   
            }
            ora.Close();
            cboCargo.Items.Insert(0, "Seleccione un Cargo");
            cboCargo.SelectedIndex = 0;
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if(cboSexo.SelectedItem.ToString() == "Seleccione el Sexo")
            {
                MessageBox.Show("Debe seleccionar sexo del Trabajador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboSexo.Select();
                return;
            }
            else if (cboCargo.Text == "Seleccione un Cargo")
            {
                MessageBox.Show("Debe seleccionar cargo del Trabajador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboCargo.Select();
                return;
            }
            else
            {
                try
                {
                    int ide2 = 0;
                    ora.Open();
                    OracleCommand comando = new OracleCommand("insertarTrabajador", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("rut", OracleType.VarChar).Value = txtRutTrabajador.Text;
                    comando.Parameters.Add("pnombre", OracleType.VarChar).Value = txtPrimerN.Text;
                    comando.Parameters.Add("snombre", OracleType.VarChar).Value = txtSegundoN.Text;
                    comando.Parameters.Add("appaterno", OracleType.VarChar).Value = txtApellidoP.Text;
                    comando.Parameters.Add("apmaterno", OracleType.VarChar).Value = txtApellidoM.Text;

                    comando.Parameters.Add("nac", OracleType.DateTime).Value = dpNac.Value;

                    if (cboSexo.SelectedItem.ToString() == "Masculino")
                    {
                        comando.Parameters.Add("se", OracleType.Char).Value = 'M';
                    }
                    else 
                    {
                        comando.Parameters.Add("se", OracleType.Char).Value = 'F';
                    }

                    comando.Parameters.Add("direcc", OracleType.VarChar).Value = txtDireccion.Text;
                    comando.Parameters.Add("nro", OracleType.Number).Value = Convert.ToInt32(txtNroDir.Text);
                    comando.Parameters.Add("ciu", OracleType.VarChar).Value = txtCiudad.Text;

                    //cbo cargo
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM CARGO WHERE DESCRIPCION_CARGO='" + cboCargo.SelectedItem.ToString() + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        ide2 = Convert.ToInt32(oraReader2.GetValue(0).ToString());
                    }

                    comando.Parameters.Add("carg", OracleType.Number).Value = ide2;

                    comando.Parameters.Add("fono", OracleType.Number).Value = Convert.ToInt64(txtFono.Text);
                    comando.Parameters.Add("mail", OracleType.VarChar).Value = txtMail.Text;
                    comando.Parameters.Add("suel", OracleType.Number).Value = Convert.ToInt32(txtSueldo.Text);
                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Trabajador ha sido insertado correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al insertar Trabajador.");
                }
                ora.Close();
                ListarTabla();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Trabajador de la tabla para editarlo.");
                return;
            }
            else if (cboSexo.SelectedItem.ToString() == "Seleccione el Sexo")
            {
                MessageBox.Show("Debe seleccionar sexo del Trabajador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboSexo.Select();
                return;
            }
            else if (cboCargo.Text == "Seleccione un Cargo")
            {
                MessageBox.Show("Debe seleccionar cargo del Trabajador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboCargo.Select();
                return;
            }
            else
            {
                int ide2 = 0;
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarTrabajador", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idt", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("rut", OracleType.VarChar).Value = txtRutTrabajador.Text;
                comando.Parameters.Add("pnombre", OracleType.VarChar).Value = txtPrimerN.Text;
                comando.Parameters.Add("snombre", OracleType.VarChar).Value = txtSegundoN.Text;
                comando.Parameters.Add("appaterno", OracleType.VarChar).Value = txtApellidoP.Text;
                comando.Parameters.Add("apmaterno", OracleType.VarChar).Value = txtApellidoM.Text;

                comando.Parameters.Add("nac", OracleType.DateTime).Value = dpNac.Value;

                if (cboSexo.SelectedItem.ToString() == "Masculino")
                {
                    comando.Parameters.Add("se", OracleType.Char).Value = 'M';
                }
                else
                {
                    comando.Parameters.Add("se", OracleType.Char).Value = 'F';
                }

                comando.Parameters.Add("direcc", OracleType.VarChar).Value = txtDireccion.Text;
                comando.Parameters.Add("nro", OracleType.Number).Value = Convert.ToInt32(txtNroDir.Text);
                comando.Parameters.Add("ciu", OracleType.VarChar).Value = txtCiudad.Text;

                //cbo cargo
                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM CARGO WHERE DESCRIPCION_CARGO='" + cboCargo.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                while (oraReader2.Read())
                {
                    ide2 = Convert.ToInt32(oraReader2.GetValue(0).ToString());
                }

                comando.Parameters.Add("carg", OracleType.Number).Value = ide2;

                comando.Parameters.Add("fono", OracleType.Number).Value = Convert.ToInt64(txtFono.Text);
                comando.Parameters.Add("mail", OracleType.VarChar).Value = txtMail.Text;
                comando.Parameters.Add("suel", OracleType.Number).Value = Convert.ToInt32(txtSueldo.Text);
                comando.ExecuteNonQuery();
                MessageBox.Show("Trabajador ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Trabajador de la tabla para eliminarlo.");
                return;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Trabajador?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();

                    //Verificar que trabajador no es parte de una venta registrada
                    bool existe = false;
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM VENTA WHERE ID_TRABAJADOR='" + txtID.Text + "'", ora);
                    OracleDataReader oraReader2 = oraCmd2.ExecuteReader();

                    while (oraReader2.Read())
                    {
                        existe = true;
                    }

                    ora.Close();

                    if (existe == true)
                    {
                        MessageBox.Show("No se puede eliminar el Trabajador seleccionado, porque es parte de una o más Ventas registradas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        ora.Open();
                        OracleCommand comando = new OracleCommand("eliminarTrabajador", ora);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Add("idt", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Trabajador ha sido eliminado correctamente.");
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
            txtRutTrabajador.ForeColor = Color.Black;
            txtRutTrabajador.Select();
            txtID.Clear();
            txtRutTrabajador.Clear();
            txtPrimerN.Clear();
            txtSegundoN.Clear();
            txtApellidoP.Clear();
            txtApellidoM.Clear();
            dpNac.Value = DateTime.Today;
            cboSexo.SelectedItem = "Seleccione el Sexo";
            cboCargo.SelectedItem = "Seleccione un Cargo";
            txtDireccion.Clear();
            txtNroDir.Clear();
            txtCiudad.Clear();
            txtFono.Clear();
            txtMail.Clear();
            txtSueldo.Clear();
        }

        private void LimpiarTextBox()
        {
            txtRutTrabajador.ForeColor = Color.Black;
            txtRutTrabajador.Select();
            txtID.Clear();
            txtRutTrabajador.Clear();
            txtPrimerN.Clear();
            txtSegundoN.Clear();
            txtApellidoP.Clear();
            txtApellidoM.Clear();
            dpNac.Value = DateTime.Today;
            cboSexo.SelectedItem = "Seleccione el Sexo";
            cboCargo.SelectedItem = "Seleccione un Cargo";
            txtDireccion.Clear();
            txtNroDir.Clear();
            txtCiudad.Clear();
            txtFono.Clear();
            txtMail.Clear();
            txtSueldo.Clear();
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

        private void MantenedorTrabajador_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminMantenedorTrabajador frm = new AdminMantenedorTrabajador();
            AdminCrud frm2 = new AdminCrud();
            frm.Dispose();
            frm2.Show();
        }

        //VALIDACIONES

        private void txtPrimerN_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtSegundoN_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtApellidoP_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtApellidoM_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo letras
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

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

        private void txtNroDir_KeyPress(object sender, KeyPressEventArgs e)
        {
            //validación de solo números
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void txtSueldo_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitudTexto = txtSueldo.Text.Trim().Length;
            //validación de solo números
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else if (e.KeyChar == Convert.ToChar(Keys.D0) && longitudTexto == 0 && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Debe ingresar un sueldo de $301.000 pesos como mínimo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void txtMail_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                //validación de campo vacío
                if (string.IsNullOrEmpty(txtMail.Text))
                {
                    MessageBox.Show("Debe ingresar un correo", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtMail.Select();
                    return;
                    //validación de formato de correo
                }
                else if (email_bien_escrito(txtMail.Text) == false)
                {
                    MessageBox.Show("Formato de correo no válido [ej: aaaa@gmail.com]", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //e.Handled = true;
                    txtMail.Select();
                    return;
                }
            }
        }

        private void txtRutTrabajador_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int n = txtRutTrabajador.TextLength;
                if (n < 7)
                {
                    txtRutTrabajador.ForeColor = Color.Red;
                    MessageBox.Show("El Rut no es válido. Debe ingresar rut con dígito verificador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRutTrabajador.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
                else
                {
                    bool respuesta = false;
                    string rut = txtRutTrabajador.Text;
                    txtRutTrabajador.Text = formatoRut(rut);
                    rut = txtRutTrabajador.Text;
                    respuesta = validarRut(rut);

                    if (respuesta == false)
                    {
                        txtRutTrabajador.Focus();
                        txtRutTrabajador.ForeColor = Color.Red;
                        MessageBox.Show("El Rut no es válido. Debe ingresar rut con dígito verificador.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        txtRutTrabajador.ForeColor = Color.Black;
                        //TxtRut.BackColor = Color.GreenYellow;
                        // MessageBox.Show("Rut OK");
                    }
                }
            }
        }

        private void txtPrimerN_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudPrimerN = txtPrimerN.Text.Trim().Length;

                if (longitudPrimerN == 0)
                {
                    MessageBox.Show("Debe ingresar el primer nombre.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtPrimerN.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtApellidoP_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudApellidoP = txtApellidoP.Text.Trim().Length;

                if (longitudApellidoP == 0)
                {
                    MessageBox.Show("Debe ingresar el apellido paterno.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtApellidoP.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtApellidoM_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudApellidoM = txtApellidoM.Text.Trim().Length;

                if (longitudApellidoM == 0)
                {
                    MessageBox.Show("Debe ingresar el apellido materno.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtApellidoM.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtDireccion_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudDirecc = txtDireccion.Text.Trim().Length;

                if (longitudDirecc == 0)
                {
                    MessageBox.Show("Debe ingresar una dirección.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtDireccion.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtNroDir_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudNroD = txtNroDir.Text.Trim().Length;

                if (longitudNroD == 0)
                {
                    MessageBox.Show("Debe ingresar el número asociado a la dirección.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNroDir.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtCiudad_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudCiudad = txtCiudad.Text.Trim().Length;

                if (longitudCiudad == 0)
                {
                    MessageBox.Show("Debe ingresar la ciudad.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCiudad.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }

        private void txtFono_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
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

        private void txtSueldo_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvTrabajadores.ContainsFocus)
            {
                return;
            }
            else
            {
                int longitudSueldo = txtSueldo.Text.Trim().Length;

                if (longitudSueldo == 0)
                {
                    MessageBox.Show("Debe ingresar un sueldo de $301.000 pesos como mínimo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSueldo.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }

                int valorSueldo = Convert.ToInt32(txtSueldo.Text);

                if (valorSueldo < 301000)
                {
                    MessageBox.Show("Debe ingresar un sueldo de $301.000 pesos como mínimo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSueldo.Select(); //se hace focus para que no pueda seguir hasta que ingrese formato válido
                    return;
                }
            }
        }
    }
}
