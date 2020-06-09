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
    public partial class AdminPrincipal : Form
    {
        public AdminPrincipal()
        {
            InitializeComponent();
        }

        private void btnGestionar_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminCrud frm = new AdminCrud();
            frm.Show();
        }

        private void AdminPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            AdminPrincipal frm = new AdminPrincipal();
            IniciarSesion frm2 = new IniciarSesion();
            frm.Dispose();
            frm2.Show();

        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Dispose();
            IniciarSesion frm2 = new IniciarSesion();
            frm2.Show();
        }

        private void btnSolicitudes_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminSolicitudProducto frm = new AdminSolicitudProducto();
            frm.Show();
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminEstadisticas frm = new AdminEstadisticas();
            frm.Show();
        }

        private void btonReportes_Click(object sender, EventArgs e)
        {
            this.Dispose();
            AdminReporte frm = new AdminReporte();
            frm.Show();
        }
    }
}
