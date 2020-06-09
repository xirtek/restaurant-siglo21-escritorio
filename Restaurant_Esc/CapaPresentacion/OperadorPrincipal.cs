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
    public partial class OperadorPrincipal : Form
    {
        public OperadorPrincipal()
        {
            InitializeComponent();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Dispose();
            IniciarSesion frm2 = new IniciarSesion();
            frm2.Show();
        }

        private void OperadorPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            OperadorPrincipal frm = new OperadorPrincipal();
            IniciarSesion frm2 = new IniciarSesion();
            frm.Dispose();
            frm2.Show();
        }

        private void btnGestionarReservas_Click(object sender, EventArgs e)
        {
            this.Dispose();
            OperadorMantenedorReserva frm = new OperadorMantenedorReserva();
            frm.Show();
        }

        private void btnGestionarClientes_Click(object sender, EventArgs e)
        {
            this.Dispose();
            OperadorMantenedorClientes frm = new OperadorMantenedorClientes();
            frm.Show();
        }
    }
}
