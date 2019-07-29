using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Page
    {
        public static double ActiveThreshold = 3;
        public static string DefaultTag = "none";

        public string Name { get; private set; }

        public Section Section { get; private set; }
        public SectionGroup SectionGroup { get; private set; }

        public List<Image> Images { get; private set; }
        public List<Ink> Inks { get; private set; }

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
