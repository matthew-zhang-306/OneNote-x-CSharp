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
        static bool doMiscTest = false;

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
                m.DoFullReportHtml();
                Console.WriteLine();

                m.DoStatusReports();
                m.DoStatusReportsHtml();
                Console.WriteLine();

                m.DoMissingAssignmentReport();
                m.DoMissingAssignmentReportHtml();
                Console.WriteLine();

                Uploader uploader = new Uploader();
                uploader.UploadHtml();
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
    }
}
