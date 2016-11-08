using SPICA.PICA.Commands;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SPICA.PICA.Converters
{
    static class TextureConverter
    {
        private static int[] SwizzleLUT =
        {
             0,  1,  8,  9,  2,  3, 10, 11,
            16, 17, 24, 25, 18, 19, 26, 27,
             4,  5, 12, 13,  6,  7, 14, 15,
            20, 21, 28, 29, 22, 23, 30, 31,
            32, 33, 40, 41, 34, 35, 42, 43,
            48, 49, 56, 57, 50, 51, 58, 59,
            36, 37, 44, 45, 38, 39, 46, 47,
            52, 53, 60, 61, 54, 55, 62, 63
        };

        public static byte[] Decode(byte[] Input, int Width, int Height, PICATextureFormat Format, bool IsGL)
        {
            //Note: OpenGL reads texture from bottom to top, so we need to invert Y (see YMask variable)
            bool ETCAlpha = Format == PICATextureFormat.ETC1A4;

            if (Format == PICATextureFormat.ETC1 || ETCAlpha)
            {
                return TextureCompression.ETC1Decompress(Input, Width, Height, ETCAlpha, IsGL);
            }
            else
            {
                int YMask = IsGL ? Height - 1 : 0;

                byte[] Output = new byte[Width * Height * 4];

                int IOffs = 0;

                for (int TY = 0; TY < Height; TY += 8)
                {
                    for (int TX = 0; TX < Width; TX += 8)
                    {
                        for (int Px = 0; Px < 64; Px++)
                        {
                            int X = SwizzleLUT[Px] & 7;
                            int Y = (SwizzleLUT[Px] - X) >> 3;
                            int OOffs = (TX + X + (((TY + Y) ^ YMask) * Width)) * 4;

                            int Value;
                            byte R, G, B, A;

                            switch (Format)
                            {
                                case PICATextureFormat.RGBA8:
                                    Output[OOffs + 0] = Input[IOffs + 1];
                                    Output[OOffs + 1] = Input[IOffs + 2];
                                    Output[OOffs + 2] = Input[IOffs + 3];
                                    Output[OOffs + 3] = Input[IOffs + 0];

                                    IOffs += 4;
                                    break;

                                case PICATextureFormat.RGB8:
                                    Output[OOffs + 0] = Input[IOffs + 0];
                                    Output[OOffs + 1] = Input[IOffs + 1];
                                    Output[OOffs + 2] = Input[IOffs + 2];
                                    Output[OOffs + 3] = byte.MaxValue;

                                    IOffs += 3;
                                    break;

                                case PICATextureFormat.RGBA5551:
                                    Value = Input[IOffs + 0];
                                    Value |= Input[IOffs + 1] << 8;

                                    R = (byte)(((Value >> 1) & 0x1f) << 3);
                                    G = (byte)(((Value >> 6) & 0x1f) << 3);
                                    B = (byte)(((Value >> 11) & 0x1f) << 3);
                                    A = (byte)((Value & 1) * byte.MaxValue);

                                    Output[OOffs + 0] = (byte)(R | (R >> 5));
                                    Output[OOffs + 1] = (byte)(G | (G >> 5));
                                    Output[OOffs + 2] = (byte)(B | (B >> 5));
                                    Output[OOffs + 3] = A;

                                    IOffs += 2;
                                    break;

                                case PICATextureFormat.RGB565:
                                    Value = Input[IOffs + 0];
                                    Value |= Input[IOffs + 1] << 8;

                                    R = (byte)((Value & 0x1f) << 3);
                                    G = (byte)(((Value >> 5) & 0x3f) << 2);
                                    B = (byte)(((Value >> 11) & 0x1f) << 3);
                                    A = byte.MaxValue;

                                    Output[OOffs + 0] = (byte)(R | (R >> 5));
                                    Output[OOffs + 1] = (byte)(G | (G >> 5));
                                    Output[OOffs + 2] = (byte)(B | (B >> 5));
                                    Output[OOffs + 3] = A;

                                    IOffs += 2;
                                    break;

                                case PICATextureFormat.RGBA4:
                                    Value = Input[IOffs + 0];
                                    Value |= Input[IOffs + 1] << 8;

                                    R = (byte)((Value >> 4) & 0xf);
                                    G = (byte)((Value >> 8) & 0xf);
                                    B = (byte)((Value >> 12) & 0xf);
                                    A = (byte)(Value & 0xf);

                                    Output[OOffs + 0] = (byte)(R | (R << 4));
                                    Output[OOffs + 1] = (byte)(G | (G << 4));
                                    Output[OOffs + 2] = (byte)(B | (B << 4));
                                    Output[OOffs + 3] = (byte)(A | (A << 4));

                                    IOffs += 2;
                                    break;

                                case PICATextureFormat.LA8:
                                case PICATextureFormat.HiLo8:
                                    Output[OOffs + 0] = Input[IOffs];
                                    Output[OOffs + 1] = Input[IOffs];
                                    Output[OOffs + 2] = Input[IOffs];
                                    Output[OOffs + 3] = Input[IOffs + 1];

                                    IOffs += 2;
                                    break;

                                case PICATextureFormat.L8:
                                    Output[OOffs + 0] = Input[IOffs];
                                    Output[OOffs + 1] = Input[IOffs];
                                    Output[OOffs + 2] = Input[IOffs];
                                    Output[OOffs + 3] = byte.MaxValue;

                                    IOffs++;
                                    break;

                                case PICATextureFormat.A8:
                                    Output[OOffs + 0] = byte.MaxValue;
                                    Output[OOffs + 1] = byte.MaxValue;
                                    Output[OOffs + 2] = byte.MaxValue;
                                    Output[OOffs + 3] = Input[IOffs];

                                    IOffs++;
                                    break;

                                case PICATextureFormat.LA4:
                                    Output[OOffs + 0] = (byte)(Input[IOffs] >> 4);
                                    Output[OOffs + 1] = (byte)(Input[IOffs] >> 4);
                                    Output[OOffs + 2] = (byte)(Input[IOffs] >> 4);
                                    Output[OOffs + 3] = (byte)(Input[IOffs] & 0xf);

                                    IOffs++;
                                    break;

                                case PICATextureFormat.L4:
                                    Value = (Input[IOffs >> 1] >> ((IOffs & 1) != 0 ? 4 : 0)) & 0xf;

                                    Output[OOffs + 0] = (byte)((Value << 4) | Value);
                                    Output[OOffs + 1] = (byte)((Value << 4) | Value);
                                    Output[OOffs + 2] = (byte)((Value << 4) | Value);
                                    Output[OOffs + 3] = byte.MaxValue;

                                    IOffs++;
                                    break;

                                case PICATextureFormat.A4:
                                    Value = (Input[IOffs >> 1] >> ((IOffs & 1) != 0 ? 4 : 0)) & 0xf;

                                    Output[OOffs + 0] = byte.MaxValue;
                                    Output[OOffs + 1] = byte.MaxValue;
                                    Output[OOffs + 2] = byte.MaxValue;
                                    Output[OOffs + 3] = (byte)((Value << 4) | Value);

                                    IOffs++;
                                    break;

                                default: throw new ArgumentException("Invalid Texture format!");
                            }

                            if (IsGL)
                            {
                                byte Temp = Output[OOffs + 0];

                                Output[OOffs + 0] = Output[OOffs + 2];
                                Output[OOffs + 2] = Temp;
                            }
                        }
                    }
                }

                return Output;
            }
        }

        public static Bitmap Decode(byte[] Input, int Width, int Height, PICATextureFormat Format)
        {
            return GetBitmap(Decode(Input, Width, Height, Format, false), Width, Height);
        }

        public static byte[] Encode(Bitmap Img, PICATextureFormat Format)
        {
            byte[] Input = GetBuffer(Img);
            byte[] Output = new byte[CalculateLength(Img.Width, Img.Height, Format)];

            int OOffs = 0;

            if (Format == PICATextureFormat.ETC1 || Format == PICATextureFormat.ETC1A4)
            {
                throw new NotImplementedException();
            }
            else
            {
                for (int TY = 0; TY < Img.Height; TY += 8)
                {
                    for (int TX = 0; TX < Img.Width; TX += 8)
                    {
                        for (int Px = 0; Px < 64; Px++)
                        {
                            int X = SwizzleLUT[Px] & 7;
                            int Y = (SwizzleLUT[Px] - X) >> 3;
                            int IOffs = (TX + X + ((TY + Y) * Img.Width)) * 4;

                            switch (Format)
                            {
                                case PICATextureFormat.RGBA8:
                                    Output[OOffs + 0] = Input[IOffs + 3];
                                    Output[OOffs + 1] = Input[IOffs + 0];
                                    Output[OOffs + 2] = Input[IOffs + 1];
                                    Output[OOffs + 3] = Input[IOffs + 2];

                                    OOffs += 4;
                                    break;

                                default: throw new NotImplementedException();
                            }
                        }
                    }
                }
            }

            return Output;
        }

        public static int CalculateLength(int Width, int Height, PICATextureFormat Format)
        {
            int Length = Width * Height;

            switch (Format)
            {
                case PICATextureFormat.RGBA8: Length *= 4; break;
                case PICATextureFormat.RGB8: Length *= 3; break;
                case PICATextureFormat.RGBA5551:
                case PICATextureFormat.RGB565:
                case PICATextureFormat.RGBA4:
                case PICATextureFormat.LA8:
                case PICATextureFormat.HiLo8:
                    Length *= 2;
                    break;
                case PICATextureFormat.L4:
                case PICATextureFormat.A4:
                case PICATextureFormat.ETC1:
                    Length /= 2;
                    break;
            }

            if ((Length & 0x7f) != 0) Length = (Length & ~0x7f) + 0x80;

            return Length;
        }

        public static Bitmap GetBitmap(byte[] Buffer, int Width, int Height)
        {
            Rectangle Rect = new Rectangle(0, 0, Width, Height);

            Bitmap Img = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            BitmapData ImgData = Img.LockBits(Rect, ImageLockMode.WriteOnly, Img.PixelFormat);

            Marshal.Copy(Buffer, 0, ImgData.Scan0, Buffer.Length);

            Img.UnlockBits(ImgData);

            return Img;
        }

        public static byte[] GetBuffer(Bitmap Img)
        {
            Rectangle Rect = new Rectangle(0, 0, Img.Width, Img.Height);

            BitmapData ImgData = Img.LockBits(Rect, ImageLockMode.ReadOnly, Img.PixelFormat);

            byte[] Output = new byte[ImgData.Stride * Img.Height];

            Marshal.Copy(ImgData.Scan0, Output, 0, Output.Length);

            Img.UnlockBits(ImgData);

            return Output;
        }
    }
}
