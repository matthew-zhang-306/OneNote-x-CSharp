using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    public class Main
    {
        public static string path { get; private set; }
        public static string reportPath { get { return path + "\\reports"; } }
        public static string htmlPath { get { return path + "\\reports\\html"; } }

        public static int missingAssignmentLookahead = 7;

        public static XmlNamespaceManager nsmgr { get; private set; }

        public List<Notebook> Notebooks { get; private set; }

        public HtmlWriter lastUpdatedHtml;

        public Main()
        {
            // Get folder path
            path = Directory.GetCurrentDirectory();

            XmlDocument xml = GetFullXml();

            // Set namespace manager to be used by every other xml based class
            nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");

            LoadNotebooks(xml);

            lastUpdatedHtml = GetLastUpdatedHtml();
        }

        XmlDocument GetFullXml()
        {
            string xmlStr;
            new Application().GetHierarchy(null, HierarchyScope.hsPages, out xmlStr);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);
            return xml;
        }

        void LoadNotebooks(XmlDocument xml)
        {
            Notebooks = new List<Notebook>();

            foreach (XmlNode notebookNode in xml.SelectNodes("//one:Notebook", nsmgr))
            {
                // Ignore QuestLearning's Notebook
                if (!notebookNode.GetAttribute("name", "QuestLearning").Contains("QuestLearning"))
                {
                    Notebooks.Add(new Notebook(notebookNode));
                }
            }
        }

        HtmlWriter GetLastUpdatedHtml()
        {
            return new HtmlWriter()
                .OpenTag("div", "reportLastUpdated")
                .AppendElement("p", "reportLastUpdatedText", "Last updated " + DateTime.Now.ToString("M/d h:mm tt"))
                .CloseTag();
        }

        public void DoFullReport()
        {
            string report = string.Join("\n\n", Notebooks.Select(notebook => notebook.FullReport()));

            Console.WriteLine(report);
            File.WriteAllText(reportPath + "\\fullreport.txt", report);
        }

        public void DoFullReportHtml()
        {
            string report = new HtmlWriter("fullReport")
                .OpenTag("div", "Container")
                    .AppendHtml(lastUpdatedHtml)
                    .AppendHtml(Notebooks.Select(notebook => notebook.FullReportHtml().AppendBreak()))
                .CloseTag()
                .ToString();

            File.WriteAllText(htmlPath + "\\FullReport.html", report);
        }

        public void DoStatusReports()
        {
            DoStatusReport(notebook => notebook.GetUngradedPages(), "ungraded");
            DoStatusReport(notebook => notebook.GetInactivePages(), "inactive");
            DoStatusReport(notebook => notebook.GetEmptyPages(), "empty");
            DoStatusReport(notebook => notebook.GetUnreviewedPages(), "unreviewed");
        }

        void DoStatusReport(Func<Notebook, List<Page>> func, string name)
        {
            List<Page> pages = new List<Page>();
            foreach (Notebook notebook in Notebooks)
            {
                pages.AddRange(func.Invoke(notebook));
            }

            string report = new Indenter()
                .Append(pages.Count + " " + name + " pages:")
                .Append(pages.Select(page => page.StatusReport()))
                .Append(" ")
                .ToString();

            Console.WriteLine(report);
            File.WriteAllText(reportPath + "\\" + name + "report.txt", report);
        }

        public void DoStatusReportsHtml()
        {
            DoStatusReportHtml(notebook => notebook.GetUngradedPages(), "UngradedPages");
            DoStatusReportHtml(notebook => notebook.GetInactivePages(), "InactivePages");
            DoStatusReportHtml(notebook => notebook.GetEmptyPages(), "EmptyPages");
            DoStatusReportHtml(notebook => notebook.GetUnreviewedPages(), "UnreviewedPages");
        }

        void DoStatusReportHtml(Func<Notebook, List<Page>> func, string name)
        {
            HtmlWriter htmlWriter = new HtmlWriter("statusReport")
                .OpenTag("div", "Container")
                    .AppendHtml(lastUpdatedHtml)
                    .OpenTag("table", "Table")
                        .OpenTag("tr", "HeaderRow")
                            .AppendElement("th", "HeaderNotebook", "Notebook")
                            .AppendElement("th", "HeaderSectionGroup", "Section Group")
                            .AppendElement("th", "HeaderSection", "Section")
                            .AppendElement("th", "HeaderPage", "Page")
                            .AppendElement("th", "HeaderTag", "Tag")
                        .CloseTag();

            foreach (Notebook notebook in Notebooks)
            {
                htmlWriter.AppendHtml(func.Invoke(notebook).Select(page => page.StatusReportHtml()));
            }

            string report = htmlWriter.CloseAllTags().ToString();
            File.WriteAllText(htmlPath + "\\" + name + ".html", report);
        }

        public void DoMissingAssignmentReport()
        {
            Indenter indenter = new Indenter();

            DateTime date = DateTime.Today;
            for (int i = 0; i < missingAssignmentLookahead; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                i++;

                indenter = indenter.Append(date.ToString("MM/dd/yyyy") + " missing:")
                    .AddIndent("    - ")
                    .Append(Notebooks.Select(notebook => notebook.MissingAssignmentReport(date)).Where(str => str.Length > 0))
                    .RemoveIndent()
                    .Append("");
            }

            string report = indenter.ToString();

            Console.WriteLine(report);
            File.WriteAllText(reportPath + "\\missingassignmentreport.txt", report);
        }

        public void DoMissingAssignmentReportHtml()
        {
            HtmlWriter htmlWriter = new HtmlWriter("missingAssignment")
                .OpenTag("div", "Container")
                    .AppendHtml(lastUpdatedHtml);

            DateTime date = DateTime.Today;
            for (int i = 0; i < missingAssignmentLookahead; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                i++;

                htmlWriter.OpenTag("div", "DayContainer")
                    .AppendElement("p", "DayHeader", date.ToString("MM/dd/yyyy"))
                    .AppendElement("p", "DaySubheader", "Assignments missing:")
                    .OpenTag("table", "DayTable")
                        .OpenTag("tbody", "TableBody")
                            .OpenTag("tr", "HeaderRow")
                                .AppendElement("th", "CellHeader", "Name")
                                .AppendHtml(Notebook.AllSubjects.Select(subject => new HtmlWriter().AppendElement("th", "CellHeader", subject)))
                            .CloseTag()
                            .AppendHtml(Notebooks.Select(notebook => notebook.MissingAssignmentReportHtml(date)).Where(html => html != null))
                        .CloseTag()
                    .CloseTag();
            }

            string report = htmlWriter.CloseAllTags().ToString();

            File.WriteAllText(htmlPath + "\\MissingAssignmentReport.html", report);
        }
    }
}
