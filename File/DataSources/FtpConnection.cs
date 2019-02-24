using System;
using System.ComponentModel;
using System.Threading;
using ScottLane.DataTidy.Core;

using Renci.SshNet;
using System.IO;

namespace ScottLane.DataTidy.File
{
    [Connection(typeof(FtpConnection), "ScottLane.DataTidy.File.Resources.FtpConnection.png")]
    public class FtpConnection : FileSystemConnection
    {
        private const FtpProtocol DEFAULT_FTP_PROTOCOL = FtpProtocol.FTP;
        private const int DEFAULT_FTP_PORT = 21;
        private const int DEFAULT_SFTP_PORT = 22;

        private FtpProtocol protocol;

        [GlobalisedCategory("FTP Connection"), GlobalisedDisplayName("Protocol"), GlobalisedDecription("The FTP protocol.")]
        public FtpProtocol Protocol
        {
            get { return protocol; }
            set
            {
                if (protocol != value)
                {
                    protocol = value;
                    OnPropertyChanged(nameof(Protocol));

                    if (port == DEFAULT_FTP_PORT && protocol == FtpProtocol.SFTP)
                    {
                        Port = DEFAULT_SFTP_PORT;
                    }
                    else if (port == DEFAULT_SFTP_PORT && protocol == FtpProtocol.FTP)
                    {
                        Port = DEFAULT_FTP_PORT;
                    }
                }
            }
        }

        private string hostName;

        [GlobalisedCategory("FTP Connection"), GlobalisedDisplayName("Host Name"), GlobalisedDecription("The host name.")]
        public string HostName
        {
            get { return hostName; }
            set
            {
                if (hostName != value)
                {
                    hostName = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }

        private int port;

        [GlobalisedCategory("FTP Connection"), GlobalisedDisplayName("Port"), GlobalisedDecription("The port."), DefaultValue(DEFAULT_FTP_PORT)]
        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        private string userName;

        [GlobalisedCategory("FTP Connection"), GlobalisedDisplayName("User Name"), GlobalisedDecription("The user name.")]
        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        private string password;

        [GlobalisedCategory("FTP Connection"), GlobalisedDisplayName("Password"), GlobalisedDecription("The password."), TypeConverter(typeof(EncryptedStringConverter))]
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(Password);
                }
            }
        }

        private BindingList<PrivateKey> privateKeys;

        [GlobalisedCategory("FTP Connection"), GlobalisedDisplayName("Private Keys"), GlobalisedDecription("The private keys."), TypeConverter(typeof(EncryptedStringConverter))]
        public BindingList<PrivateKey> PrivateKeys
        {
            get { return privateKeys; }
            set
            {
                if (privateKeys != default(BindingList<PrivateKey>))
                {
                    privateKeys = value;
                    OnPropertyChanged(nameof(PrivateKeys));
                    privateKeys.ListChanged += PrivateKeys_ListChanged;
                }
            }
        }

        private void PrivateKeys_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PrivateKeys));
        }

        public FtpConnection(Project parentProject) : base(parentProject)
        { }

        public override ConnectivityResult IsAvailable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            ConnectivityResult result = new ConnectivityResult(false);

            try
            {
                using (SftpClient client = new SftpClient(hostName, port, userName, new AESEncrypter().Decrypt(password)))
                {
                    client.Connect();
                }

                result.IsAvailable = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(hostName == default(string), "Please specify the Host Name", nameof(HostName));
                result.AddErrorIf(userName == default(string), "Please specify the User Name", nameof(UserName));
                result.AddErrorIf(password == default(string), "Please specify the Password", nameof(Password));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        public override FileStream GetFileStream(string path)
        {
            string tempPath = string.Concat(Path.GetTempFileName(), Path.GetExtension(path));

            using (SftpClient client = new SftpClient(hostName, port, userName, new AESEncrypter().Decrypt(password)))
            {
                client.Connect();

                using (FileStream tempFileStream = new FileStream(path, FileMode.Create))
                {
                    client.DownloadFile(path, tempFileStream);
                }
            }

            return base.GetFileStream(tempPath);
        }
    }

    public enum FtpProtocol
    {
        FTP,
        SFTP
    }
}
