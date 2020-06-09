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
    public partial class BodegaPrincipal : Form
    {
        public BodegaPrincipal()
        {
            InitializeComponent();
        }

        private void btnGestionarProductos_Click(object sender, EventArgs e)
        {
            this.Dispose();
            BodegaMantenedorProducto frm = new BodegaMantenedorProducto();
            frm.Show();
        }

        private void BodegaPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            BodegaPrincipal frm = new BodegaPrincipal();
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
            BodegaSolicitud frm = new BodegaSolicitud();
            frm.Show();
        }
    }
}
