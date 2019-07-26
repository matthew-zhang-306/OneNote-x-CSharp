using System;
using System.Collections.Generic;
using System.Xml;

namespace OneNote_x_CSharp
{
    class Notebook
    {
        public readonly static List<string> AllSubjects = new List<string> { "Math", "Reading", "Grammar" };

        public string Name { get; private set; }

        public List<string> Subjects { get; private set; }

        public Notebook(XmlNode notebookNode)
        {
            Name = notebookNode.Attributes?["name"]?.Value ?? "";
        }

        public void AddSubject(string subject)
        {
            if (Subjects == null)
                Subjects = new List<string>();

            if (!Subjects.Contains(subject))
                Subjects.Add(subject);
        }
    }
}
