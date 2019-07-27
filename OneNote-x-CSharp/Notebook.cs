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

        public Notebook(XmlNode notebookNode)
        {
            Name = notebookNode.Attributes?["name"]?.Value ?? "";

            LoadSections(notebookNode);
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

            foreach (XmlNode sectionNode in notebookNode.SelectNodes("//one:Section", Main.nsmgr))
            {
                Sections.Add(new Section(sectionNode, this));
            }
        }

        public string FullReport()
        {
            return new Indenter(Name)
                .AddIndent("    ")
                .Append(Sections.Select(section => section.FullReport()))
                .ToString();
        }
    }
}
