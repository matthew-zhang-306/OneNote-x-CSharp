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

            LoadSections(notebookNode);
            LoadSectionGroups(notebookNode);
        }

        public void AddSubject(string subject)
        {
            if (Subjects == null)
                Subjects = new List<string>();

            if (!Subjects.Contains(subject))
                Subjects.Add(subject);
        }

        void LoadSections(XmlNode notebookNode)
        {
            Sections = new List<Section>();

            foreach (XmlNode sectionNode in notebookNode.SelectNodes("./one:Section", Main.nsmgr))
            {
                Sections.Add(new Section(sectionNode, this));
            }
        }

        void LoadSectionGroups(XmlNode notebookNode)
        {
            SectionGroups = new List<SectionGroup>();

            foreach (XmlNode sectionGroupNode in notebookNode.SelectNodes("./one:SectionGroup", Main.nsmgr))
            {
                if (sectionGroupNode.GetAttribute("isRecycleBin") != "true")
                    SectionGroups.Add(new SectionGroup(sectionGroupNode, this));
            }
        }

        public string FullReport()
        {
            return new Indenter(Name + " " + Subjects.Print())
                .Append("----------------")
                .Append(SectionGroups.Select(sectionGroup => sectionGroup.FullReport()))
                .ToString();
        }
    }
}
