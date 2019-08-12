using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinSCP;

namespace OneNote_x_CSharp
{
    public class Uploader
    {
        string hostName;
        string userName;
        string password;
        string filePath;

        string lastFileName;

        public Uploader()
        {
            using (StreamReader sr = new StreamReader(Main.path + "\\config.txt"))
            {
                hostName = sr.ReadLine();
                userName = sr.ReadLine();
                password = sr.ReadLine();
                filePath = sr.ReadLine();
            }

            lastFileName = "";
        }

        public void UploadHtml()
        {
            SessionOptions ops = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            using (Session session = new Session())
            {
                session.FileTransferProgress += LogProgress;
                session.Open(ops);

                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;

                TransferOperationResult res = session.PutFiles(Main.htmlPath, filePath, false, transferOptions);
                res.Check();

                Console.WriteLine();
                foreach (TransferEventArgs transfer in res.Transfers)
                {
                    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                }
            }
        }

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
