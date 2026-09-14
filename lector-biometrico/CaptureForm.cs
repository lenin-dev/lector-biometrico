using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace lector_biometrico
{
	/* NOTE: This form is a base for the EnrollmentForm and the VerificationForm,
		All changes in the CaptureForm will be reflected in all its derived forms.
	*/
	public partial class CaptureForm : Form, DPFP.Capture.EventHandler
	{

        //private delegate void Function();

        public CaptureForm()
		{
			InitializeComponent();
		}

		protected virtual void Init()
		{
            try
            {
                Capturer = new DPFP.Capture.Capture();				// Create a capture operation.

                if ( null != Capturer )
                    Capturer.EventHandler = this;					// Subscribe for capturing events.
                else
                    SetPrompt("¡No se puede iniciar la operación de captura!");
            }
            catch
            {               
                MessageBox.Show("¡No se puede iniciar la operación de captura!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }
		}

		protected virtual void Process(DPFP.Sample Sample)
		{
			// Draw fingerprint sample image.
			DrawPicture(ConvertSampleToBitmap(Sample));
		}

		protected void Start()
		{
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Escanea tu huella dactilar utilizando el lector de huellas.");
                }
                catch
                {
                    SetPrompt("¡No se puede iniciar la captura!");
                }
            }
		}

		protected void Stop()
		{
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    SetPrompt("¡No se puede finalizar la captura!");
                }
            }
		}

        private void SafeInvoke(Action action)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;

            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        #region Form Event Handlers:

        private void CaptureForm_Load(object sender, EventArgs e)
		{
			Init();												// Start capture operation.
		}

        private void CaptureForm_Shown(object sender, EventArgs e)
        {
            Start(); // arranca captura solo cuando el form ya está completamente visible
        }

        private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Stop();
		}
	#endregion

	#region EventHandler Members:

		public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
		{
			MakeReport("Se capturó la muestra de huella dactilar.");
			SetPrompt("Escanea la misma huella dactilar de nuevo.");
			Process(Sample);
		}

		public void OnFingerGone(object Capture, string ReaderSerialNumber)
		{
			MakeReport("Se retiró el dedo del lector de huellas dactilares.");
		}

		public void OnFingerTouch(object Capture, string ReaderSerialNumber)
		{
			MakeReport("Se tocó el lector de huellas dactilares.");
		}

		public void OnReaderConnect(object Capture, string ReaderSerialNumber)
		{
			MakeReport("El lector de huellas digitales estaba conectado.");
		}

		public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
		{
			MakeReport("El lector de huellas dactilares estaba desconectado.");
		}

		public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
		{
			if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
				MakeReport("La calidad de la muestra de huella dactilar es buena.");
			else
				MakeReport("La calidad de la muestra de huella dactilar es deficiente.");
		}
	#endregion

		protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
		{
			DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();	// Create a sample convertor.
			Bitmap bitmap = null;												            // TODO: the size doesn't matter
			Convertor.ConvertToPicture(Sample, ref bitmap);									// TODO: return bitmap as a result
			return bitmap;
		}

		protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
		{
			DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();	// Create a feature extractor
			DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
			DPFP.FeatureSet features = new DPFP.FeatureSet();
			Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);			// TODO: return features as a result?
			if (feedback == DPFP.Capture.CaptureFeedback.Good)
				return features;
			else
				return null;
		}

        protected void SetStatus(string status)
        {
            SafeInvoke(() => StatusLine.Text = status);
        }

        protected void SetPrompt(string prompt)
        {
            SafeInvoke(() => Prompt.Text = prompt);
        }

        protected void MakeReport(string message)
        {
            SafeInvoke(() => StatusText.AppendText(message + "\r\n"));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            SafeInvoke(() => Picture.Image = new Bitmap(bitmap, Picture.Size));
        }

        //protected void SetStatus(string status)
        //{
        //	this.Invoke(new Action(delegate() {
        //		StatusLine.Text = status;
        //	}));
        //}

        //protected void SetPrompt(string prompt)
        //{
        //	this.Invoke(new Action(delegate() {
        //		Prompt.Text = prompt;
        //	}));
        //}
        //protected void MakeReport(string message)
        //{
        //	this.Invoke(new Action(delegate() {
        //		StatusText.AppendText(message + "\r\n");
        //	}));
        //}

        //private void DrawPicture(Bitmap bitmap)
        //{
        //	this.Invoke(new Action(delegate() {
        //		Picture.Image = new Bitmap(bitmap, Picture.Size);	// fit the image into the picture box
        //	}));
        //}

        private DPFP.Capture.Capture Capturer;

	}
}