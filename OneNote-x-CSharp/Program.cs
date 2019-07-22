using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            new OneNote();

            // Test();
        }

        static void Test()
        {
            /*
             * Load XML from OneNote data 
             * This works
             */
            String strXML;
            Application app = new Application();

            app.GetHierarchy(null, HierarchyScope.hsPages, out strXML);
            // Console.WriteLine(strXML);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(strXML);
            XmlElement hierarchy = xml.DocumentElement;

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");

            Console.WriteLine(hierarchy.FirstChild.InnerXml);

            string path = Directory.GetCurrentDirectory();
            string combinedPath = Path.Combine(Directory.GetCurrentDirectory(), "dummy\\Dummy.txt");
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "dummy\\config.txt");

            Console.WriteLine(path);

            /*
             * Reads config file containing sensitive data
             * This works 
             */
            string[] lines;
            var list = new List<string>();
            var fileStream = new FileStream(configPath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            lines = list.ToArray();

            WinSCP.SessionOptions ops = new WinSCP.SessionOptions
            {
                Protocol = WinSCP.Protocol.Ftp,
                HostName = lines[0],
                UserName = lines[1],
                Password = lines[2]
            };

            using (WinSCP.Session session = new WinSCP.Session())
            {
                session.Open(ops);

                WinSCP.TransferOptions transferOptions = new WinSCP.TransferOptions();
                transferOptions.TransferMode = WinSCP.TransferMode.Binary;

                /*
                 * Upload dummy file to the site
                 * This works
                 */
                WinSCP.TransferOperationResult res = session.PutFiles(combinedPath, "/qlohome/qlodatabase/Reports/Dummy.txt", false, transferOptions);
                res.Check();

                foreach (WinSCP.TransferEventArgs transfer in res.Transfers)
                {
                    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                }

            }
        }
    }
}
