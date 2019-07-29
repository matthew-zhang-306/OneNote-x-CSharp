using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class SectionGroup
    {
        public string Name { get; private set; }

        public Notebook Notebook { get; private set; }

        public List<Section> Sections { get; private set; }

        public SectionGroup(XmlNode sectionGroupNode, Notebook notebook)
        {
            Name = sectionGroupNode.GetAttribute("name", "untitled");
            Notebook = notebook;

            LoadSections(sectionGroupNode);
        }

        void LoadSections(XmlNode sectionGroupNode)
        {
            Sections = new List<Section>();

            foreach (XmlNode sectionNode in sectionGroupNode.SelectNodes("./one:Section", Main.nsmgr))
            {
                Sections.Add(new Section(sectionNode, this));
            }
        }

        public string FullReport()
        {
            // Add actual report
            return new Indenter("# SectionGroup: " + Name + " #")
                .AddIndent()
                .Append(Sections.Select(section => section.FullReport()))
                .ToString();
        }
    }
}
