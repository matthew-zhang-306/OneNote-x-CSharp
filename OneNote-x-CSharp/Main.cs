using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    public class Main
    {
        public static XmlNamespaceManager nsmgr { get; private set; }

        public List<Notebook> Notebooks { get; private set; }

        public Main()
        {
            XmlDocument xml = GetFullXml();

            // Set namespace manager to be used by every other xml based class
            nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");

            LoadNotebooks(xml);
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
                    Notebooks.Add(new Notebook(notebookNode));
            }
        }

        public void DoFullReport()
        {
            string report = string.Join("\n\n", Notebooks.Select(notebook => notebook.FullReport()));

            Console.WriteLine(report);
            // write to file
        }
    }
}
