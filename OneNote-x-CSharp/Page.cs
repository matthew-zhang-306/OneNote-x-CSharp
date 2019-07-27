using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Page
    {
        public string Name { get; private set; }

        public Section Section { get; private set; }

        public Page(XmlNode pageNode, Section section)
        {
            Name = pageNode.Attributes?["name"]?.Value ?? "";
            Section = section;

            // Add more functionality
        }

        public string FullReport()
        {
            // Add actual report
            return Name;
        }
    }
}
