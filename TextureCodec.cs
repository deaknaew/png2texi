using Ohana3DS_Rebirth.Ohana;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//to do png2bclm
namespace Loadtexi
{
    public class TextureCodec
    {
        private static int[] tileOrder = { 0, 1, 8, 9, 2, 3, 10, 11, 16, 17, 24, 25, 18, 19, 26, 27, 4, 5, 12, 13, 6, 7, 14, 15, 20, 21, 28, 29, 22, 23, 30, 31, 32, 33, 40, 41, 34, 35, 42, 43, 48, 49, 56, 57, 50, 51, 58, 59, 36, 37, 44, 45, 38, 39, 46, 47, 52, 53, 60, 61, 54, 55, 62, 63 };
        private static int[,] etc1LUT = { { 2, 8, -2, -8 }, { 5, 17, -5, -17 }, { 9, 29, -9, -29 }, { 13, 42, -13, -42 }, { 18, 60, -18, -60 }, { 24, 80, -24, -80 }, { 33, 106, -33, -106 }, { 47, 183, -47, -183 } };

        /// <summary>
        ///     Decodes a PICA200 Texture.
        /// </summary>
        /// <param name="data">Buffer with the Texture</param>
        /// <param name="width">Width of the Texture</param>
        /// <param name="height">Height of the Texture</param>
        /// <param name="format">Pixel Format of the Texture</param>
        /// <returns></returns>
        public static Bitmap decode(byte[] data, int Width, int Height, RenderBase.OTextureFormat format, out int dataOffset)
        {
            byte[] output = new byte[Width * Height * 4];
            dataOffset = 0;
            bool toggle = false;

            switch (format)
            {
                case RenderBase.OTextureFormat.rgba8:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + ((tY * 8 + y) * Width)) * 4;

                                Buffer.BlockCopy(data, (int)dataOffset + 1, output, (int)outputOffset, 3);
                                output[outputOffset + 3] = data[dataOffset];

                                dataOffset += 4;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.rgb8:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                Buffer.BlockCopy(data, (int)dataOffset, output, (int)outputOffset, 3);
                                output[outputOffset + 3] = 0xff;

                                dataOffset += 3;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.rgba5551:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                ushort pixelData = (ushort)(data[dataOffset] | (data[dataOffset + 1] << 8));

                                byte r = (byte)(((pixelData >> 1) & 0x1f) << 3);
                                byte g = (byte)(((pixelData >> 6) & 0x1f) << 3);
                                byte b = (byte)(((pixelData >> 11) & 0x1f) << 3);
                                byte a = (byte)((pixelData & 1) * 0xff);

                                output[outputOffset] = (byte)(r | (r >> 5));
                                output[outputOffset + 1] = (byte)(g | (g >> 5));
                                output[outputOffset + 2] = (byte)(b | (b >> 5));
                                output[outputOffset + 3] = a;

                                dataOffset += 2;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.rgb565:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                ushort pixelData = (ushort)(data[dataOffset] | (data[dataOffset + 1] << 8));

                                byte r = (byte)((pixelData & 0x1f) << 3);
                                byte g = (byte)(((pixelData >> 5) & 0x3f) << 2);
                                byte b = (byte)(((pixelData >> 11) & 0x1f) << 3);

                                output[outputOffset] = (byte)(r | (r >> 5));
                                output[outputOffset + 1] = (byte)(g | (g >> 6));
                                output[outputOffset + 2] = (byte)(b | (b >> 5));
                                output[outputOffset + 3] = 0xff;

                                dataOffset += 2;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.rgba4:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                ushort pixelData = (ushort)(data[dataOffset] | (data[dataOffset + 1] << 8));

                                byte r = (byte)((pixelData >> 4) & 0xf);
                                byte g = (byte)((pixelData >> 8) & 0xf);
                                byte b = (byte)((pixelData >> 12) & 0xf);
                                byte a = (byte)(pixelData & 0xf);

                                output[outputOffset] = (byte)(r | (r << 4));
                                output[outputOffset + 1] = (byte)(g | (g << 4));
                                output[outputOffset + 2] = (byte)(b | (b << 4));
                                output[outputOffset + 3] = (byte)(a | (a << 4));

                                dataOffset += 2;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.la8:
                case RenderBase.OTextureFormat.hilo8:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                output[outputOffset] = data[dataOffset];
                                output[outputOffset + 1] = data[dataOffset];
                                output[outputOffset + 2] = data[dataOffset];
                                output[outputOffset + 3] = data[dataOffset + 1];

                                dataOffset += 2;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.l8:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                output[outputOffset] = data[dataOffset];
                                output[outputOffset + 1] = data[dataOffset];
                                output[outputOffset + 2] = data[dataOffset];
                                output[outputOffset + 3] = 0xff;

                                dataOffset++;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.a8:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                output[outputOffset] = 0xff;
                                output[outputOffset + 1] = 0xff;
                                output[outputOffset + 2] = 0xff;
                                output[outputOffset + 3] = data[dataOffset];

                                dataOffset++;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.la4:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                output[outputOffset] = (byte)(data[dataOffset] >> 4);
                                output[outputOffset + 1] = (byte)(data[dataOffset] >> 4);
                                output[outputOffset + 2] = (byte)(data[dataOffset] >> 4);
                                output[outputOffset + 3] = (byte)(data[dataOffset] & 0xf);

                                dataOffset++;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.l4:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                byte c = toggle ? (byte)((data[dataOffset++] & 0xf0) >> 4) : (byte)(data[dataOffset] & 0xf);
                                toggle = !toggle;
                                c = (byte)((c << 4) | c);
                                output[outputOffset] = c;
                                output[outputOffset + 1] = c;
                                output[outputOffset + 2] = c;
                                output[outputOffset + 3] = 0xff;
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.a4:
                    for (int tY = 0; tY < Height / 8; tY++)
                    {
                        for (int tX = 0; tX < Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long outputOffset = ((tX * 8) + x + (((tY * 8 + y)) * Width)) * 4;

                                output[outputOffset] = 0xff;
                                output[outputOffset + 1] = 0xff;
                                output[outputOffset + 2] = 0xff;
                                byte a = toggle ? (byte)((data[dataOffset++] & 0xf0) >> 4) : (byte)(data[dataOffset] & 0xf);
                                toggle = !toggle;
                                output[outputOffset + 3] = (byte)((a << 4) | a);
                            }
                        }
                    }
                    break;

