using DPFP;
using DPFP.Capture;
using DPFP.Verification;
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
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lector_biometrico
{
    public partial class frmPrincipal : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        private readonly SocketService _socketService;
        public frmPrincipal()
        {
            InitializeComponent();

            //Properties.Settings.Default.PrimeraVez = true;
            //Properties.Settings.Default.conn = "";
            //Properties.Settings.Default.Save(); // <- esto escribe a disco
            // PEDIR DATO INICIAL SI ES LA PRIMERA VEZ QUE SE EJECUTA
            if (Properties.Settings.Default.PrimeraVez)
            {
                MessageBox.Show("Hace falta configurar la conexión", "Configuracion faltante", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                PedirDatoInicial();
            }

            var env = new env();
            _socketService = new SocketService(env.entorno().ToString(), Properties.Settings.Default.conn);
        }

        private void PedirDatoInicial()
        {
            // aquí muestras tu ventana/diálogo pidiendo el dato
            string dato = Microsoft.VisualBasic.Interaction.InputBox("Ingresa la cadena de conexión:", "Configuración inicial", "");

            if (!string.IsNullOrWhiteSpace(dato))
            {
                Properties.Settings.Default.conn = dato;
                Properties.Settings.Default.PrimeraVez = false;
                Properties.Settings.Default.Save(); // <- esto escribe a disco
            }
        }

        //private void dataGridView1_DataError(
        //    object sender,
        //    DataGridViewDataErrorEventArgs e)
        //{
        //    MessageBox.Show(
        //        $"Error en DataGridView\n\n" +
        //        $"Fila: {e.RowIndex}\n" +
        //        $"Columna: {e.ColumnIndex}\n" +
        //        $"Nombre columna: {dataGridView1.Columns[e.ColumnIndex].Name}\n" +
        //        $"Valor: {dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value}"
        //    );

        //    e.ThrowException = false;
        //}

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                await _socketService.ConectarAsync();

                var service = new clienteService();
                var clientes = await service.ObtenerClientes();
                // mostrar en el grid
                //dataGridView1.DataSource = ClienteStore.Clientes;

                //// ocultar datos en el grid
                //if (dataGridView1.Columns.Contains("idcliente"))
                //{
                //    dataGridView1.Columns["idcliente"].Visible = false;
                //}
                //if (dataGridView1.Columns.Contains("idusuario"))
                //{
                //    dataGridView1.Columns["idusuario"].Visible = false;
                //}
                //if (dataGridView1.Columns.Contains("huella"))
                //{
                //    dataGridView1.Columns["huella"].Visible = false;
                //}

                inicializarLector();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo conectar al servidor:\n{ex.Message}",
                    "Socket.IO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        public async void inciarPrincipal(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
        }

        private void iniciarCaptura()
        {
            try
            {
                if (Capturer != null)
                {
                    Capturer.StartCapture();
                    lblEstadoHuella.Text = "Esperando huella...";
                    centrarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar lector:\n{ex.Message}");
            }
        }

        private void detenerLector()
        {
            try
            {
                if (Capturer != null)
                {
                    Capturer.StopCapture();
                }

                lblEstadoHuella.Text = "Lector detenido";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al detener lector:\n{ex.Message}");
            }
        }

        //private void ClienteGuardado()
        //{
        //    dataGridView1.DataSource = null;
        //    dataGridView1.DataSource = ClienteStore.Clientes;
        //    // ocultar datos en el grid
        //    if (dataGridView1.Columns.Contains("idcliente"))
        //    {
        //        dataGridView1.Columns["idcliente"].Visible = false;
        //    }
        //    if (dataGridView1.Columns.Contains("idusuario"))
        //    {
        //        dataGridView1.Columns["idusuario"].Visible = false;
        //    }
        //    if (dataGridView1.Columns.Contains("huella"))
        //    {
        //        dataGridView1.Columns["huella"].Visible = false;
        //    }
        //}

        private void inicializarLector()
        {
            try
            {
                // Inicializar lector
                Capturer = new DPFP.Capture.Capture();

                if (Capturer == null)
                {
                    MessageBox.Show("No se pudo inicializar el lector.");
                    return;
                }

                Capturer.EventHandler = (DPFP.Capture.EventHandler)this;

                // Iniciar captura
                Capturer.StartCapture();

                lblEstadoHuella.Text = "Esperando huella...";
                centrarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al inicializar:\n{ex.Message}"
                );
            }
        }

        private void centrarDatos()
        {

            // Centrar horizontalmente
            lblEstadoHuella.Left = (this.ClientSize.Width - lblEstadoHuella.Width) / 2;

            // Segunda label debajo de la primera
            lbDatosCliente.Left = (this.ClientSize.Width - lbDatosCliente.Width) / 2;
            lbDatosCliente.Top = lblEstadoHuella.Bottom + 10;
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            try
            {
                // Convertir la huella capturada a características
                DPFP.FeatureSet features = ExtractFeatures(
                    Sample,
                    DPFP.Processing.DataPurpose.Verification
                );

                if (features == null)
                {
                    Invoke(new Action(() =>
                    {
                        lblEstadoHuella.Text = "Huella de baja calidad";
                        centrarDatos();
                    }));

                    return;
                }

                Cliente clienteEncontrado = null;
                var verificator = new DPFP.Verification.Verification();

                foreach (var cliente in ClienteStore.Clientes)
                {
                    if (cliente.huella == null || cliente.huella.Length == 0)
                        continue;

                    try
                    {
                        // Convertir los bytes almacenados en Mongo
                        // nuevamente a DPFP.Template
                        DPFP.Template template;
                        using (var stream = new MemoryStream(cliente.huella))
                        {
                            template = new DPFP.Template(stream);
                        }
                        // Comparar huella del lector contra template
                        var result = new DPFP.Verification.Verification.Result();

                        verificator.Verify(
                            features,
                            template,
                            ref result
                        );

                        if (result.Verified)
                        {
                            clienteEncontrado = cliente;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error verificando {cliente.nombre}: {ex.Message}");
                    }
                }

                Invoke(new Action(async () =>
                {
                    if (clienteEncontrado != null)
                    {
                        lblEstadoHuella.Text = $"Cliente encontrado: {clienteEncontrado.nombre}";

                        lbDatosCliente.Text = $"Huella reconocida\n\n" +
                        $"Cliente: {clienteEncontrado.nombre}\n" +
                        $"Teléfono: {clienteEncontrado.telefono}";
                        centrarDatos();

                        var verificacion = new VerificarMembresia();
                        var membresia = await verificacion.VerificarMembresiaValida(clienteEncontrado.idcliente);

                        if (membresia != null)
                        {
                            if(membresia.estado == "vigente")
                            {
                                this.BackColor = Color.Green;
                            } else if(membresia.estado == "cancelada")
                            {
                                this.BackColor = Color.Blue;
                            } else
                            {
                                //this.ForeColor = Color.White;
                                this.BackColor = Color.Red;
                            }
                            lblEstadoHuella.Text = $"Cliente encontrado: {clienteEncontrado.nombre}";

                            lbDatosCliente.Text =
                                $"Huella reconocida\n\n" +
                                $"Cliente: {clienteEncontrado.nombre}\n" +
                                $"Teléfono: {clienteEncontrado.telefono}\n" +
                                $"Membresía: {membresia.estado}\n" +
                                $"Inicio: {membresia.fechaInicio:dd/MM/yyyy}\n" +
                                $"Fin: {membresia.fechaFin:dd/MM/yyyy}\n" +
                                $"Vencimiento: {membresia.fechaVencimiento:dd/MM/yyyy HH:mm:ss}\n" +
                                $"Días restantes: {membresia.diasRestantes}";

                            centrarDatos();
                        }
                        else
                        {
                            this.BackColor = Color.Red;

                            lblEstadoHuella.Text = $"Cliente encontrado: {clienteEncontrado.nombre}";
                            lbDatosCliente.Text =
                                $"Huella reconocida\n\n" +
                                "No tiene una membresia asignada";
                            centrarDatos();
                        }

                        await Task.Delay(10000);
                        lblEstadoHuella.Text = "Esperando huella...";
                        lbDatosCliente.Text = "";
                        centrarDatos();
                        this.BackColor = Color.White;
                        //this.ForeColor = Color.Black;
                    }
                    else
                    {
                        this.BackColor = Color.Red;
                        await Task.Delay(10000);

                        lblEstadoHuella.Text = "Huella no reconocida";
                        lbDatosCliente.Text =
                            "Esta huella no pertenece a ningun cliente registrado.\n\n" +
                            "Acceso denegado";
                        centrarDatos();
                        //this.ForeColor = Color.Black;
                        this.BackColor = Color.White;

                    }
                }));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show(
                        $"Error al verificar huella:\n{ex.Message}"
                    );
                }));
            }
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            //throw new NotImplementedException();
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            Invoke(new Action(() =>
            {
                lblEstadoHuella.Text = "Dedo detectado";
                centrarDatos();
            }));
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            //throw new NotImplementedException();
            Invoke(new Action(() =>
            {
                lblEstadoHuella.Text = "Lector detectado";
                centrarDatos();
            }));
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            //throw new NotImplementedException();
            Invoke(new Action(() =>
            {
                lblEstadoHuella.Text = "Lector esta desconectado";
                centrarDatos();
            }));
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback)
        {
            //throw new NotImplementedException();
        }

        private DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose purpose)
        {
            DPFP.Processing.FeatureExtraction extractor =
                new DPFP.Processing.FeatureExtraction();

            DPFP.Capture.CaptureFeedback feedback =
                DPFP.Capture.CaptureFeedback.None;

            DPFP.FeatureSet features =
                new DPFP.FeatureSet();

            extractor.CreateFeatureSet(
                Sample,
                purpose,
                ref feedback,
                ref features
            );

            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;

            return null;
        }

        private void btnCambiarConexcion_Click(object sender, EventArgs e)
        {
            PedirDatoInicial();
        }

        private async void btnRecargarClientes_Click(object sender, EventArgs e)
        {
            try
            {
                var service = new clienteService();
                var clientes = await service.ObtenerClientes();

                //dataGridView1.DataSource = ClienteStore.Clientes;

                //// ocultar datos en el grid
                //if (dataGridView1.Columns.Contains("_id"))
                //{
                //    dataGridView1.Columns["_id"].Visible = false;
                //}
                //if (dataGridView1.Columns.Contains("huella"))
                //{
                //    dataGridView1.Columns["huella"].Visible = false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al obtener clientes:\n{ex.Message}"
                );
            }
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                var service = new clienteService();
                var clientes = await service.ObtenerClientes();

                //dataGridView1.DataSource = ClienteStore.Clientes;

                //// ocultar datos en el grid
                //if (dataGridView1.Columns.Contains("_id"))
                //{
                //    dataGridView1.Columns["_id"].Visible = false;
                //}
                //if (dataGridView1.Columns.Contains("huella"))
                //{
                //    dataGridView1.Columns["huella"].Visible = false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al obtener clientes:\n{ex.Message}"
                );
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PedirDatoInicial();
        }

        private void toolStripRegistrarCliente_Click(object sender, EventArgs e)
        {
            detenerLector();

            frmRegistrar registrar = new frmRegistrar();
            registrar.ShowDialog(this);
            //ClienteGuardado();

            iniciarCaptura();
        }

        private void lblEstadoHuella_Click(object sender, EventArgs e)
        {

        }

        private void toolStripRegistrarHuella_Click(object sender, EventArgs e)
        {
            detenerLector();

            var asignarHuella = new frmAsignarHuella();
            asignarHuella.ShowDialog();
            //ClienteGuardado();

            iniciarCaptura();
        }
    }
}
