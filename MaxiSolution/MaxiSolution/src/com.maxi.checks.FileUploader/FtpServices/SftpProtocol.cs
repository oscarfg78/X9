using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;
using Renci.SshNet.Sftp;
// 3 com.maxillc.checks.integration.fileTransfer
namespace Maxi.Services.SouthSide.FtpServices
{
    public class SftpProtocol : IFtpManager
    {
        #region Methods

        private SftpClient GetSftpConnected(string address, int port, string userName, string password)
        {
            var connectionInfo = new ConnectionInfo(address, port, userName, new PasswordAuthenticationMethod(userName, password));
            SftpClient sshCp = new SftpClient(connectionInfo);
            sshCp.Connect();
            return sshCp;
        }

        #endregion

        #region IFtpManager Methods

        public void Upload(
            string address,
            int port, 
            string userName, 
            string password, 
            string remotePath, 
            string remoteFileName, 
            string localPath, 
            string localFileName)
        {
            Exception exception = null;
            SftpClient sshCp = null;
            try
            {
                FtpUtils.ValidateLocalPath(localPath, localFileName);
                sshCp = GetSftpConnected(address, port, userName, password);
                FileInfo f = new FileInfo(Path.Combine(localPath, localFileName));
                var fileStream = new FileStream(f.FullName, FileMode.Open);
                sshCp.BufferSize = 4 * 1024;
                sshCp.UploadFile(fileStream, remotePath + f.Name, null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (sshCp != null)
                {
                    sshCp.Disconnect();
                    sshCp.Dispose();
                }
            }
            if (exception != null)
            {
                throw exception;
            }
        }

        public void Upload(string address, int port, string userName, string password, string fileName, string remotePath, string localPath)
        {
            Upload(address, port, userName, password, remotePath, fileName, localPath, fileName);
        }

        public void Download(string address, int port, string userName, string password, string remotePath, string remoteFileName, string localPath, string localFileName)
        {
            Exception exception = null;
            SftpClient sshCp = null;
            try
            {
                FtpUtils.ValidateLocalPath(localPath);
                sshCp = GetSftpConnected(address, port, userName, password);
                using (Stream fileStream = File.Create(Path.Combine(localPath, localFileName)))
                {
                    sshCp.DownloadFile(FtpUtils.Combine(remotePath, remoteFileName), fileStream);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (sshCp != null)
                {
                    sshCp.Disconnect();
                    sshCp.Dispose();
                }
            }
            if (exception != null)
            {
                throw exception;
            }
        }

        public void Download(string address, int port, string userName, string password, string fileName, string remotePath, string localPath)
        {
            Download(address, port, userName, password, remotePath, fileName, localPath, fileName);
        }

        public List<string> GetFileList(string address, int port, string userName, string password, string remotePath)
        {
            Exception exception = null;
            SftpClient sshCp = null;
            IEnumerable<SftpFile> list = new List<SftpFile>();
            try
            {
                sshCp = GetSftpConnected(address, port, userName, password);
                list = sshCp.ListDirectory(remotePath);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (sshCp != null)
                {
                    sshCp.Disconnect();
                    sshCp.Dispose();
                }
            }
            if (exception != null)
            {
                throw exception;
            }

            return list != null && list.Count() > 0
                       ? list.ToList().Select(f => f.ToString()).Where(f => f != "." && f != "..").ToList()
                       : new List<string>();
        }

        public void Delete(string address, int port, string userName, string password, string fileName, string remotePath)
        {
            Exception exception = null;
            SftpClient sshCp = null;
            try
            {
                sshCp = GetSftpConnected(address, port, userName, password);
                sshCp.Delete(FtpUtils.Combine(remotePath, fileName));
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (sshCp != null)
                {
                    sshCp.Disconnect();
                    sshCp.Dispose();
                }
            }
            if (exception != null)
            {
                throw exception;
            }
        }
        #endregion
    }
}
