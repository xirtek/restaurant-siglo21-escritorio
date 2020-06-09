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
    public partial class BodegaMantenedorProducto : Form
    {

        //OracleConnection ora = new OracleConnection("DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;");
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public BodegaMantenedorProducto()
        {
            InitializeComponent();
            ListarTabla(); //se lista la tabla al iniciar la ventana
            LlenarCbo(); //llena cbo con proveedores
            LlenarCboDetalle();
        }

        //Listar elementos de la tabla
        private void ListarTabla()
        {
            ora.Open();
            OracleCommand comando = new OracleCommand("seleccionarProducto", ora);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dgvProductosBodega.DataSource = tabla;
            ora.Close();

            //Cada vez que se actualiza la tabla se limpian los textbox
            LimpiarTextBox();
        }

        private void LlenarCbo()
        {
            ora.Open();
            cboProveedor.Refresh();//refrescamos el ComboBox1
            OracleCommand oraCmd = new OracleCommand("SELECT * FROM PROVEEDOR ", ora);
            OracleDataReader oraReader = oraCmd.ExecuteReader();

            while (oraReader.Read())
            {
                cboProveedor.Refresh();
                cboProveedor.Items.Add(oraReader.GetValue(3).ToString());//se llena cbo   
            }
            ora.Close();
            cboProveedor.Items.Insert(0, "Seleccione un Proveedor");
            cboProveedor.SelectedIndex = 0;
        }

        private void LlenarCboDetalle()
        {
            cboDetalleStock.Items.Add("Seleccione Unidad Stock");
            cboDetalleStock.Items.Add("unidades");
            cboDetalleStock.Items.Add("kilos");
            cboDetalleStock.Items.Add("gramos");
            cboDetalleStock.SelectedIndex = 0;
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (cboDetalleStock.SelectedItem.ToString() == "Seleccione Unidad Stock")
            {
                MessageBox.Show("Debe seleccionar tipo de unidad de stock del producto.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboDetalleStock.Select();
                return;
            }
            else if (cboProveedor.Text == "Seleccione un Proveedor")
            {
                MessageBox.Show("Debe seleccionar el Proveedor del producto.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboProveedor.Select();
                return;
            }
            else
            {
                try
                {
                    int ide2 = 0;
                    ora.Open();
                    OracleCommand comando = new OracleCommand("insertarProducto", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("descp", OracleType.VarChar).Value = txtDescripcion.Text;
                    comando.Parameters.Add("stk", OracleType.Number).Value = Convert.ToInt32(txtStock.Text);
                    comando.Parameters.Add("detalle", OracleType.VarChar).Value = cboDetalleStock.SelectedItem.ToString();
                    //cbo
                    OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PROVEEDOR WHERE DESCRIPCION='" + cboProveedor.SelectedItem.ToString() + "'", ora);
                    OracleDataReader oraReader = oraCmd2.ExecuteReader();

                    while (oraReader.Read())
                    {
                        ide2 = Convert.ToInt32(oraReader.GetValue(0).ToString());
                    }

                    comando.Parameters.Add("idprov", OracleType.Number).Value = ide2;

                    comando.ExecuteNonQuery(); //ejecuta proc almacenado
                    MessageBox.Show("Producto ha sido insertado correctamente.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Falló al insertar Producto.");
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
            else if (cboDetalleStock.SelectedItem.ToString() == "Seleccione Unidad Stock")
            {
                MessageBox.Show("Debe seleccionar tipo de unidad de stock del producto.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboDetalleStock.Select();
                return;
            }
            else if (cboProveedor.Text == "Seleccione un Proveedor")
            {
                MessageBox.Show("Debe seleccionar el Proveedor del producto.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboProveedor.Select();
                return;
            }
            else
            {
                int ide2 = 0;
                ora.Open();
                OracleCommand comando = new OracleCommand("actualizarProducto", ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                comando.Parameters.Add("descp", OracleType.VarChar).Value = txtDescripcion.Text;
                comando.Parameters.Add("stk", OracleType.Number).Value = Convert.ToInt32(txtStock.Text);
                comando.Parameters.Add("detalle", OracleType.VarChar).Value = cboDetalleStock.SelectedItem.ToString();
                //cbo
                OracleCommand oraCmd2 = new OracleCommand("SELECT * FROM PROVEEDOR WHERE DESCRIPCION='" + cboProveedor.SelectedItem.ToString() + "'", ora);
                OracleDataReader oraReader = oraCmd2.ExecuteReader();

                while (oraReader.Read())
                {
                    ide2 = Convert.ToInt32(oraReader.GetValue(0).ToString());
                }

                comando.Parameters.Add("idprov", OracleType.Number).Value = ide2;

                comando.ExecuteNonQuery();
                MessageBox.Show("Producto ha sido actualizado correctamente.");
                ora.Close();
                ListarTabla();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Debe elegir primero un Producto de la tabla para eliminarlo.");
                return;
            }
            else
            {            
                DialogResult dialogResult = MessageBox.Show("¿Desea eliminar este Producto?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    ora.Open();
                    OracleCommand comando = new OracleCommand("eliminarProducto", ora);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("idp", OracleType.Number).Value = Convert.ToInt32(txtID.Text);
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Producto ha sido eliminado correctamente.");
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
            txtDescripcion.Select();
            txtID.Clear();
            txtDescripcion.Clear();
            txtStock.Clear();
            //cbo
            cboDetalleStock.SelectedItem = "Seleccione Unidad Stock";
            cboProveedor.SelectedItem = "Seleccione un Proveedor";
        }

        private void LimpiarTextBox()
        {
            txtDescripcion.Select();
            txtID.Clear();
            txtDescripcion.Clear();
            txtStock.Clear();
            //cbo
            cboDetalleStock.SelectedItem = "Seleccione Unidad Stock";
            cboProveedor.SelectedItem = "Seleccione un Proveedor";

        }

        private void BodegaMantenedorProducto_FormClosing(object sender, FormClosingEventArgs e)
        {
            BodegaMantenedorProducto frm = new BodegaMantenedorProducto();
            BodegaPrincipal frm2 = new BodegaPrincipal();
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
            BodegaPrincipal frm2 = new BodegaPrincipal();
            frm2.Show();
        }

        private void dgvProductosBodega_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvProductosBodega.CurrentRow.Cells[0].Value.ToString();
            txtDescripcion.Text = dgvProductosBodega.CurrentRow.Cells[1].Value.ToString();
            txtStock.Text = dgvProductosBodega.CurrentRow.Cells[2].Value.ToString();
            cboDetalleStock.Text = dgvProductosBodega.CurrentRow.Cells[3].Value.ToString();
            //cbo
            string valor = dgvProductosBodega.CurrentRow.Cells[4].Value.ToString();
            cboProveedor.SelectedItem = valor;
        }

        //VALIDACIONES

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitudTexto = txtStock.Text.Trim().Length;
            //validación de solo números
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else if (e.KeyChar == Convert.ToChar(Keys.D0) && longitudTexto == 0 && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Tiene que ser distinto que 0", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtDescripcion_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProductosBodega.ContainsFocus)
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

        private void txtStock_Leave(object sender, EventArgs e)
        {
            if (btnVolver.ContainsFocus || btnCerrarSesion.ContainsFocus || btnLimpiar.ContainsFocus || btnEliminar.ContainsFocus || dgvProductosBodega.ContainsFocus)
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
    }
}
