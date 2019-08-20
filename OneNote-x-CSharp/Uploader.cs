using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinSCP;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Uploader</c> handles the FTP process of uploading html files to the site
    /// </summary>
    public class Uploader
    {
        string hostName;
        string userName;
        string password;
        string filePath;

        SessionOptions sessionOptions;
        TransferOptions transferOptions;

        string lastFileName;

        /// <summary>
        /// Creates a new Uploader object and loads options.
        /// </summary>
        public Uploader()
        {
            using (StreamReader sr = new StreamReader(Main.path + "\\config.txt"))
            {
                hostName = sr.ReadLine();
                userName = sr.ReadLine();
                password = sr.ReadLine();
                filePath = sr.ReadLine();
            }

            sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            transferOptions = new TransferOptions
            {
                TransferMode = TransferMode.Binary
            };

            lastFileName = "";
        }

        /// <summary>
        /// Uploads all files located in the html folder to the site
        /// </summary>
        public void UploadHtml()
        {
            using (Session session = new Session())
            {
                session.FileTransferProgress += LogProgress;
                session.Open(sessionOptions);

                TransferOperationResult res = session.PutFiles(Main.htmlPath, filePath, false, transferOptions);
                res.Check();

                Console.WriteLine();
                foreach (TransferEventArgs transfer in res.Transfers)
                {
                    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                }
            }
        }

        /// <summary>
        /// Listener for the file transfer progress event which logs upload progress to the console.
        /// </summary>
        void LogProgress(object sender, FileTransferProgressEventArgs e)
        {
            if (lastFileName != e.FileName)
            {
                Console.WriteLine("Uploading {0}...", e.FileName);
            }

            lastFileName = e.FileName;
        }
    }
}