                case RenderBase.OTextureFormat.etc1:
                case RenderBase.OTextureFormat.etc1a4:
                    int offset;
                    byte[] decodedData = etc1Decode(data, Width, Height, format == RenderBase.OTextureFormat.etc1a4, out offset);
                    int[] etc1Order = etc1Scramble(Width, Height);

                    int i = 0;
                    for (int tY = 0; tY < Height / 4; tY++)
                    {
                        for (int tX = 0; tX < Width / 4; tX++)
                        {
                            int TX = etc1Order[i] % (Width / 4);
                            int TY = (etc1Order[i] - TX) / (Width / 4);
                            for (int y = 0; y < 4; y++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    dataOffset = ((TX * 4) + x + (((TY * 4) + y) * Width)) * 4;
                                    long outputOffset = ((tX * 4) + x + (((tY * 4 + y)) * Width)) * 4;

                                    Buffer.BlockCopy(decodedData, (int)dataOffset, output, (int)outputOffset, 4);
                                }
                            }
                            i += 1;
                        }
                    }
                    dataOffset = offset;
                    break;
            }

            return TextureUtils.getBitmap(output.ToArray(), Width, Height);
        }


        internal static Bitmap getIMG(byte[] data, int Width, int Height, RenderBase.OTextureFormat format, out int dataOffset)
        {
            dataOffset = 0;
            Bitmap img = new Bitmap(Width, Height);
            int area = img.Width * img.Height;
            // Tiles Per Width
            int p = gcm(img.Width, 8) / 8;
            if (p == 0) p = 1;
            using (Stream BitmapStream = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(BitmapStream))
                for (uint i = 0; i < area; i++) // for every pixel
                {
                    uint x;
                    uint y;
                    d2xy(i % 64, out x, out y);
                    uint tile = i / 64;

                    // Shift Tile Coordinate into Tilemap
                    x += (uint)(tile % p) * 8;
                    y += (uint)(tile / p) * 8;

                    // Get Color
                    Color c;
                    switch (format)
                    {
                        case RenderBase.OTextureFormat.l8:  // L8        // 8bit/1 byte
                        case RenderBase.OTextureFormat.a8:  // A8
                        case RenderBase.OTextureFormat.la4:  // LA4
                            c = DecodeColor(br.ReadByte(), format);
                            break;
                        case RenderBase.OTextureFormat.la8:  // LA8       // 16bit/2 byte
                        case RenderBase.OTextureFormat.hilo8:  // HILO8
                        case RenderBase.OTextureFormat.rgb565:  // RGB565
                        case RenderBase.OTextureFormat.rgba4:  // RGBA4444
                        case RenderBase.OTextureFormat.rgba5551:  // RGBA5551
                            c = DecodeColor(br.ReadUInt16(), format);
                            break;
                        case RenderBase.OTextureFormat.rgb8:  // RGB8:     // 24bit
                            byte[] data1 = br.ReadBytes(3); Array.Resize(ref data1, 4);
                            c = DecodeColor(BitConverter.ToUInt32(data1, 0), format);
                            break;
                        case RenderBase.OTextureFormat.rgba8:  // RGBA8888
                            c = DecodeColor(br.ReadUInt32(), format);
                            break;
                        case RenderBase.OTextureFormat.l4:  // L4
                        case RenderBase.OTextureFormat.a4:  // A4        // 4bit - Do 2 pixels at a time.
                            uint val = br.ReadByte();
                            img.SetPixel((int)x, (int)y, DecodeColor(val & 0xF, format)); // lowest bits for the low pixel
                            i++; x++;
                            c = DecodeColor(val >> 4, format);   // highest bits for the high pixel
                            break;
                        default: throw new Exception("Invalid FileFormat.");
                    }
                    img.SetPixel((int)x, (int)y, c);
                    dataOffset  = (int)br.BaseStream.Position;
                }

            return img;
        }
        /// <summary>
        ///     Encodes a PICA200 Texture.
        /// </summary>
        /// <param name="img">Input image to be encoded</param>
        /// <param name="format">Pixel Format of the Texture</param>
        /// <returns></returns>
        /// 
        public static byte[] encode(Bitmap img, RenderBase.OTextureFormat format)
        {
            byte[] data = TextureUtils.getArray(img);
            byte[] output = new byte[data.Length];
            bool toggle = false;
            uint outputOffset = 0;
            switch (format)
            {
                case RenderBase.OTextureFormat.rgba8:
                    for (int tY = 0; tY < img.Height / 8; tY++)
                    {
                        for (int tX = 0; tX < img.Width / 8; tX++)
                        {
                            for (int pixel = 0; pixel < 64; pixel++)
                            {
                                int x = tileOrder[pixel] % 8;
                                int y = (tileOrder[pixel] - x) / 8;
                                long dataOffset = ((tX * 8) + x + ((tY * 8 + y) * img.Width)) * 4;

                                Buffer.BlockCopy(data, (int)dataOffset, output, (int)outputOffset + 1, 3);
                                output[outputOffset] = data[dataOffset + 3];

                                outputOffset += 4;
                            }
                        }
                    }
                    break;
                case RenderBase.OTextureFormat.rgb8:
                    {
                        output = new byte[img.Width * img.Height * 3];
                        for (int tY = 0; tY < img.Height / 8; tY++)
                        {
                            for (int tX = 0; tX < img.Width / 8; tX++)
                            {
                                for (int pixel = 0; pixel < 64; pixel++)
                                {
                                    int x = tileOrder[pixel] % 8;
                                    int y = (tileOrder[pixel] - x) / 8;
                                    long dataOffset = ((tX * 8) + x + (((tY * 8 + y)) * img.Width)) * 4;
                                    if (data[dataOffset + 3] != 0xFF)
                                    {

                                    }
                                    Buffer.BlockCopy(data, (int)dataOffset, output, (int)outputOffset, 3);

                                    outputOffset += 3;
                                }
                            }
                        }
                    }
                    break;
                case RenderBase.OTextureFormat.rgba5551:
                    {
                        output = new byte[img.Width * img.Height * 2];
                        for (int tY = 0; tY < img.Height / 8; tY++)
                        {
                            for (int tX = 0; tX < img.Width / 8; tX++)
                            {
                                for (int pixel = 0; pixel < 64; pixel++)
                                {
                                    int x = tileOrder[pixel] % 8;
                                    int y = (tileOrder[pixel] - x) / 8;
                                    long dataOffset = ((tX * 8) + x + (((tY * 8 + y)) * img.Width)) * 4;
                                    int r = (data[dataOffset + 0] * 31 / 255);
                                    int g = (data[dataOffset + 1] * 31 / 255);
                                    int b = (data[dataOffset + 2] * 31 / 255);
                                    int a = (data[dataOffset + 3] > 0 ? 1 : 0);
                                    ushort rShift = (ushort)(r << 1);
                                    ushort gShift = (ushort)(g << 6);
                                    ushort bShift = (ushort)(b << 11);
                                    ushort t = (ushort)(rShift | gShift | bShift | a);
                                    output[outputOffset] = (byte)(t);
                                    output[outputOffset + 1] = (byte)(t >> 8);




                                    outputOffset += 2;
                                }
                            }
                        }
                    }
                    break;

                default: throw new NotImplementedException();
            }

            return output;
        }
        public static byte[] getPixelData(Bitmap img, RenderBase.OTextureFormat format)
        {
            int w = img.Width;
            int h = img.Height;


            using (MemoryStream mz = new MemoryStream())
            using (BinaryWriter bz = new BinaryWriter(mz))
            {
                int p = gcm(w, 8) / 8;
                if (p == 0) p = 1;
                for (uint i = 0; i < w * h; i++)
                {
                    uint x;
                    uint y;
                    d2xy(i % 64, out x, out y);

                    // Get Shift Tile
                    uint tile = i / 64;

                    // Shift Tile Coordinate into Tilemap
                    x += (uint)(tile % p) * 8;
                    y += (uint)(tile / p) * 8;

                    // Don't write data
                    Color c;
                    if (x >= img.Width || y >= img.Height)
                    { c = Color.FromArgb(0, 0, 0, 0); }
                    else
                    { c = img.GetPixel((int)x, (int)y);  }

                    switch (format)
                    {
                        case RenderBase.OTextureFormat.l8: bz.Write(GetL8(c)); break;                // L8
                        case RenderBase.OTextureFormat.a8: bz.Write(GetA8(c)); break;                // A8
                        case RenderBase.OTextureFormat.la4: bz.Write(GetLA4(c)); break;               // LA4(4)
                        case RenderBase.OTextureFormat.la8: bz.Write(GetLA8(c)); break;             // LA8(8)
                        case RenderBase.OTextureFormat.hilo8: bz.Write(GetHILO8(c)); break;           // HILO8
                        case RenderBase.OTextureFormat.rgb565: bz.Write(GetRGB565(c)); break;          // RGB565
                        case RenderBase.OTextureFormat.rgba5551: bz.Write(GetRGBA5551(c)); break;        // RGBA5551
                        case RenderBase.OTextureFormat.rgba4: bz.Write(GetRGBA4444(c)); break;        // RGBA4444
                        case RenderBase.OTextureFormat.rgba8: bz.Write(GetRGBA8888(c)); break;          // RGBA8
                        case RenderBase.OTextureFormat.rgb8: bz.Write(GetRGB8(c)); break;          // RGB8
                        default: throw new Exception("Unsupport format.");
                    }
                }
                //while (mz.Length < nlpo2((int)mz.Length)) // pad
                //    bz.Write((byte)0);
                return mz.ToArray();

            }
        }
        internal static int[] Convert5To8 = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
                                              0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
                                              0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
                                              0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };

        internal static Color DecodeColor(uint val, RenderBase.OTextureFormat format)
        {
            int alpha = 0xFF, red, green, blue;
            switch (format)
            {
                case RenderBase.OTextureFormat.l8: // L8
                    return Color.FromArgb(alpha, (byte)val, (byte)val, (byte)val);
                case RenderBase.OTextureFormat.a8: // A8
                    return Color.FromArgb((byte)val, alpha, alpha, alpha);
                case RenderBase.OTextureFormat.la4: // LA4
                    red = (byte)(val >> 4);
                    alpha = (byte)(val & 0x0F);
                    return Color.FromArgb(alpha, red, red, red);
                case RenderBase.OTextureFormat.la8: // LA8
                    red = (byte)((val >> 8 & 0xFF));
                    alpha = (byte)(val & 0xFF);
                    return Color.FromArgb(alpha, red, red, red);
                case RenderBase.OTextureFormat.hilo8: // HILO8
                    red = (byte)(val >> 8);
                    green = (byte)(val & 0xFF);
                    return Color.FromArgb(alpha, red, green, 0xFF);
                case RenderBase.OTextureFormat.rgb565: // RGB565
                    red = Convert5To8[(val >> 11) & 0x1F];
                    green = (byte)(((val >> 5) & 0x3F) * 4);
                    blue = Convert5To8[val & 0x1F];
                    return Color.FromArgb(alpha, red, green, blue);
                case RenderBase.OTextureFormat.rgb8: // RGB8
                    red = (byte)((val >> 16) & 0xFF);
                    green = (byte)((val >> 8) & 0xFF);
                    blue = (byte)(val & 0xFF);
                    return Color.FromArgb(alpha, red, green, blue);
                case RenderBase.OTextureFormat.rgba5551: // RGBA5551
                    red = Convert5To8[(val >> 11) & 0x1F];
                    green = Convert5To8[(val >> 6) & 0x1F];
                    blue = Convert5To8[(val >> 1) & 0x1F];
                    alpha = (val & 0x0001) == 1 ? 0xFF : 0x00;
                    return Color.FromArgb(alpha, red, green, blue);
                case RenderBase.OTextureFormat.rgba4: // RGBA4444
                    alpha = (byte)(0x11 * (val & 0xf));
                    red = (byte)(0x11 * ((val >> 12) & 0xf));
                    green = (byte)(0x11 * ((val >> 8) & 0xf));
                    blue = (byte)(0x11 * ((val >> 4) & 0xf));
                    return Color.FromArgb(alpha, red, green, blue);
                case RenderBase.OTextureFormat.rgba8: // RGBA8888
                    red = (byte)((val >> 24) & 0xFF);
                    green = (byte)((val >> 16) & 0xFF);
                    blue = (byte)((val >> 8) & 0xFF);
                    alpha = (byte)(val & 0xFF);
                    return Color.FromArgb(alpha, red, green, blue);
                // case 10:
                // case 11:
                case RenderBase.OTextureFormat.l4: // L4
                    return Color.FromArgb(alpha, (byte)(val * 0x11), (byte)(val * 0x11), (byte)(val * 0x11));
                case RenderBase.OTextureFormat.a4: // A4
                    return Color.FromArgb((byte)(val * 0x11), alpha, alpha, alpha);
                default:
                    return Color.White;
            }
        }

        /// <summary>
        /// Greatest common multiple (to round up)
        /// </summary>
        /// <param name="n">Number to round-up.</param>
        /// <param name="m">Multiple to round-up to.</param>
        /// <returns>Rounded up number.</returns>
        internal static int gcm(int n, int m)
        {
            return ((n + m - 1) / m) * m;
        }

        /// <summary>
        /// Next Largest Power of 2
        /// </summary>
        /// <param name="x">Input to round up to next 2^n</param>
        /// <returns>2^n > x && x > 2^(n-1) </returns>
        internal static int nlpo2(int x)
        {
            x--; // comment out to always take the next biggest power of two, even if x is already a power of two
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return (x + 1);
        }

        // Morton Translation
        /// <summary>
        /// Combines X/Y Coordinates to a decimal ordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal static uint xy2d(uint x, uint y)
        {
            x &= 0x0000ffff;
            y &= 0x0000ffff;
            x |= (x << 8);
            y |= (y << 8);
            x &= 0x00ff00ff;
            y &= 0x00ff00ff;
            x |= (x << 4);
            y |= (y << 4);
            x &= 0x0f0f0f0f;
            y &= 0x0f0f0f0f;
            x |= (x << 2);
            y |= (y << 2);
            x &= 0x33333333;
            y &= 0x33333333;
            x |= (x << 1);
            y |= (y << 1);
            x &= 0x55555555;
            y &= 0x55555555;
            return x | (y << 1);
        }

        /// <summary>
        /// Decimal Ordinate In to X / Y Coordinate Out
        /// </summary>
        /// <param name="d">Loop integer which will be decoded to X/Y</param>
        /// <param name="x">Output X coordinate</param>
        /// <param name="y">Output Y coordinate</param>
        internal static void d2xy(uint d, out uint x, out uint y)
        {
            x = d;
            y = (x >> 1);
            x &= 0x55555555;
            y &= 0x55555555;
            x |= (x >> 1);
            y |= (y >> 1);
            x &= 0x33333333;
            y &= 0x33333333;
            x |= (x >> 2);
            y |= (y >> 2);
            x &= 0x0f0f0f0f;
            y &= 0x0f0f0f0f;
            x |= (x >> 4);
            y |= (y >> 4);
            x &= 0x00ff00ff;
            y &= 0x00ff00ff;
            x |= (x >> 8);
            y |= (y >> 8);
            x &= 0x0000ffff;
            y &= 0x0000ffff;
        }
        // Color Conversion
        internal static byte GetL8(Color c)
        {
            byte red = c.R;
            byte green = c.G;
            byte blue = c.B;
            // Luma (Y’) = 0.299 R’ + 0.587 G’ + 0.114 B’ from wikipedia
            return (byte)(((0x4CB2 * red + 0x9691 * green + 0x1D3E * blue) >> 16) & 0xFF);
        }        // L8
        internal static byte GetA8(Color c)
        {
            return c.A;
        }        // A8
        internal static byte GetLA4(Color c)
        {
            return (byte)((c.A / 0x11) + (c.R / 0x11) << 4);
        }       // LA4
        internal static ushort GetLA8(Color c)
        {
            return (ushort)((c.A) + ((c.R) << 8));
        }     // LA8
        internal static ushort GetHILO8(Color c)
        {
            return (ushort)((c.G) + ((c.R) << 8));
        }   // HILO8
        internal static ushort GetRGB565(Color c)
        {
            int val = 0;
            // val += c.A >> 8; // unused
            val += convert8to5(c.B) >> 3;
            val += (c.G >> 2) << 5;
            val += convert8to5(c.R) << 10;
            return (ushort)val;
        }  // RGB565
        // RGB8
        internal static ushort GetRGBA5551(Color c)
        {
            int val = 0;
            val += (byte)(c.A > 0x80 ? 1 : 0);
            val += convert8to5(c.R) << 11;
            val += convert8to5(c.G) << 6;
            val += convert8to5(c.B) << 1;
            ushort v = (ushort)val;

            return v;
        }// RGBA5551
        internal static ushort GetRGBA4444(Color c)
        {
            int val = 0;
            val += (c.A / 0x11);
            val += ((c.B / 0x11) << 4);
            val += ((c.G / 0x11) << 8);
            val += ((c.R / 0x11) << 12);
            return (ushort)val;
        }// RGBA4444
        internal static uint GetRGBA8888(Color c)     // RGBA8888
        {
            uint val = 0;
            val += c.A;
            val += (uint)(c.B << 8);
            val += (uint)(c.G << 16);
            val += (uint)(c.R << 24);
            return val;
        }
        internal static byte[] GetRGB8(Color c)     // RGBA8888
        {
            byte[] val = new byte[3];
            val[2] = c.R;
            val[1] = c.G;
            val[0] = c.B;
            //uint val = 0;
            //val += (uint)(c.B);
            //val += (uint)(c.G << 8);
            //val += (uint)(c.R << 16);
            return val;
        }

        // Unit Conversion
        internal static byte convert8to5(int colorval)
        {

            byte[] Convert8to5 = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
                                   0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
                                   0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
                                   0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };
            byte i = 0;
            while (colorval > Convert8to5[i]) i++;
            return i;
        }

        #region "ETC1"
        private static byte[] etc1Decode(byte[] input, int width, int height, bool alpha, out int offset)
        {
            byte[] output = new byte[(width * height * 4)];
            offset = 0;

            for (int y = 0; y < height / 4; y++)
            {
                for (int x = 0; x < width / 4; x++)
                {
                    byte[] colorBlock = new byte[8];
                    byte[] alphaBlock = new byte[8];
                    if (alpha)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            colorBlock[7 - i] = input[offset + 8 + i];
                            alphaBlock[i] = input[offset + i];
                        }
                        offset += 16;
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            colorBlock[7 - i] = input[offset + i];
                            alphaBlock[i] = 0xff;
                        }
                        offset += 8;
                    }

                    colorBlock = etc1DecodeBlock(colorBlock);

                    bool toggle = false;
                    long alphaOffset = 0;
                    for (int tX = 0; tX < 4; tX++)
                    {
                        for (int tY = 0; tY < 4; tY++)
                        {
                            int outputOffset = (x * 4 + tX + ((y * 4 + tY) * width)) * 4;
                            int blockOffset = (tX + (tY * 4)) * 4;
                            Buffer.BlockCopy(colorBlock, blockOffset, output, outputOffset, 3);

                            byte a = toggle ? (byte)((alphaBlock[alphaOffset++] & 0xf0) >> 4) : (byte)(alphaBlock[alphaOffset] & 0xf);
                            output[outputOffset + 3] = (byte)((a << 4) | a);
                            toggle = !toggle;
                        }
                    }
                }
            }

            return output;
        }

        private static byte[] etc1DecodeBlock(byte[] data)
        {
            uint blockTop = BitConverter.ToUInt32(data, 0);
            uint blockBottom = BitConverter.ToUInt32(data, 4);

            bool flip = (blockTop & 0x1000000) > 0;
            bool difference = (blockTop & 0x2000000) > 0;

            uint r1, g1, b1;
            uint r2, g2, b2;

            if (difference)
            {
                r1 = blockTop & 0xf8;
                g1 = (blockTop & 0xf800) >> 8;
                b1 = (blockTop & 0xf80000) >> 16;

                r2 = (uint)((sbyte)(r1 >> 3) + ((sbyte)((blockTop & 7) << 5) >> 5));
                g2 = (uint)((sbyte)(g1 >> 3) + ((sbyte)((blockTop & 0x700) >> 3) >> 5));
                b2 = (uint)((sbyte)(b1 >> 3) + ((sbyte)((blockTop & 0x70000) >> 11) >> 5));

                r1 |= r1 >> 5;
                g1 |= g1 >> 5;
                b1 |= b1 >> 5;

                r2 = (r2 << 3) | (r2 >> 2);
                g2 = (g2 << 3) | (g2 >> 2);
                b2 = (b2 << 3) | (b2 >> 2);
            }
            else
            {
                r1 = blockTop & 0xf0;
                g1 = (blockTop & 0xf000) >> 8;
                b1 = (blockTop & 0xf00000) >> 16;

                r2 = (blockTop & 0xf) << 4;
                g2 = (blockTop & 0xf00) >> 4;
                b2 = (blockTop & 0xf0000) >> 12;

                r1 |= r1 >> 4;
                g1 |= g1 >> 4;
                b1 |= b1 >> 4;

                r2 |= r2 >> 4;
                g2 |= g2 >> 4;
                b2 |= b2 >> 4;
            }

            uint table1 = (blockTop >> 29) & 7;
            uint table2 = (blockTop >> 26) & 7;

            byte[] output = new byte[(4 * 4 * 4)];
            if (!flip)
            {
                for (int y = 0; y <= 3; y++)
                {
                    for (int x = 0; x <= 1; x++)
                    {
                        Color color1 = etc1Pixel(r1, g1, b1, x, y, blockBottom, table1);
                        Color color2 = etc1Pixel(r2, g2, b2, x + 2, y, blockBottom, table2);

                        int offset1 = (y * 4 + x) * 4;
                        output[offset1] = color1.B;
                        output[offset1 + 1] = color1.G;
                        output[offset1 + 2] = color1.R;

                        int offset2 = (y * 4 + x + 2) * 4;
                        output[offset2] = color2.B;
                        output[offset2 + 1] = color2.G;
                        output[offset2 + 2] = color2.R;
                    }
                }
            }
            else
            {
                for (int y = 0; y <= 1; y++)
                {
                    for (int x = 0; x <= 3; x++)
                    {
                        Color color1 = etc1Pixel(r1, g1, b1, x, y, blockBottom, table1);
                        Color color2 = etc1Pixel(r2, g2, b2, x, y + 2, blockBottom, table2);

                        int offset1 = (y * 4 + x) * 4;
                        output[offset1] = color1.B;
                        output[offset1 + 1] = color1.G;
                        output[offset1 + 2] = color1.R;

                        int offset2 = ((y + 2) * 4 + x) * 4;
                        output[offset2] = color2.B;
                        output[offset2 + 1] = color2.G;
                        output[offset2 + 2] = color2.R;
                    }
                }
            }

            return output;
        }

        private static Color etc1Pixel(uint r, uint g, uint b, int x, int y, uint block, uint table)
        {
            int index = x * 4 + y;
            uint MSB = block << 1;

            int pixel = index < 8
                ? etc1LUT[table, ((block >> (index + 24)) & 1) + ((MSB >> (index + 8)) & 2)]
                : etc1LUT[table, ((block >> (index + 8)) & 1) + ((MSB >> (index - 8)) & 2)];

            r = saturate((int)(r + pixel));
            g = saturate((int)(g + pixel));
            b = saturate((int)(b + pixel));

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        private static byte saturate(int value)
        {
            if (value > 0xff) return 0xff;
            if (value < 0) return 0;
            return (byte)(value & 0xff);
        }

        private static int[] etc1Scramble(int width, int height)
        {
            //Maybe theres a better way to do this?
            int[] tileScramble = new int[((width / 4) * (height / 4))];
            int baseAccumulator = 0;
            int rowAccumulator = 0;
            int baseNumber = 0;
            int rowNumber = 0;

            for (int tile = 0; tile < tileScramble.Length; tile++)
            {
                if ((tile % (width / 4) == 0) && tile > 0)
                {
                    if (rowAccumulator < 1)
                    {
                        rowAccumulator += 1;
                        rowNumber += 2;
                        baseNumber = rowNumber;
                    }
                    else
                    {
                        rowAccumulator = 0;
                        baseNumber -= 2;
                        rowNumber = baseNumber;
                    }
                }

                tileScramble[tile] = baseNumber;

                if (baseAccumulator < 1)
                {
                    baseAccumulator++;
                    baseNumber++;
                }
                else
                {
                    baseAccumulator = 0;
                    baseNumber += 3;
                }
            }

            return tileScramble;
        }
        #endregion
    }
}
