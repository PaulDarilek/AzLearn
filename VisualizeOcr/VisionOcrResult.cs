using AzLearn.Core.Vision;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VisualizeOcr
{
    public partial class VisionOcrResult : Form
    {
        VisionOcrV3Result _visionResult;

        public VisionOcrResult()
        {
            InitializeComponent();
        }

        private void VisionOcrResult_Load(object sender, EventArgs e)
        {
            var dirInfo = new DirectoryInfo(@"..\..\..\..\..\");
            var imgFile = dirInfo
                .GetFiles("Form-request-for-separation-info.tiff", SearchOption.AllDirectories)
                .FirstOrDefault();

            if (imgFile != null)
            {
                pic.Image = Image.FromFile(imgFile.FullName);
                float scaleX = pic.Image.Width / Width;
                float scaleY = pic.Image.Height / Height;
                float scale = Math.Min(scaleX, scaleY);
                pic.Scale(new SizeF(scale, scale));

                var pages = pic.Image.GetFrameCount(FrameDimension.Page);
                udPage.Maximum = pages;
            }

            var resultFile = dirInfo
                .GetFiles("ProcessImageAsync__handwritten-request-for-separation-info.json", SearchOption.AllDirectories)
                .FirstOrDefault();
            if (resultFile != null)
            {
                var json = File.ReadAllText(resultFile.FullName);
                _visionResult = JsonConvert.DeserializeObject<VisionOcrV3Result>(json);
            }

            udPage.Value = 1;
            udLine.Value = 1;
            udWord.Value = 0;

            SetText();
        }

        private void udPage_ValueChanged(object sender, EventArgs e)
        {
            if (udPage.Value < 1)
            {
                udPage.Value = 1;
            }

            udLine.Value = 1;
            udWord.Value = 0;
       
            pic.Image.SelectActiveFrame(FrameDimension.Page, (int)udPage.Value -1);
            SetText();
        }

        private void udLine_ValueChanged(object sender, EventArgs e)
        {
            if (udLine.Value < 1)
                udLine.Value = 1;
            udWord.Value = 0;
            SetText();
        }

        private void udWord_ValueChanged(object sender, EventArgs e)
        {
            if (udWord.Value < 0)
                udLine.Value = 0;
            SetText();
        }

        private void SetText()
        {
            if (_visionResult?.AnalyzeResult?.ReadResults == null)
                return;

            var results = _visionResult.AnalyzeResult.ReadResults;
            
            var page =
                results.Where(x => x.Page == (int)udPage.Value).FirstOrDefault();
            if (page == null)
            {
                txtWord.Text = $"{lblPage.Text} OCR supports 1 - {results.Max(x => x.Page)}";
                pic.Refresh();
                return;
            }

            if (udLine.Value < 1 || udLine.Value > page.Lines.Length)
            {
                txtWord.Text = $"{lblLine.Text} should be 1 - {page.Lines.Length}";
                return;
            }

            LineV3 line = page?.Lines[(int)udLine.Value -1];

            if(udWord.Value < 1)
            {
                txtWord.Text = line.Text;
                DrawBox(line.BoundingBox);
                return;
            }

            if (udWord.Value > line.Words.Length)
            {
                txtWord.Text = $"{lblWord.Text} should be 0 - {line.Words.Length}";
                return;
            }

            WordV3 word = line.Words[(int)udWord.Value - 1];
            txtWord.Text = word.Text;
            DrawBox(word.BoundingBox);
        }

        private void DrawBox(decimal[] boundingBox)
        {
            var graphics = pic.CreateGraphics();
            int x = (int)boundingBox[0];
            int y = (int)boundingBox[1];

            // scroll top corner into place.
            panel1.VerticalScroll.Value = y > 16 ? y - 16 : y;
            panel1.HorizontalScroll.Value = x > 16 ? x - 16 : x;

            int width = (int)boundingBox[4] - x;
            int height = (int)boundingBox[5] - y;
            var rectangle = new Rectangle(x, y, width, height);

            pic.Refresh();
            Brush brush = Brushes.Red;
            graphics.DrawRectangle(new Pen(brush, (float)3.0), rectangle);
        }

    }
}
