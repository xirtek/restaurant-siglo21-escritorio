using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using System.Data.OracleClient;

namespace CapaPresentacion
{
    public partial class AdminReporte : Form
    {
        OracleConnection ora = new OracleConnection(Conexion.obtenerCadena());

        public AdminReporte()
        {
            InitializeComponent();
            LlenarCboVentasxFecha();
        }

        private void ListarTabla(string tipodato)
        {
                ora.Open();
                OracleCommand comando = new OracleCommand(tipodato, ora);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("registros", OracleType.Cursor).Direction = ParameterDirection.Output;
                OracleDataAdapter adaptador = new OracleDataAdapter();
                adaptador.SelectCommand = comando;
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dgvver.DataSource = tabla;
                ora.Close();
        }

        private void LlenarCboVentasxFecha()
        {
            cboMostrar.Items.Add("Seleccione un dato");
            cboMostrar.Items.Add("Cargo");
            cboMostrar.Items.Add("Cliente");
            cboMostrar.Items.Add("Mesa");
            cboMostrar.Items.Add("Producto");
            cboMostrar.Items.Add("Categoría");
            cboMostrar.Items.Add("Perfil");
            cboMostrar.Items.Add("Reserva");
            cboMostrar.Items.Add("Trabajador");
            cboMostrar.Items.Add("Plato");
            cboMostrar.Items.Add("Bebestible");
            cboMostrar.Items.Add("Postre");
            cboMostrar.Items.Add("Proveedor");
            cboMostrar.Items.Add("Venta");
            cboMostrar.Items.Add("Pedido");
            cboMostrar.Items.Add("Órden");
            cboMostrar.Items.Add("Usuario");
            cboMostrar.SelectedIndex = 0;
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
            AdminPrincipal frm2 = new AdminPrincipal();
            frm2.Show();
        }

        private void AdminReporte_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminReporte frm = new AdminReporte();
            AdminPrincipal frm2 = new AdminPrincipal();
            frm.Dispose();
            frm2.Show();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (cboMostrar.SelectedItem.ToString() == "Seleccione un dato")
            {
                MessageBox.Show("Debe seleccionar un dato.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboMostrar.Select();
                return;
            }
            else
            {
                PrintDocument doc = new PrintDocument();
                doc.DefaultPageSettings.Landscape = true;
                doc.PrinterSettings.PrintToFile = true;

                doc.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                PrintPreviewDialog ppd = new PrintPreviewDialog { Document = doc };
                ((Form)ppd).WindowState = FormWindowState.Maximized;

                doc.PrintPage += delegate (object ev, PrintPageEventArgs ep)
                {
                    const int DGV_ALTO = 35;
                    int left = ep.MarginBounds.Left, top = ep.MarginBounds.Top;

                    foreach (DataGridViewColumn col in dgvver.Columns)
                    {
                        ep.Graphics.DrawString(col.HeaderText, new Font("Segoe UI", 10, FontStyle.Bold), Brushes.DeepSkyBlue, left, top);
                        left += col.Width;

                        if (col.Index < dgvver.ColumnCount - 1)
                            ep.Graphics.DrawLine(Pens.Gray, left - 5, top, left - 5, top + 43 + (dgvver.RowCount - 1) * DGV_ALTO);
                    }
                    left = ep.MarginBounds.Left;
                    ep.Graphics.FillRectangle(Brushes.Black, left, top + 40, ep.MarginBounds.Right - left, 3);
                    top += 43;

                    foreach (DataGridViewRow row in dgvver.Rows)
                    {
                        if (row.Index == dgvver.RowCount - 1) break;
                        left = ep.MarginBounds.Left;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            ep.Graphics.DrawString(Convert.ToString(cell.Value), new Font("Segoe UI", 10), Brushes.Black, left, top + 4);
                            left += cell.OwningColumn.Width;
                        }
                        top += DGV_ALTO;
                        ep.Graphics.DrawLine(Pens.Gray, ep.MarginBounds.Left, top, ep.MarginBounds.Right, top);
                    }
                };
                ppd.ShowDialog();
            }
        }

        private void cboMostrar_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cboMostrar.SelectedItem.ToString() == "Seleccione un dato")
            {
                dgvver.DataSource = "";
            }
            else
            {
                string datop = "";
                if (cboMostrar.SelectedItem.ToString() == "Cargo")
                {
                    datop = "seleccionarCargo";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Cliente")
                {
                    datop = "seleccionarCliente";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Mesa")
                {
                    datop = "seleccionarMesa";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Producto")
                {
                    datop = "seleccionarProducto";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Categoría")
                {
                    datop = "seleccionarCategoria";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Perfil")
                {
                    datop = "seleccionarPerfil";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Reserva")
                {
                    datop = "seleccionarReserva";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Trabajador")
                {
                    datop = "seleccionarTrabajador";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Plato")
                {
                    datop = "seleccionarPlato";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Bebestible")
                {
                    datop = "seleccionarBebestible";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Postre")
                {
                    datop = "seleccionarPostre";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Proveedor")
                {
                    datop = "seleccionarProveedor";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Venta")
                {
                    datop = "seleccionarVenta";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Pedido")
                {
                    datop = "seleccionarPedido";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Órden")
                {
                    datop = "seleccionarOrden";
                }
                else if (cboMostrar.SelectedItem.ToString() == "Usuario")
                {
                    datop = "seleccionarUsuario";
                }
                ListarTabla(datop);
            }
        }
    }
}
