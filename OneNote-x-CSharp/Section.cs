using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Section
    {
        public string Name { get; private set; }

        public bool Deleted { get; private set; }

        public string Subject { get; private set; }

        public Notebook Notebook { get; private set; }

        public SectionGroup SectionGroup { get; private set; }

        public List<Page> Pages { get; private set; }

        public Section(XmlNode sectionNode, Notebook notebook, SectionGroup sectionGroup = null)
        {
            bool hasSectionGroup = sectionGroup != null;

            Name = sectionNode.GetAttribute("name", "untitled");
            Deleted = sectionNode.GetAttribute("isInRecycleBin", "false") == "true";

            Notebook = notebook;
            SectionGroup = sectionGroup;

            CheckForSubject(hasSectionGroup);

            if (hasSectionGroup)
            {
                LoadPages(sectionNode);
            }
        }

        void CheckForSubject(bool hasSectionGroup)
        {
            foreach (string subject in Notebook.AllSubjects.Where(sub => Name.ContainsIgnoreCase(sub)))
            {
                Subject = subject;
                if (!hasSectionGroup)
                {
                    Notebook.AddSubject(subject);
                }
            }
        }

        void LoadPages(XmlNode sectionNode)
        {
            Pages = new List<Page>();

            foreach (XmlNode pageNode in sectionNode.SelectNodes("./one:Page", Main.nsmgr))
            {
                Pages.Add(new Page(pageNode, this));
            }
        }

        public string FullReport()
        {
            return new Indenter("# Section: " + Name + " #")
                .AppendOnSameLine(Deleted ? " (deleted)" : "")
                .AddIndent()
                .Append(Pages.Select(page => page.FullReport()))
                .ToString();
        }
    }
}
