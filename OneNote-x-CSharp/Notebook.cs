using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Notebook</c> models a student notebook object in the OneNote hierarchy.
    /// </summary>
    public class Notebook
    {
        /// <summary>
        /// A readonly array containing the names of all possible subjects which a student can be assigned.
        /// </summary>
        public readonly static string[] AllSubjects = new string[] { "Math", "Reading", "Grammar" };

        /// <summary>
        /// The name of the notebook.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The subjects assigned to this student.
        /// </summary>
        public List<string> Subjects { get; private set; }

        /// <summary>
        /// All sections contained in the notebook's sectiongroups.
        /// </summary>
        public List<Section> Sections { get; private set; }

        /// <summary>
        /// All sectiongroups which belong to the notebook.
        /// </summary>
        public List<SectionGroup> SectionGroups { get; private set; }

        /// <summary>
        /// Creates a new Notebook object and any contained SectionGroups, Sections, Pages, etc.
        /// </summary>
        /// <param name="notebookNode">The one:Notebook node representing a student's notebook.</param>
        public Notebook(XmlNode notebookNode)
        {
            Name = notebookNode.GetAttribute("name", "untitled");
            Subjects = new List<string>();

            LoadSectionGroups(notebookNode);
            LoadSections(notebookNode);
        }

        /// <summary>
        /// Adds a subject name to the list of assigned subjects, if it is not already in the list.
        /// </summary>
        /// <param name="subject">The name of the subject to add.</param>
        public void AddSubject(string subject)
        {
            if (!Subjects.Contains(subject))
            {
                Subjects.Add(subject);
            }
        }

        /// <summary>
        /// Searches and creates SectionGroup objects for the notebook's contained sectiongroups.
        /// </summary>
        /// <param name="notebookNode">The one:Notebook node representing a student's notebook.</param>
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

        /// <summary>
        /// Loads the Section list and goes through any sections placed outside of sectiongroups within the notebook.
        /// </summary>
        /// <param name="notebookNode">The one:Notebook node representing a student's notebook.</param>
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

        /// <summary>
        /// Returns the pages contained in the notebook which satisfy a given check function.
        /// </summary>
        /// <param name="check">A conditional predicate for a Page object.</param>
        /// <returns>A list of pages for which the function returns true.</returns>
        public List<Page> GetPagesWhere(Func<Page, bool> check) => Sections.Aggregate(new List<Page>(), (list, section) => list.Concat(section.Pages.Where(check)).ToList());

        /// <summary>
        /// Returns whether or not the notebook contains any pages which satisfy a given check function.
        /// </summary>
        /// <param name="check">A conditional predicate for a Page object.</param>
        /// <returns>True if at least one page contained in the notebook returns true when passed into the function.</returns>
        public bool HasPagesWhere(Func<Page, bool> check) => Sections.Any((section) => section.Pages.Any(check));

        /// <summary>
        /// Returns the notebook's ungraded pages.
        /// </summary>
        /// <returns>A list of pages which pass an ungradedness check.</returns>
        public List<Page> GetUngradedPages()   => GetPagesWhere((page) => page.Changed && page.HasWork);

        /// <summary>
        /// Returns the notebook's inactive pages.
        /// </summary>
        /// <returns>A list of pages which pass an inactivity check.</returns>
        public List<Page> GetInactivePages()   => GetPagesWhere((page) => !page.Active);

        /// <summary>
        /// Returns the notebook's empty pages.
        /// </summary>
        /// <returns>A list of pages which pass an emptiness check.</returns>
        public List<Page> GetEmptyPages()      => GetPagesWhere((page) => page.Empty);

        /// <summary>
        /// Returns the notebook's unreviewed pages.
        /// </summary>
        /// <returns>A list of pages which pass an unreviewedness check.</returns>
        public List<Page> GetUnreviewedPages() => GetPagesWhere((page) => page.TagName.ContainsIgnoreCase("review"));

        /// <summary>
        /// Returns whether the notebook contains pages assigned for a given subject on a given date.
        /// </summary>
        /// <param name="subject">The subject of the assigned page.</param>
        /// <param name="date">The date of assignment for the page.</param>
        /// <returns>True if any page in the notebook is not empty and matches the subject and date.</returns>
        public bool HasAssignedPages(string subject, DateTime date) => HasPagesWhere((page) => !page.Empty && page.Subject.EqualsIgnoreCase(subject) && page.OriginalAssignmentDate.Date == date.Date);

        /// <summary>
        /// Returns the text full report for the notebook.
        /// </summary>
        /// <returns>The full report for the notebook.</returns>
        public string FullReport()
        {
            return new Indenter(Name + " " + Subjects.Print())
                .Append("--------------------------------")
                .Append(SectionGroups.Select(sectionGroup => sectionGroup.FullReport()))
                .ToString();
        }

        /// <summary>
        /// Returns the html full report for the notebook.
        /// </summary>
        /// <returns>The full report for the notebook.</returns>
        public HtmlWriter FullReportHtml()
        {
            return new HtmlWriter("fullReport")
                .OpenTag("div", "NotebookContainer")
                    .AppendElement("p", "NotebookName", Name)
                    .OpenTag("div", "SectionTableContainer")
                        .OpenTag("table", "SectionTable")
                            .OpenTag("tr", "SectionGroupHeaderRow")
                                .AppendHtml(SectionGroups.Select(sectiongroup => sectiongroup.FullReportHtml(true)))
                            .CloseTag()
                            .OpenTag("tr", "SectionGroupRow")
                                .AppendHtml(SectionGroups.Select(sectiongroup => sectiongroup.FullReportHtml(false)))
                .CloseAllTags();
        }

        /// <summary>
        /// Returns the text missing assignment report for the notebook for a given date.
        /// </summary>
        /// <param name="date">The date for which assignments should be checked.</param>
        /// <returns>The missing assignment report for the notebook.</returns>
        public string MissingAssignmentReport(DateTime date)
        {
            return new Indenter()
                .Append(Subjects.Where(subject => !HasAssignedPages(subject, date)).Select(subject => Name + " - " + subject))
                .ToString();
        }

        /// <summary>
        /// Returns the html missing assignment report for the notebook for a given date.
        /// </summary>
        /// <param name="date">The date for which assignments should be checked.</param>
        /// <returns>The missing assignment report for the notebook.</returns>
        public HtmlWriter MissingAssignmentReportHtml(DateTime date)
        {
            HtmlWriter htmlWriter = new HtmlWriter("missingAssignment")
                .OpenTag("tr", "StudentRow")
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
