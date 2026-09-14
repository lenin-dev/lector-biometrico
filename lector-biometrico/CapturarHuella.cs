using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lector_biometrico
{
    public partial class CapturarHuella : CaptureForm
    {
        public delegate void OnTempladeEventHandler(DPFP.Template template);
        public event OnTempladeEventHandler OnTemplate;
        private DPFP.Processing.Enrollment Enroller;

        protected override void Init()
        {
            base.Init();
            base.Text = "Dar de alta Huella";
            Enroller = new DPFP.Processing.Enrollment();
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            // Show number of samples needed.
            SetStatus(String.Format("Se necesitan muestras de huellas dactilares: {0}", Enroller.FeaturesNeeded));
        }

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            // Check quality of the sample and add to enroller if it's good
            if (features != null) try
                {
                    MakeReport("Se creó el conjunto de funciones de huella digital.");
                    Enroller.AddFeatures(features);     // Add feature set to template.
                }
                finally
                {
                    UpdateStatus();

                    // Check if template has been created.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:   // report success and stop capturing
                            OnTemplate(Enroller.Template);
                            SetPrompt("Haga clic en Cerrar y, a continuación, en Verificación de huella dactilar.");
                            Stop();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:  // report failure and restart capturing
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            Start();
                            break;
                    }
                }
        }

        public CapturarHuella()
        {
            InitializeComponent();
        }
    }
}
