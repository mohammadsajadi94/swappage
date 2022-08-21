using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace swappage
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private List<Stream> splittiff = new List<Stream>();
        private System.Drawing.Image imagetiff = null;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "TIF Files|*.tif|TIFF Files|*.tiff";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream streemtif = openFileDialog1.OpenFile();
                System.Drawing.Image imagetiff = System.Drawing.Image.FromStream(streemtif);
                int pagenum = imagetiff.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
                for (int i = 0; i < pagenum; i++)
                {
                    Stream test = new MemoryStream();
                    imagetiff.SelectActiveFrame(FrameDimension.Page, i);
                    imagetiff.Save(test, ImageFormat.Tiff);
                    splittiff.Add(test);
                    comboBoxfrom.Items.Add(i+1);
                    comboBoxto.Items.Add(i+1);
                }
                //splittiff=Move(splittiff, 2, 1);
              
                //foreach (var item in splittiff)
                //{
                    
                //}
                
                //TIFFDocument tifffile = new TIFFDocument(streemtif);
                //tifffile.SwapTwoPages(1, 2);
                //tifffile.Save(@"d:\swap.tif");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int oldIndex =(int) comboBoxfrom.SelectedItem-1;
            int newIndex = (int)comboBoxto.SelectedItem-1;
            var olditem = splittiff[oldIndex];
            var newitem = splittiff[newIndex];
            splittiff[oldIndex] = newitem;
            splittiff[newIndex] = olditem;
            for (int j = 0; j < splittiff.Count; j++)
            {
                imagetiff = System.Drawing.Image.FromStream(splittiff[j]);
                imagetiff.Save(@"d:\swap" + j + ".tiff");
            }
                // set the image codec
                ImageCodecInfo info = null;
                foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders())
                {
                    if (ice.MimeType == "image/tiff")
                    {
                        info = ice;
                        break;
                    }
                }

                EncoderParameters ep = new EncoderParameters(2);

                bool firstPage = true;

                System.Drawing.Image img = null;

                // create an image instance from the 1st image
                for (int nLoopfile = 0; nLoopfile < splittiff.Count; nLoopfile++)

                {
                    //get image from src file
                    System.Drawing.Image img_src = System.Drawing.Image.FromStream(splittiff[nLoopfile]);

                    Guid guid = img_src.FrameDimensionsList[0];
                    System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(guid);

                    //get the frames from src file
                    for (int nLoopFrame = 0; nLoopFrame < img_src.GetFrameCount(dimension); nLoopFrame++)
                    {
                        img_src.SelectActiveFrame(dimension, nLoopFrame);

                        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, Convert.ToInt32(EncoderValue.CompressionLZW));

                        // if first page, then create the initial image
                        if (firstPage)
                        {
                            img = img_src;

                            ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, Convert.ToInt32(EncoderValue.MultiFrame));
                            img.Save(@"d:\swapmulti.tiff", info, ep);

                            firstPage = false;
                            continue;
                        }

                        // add image to the next frame
                        ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, Convert.ToInt32(EncoderValue.FrameDimensionPage));
                        img.SaveAdd(img_src, ep);
                    }
                }

                ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, Convert.ToInt32(EncoderValue.Flush));
                img.SaveAdd(ep);
            }
    }
}
