using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    public class Main
    {
        public static string path { get; private set; }
        public static string reportPath { get { return path + "\\reports"; } }
        public static string htmlPath { get { return path + "\\reports\\html"; } }

        public static int missingAssignmentLookahead = 7;

        public static XmlNamespaceManager nsmgr { get; private set; }

        public List<Notebook> Notebooks { get; private set; }

        public string lastUpdatedHtml;

        public Main()
        {
            // Get folder path
            path = Directory.GetCurrentDirectory();

            XmlDocument xml = GetFullXml();

            // Set namespace manager to be used by every other xml based class
            nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");

            LoadNotebooks(xml);

            lastUpdatedHtml = GetLastUpdatedHtml();
        }

        XmlDocument GetFullXml()
        {
            string xmlStr;
            new Application().GetHierarchy(null, HierarchyScope.hsPages, out xmlStr);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);
            return xml;
        }

        void LoadNotebooks(XmlDocument xml)
        {
            Notebooks = new List<Notebook>();

            foreach (XmlNode notebookNode in xml.SelectNodes("//one:Notebook", nsmgr))
            {
                // Ignore QuestLearning's Notebook
                if (!notebookNode.GetAttribute("name", "QuestLearning").Contains("QuestLearning"))
                {
                    Notebooks.Add(new Notebook(notebookNode));
                }
            }
        }

        string GetLastUpdatedHtml()
        {
            return new HtmlWriter()
                .AddTag("div", "reportLastUpdated")
                .AddElement("p", "reportLastUpdatedText", "Last updated " + DateTime.Now.ToString("M/d h:mm tt"))
                .CloseTag()
                .ToString();
        }

        public void DoFullReport()
        {
            string report = string.Join("\n\n", Notebooks.Select(notebook => notebook.FullReport()));

            Console.WriteLine(report);
            File.WriteAllText(reportPath + "\\fullreport.txt", report);
        }

        public void DoFullReportHtml()
        {

        }

        public void DoStatusReports()
        {

        }

        public void DoStatusReportsHtml()
        {

        }

        public void DoMissingAssignmentReport()
        {

        }

        public void DoMissingAssignmentReportHtml()
        {

        }
    }
}
