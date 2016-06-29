using Ohana3DS_Rebirth.Ohana;
using Ohana3DS_Rebirth.Ohana.Models.NewLovePlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;


namespace Loadtexi
{
    class Program
    {
        static string filetype = ".png";
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (string Argument in args)
                {

                    if (Argument.ToLower().EndsWith(".texi"))
                    {
                        if (File.Exists(Argument))
                        {
                            if (File.Exists(Argument + ".xml"))
                            {
                                var a = loadTexture(Argument);
                                
                                if (a != null)
                                {
                                    var enc = GetEncoderInfo("image/png");
                                    EncoderParameters parameters = new EncoderParameters(2);
                                    parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                                    parameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionNone);

                                    a.texture.Save(Path.GetFullPath(Argument).Replace(".texi", filetype), enc, parameters);
                                    if (a.miptexture != null)
                                        foreach (Bitmap mip in a.miptexture)
                                        {
                                            mip.Save(Path.GetFullPath(Argument).Replace(".texi", ".mip" + mip.Width + "X" + mip.Height + filetype));
                                        }
                                }
                            }
                            else
                            {
                                Console.WriteLine("xml for file {0} not found", Path.GetFileNameWithoutExtension(Argument));
                            }
                        }
                    }
                    else if (Argument.EndsWith(filetype))
                    {
                        if (File.Exists(Argument))
                        {
                            if (File.Exists(Argument.Replace(filetype, ".texi.xml")))
                            {
                                var a = EncodeTexture(Argument);
                                if (a != null)
                                {
                                    File.WriteAllBytes(Path.GetFullPath(Argument).Replace(filetype, ".texi"), a);
                                    //a.texture.Save(Path.GetFullPath(Argument).Replace(".texi", "") + ".png");
                                }
                            }
                            else
                            {
                                Console.WriteLine("xml for file {0} not found", Path.GetFileNameWithoutExtension(Argument));
                            }
                        }
                    }
                }
            }
        }

        public static RenderBase.OTexture loadTexture(string fileName)
        {
            if (File.Exists(fileName + ".xml"))
            {
                Serialization.SERI tex = Serialization.getSERI(fileName + ".xml");

                int width = tex.getIntegerParameter("w");
                int height = tex.getIntegerParameter("h");
                int mipmap = tex.getIntegerParameter("mipmap");
                int format = tex.getIntegerParameter("format");
                string textureName = tex.getStringParameter("tex");
                string fullTextureName = fileName;

                if (File.Exists(fullTextureName))
                {
                    RenderBase.OTextureFormat fmt = RenderBase.OTextureFormat.dontCare;
                    switch (format)
                    {
                        case 0: fmt = RenderBase.OTextureFormat.l4; break;
                        case 1: fmt = RenderBase.OTextureFormat.l8; break;
                        case 3: fmt = RenderBase.OTextureFormat.la8; break;

                        case 7: fmt = RenderBase.OTextureFormat.rgb565; break;
                        case 8: fmt = RenderBase.OTextureFormat.rgba5551; break;
                        case 9: fmt = RenderBase.OTextureFormat.rgba4; break;
                        case 0xa: fmt = RenderBase.OTextureFormat.rgba8; break;
                        case 0xb: fmt = RenderBase.OTextureFormat.rgb8; break;
                        case 0xc: fmt = RenderBase.OTextureFormat.etc1; break;
                        case 0xd: fmt = RenderBase.OTextureFormat.etc1a4; break;
                        default: Debug.WriteLine("NLP Model: Unknown Texture format 0x" + format.ToString("X8")); break;
                    }

                    string name = Path.GetFileNameWithoutExtension(textureName);
                    byte[] buffer = File.ReadAllBytes(fullTextureName);
                    if (mipmap > 1)
                    {
                        int lenght = 0;
                        var texture = TextureCodec.decode(buffer, width, height, fmt, out lenght);
                        List<Bitmap> miptexture = new List<Bitmap>();
                        int lenghtall = lenght;
                        for (var i = 1; i < mipmap; i++)
                        {
                            var mip = new byte[lenghtall];
                            Buffer.BlockCopy(buffer, lenghtall, mip, 0, buffer.Length - lenghtall);
                            miptexture.Add(TextureCodec.decode(mip, width / Convert.ToInt32(Math.Pow(2, i)), height / Convert.ToInt32(Math.Pow(2, i)), fmt, out lenght));
                            lenghtall += lenght;
                        }

                        return new RenderBase.OTexture(texture, miptexture, name);
                    }
                    else {
                        int dataOffset;

                        //TextureCodec.encode(TextureCodec.decode(buffer, width, height, fmt, out dataOffset), fmt);
                        return new RenderBase.OTexture(TextureCodec.decode(buffer, width, height, fmt, out dataOffset), name);
                    }
                }
            }

            return null;
        }
        public static byte[] EncodeTexture(string fileName)
        {
            if (File.Exists(fileName))
            {

                if (File.Exists(fileName.Replace(filetype, ".texi.xml")))
                {
                    Serialization.SERI tex = Serialization.getSERI(fileName.Replace(filetype, ".texi.xml"));
                    int width = tex.getIntegerParameter("w");
                    int height = tex.getIntegerParameter("h");
                    int mipmap = tex.getIntegerParameter("mipmap");
                    int format = tex.getIntegerParameter("format");
                    string textureName = tex.getStringParameter("tex");
                    //string fullTextureName = Path.Combine(Path.GetDirectoryName(fileName), textureName);


                    RenderBase.OTextureFormat fmt = RenderBase.OTextureFormat.dontCare;
                    switch (format)
                    {
                        case 0: fmt = RenderBase.OTextureFormat.l4; break;
                        case 1: fmt = RenderBase.OTextureFormat.l8; break;
                        case 3: fmt = RenderBase.OTextureFormat.la8; break;
                        case 7: fmt = RenderBase.OTextureFormat.rgb565; break;
                        case 8: fmt = RenderBase.OTextureFormat.rgba5551; break;
                        case 9: fmt = RenderBase.OTextureFormat.rgba4; break;
                        case 0xa: fmt = RenderBase.OTextureFormat.rgba8; break;
                        case 0xb: fmt = RenderBase.OTextureFormat.rgb8; break;
                        case 0xc: fmt = RenderBase.OTextureFormat.etc1; break;
                        case 0xd: fmt = RenderBase.OTextureFormat.etc1a4; break;
                        default: Debug.WriteLine("NLP Model: Unknown Texture format 0x" + format.ToString("X8")); break;
                    }

                    Stream imageStreamSource = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TiffBitmapDecoder decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    int frame = 0; // adjust/ loop for multiframe tiffs
                    BitmapSource bitmapSource = decoder.Frames[0];

                    // this piece works for 8-bit grayscale. Adjust for other formats.
                    Bitmap img = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, PixelFormat.Format32bppArgb);
                    BitmapData data = img.LockBits(new Rectangle(System.Drawing.Point.Empty, img.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                    img.UnlockBits(data);
                    
                    IEnumerable<byte> main = TextureCodec.encode(img, fmt);

                    if (mipmap > 1)
                    {

                        for (var i = 1; i < mipmap; i++)
                        {
                            var mimefile = fileName.Replace(filetype, ".mip" + (img.Width / Convert.ToInt32(Math.Pow(2, i))).ToString() + "X" + (img.Height / Convert.ToInt32(Math.Pow(2, i))).ToString() + filetype);
                            if (File.Exists(mimefile))
                            {
                                Bitmap mip = new Bitmap(mimefile);
                                main = main.Concat(TextureCodec.encode(mip, fmt));
                                mip.Dispose();
                            }
                            else
                            {
                                Console.WriteLine(Path.GetFileName(mimefile) + " not found");
                            }
                        }
                    }
                    img.Dispose();
                    return main.ToArray();
                }
            }

            return null;
        }



        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
