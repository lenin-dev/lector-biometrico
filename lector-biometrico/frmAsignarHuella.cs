using lector_biometrico.models;
using lector_biometrico.services;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lector_biometrico
{
    public partial class frmAsignarHuella : Form
    {
        private DPFP.Template Template;
        public event Action<Cliente> ClienteGuardado;
        private Cliente clienteSeleccionado;
        public frmAsignarHuella()
        {
            InitializeComponent();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void frmAsignarHuella_Load(object sender, EventArgs e)
        {
            label3.Left = (this.ClientSize.Width - label3.Width) / 2;
        }

        private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            string busqueda = txtBuscarCliente.Text.Trim();
            var clientes = ClienteStore.Clientes;

            if (string.IsNullOrEmpty(busqueda))
            {
                cmbClientes.DataSource = null;
                return;
            }

            var resultados = clientes
                .Where(c =>
                    c.nombre.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.telefono ?? "").IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0
                )
                .ToList();

            cmbClientes.DataSource = null;
            cmbClientes.DataSource = resultados;
            cmbClientes.DisplayMember = "nombre";
            cmbClientes.ValueMember = "idusuario" ?? "idcliente";

            if (resultados.Count > 0)
            {
                cmbClientes.SelectedIndex = 0;
            }
        }

        private void cmbClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbClientes.SelectedItem is Cliente cliente)
            {
                clienteSeleccionado = cliente;
                label3.Text = $"Coloca la huella de {cliente.nombre}";
                label3.Left = (this.ClientSize.Width - label3.Width) / 2;
            }
        }

        private void btnCapturarHuella_Click(object sender, EventArgs e)
        {
            CapturarHuella capturar = new CapturarHuella();
            capturar.OnTemplate += this.OnTemplate;
            capturar.ShowDialog();
        }

        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Action(delegate ()
            {
                Template = template;
                btnGuardarAsignacion.Enabled = (Template != null);
                if (Template != null)
                {
                    MessageBox.Show("La plantilla de huella dactilar está lista para la verificación de la huella.");
                    label3.Text = "Huella capturada correctamente";
                    label3.Left = (this.ClientSize.Width - label3.Width) / 2;
                }
                else
                {
                    MessageBox.Show("La plantilla de huella dactilar no es válida. Repita el registro de la huella dactilar.");
                }
            }));
        }

        private async void btnGuardarAsignacion_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] templateBytes;
                using (var stream = new MemoryStream())
                {
                    Template.Serialize(stream);
                    templateBytes = stream.ToArray();
                }

                // Validaciones
                if (string.IsNullOrWhiteSpace(cmbClientes.Text))
                {
                    MessageBox.Show("El cliente es obligatorio.");
                    return;
                }
                if (Template == null || Template.Bytes == null)
                {
                    MessageBox.Show("La huella es obligatorio.");
                    return;
                }

                // Guardar en API
                var service = new clienteService();
                var clienteGuardado = await service.AsignarHuellaCliente(clienteSeleccionado.idusuario, clienteSeleccionado.idcliente, templateBytes);
                ClienteGuardado?.Invoke(clienteGuardado);

                MessageBox.Show(
                    $"Cliente guardado correctamente.\n\n"
                );

                ClienteStore.Clientes.Add(clienteGuardado);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al guardar cliente:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void centrarDatos()
        {
            // Centrar horizontalmente
            label3.Left = (this.ClientSize.Width - label3.Width) / 2;

            //// Colocarla debajo del componente
            //lblEstadoHuella.Top = dataGridView1.Bottom + 20;

            //// Segunda label debajo de la primera
            //lbDatosCliente.Left = (this.ClientSize.Width - lbDatosCliente.Width) / 2;
            //lbDatosCliente.Top = lblEstadoHuella.Bottom + 10;
        }
    }
}
