using lector_biometrico;
using lector_biometrico.models;
using lector_biometrico.services;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lector_biometrico
{
    public partial class frmRegistrar : Form
    {
        private DPFP.Template Template;
        public event Action<Cliente> ClienteGuardado;

        public frmRegistrar()
        {
            InitializeComponent();
        }

        private void btnRegistrarHuella_Click(object sender, EventArgs e)
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
                btnGuardarCliente.Enabled = (Template != null);
                if (Template != null)
                {
                    MessageBox.Show("La plantilla de huella dactilar está lista para la verificación de la huella.");
                    txtHuella.Text = "Huella capturada correctamente";
                }
                else
                {
                    MessageBox.Show("La plantilla de huella dactilar no es válida. Repita el registro de la huella dactilar.");
                }
            }));
        }

        private async void btnGuardarCliente_Click(object sender, EventArgs e)
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
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre es obligatorio.");
                    txtNombre.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtTelefono.Text))
                {
                    MessageBox.Show("El teléfono es obligatorio.");
                    txtTelefono.Focus();
                    return;
                }
                if (Template == null || Template.Bytes == null)
                {
                    MessageBox.Show("La huella es obligatorio.");
                    txtHuella.Focus();
                    return;
                }

                // Crear objeto Cliente
                var cliente = new Cliente
                {
                    nombre = txtNombre.Text.Trim(),
                    telefono = txtTelefono.Text.Trim(),
                    email = string.IsNullOrWhiteSpace(txtEmail.Text)
                        ? null
                        : txtEmail.Text.Trim(),
                    direccion = string.IsNullOrWhiteSpace(txtDireccion.Text)
                        ? null
                        : txtDireccion.Text.Trim(),
                    // Guardar la huella como un arreglo de bytes
                    huella = templateBytes
                };

                // Guardar en API
                var service = new clienteService();
                var clienteGuardado = await service.GuardarClientes(cliente);
                ClienteGuardado?.Invoke(clienteGuardado);

                MessageBox.Show(
                    $"Cliente guardado correctamente.\n\n"
                );

                ClienteStore.Clientes.Add(cliente);
                limpiar();
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

        private void limpiar()
        {
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            txtDireccion.Text = "";
            txtHuella.Text = "";
            Template = null;
        }
    }
}
