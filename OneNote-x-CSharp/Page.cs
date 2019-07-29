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
            Name = pageNode.GetAttribute("name", "");
            Section = section;

            // Add more functionality
        }

        public string FullReport()
        {
            // Add actual report
            return new Indenter(Name.PadRight(40) + "(date)")
                .ToString();
        }
    }
}
