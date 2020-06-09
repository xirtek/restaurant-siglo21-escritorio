using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class AdminCrud : Form
    {
        public AdminCrud()
        {
            InitializeComponent();
            
        }

        private void btnPerfil_Click(object sender, EventArgs e)
        {
            //this.Hide();
            this.Dispose();
            AdminMantenedorPerfil frm = new AdminMantenedorPerfil();
            frm.Show();
        }

        private void AdminPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminCrud frm = new AdminCrud();
            AdminPrincipal frm2 = new AdminPrincipal();
            frm.Dispose();
            frm2.Show();
        }

        private void btnProveedor_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorProveedor frm = new AdminMantenedorProveedor();
            frm.Show();
        }

        private void btnProducto_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorProducto frm = new AdminMantenedorProducto();
            frm.Show();
        }

        private void btnPlato_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorPlato frm = new AdminMantenedorPlato();
            frm.Show();
        }

        private void btnTrabajador_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorTrabajador frm = new AdminMantenedorTrabajador();
            frm.Show();
        }

        private void btnBebestible_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorBebestible frm = new AdminMantenedorBebestible();
            frm.Show();
        }

        private void btnCategoria_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorCategoria frm = new AdminMantenedorCategoria();
            frm.Show();
        }

        private void btnPostre_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorPostre frm = new AdminMantenedorPostre();
            frm.Show();
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorCliente frm = new AdminMantenedorCliente();
            frm.Show();
        }

        private void btnMesa_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorMesa frm = new AdminMantenedorMesa();
            frm.Show();
        }

        private void btnUsuario_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorUsuario frm = new AdminMantenedorUsuario();
            frm.Show();
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

        private void btnReserva_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorReserva frm2 = new AdminMantenedorReserva();
            frm2.Show();
        }

        private void btnVenta_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorVenta frm2 = new AdminMantenedorVenta();
            frm2.Show();
        }

        private void btnPedido_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorPedido frm2 = new AdminMantenedorPedido();
            frm2.Show();
        }

        private void btnOrden_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorOrden frm2 = new AdminMantenedorOrden();
            frm2.Show();
        }

        private void btnCargo_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminMantenedorCargo frm2 = new AdminMantenedorCargo();
            frm2.Show();
        }
    }
}
