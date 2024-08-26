using BitMiracle.LibTiff.Classic;
using Boz.Services.Contracts.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Maxi.Services.SouthSide.SouthsideFile
{
    public class ImageUtils
    {
        public static byte[] GetTiffImageBytes(Bitmap img, bool byScanlines)
        {
            try
            {
                img = ApplyInvert(img);
                byte[] imageRasterBytes = GetImageRasterBytes(img);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (Tiff tiff = Tiff.ClientOpen("InMemory", "w", memoryStream, new TiffStream()))
                    {
                        if (tiff == null)
                            return null;
                        tiff.SetField(TiffTag.IMAGEWIDTH, (object)img.Width);
                        tiff.SetField(TiffTag.IMAGELENGTH, (object)img.Height);
                        tiff.SetField(TiffTag.COMPRESSION, (object)Compression.CCITTFAX4);
                        tiff.SetField(TiffTag.PHOTOMETRIC, (object)Photometric.MINISWHITE);
                        tiff.SetField(TiffTag.THRESHHOLDING, (object)1);
                        tiff.SetField(TiffTag.ROWSPERSTRIP, (object)img.Height);
                        tiff.SetField(TiffTag.XRESOLUTION, (object)img.HorizontalResolution);
                        tiff.SetField(TiffTag.YRESOLUTION, (object)img.VerticalResolution);
                        tiff.SetField(TiffTag.SUBFILETYPE, (object)0);
                        tiff.SetField(TiffTag.BITSPERSAMPLE, (object)1);
                        tiff.SetField(TiffTag.FILLORDER, (object)FillOrder.MSB2LSB);
                        tiff.SetField(TiffTag.ORIENTATION, (object)Orientation.TOPLEFT);
                        tiff.SetField(TiffTag.SAMPLESPERPIXEL, (object)1);
                        tiff.SetField(TiffTag.GROUP4OPTIONS, (object)0);
                        tiff.SetField(TiffTag.RESOLUTIONUNIT, (object)ResUnit.INCH);
                        tiff.CheckpointDirectory();
                        int num = tiff.ScanlineSize();
                        int num2 = imageRasterBytes.Length / img.Height;
                        if (byScanlines)
                        {
                            int i = 0;
                            int num3 = 0;
                            for (; i < img.Height; i++)
                            {
                                if (!tiff.WriteScanline(imageRasterBytes, num3, i, 0))
                                {
                                    return null;
                                }
                                num3 += num2;
                            }
                        }
                        else if (num < num2)
                        {
                            byte[] array = new byte[num * img.Height];
                            int j = 0;
                            int num4 = 0;
                            int num5 = 0;
                            for (; j < img.Height; j++)
                            {
                                Buffer.BlockCopy(imageRasterBytes, num4, array, num5, num);
                                num4 += num2;
                                num5 += num;
                            }
                            if (tiff.WriteEncodedStrip(0, array, array.Length) <= 0)
                            {
                                return null;
                            }
                        }
                        else if (tiff.WriteEncodedStrip(0, imageRasterBytes, imageRasterBytes.Length) <= 0)
                        {
                            return null;
                        }
                    }
                    return memoryStream.GetBuffer();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] GetImageRasterBytes(Bitmap img)
        {
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            Bitmap bitmap = img;
            try
            {
                BitmapData bitmapData = img.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
                byte[] array = new byte[bitmapData.Stride * bitmapData.Height];
                Marshal.Copy(bitmapData.Scan0, array, 0, array.Length);
                img.UnlockBits(bitmapData);
                return array;
            }
            finally
            {
                if (bitmap != img)
                    bitmap.Dispose();
            }
        }

        public static Bitmap ApplyInvert(Bitmap bitmapImage)
        {
            // Bloque de datos de la imagen original
            Rectangle rect = new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height);
            BitmapData originalData = bitmapImage.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Obtener el tamaño de la imagen en bytes
            int bytes = originalData.Stride * originalData.Height;

            // Crear un arreglo de bytes para almacenar los datos de píxeles
            byte[] pixelBuffer = new byte[bytes];
            Marshal.Copy(originalData.Scan0, pixelBuffer, 0, bytes);

            // Invertir los colores de los píxeles en el arreglo de bytes
            for (int i = 0; i < bytes; i += 4)
            {
                pixelBuffer[i] = (byte)(255 - pixelBuffer[i]);       // Componente azul
                pixelBuffer[i + 1] = (byte)(255 - pixelBuffer[i + 1]); // Componente verde
                pixelBuffer[i + 2] = (byte)(255 - pixelBuffer[i + 2]); // Componente rojo
                                                                       // El componente alfa (transparencia) permanece sin cambios
            }

            // Copiar los datos invertidos de vuelta a la imagen original
            Marshal.Copy(pixelBuffer, 0, originalData.Scan0, bytes);

            // Liberar el bloque de datos de la imagen original
            bitmapImage.UnlockBits(originalData);
            return bitmapImage;
        }

        public static void DrawTellerSpray(Bitmap currentBitmapJpg, string tellerSpray)
        {
            int num = 200;
            int num2 = 60;
            Graphics graphics = Graphics.FromImage(currentBitmapJpg);
            StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            graphics.DrawString(tellerSpray, new Font("Arial Black", 30f, GraphicsUnit.Pixel), Brushes.Black, new RectangleF((currentBitmapJpg.Width - num) / 2, (currentBitmapJpg.Height - num2) / 2, num, num2), stringFormat);
        }

        public static void DrawAgentName(Bitmap currentBitmapJpg, string imageDataAgentName)
        {
            int num = 800;
            int num2 = 30;
            Graphics graphics = Graphics.FromImage(currentBitmapJpg);
            StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            graphics.DrawString(imageDataAgentName, new Font("Arial Black", 20f, GraphicsUnit.Pixel), Brushes.Black, new RectangleF(0f, 650f, num, num2), stringFormat);
        }

        public static void DrawReclear(Bitmap currentBitmapJpg)
        {
            using (Graphics graphics = Graphics.FromImage(currentBitmapJpg))
            {
                SizeF sizeF = graphics.MeasureString("RE-CLEAR", new Font("Arial Narrow", ConstantsSendService.FontSizeReclear));
                SizeF size = new SizeF(sizeF.Width / 3f, sizeF.Height / 4f);
                PointF location = new PointF(currentBitmapJpg.Width > 0 ? currentBitmapJpg.Width - currentBitmapJpg.Width / 2 - size.Width / 2f : 0.0f, currentBitmapJpg.Height > 0 ? currentBitmapJpg.Height - currentBitmapJpg.Height / 3 : 0.0f);
                RectangleF rect = new RectangleF(location, size);
                graphics.FillRectangle(Brushes.White, rect);
                graphics.DrawString("RE-CLEAR", new Font("Arial Narrow", ConstantsSendService.FontSizeReclear, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, new PointF(location.X - 10f, location.Y - 10f));
                graphics.Dispose();
            }
        }

        public static void DrawResume(Bitmap currentBitmapJpg, string imageDataResume)
        {
            int num = 600;
            int num2 = 90;
            Graphics graphics = Graphics.FromImage(currentBitmapJpg);
            StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
            graphics.DrawString(imageDataResume, new Font("Consolas", 25f, GraphicsUnit.Pixel), Brushes.Black, new RectangleF(1050f, 620f, num, num2), stringFormat);
        }

        public static void DrawCheckDetail(
          Bitmap currentBitmapJpg,
          String imageDataCheckDetail,
          bool isFront)
        {
            int num = 390;
            int num2 = 570;
            int num3 = 437;
            int num4 = imageDataCheckDetail.Length / num3 + ((imageDataCheckDetail.Length % num3 != 0) ? 1 : 0);
            for (int i = 1; i <= num4; i++)
            {
                if ((isFront && i <= 4) || (!isFront && i >= 5))
                {
                    string s = imageDataCheckDetail.Substring((i - 1) * num3, ((i - 1) * num3 + num3 > imageDataCheckDetail.Length) ? (imageDataCheckDetail.Length - (i - 1) * num3) : num3);
                    Graphics graphics = Graphics.FromImage(currentBitmapJpg);
                    StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Near;
                    graphics.DrawString(s, new Font("Consolas", 25f, GraphicsUnit.Pixel), Brushes.Black, new RectangleF(150 + (i - (isFront ? 1 : 5)) * num, 50f, num, num2), stringFormat);
                }
            }
        }
    }
}
