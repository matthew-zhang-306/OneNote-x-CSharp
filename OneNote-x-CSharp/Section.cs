using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Section</c> models a section object in a student's notebook.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// The name of the section.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether the section is in the recycle bin.
        /// </summary>
        public bool Deleted { get; private set; }

        /// <summary>
        /// The subject for which the section pertains.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// The parent notebook which contains the section.
        /// </summary>
        public Notebook Notebook { get; private set; }

        /// <summary>
        /// The parent sectiongroup which contains the section, if any.
        /// </summary>
        public SectionGroup SectionGroup { get; private set; }

        /// <summary>
        /// The list of pages contained within the section.
        /// </summary>
        public List<Page> Pages { get; private set; }

        /// <summary>
        /// Creates a new Section object and either loads child objects if it is inside a sectiongroup or updates the notebook with data if it is not.
        /// </summary>
        /// <param name="sectionNode">The one:Section node representing a section in a student's notebook.</param>
        /// <param name="notebook">The parent notebook object.</param>
        /// <param name="sectionGroup">The parent sectiongroup object.</param>
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

        /// <summary>
        /// Assigns the subject property of the section and, optionally, updates the notebook's list of subjects.
        /// </summary>
        /// <param name="hasSectionGroup">Whether the section has a sectiongroup. It will update the notebook's list if this value is false.</param>
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

        /// <summary>
        /// Loads and creates page objects contained in the section.
        /// </summary>
        /// <param name="sectionNode">The one:Section node.</param>
        void LoadPages(XmlNode sectionNode)
        {
            Pages = new List<Page>();

            foreach (XmlNode pageNode in sectionNode.SelectNodes("./one:Page", Main.nsmgr))
            {
                Pages.Add(new Page(pageNode, this));
            }
        }

        /// <summary>
        /// Returns the text full report for the section.
        /// </summary>
        /// <returns>The full report for the section.</returns>
        public string FullReport()
        {
            return new Indenter("# Section: " + Name + " #")
                .AppendOnSameLine(Deleted ? " (deleted)" : "")
                .AddIndent()
                .Append(Pages.Select(page => page.FullReport()))
                .ToString();
        }

        /// <summary>
        /// Returns the html full report for the section.
        /// </summary>
        /// <returns>The full report for the section.</returns>
        public HtmlWriter FullReportHtml()
        {
            return new HtmlWriter("fullReport")
                .AppendElement("p", "SectionHeader", Name)
                .AppendElement("div", "PageItem", Pages.Select(page => page.FullReportHtml()));
        }
    }
}
