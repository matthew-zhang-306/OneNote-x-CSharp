using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    public class Main
    {
        public Main()
        {
            // Do things here
            LoadContent();
        }

        void LoadContent()
        {
            string xmlStr;
            new Application().GetHierarchy(null, HierarchyScope.hsPages, out xmlStr);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");

            Console.WriteLine(xml.Print());
        }
    }
}
