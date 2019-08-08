using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    class Program
    {
        static bool doMiscTest = true;

        static void Main(string[] args)
        {
            if (doMiscTest)
            {
                MoveTest();
            }
            else
            {
                Main m = new Main();
                m.DoFullReport();

                // Add more reports

                Uploader uploader = new Uploader();

                // Upload
            }
        }

        static void MoveTest()
        {
            Application app = new Application();

            string fullXmlStr;
            app.GetHierarchy(null, HierarchyScope.hsPages, out fullXmlStr);

            XmlDocument fullXml = new XmlDocument();
            fullXml.LoadXml(fullXmlStr);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(fullXml.NameTable);
            nsmgr.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");

            XmlNode notebook = fullXml.SelectSingleNode("//one:Notebook[contains(@name,'Sai')]", nsmgr);

            XmlNode targetPage = notebook.SelectSingleNode("./one:SectionGroup[contains(@name,'Thursday')]/one:Section/one:Page[contains(@name,'to be moved')]", nsmgr);
            XmlNode toSection = notebook.SelectSingleNode("./one:SectionGroup[contains(@name,'Friday')]/one:Section[contains(@name,'Math')]", nsmgr);

            Console.WriteLine(toSection.Print());
            Console.WriteLine(toSection.GetAttribute("ID"));

            string newId;
            app.CreateNewPage(toSection.GetAttribute("ID"), out newId);

            string pageXmlStr;
            app.GetPageContent(targetPage.GetAttribute("ID"), out pageXmlStr);

            XmlDocument pageXml = new XmlDocument();
            pageXml.LoadXml(pageXmlStr);
            pageXml.DocumentElement.SetAttribute("name", newId);

            Console.WriteLine(pageXml.Print());

            app.UpdatePageContent(pageXml.ToString());
            app.DeleteHierarchy(targetPage.GetAttribute("ID"));
        }

        static void Test()
        {
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
