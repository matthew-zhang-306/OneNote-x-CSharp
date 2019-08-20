using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Notebook
    {
        public readonly static List<string> AllSubjects = new List<string> { "Math", "Reading", "Grammar" };

        public string Name { get; private set; }

        public List<string> Subjects { get; private set; }

        public List<Section> Sections { get; private set; }

        public List<SectionGroup> SectionGroups { get; private set; }

        public Notebook(XmlNode notebookNode)
        {
            Name = notebookNode.GetAttribute("name", "untitled");
            Subjects = new List<string>();

            LoadSectionGroups(notebookNode);
            LoadSections(notebookNode);
        }

        public void AddSubject(string subject)
        {
            if (!Subjects.Contains(subject))
            {
                Subjects.Add(subject);
            }
        }

        void LoadSectionGroups(XmlNode notebookNode)
        {
            SectionGroups = new List<SectionGroup>();

            foreach (XmlNode sectionGroupNode in notebookNode.SelectNodes("./one:SectionGroup", Main.nsmgr))
            {
                if (sectionGroupNode.GetAttribute("isRecycleBin") != "true")
                {
                    SectionGroups.Add(new SectionGroup(sectionGroupNode, this));
                }
            }
        }

        void LoadSections(XmlNode notebookNode)
        {
            Sections = new List<Section>();
            SectionGroups.ForEach(sectionGroup => Sections.AddRange(sectionGroup.Sections));

            foreach (XmlNode sectionNode in notebookNode.SelectNodes("./one:Section", Main.nsmgr))
            {
                // Sections outside of section groups created but not stored, as their sole purpose is to provide data for the notebook
                new Section(sectionNode, this);
            }
        }

        public List<Page> GetPagesWhere(Func<Page, bool> check) => Sections.Aggregate(new List<Page>(), (list, section) => list.Concat(section.Pages.Where(check)).ToList());
        public bool HasPagesWhere(Func<Page, bool> check) => Sections.Any((section) => section.Pages.Any(check));

        public List<Page> GetUngradedPages()   => GetPagesWhere((page) => page.Changed && page.HasWork);
        public List<Page> GetInactivePages()   => GetPagesWhere((page) => !page.Active);
        public List<Page> GetEmptyPages()      => GetPagesWhere((page) => page.Empty);
        public List<Page> GetUnreviewedPages() => GetPagesWhere((page) => page.TagName.ContainsIgnoreCase("review"));

        public bool HasAssignedPages(string subject, DateTime date) => HasPagesWhere((page) => !page.Empty && page.Subject.EqualsIgnoreCase(subject) && page.OriginalAssignmentDate.Date == date.Date);

        public string FullReport()
        {
            return new Indenter(Name + " " + Subjects.Print())
                .Append("--------------------------------")
                .Append(SectionGroups.Select(sectionGroup => sectionGroup.FullReport()))
                .ToString();
        }

        public string MissingAssignmentReport(DateTime date)
        {
            return new Indenter()
                .Append(Subjects.Where(subject => !HasAssignedPages(subject, date)).Select(subject => Name + " - " + subject))
                .ToString();
        }

        public HtmlWriter MissingAssignmentReportHtml(DateTime date)
        {
            HtmlWriter htmlWriter = new HtmlWriter("missingAssignment")
                .AddTag("tr", "StudentRow")
                    .AppendElement("td", "CellItem", Name);

            bool flag = false;
            foreach (string subject in AllSubjects)
            {
                string className = "CellItem", content;

                if (!Subjects.Contains(subject))
                {
                    className += "NA";
                    content = "N/A";
                }
                else if (HasAssignedPages(subject, date))
                {
                    className += "OK";
                    content = "&nbsp;";
                }
                else
                {
                    className += "X";
                    content = "X";
                    flag = true;
                }

                htmlWriter.AppendElement("td", className, content);
            }

            return flag ? htmlWriter.CloseAllTags() : null;
        }
    }
}
