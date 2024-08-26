using Boz.Services.Contracts.Utils;
using Maxi.Services.SouthSide.SouthsideFile.Types;
using Maxi.Services.SouthSide.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Maxi.Services.SouthSide.SouthsideFile
{

    public interface IServiceUtils
    {
        byte[] RecordBegining(int lenght);
        byte[] AppendBytes(byte[] b, byte[] b2);
        byte[] AppendBytes(byte[] b, byte[] b2, byte[] b3);
        byte[] GetFile(string filePath, ref int size);
        byte[] GetCheckFile(String filePath, bool isReclear, ref int size, String tellerSpray, bool addTellerSpray);
        byte[] ConvertAsciiToEbcdic(byte[] asciiData);
        byte[] ConvertEbcdicToAscii(byte[] ebcdicData);
        bool CreateFile(FileWf file, string path);
        byte[] GetCashLetterFile(String filePath, bool isFront, String imageDataAgentName, String imageDataResume, String imageDataCheckDetail, ref int size);
        Encoding GetEncoding();
    }


    public class ServiceUtils : IServiceUtils
    {
        private readonly ILogger<ServiceUtils> _logger;
        private IConfiguration _configuration;
        private string _serviceCode { get; set; }
        public ServiceUtils(ILogger<ServiceUtils> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceCode = _configuration.GetValue<string>("ServiceCode");
        }
        public Encoding CurrentEncode
        {
            get
            {
                return Encoding.GetEncoding("IBM037");
            }
        }

        public Encoding GetEncoding()
        {
            return CurrentEncode;
        }

        public byte[] RecordBegining(int lenght)
        {
            return ((IEnumerable<byte>)BitConverter.GetBytes(lenght)).Reverse<byte>().ToArray<byte>();
        }

        public byte[] AppendBytes(byte[] b, byte[] b2)
        {
            byte[] array = new byte[b.Length + b2.Length];
            Buffer.BlockCopy(b, 0, array, 0, b.Length);
            Buffer.BlockCopy(b2, 0, array, b.Length, b2.Length);
            return array;
        }

        public byte[] AppendBytes(byte[] b, byte[] b2, byte[] b3)
        {
            byte[] array = new byte[b.Length + b2.Length + b3.Length];
            Buffer.BlockCopy(b, 0, array, 0, b.Length);
            Buffer.BlockCopy(b2, 0, array, b.Length, b2.Length);
            Buffer.BlockCopy(b3, 0, array, b.Length + b2.Length, b3.Length);
            return array;
        }

        public byte[] GetFile(string filePath, ref int size)
        {
            size = 0;
            try
            {
                var bufferSize = 2 << 16; // 128KB
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize))
                {
                    size = (int)fileStream.Length;
                    byte[] array = new byte[size];
                    int num = 0;
                    int num2;
                    while ((num2 = fileStream.Read(array, num, size - num)) > 0)
                    {
                        num += num2;
                    }
                    return array;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceUtils.GetFile", ex, "SOUTHSIDESEND"));
                byte[] array = null;
                size = 0;
                return array;
            }
        }

        public byte[] GetCheckFile(String filePath, bool isReclear, ref int size, String tellerSpray, bool addTellerSpray)
        {
            size = 0;
            try
            {
                if (isReclear)
                {
                    using (Bitmap bitmap = new Bitmap(filePath))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            bitmap.Save(stream, ImageFormat.Jpeg);
                            using (Bitmap bitmap2 = new Bitmap(stream))
                            {
                                ImageUtils.DrawReclear(bitmap2);
                                byte[] tiffImageBytes = ImageUtils.GetTiffImageBytes(bitmap2, byScanlines: false);
                                size = tiffImageBytes.Length;
                                return tiffImageBytes;
                            }
                        }
                    }
                }

                if (addTellerSpray)
                {
                    using (Bitmap bitmap3 = new Bitmap(filePath))
                    {
                        using (MemoryStream stream2 = new MemoryStream())
                        {
                            bitmap3.Save(stream2, ImageFormat.Jpeg);
                            using (Bitmap bitmap4 = new Bitmap(stream2))
                            {
                                ImageUtils.DrawTellerSpray(bitmap4, tellerSpray);
                                byte[] tiffImageBytes = ImageUtils.GetTiffImageBytes(bitmap4, byScanlines: false);
                                size = tiffImageBytes.Length;
                                return tiffImageBytes;
                            }
                        }
                    }
                }

                var bufferSize = 2 << 16; // 128KB
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize))
                {
                    size = (int)fileStream.Length;
                    byte[] tiffImageBytes = new byte[size];
                    int num = 0;
                    int num2;
                    while ((num2 = fileStream.Read(tiffImageBytes, num, size - num)) > 0)
                    {
                        num += num2;
                    }
                    return tiffImageBytes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceUtils.GetFile", ex, "SOUTHSIDESEND"));
                byte[] tiffImageBytes = null;
                size = 0;
                return tiffImageBytes;
            }
        }

        public byte[] ConvertAsciiToEbcdic(byte[] asciiData)
        {
            return Encoding.Convert(Encoding.ASCII, Encoding.GetEncoding("IBM037"), asciiData);
        }

        public byte[] ConvertEbcdicToAscii(byte[] ebcdicData)
        {
            return Encoding.Convert(Encoding.GetEncoding("IBM037"), Encoding.ASCII, ebcdicData);
        }

        public bool CreateFile(FileWf file, string path)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                _logger.LogInformation($"Service {_serviceCode}: Begins write file X9 time elapsed: {stopwatch.Elapsed}");
                var bufferSize = 2 << 16; // 128KB
                using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize))
                {
                    byte[] binary = file.FileHeader.GetBinary();
                    fileStream.Write(binary, 0, binary.Length);
                    byte[] binary2 = file.CashLetter.CashLetterHeader.GetBinary();
                    fileStream.Write(binary2, 0, binary2.Length);
                    _logger.LogInformation($"Service {_serviceCode}: Begins write bundle list time elapsed: {stopwatch.Elapsed}");
                    foreach (BundleWf bundle in file.CashLetter.Bundles)
                    {
                        byte[] binary3 = bundle.BundleHeader.GetBinary();
                        fileStream.Write(binary3, 0, binary3.Length);
                        if (bundle.Credit != null)
                        {
                            byte[] binary4 = bundle.Credit.GetBinary();
                            fileStream.Write(binary4, 0, binary4.Length);
                        }
                        foreach (CheckWf check in bundle.Checks)
                        {
                            byte[] binary5 = check.GetBinary();
                            fileStream.Write(binary5, 0, binary5.Length);
                        }
                        byte[] binary6 = bundle.BundleControl.GetBinary();
                        fileStream.Write(binary6, 0, binary6.Length);
                    }
                    _logger.LogInformation($"Service {_serviceCode}: End write bundle list time elapsed: {stopwatch.Elapsed}");
                    byte[] binary7 = file.CashLetter.CashLetterControl.GetBinary();
                    fileStream.Write(binary7, 0, binary7.Length);
                    byte[] binary8 = file.FileControl.GetBinary();
                    fileStream.Write(binary8, 0, binary8.Length);
                    _logger.LogInformation($"Service {_serviceCode}: End write file X9 time elapsed: {stopwatch.Elapsed}");
                    stopwatch.Stop();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceUtils.CreateFile", ex, "SOUTHSIDESEND"));
                throw;
            }
        }

        public byte[] GetCashLetterFile(String filePath, bool isFront, String imageDataAgentName, String imageDataResume, String imageDataCheckDetail, ref int size)
        {
            size = 0;
            try
            {
                using (Bitmap bitmap = new Bitmap(filePath))
                {
                    ImageUtils.DrawCheckDetail(bitmap, imageDataCheckDetail, isFront);
                    if (isFront)
                    {
                        ImageUtils.DrawAgentName(bitmap, imageDataAgentName);
                        ImageUtils.DrawResume(bitmap, imageDataResume);
                    }
                    byte[] tiffImageBytes = ImageUtils.GetTiffImageBytes(bitmap, byScanlines: false);
                    size = tiffImageBytes.Length;
                    return tiffImageBytes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorUtils.FormatError("ServiceUtils.GetCashLetterFile", ex, "SOUTHSIDESEND"));

                byte[] tiffImageBytes = null;
                size = 0;
                return tiffImageBytes;
            }
        }
    }
}
