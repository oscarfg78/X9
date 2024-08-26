using System;
using System.IO;

namespace Maxi.Services.SouthSide.FtpServices
{
    public static class FtpUtils
    {
        #region Methods

        public static string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return $"{uri1}/{uri2}";
        }

        public static void ValidateLocalPath(String localPath, String fileName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(localPath);
            if (!directoryInfo.Exists)
            {
                throw new Exception($"'{localPath}' don't exist");
            }
            FileInfo fileInfo = new FileInfo(Path.Combine(localPath, fileName));
            if (!fileInfo.Exists)
            {
                throw new Exception($"'{Path.Combine(localPath, fileName)}' don't exist");
            }
        }

        public static void ValidateLocalPath(String localPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(localPath);
            if (!directoryInfo.Exists)
            {
                throw new Exception($"'{localPath}' don't exist");
            }
        }

        #endregion
    }
}
