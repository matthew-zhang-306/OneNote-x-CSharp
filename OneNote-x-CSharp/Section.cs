using System;
using System.Collections.Generic;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Section
    {
        public string Name { get; private set; }

        public Notebook Notebook { get; private set; }

        public List<Page> Pages { get; private set; }

        public Section(XmlNode sectionNode, Notebook notebook)
        {
            Name = sectionNode.Attributes?["name"]?.Value ?? "";
            Notebook = notebook;

            LoadPages(sectionNode);
        }

        void LoadPages(XmlNode sectionNode)
        {
            Pages = new List<Page>();

            foreach (XmlNode pageNode in sectionNode.SelectNodes("//one:Page", Main.nsmgr))
            {
                Pages.Add(new Page(pageNode, this));
            }
        }

        public string FullReport()
        {
            // Add actual report
            return new Indenter("SECTION: " + Name)
                .AddIndent("  - ")
                .Append("This is a test")
                .AddIndent("----")
                .Append("to see if indents")
                .Append("work properly")
                .RemoveIndent()
                .Append("with many lines")
                .ToString();
        }
    }
}
