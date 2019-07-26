using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    public class Main
    {
        public static XmlNamespaceManager nsmgr { get; private set; }

        List<Notebook> notebooks;

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
            notebooks = new List<Notebook>();

            foreach (XmlNode notebookNode in xml.SelectNodes("//one:Notebook", nsmgr))
            {
                notebooks.Add(new Notebook(notebookNode));
                Console.WriteLine(notebooks[notebooks.Count - 1].Name);
            }
        }
    }
}
