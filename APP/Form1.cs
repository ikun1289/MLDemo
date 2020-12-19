using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLDemoML.Model;
namespace MLAPPML.ConsoleApp
{
    public partial class Form1 : Form
    {
        DrawNote drawnote;
        Bitmap bm;
        string source;
        MLContext mlContext;
        ITransformer mlModel;
        public static string[] label;

        public Form1()
        {
            InitializeComponent();
            drawnote = new DrawNote();
            mlContext = new MLContext();
            pictureBox1.Image = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
            mlModel = mlContext.Model.Load("MLModel.zip", out var modelInputSchema);
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.pictureBox1.Focus();
            drawnote.isDraw = true;
            drawnote.X = e.X;
            drawnote.Y = e.Y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            drawnote.isDraw = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawnote.isDraw)
            {
                Graphics G = Graphics.FromImage(pictureBox1.Image);
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                Graphics thum = this.pictureBox1.CreateGraphics();
                thum.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                thum.DrawLine(drawnote.pen, drawnote.X, drawnote.Y, e.X, e.Y);

                G.DrawLine(drawnote.pen, drawnote.X, drawnote.Y, e.X, e.Y);
                drawnote.X = e.X;
                drawnote.Y = e.Y;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Lưu lại ảnh mới vẽ
            using (var b = new Bitmap(this.pictureBox1.Image.Width, this.pictureBox1.Image.Height))
            {
                b.SetResolution(this.pictureBox1.Image.HorizontalResolution, this.pictureBox1.Image.VerticalResolution);

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(this.pictureBox1.Image, 0, 0);
                }
                b.Save(String.Format("check.jpg"));
            }

            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            ModelInput sampleData = new ModelInput()
            {
                ImageSource = "check.jpg",
            };


            //// Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);
            var predictionEngine = ConsumeModel.CreatePredictionEngine();
            var labelBuffer = new VBuffer<ReadOnlyMemory<char>>();
            predictionEngine.OutputSchema["Score"].Annotations.GetValue("SlotNames", ref labelBuffer);
            var labels = labelBuffer.DenseValues().Select(l => l.ToString()).ToArray();
            var top10scores = labels.ToDictionary(
            l => l,
            l => (decimal)predictionResult.Score[Array.IndexOf(labels, l)]
            )
            .OrderByDescending(kv => kv.Value)
            .Take(10);
            //xuất kết quả dự báo
            this.richTextBox1.Text = $"Predicted Label value {predictionResult.Prediction} \nPredicted Label scores: \n";
            for (int i=0;i<10;i++)
            this.richTextBox1.Text += top10scores.ToList()[i].ToString() +"\n";

            
            pictureBox1.CreateGraphics().Clear(Color.White);
            this.pictureBox1.Image = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
        }



        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
