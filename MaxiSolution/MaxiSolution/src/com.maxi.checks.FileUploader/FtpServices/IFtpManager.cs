using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maxi.Services.SouthSide.FtpServices
{
    public interface IFtpManager
    {
        void Upload(string address, Int32 port, string userName, string password, string remotePath,
                    string remoteFileName, string localPath, string localFileName);
        void Upload(string address, Int32 port, string userName, string password, string fileName, string remotePath,
                    string localPath);
        void Download(string address, Int32 port, string userName, string password, string remotePath,
                      string remoteFileName, string localPath, string localFileName);
        void Download(string address, Int32 port, string userName, string password, string fileName, string remotePath,
                      string localPath);
        List<String> GetFileList(string address, Int32 port, string userName, string password, string remotePath);
        void Delete(string address, Int32 port, string userName, string password, String fileName, string remotePath);
    }
}
