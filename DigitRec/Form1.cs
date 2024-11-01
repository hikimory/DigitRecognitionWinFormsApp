using DigitRec.ActivationFunctions;
using DigitRec.Utils;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;

namespace DigitRec
{
    public partial class Form1 : Form
    {
        private Bitmap bitmap;
        private bool isDrawing = false;
        private Dictionary<int, ProgressBar> bars;
        private List<Classification> classifications;
        private NeuralNetwork nn;

        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            InitializeProgressBars();
            InitializeClassifications();
            InitializeNN();
        }

        private void InitializeProgressBars()
        {
            bars = new Dictionary<int, ProgressBar>
            {
                { 0, progressBar0 },
                { 1, progressBar1 },
                { 2, progressBar2 },
                { 3, progressBar3 },
                { 4, progressBar4 },
                { 5, progressBar5 },
                { 6, progressBar6 },
                { 7, progressBar7 },
                { 8, progressBar8 },
                { 9, progressBar9 }
            };
        }

        private void InitializeClassifications()
        {
            classifications = new List<Classification>
            {
                new Classification("0", new double[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                new Classification("1", new double[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }),
                new Classification("2", new double[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }),
                new Classification("3", new double[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 }),
                new Classification("4", new double[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }),
                new Classification("5", new double[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 }),
                new Classification("6", new double[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 }),
                new Classification("7", new double[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }),
                new Classification("8", new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 }),
                new Classification("9", new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 })
            };
            comboBox1.DataSource = classifications;
            comboBox1.DisplayMember = "targetLabel";
        }

        private void InitializeNN()
        {
            nn = new NeuralNetwork(new SigmoidActivationFunction(), new uint[] { 784, 128, 64, 10 });
            nn.LoadWeightsAndBiases("DigitRecognition_Task.txt");
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Draw(e.X, e.Y);
            }
        }

        private void Draw(int x, int y)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.Black), x, y, 10, 10);
            }
            pictureBox1.Image = bitmap;
        }

        private void ClearScreen()
        {
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;
            listBox1.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearScreen();
        }

        public Bitmap NormalizeAndCenterImage(Bitmap originalImage)
        {
            Rectangle boundingBox = DrawnSquare(bitmap);
            int newWidth = 20;
            int newHeight = 20;

            float ratio = Math.Min((float)newWidth / originalImage.Width, (float)newHeight / originalImage.Height);
            int resizedWidth = (int)(originalImage.Width * ratio);
            int resizedHeight = (int)(originalImage.Height * ratio);

            Bitmap resizedImage = new Bitmap(resizedWidth, resizedHeight);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap,
                            new Rectangle(0, 0, resizedWidth, resizedHeight),
                            boundingBox,
                            GraphicsUnit.Pixel);
            }

            Bitmap centeredImage = new Bitmap(28, 28);
            using (Graphics g = Graphics.FromImage(centeredImage))
            {
                g.Clear(Color.White);

                int offsetX = (28 - resizedWidth) / 2;
                int offsetY = (28 - resizedHeight) / 2;

                g.DrawImage(resizedImage, offsetX, offsetY);
            }

            centeredImage.Save("centeredImage.png");
            return centeredImage;
        }

        public double[] GetImage()
        {
            Bitmap processedImage = NormalizeAndCenterImage((Bitmap)bitmap.Clone());
            double[] pixels = new double[processedImage.Width * processedImage.Height];
            int index = 0;

            for (int y = 0; y < processedImage.Height; y++)
            {
                for (int x = 0; x < processedImage.Width; x++)
                {
                    Color pixelColor = processedImage.GetPixel(x, y);
                    pixels[index++] = pixelColor.GetBrightness() > 0.4 ? 0 : 1;
                    //pixels[index++] = pixelColor.ToArgb() == Color.White.ToArgb() ? 0 : 1;
                }
            }
            int white = pixels.Where(x => x == 0).Count();
            int black = pixels.Where(x => x == 1).Count();
            Print(pixels);
            return pixels;
        }

        private void Print(double[] pixels)
        {
            listBox1.Items.Clear();
            string str;
            for (int i = 0; i < 28; ++i)
            {
                str = "";
                for (int j = 0; j < 28; ++j)
                {
                    int idx = i * 28 + j;
                    if (pixels[idx] == 0)
                        str += "#";
                    else
                        str += "$";
                }
                listBox1.Items.Add(str);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            var img = GetImage();
            var result = nn.FeedForward(img);
            UpdateOutput(result);
            button2.Enabled = true;
        }

        private void UpdateOutput(double[] val)
        {
            int predictedClass = val.ToList().IndexOf(val.Max());
            for (int i = 0; i < val.Length; i++)
            {
                bars[i].Value = (int)(val[i] * 100);
            }
            resultLable.Text = $"Answer: {predictedClass}";
        }


        public Rectangle DrawnSquare(Bitmap bitmap)
        {
            var fromX = int.MaxValue;
            var toX = int.MinValue;
            var fromY = int.MaxValue;
            var toY = int.MinValue;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.ToArgb() == Color.Black.ToArgb())
                    {
                        if (x < fromX)
                            fromX = x;
                        if (x > toX)
                            toX = x;
                        if (y < fromY)
                            fromY = y;
                        if (y > toY)
                            toY = y;
                    }
                }
            }

            var dx = toX - fromX;
            var dy = toY - fromY;
            var side = Math.Max(dx, dy);
            if (dy > dx)
                fromX -= (side - dx) / 2;
            else
                fromY -= (side - dy) / 2;

            return new Rectangle(fromX, fromY, side + 1, side + 1);
        }

        public Bitmap CropToDigit()
        {
            Rectangle boundingBox = DrawnSquare(bitmap);
            Bitmap croppedBitmap = new Bitmap(boundingBox.Width, boundingBox.Height);

            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(bitmap,
                            new Rectangle(0, 0, boundingBox.Width, boundingBox.Height),
                            boundingBox,
                            GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            double[] outputData = ((Classification)comboBox1.SelectedItem).target;
            int numEpoch = Convert.ToInt32(textBox1.Text);
            double[] inputData = GetImage();
            nn.Train(inputData, outputData, numEpoch);
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fileName = $"NN-{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}.txt";
            nn.SaveWeightsAndBiases(fileName);
            MessageBox.Show($"FileName {fileName}", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}