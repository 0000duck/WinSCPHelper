using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using System.IO;
using WinScpHelper.Common;

namespace WinScpHelper
{
    public class WinScpConnection
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool DeleteWhenCopied { get; set; }
        public Protocol ConnectionProtocol { get; set; }
        public bool HasNoFingerprint { get; set; }

        public WinScpConnection() { }

        public WinScpConnection(string hostname, string username, string password, Protocol protocol = Protocol.Sftp, bool hasNoFingerprint = true)
        {
            this.ConnectionProtocol = protocol;
            this.HostName = hostname;
            this.Username = username;
            this.Password = password;
            this.HasNoFingerprint = hasNoFingerprint;
        }

        public SessionOptions MakeSessionOptions(string hostname, string username, string password, Protocol protocol, bool hasNoFingerprint)
        {
            SessionOptions option = new SessionOptions
            {
                Protocol = protocol,
                HostName = hostname,
                UserName = username,
                Password = password
            };

            option.GiveUpSecurityAndAcceptAnySshHostKey = hasNoFingerprint;

            return option;
        }

        /// <summary>
        /// Create connection session
        /// </summary>
        /// <param name="showProgress"></param>
        public Session Open(bool showProgress = false)
        {
            try
            {
                Session currentSession = new Session();

                if (showProgress)
                {
                    currentSession.FileTransferProgress += this.SessionFileTransferProgress;
                }

                currentSession.Open(MakeSessionOptions(this.HostName, this.Username, this.Password, this.ConnectionProtocol, this.HasNoFingerprint));

                return currentSession;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Uploads all files from local directory to remote directory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="sourceLocalLocation"></param>
        /// <param name="targetRemoteLocation"></param>
        public void Upload(Session session, string sourceLocalLocation, string targetRemoteLocation)
        {
            TransferOperationResult transferResult;
            transferResult = session.PutFiles(sourceLocalLocation, targetRemoteLocation);

            transferResult.Check();
        }

        /// <summary>
        /// Renames file on remote directory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="transferCollection"></param>
        public void RenameRemoteFiles(Session session, TransferEventArgsCollection transferCollection)
        {
            foreach (TransferEventArgs transfer in transferCollection)
            {
                string finalName = transfer.Destination;
                session.MoveFile(transfer.Destination, finalName);
            }
        }

        /// <summary>
        /// List files from directory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="targetRemoteDirectory"></param>
        /// <param name="extensionName"></param>
        public List<CurrentFile> ListFilesFromRemoteDirectory(Session session, string targetRemoteDirectory, string extensionName)
        {
            var currentFileList = new List<CurrentFile>();
            RemoteDirectoryInfo directory = session.ListDirectory(targetRemoteDirectory);

            foreach (RemoteFileInfo fileInfo in directory.Files)
            {
                string extension = Path.GetExtension(fileInfo.Name);
                if (string.Compare(extension, extensionName, true) == 0)
                {
                    currentFileList.Add(new CurrentFile
                    {
                        FileName = fileInfo.Name
                    });
                }
            }

            return currentFileList;
        }

        /// <summary>
        /// Delete remote file
        /// </summary>
        /// <param name="session"></param>
        /// <param name="sourceRemoteLocation"></param>
        public void Delete(Session session, string sourceRemoteLocation)
        {
            var removalResult = session.RemoveFiles(sourceRemoteLocation);

            if (removalResult.IsSuccess)
            {
                Console.WriteLine("Source file successfully deleted");
            }
        }

        /// <summary>
        /// Download files from remote directory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="sourceRemoteLocation"></param>
        /// <param name="targetLocalLocation"></param>
        public List<CurrentFile> Download(Session session, string sourceRemoteLocation, string targetLocalLocation = "")
        {
            try
            {                
                var currentFileList = new List<CurrentFile>();

                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;

                TransferOperationResult transferResult;
                transferResult = session.GetFiles(sourceRemoteLocation, targetLocalLocation, false, transferOptions);

                transferResult.Check();

                foreach (TransferEventArgs transfer in transferResult.Transfers)
                {
                    currentFileList.Add(new CurrentFile
                    {
                        FileName = transfer.FileName
                    });
                }

                return currentFileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SessionFileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            Console.Write("\r{0} ({1:P0})", e.FileName, e.FileProgress);
        }
    }
}
