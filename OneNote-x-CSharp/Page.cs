using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Page</c> models a page object in a student's notebook.
    /// </summary>
    public class Page
    {
        /// <summary>
        /// The number of days that must elapse before the page is considered inactive.
        /// </summary>
        public static double ActiveThreshold = 3;

        /// <summary>
        /// The tag name to be used when there is no tag on the page.
        /// </summary>
        public static string DefaultTag = "none";

        /// <summary>
        /// The name of the page.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The subject for which the page pertains.
        /// </summary>
        public string Subject { get { return Section.Subject; } }

        /// <summary>
        /// The xml representation of the tag attached to the page, if any.
        /// </summary>
        public XmlNode Tag { get; private set; }

        /// <summary>
        /// The name of the tag attached to the page.
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// The time when the page was created.
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// The time when the page was last tagged by an instructor.
        /// </summary>
        public DateTime LastAssignedTime { get; private set; }

        /// <summary>
        /// The time when the page was previously edited.
        /// </summary>
        public DateTime LastModifiedTime { get; private set; }

        /// <summary>
        /// The date when the page was intended to be completed.
        /// </summary>
        public DateTime OriginalAssignmentDate { get; private set; }

        /// <summary>
        /// Whether the page has been updated recently.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Whether the page was updated since the last assignment time.
        /// </summary>
        public bool Changed { get; private set; }

        /// <summary>
        /// Whether the page contains images with work.
        /// </summary>
        public bool HasWork { get; private set; }

        /// <summary>
        /// Whether the page contains no images.
        /// </summary>
        public bool Empty { get; private set; }

        /// <summary>
        /// The parent section of the page.
        /// </summary>
        public Section Section { get; private set; }

        /// <summary>
        /// The parent sectiongroup of the page.
        /// </summary>
        public SectionGroup SectionGroup { get { return Section.SectionGroup; } }

        /// <summary>
        /// The image objects contained in the page.
        /// </summary>
        public List<Image> Images { get; private set; }

        /// <summary>
        /// The ink objects contained in the page.
        /// </summary>
        public List<Ink> Inks { get; private set; }

        /// <summary>
        /// Creates a Page object and calculates its information using the xml nodes contained in it.
        /// </summary>
        /// <param name="pageNode">The one:Page node representing a page in a student's notebook.</param>
        /// <param name="section">The section which contains the page.</param>
        public Page(XmlNode pageNode, Section section)
        {
            Name = pageNode.GetAttribute("name", "");
            Section = section;

            XmlDocument pageXml = GetPageXml(pageNode);

            // Debug
            if (Name == "Pre - Algebra 3 Graphing and Comparing Fractions and Decimals 7-8 (8 Errors)")
            {
                Console.WriteLine(pageXml.Print() + "\n");
            }

            LoadTags(pageXml);
            LoadDates(pageXml);
            LoadInks(pageXml);
            LoadImages(pageXml);
            SetStatus();
        }

        /// <summary>
        /// Fetches the full xml of the page in OneNote.
        /// </summary>
        /// <param name="pageNode">The one:Page node.</param>
        /// <returns>An xml document with a one:Page root and content nodes underneath.</returns>
        XmlDocument GetPageXml(XmlNode pageNode)
        {
            string pageXmlStr;
            new Application().GetPageContent(pageNode.GetAttribute("ID"), out pageXmlStr, PageInfo.piBasic);

            XmlDocument pageXml = new XmlDocument();
            pageXml.LoadXml(pageXmlStr);
            return pageXml;
        }

        /// <summary>
        /// Loads tag information from the page content.
        /// </summary>
        /// <param name="pageXml">The xml document containing the page content.</param>
        void LoadTags(XmlDocument pageXml)
        {
            Tag = pageXml.SelectSingleNode("//one:Tag", Main.nsmgr);

            if (Tag == null)
            {
                TagName = DefaultTag;
            }
            else
            {
                TagName = pageXml.SelectSingleNode("//one:TagDef", Main.nsmgr)?.GetAttribute("name", DefaultTag) ?? DefaultTag;
            }
        }

        /// <summary>
        /// Loads and calculates date information from the page content.
        /// </summary>
        /// <param name="pageXml">The xml document containing the page content.</param>
        void LoadDates(XmlDocument pageXml)
        {
            CreationTime = DateTime.Parse(pageXml.GetAttribute("dateTime", ""));
            LastModifiedTime = DateTime.Parse(pageXml.GetAttribute("lastModifiedTime", ""));

            LastAssignedTime = DateTime.Parse(Tag?.GetAttribute("creationDate") ?? CreationTime.ToString());
            
            if (Helpers.IsWeekday(SectionGroup.Name))
            {
                for (OriginalAssignmentDate = CreationTime.Date; OriginalAssignmentDate.ToString("dddd") != SectionGroup.Name.Capitalized();)
                {
                    OriginalAssignmentDate = OriginalAssignmentDate.AddDays(1);
                }
            }
            else
            {
                OriginalAssignmentDate = LastAssignedTime.Date;
            }
        }

        /// <summary>
        /// Loads and creates ink objects contained in the page.
        /// </summary>
        /// <param name="pageXml">The xml document containing the page content.</param>
        void LoadInks(XmlDocument pageXml)
        {
            Inks = new List<Ink>();

            foreach (XmlNode inkNode in pageXml.SelectNodes("//one:InkDrawing", Main.nsmgr))
            {
                Inks.Add(new Ink(inkNode, false));
            }

            foreach (XmlNode inkNode in pageXml.SelectNodes("//one:InkWord", Main.nsmgr))
            {
                Inks.Add(new Ink(inkNode, true));
            }
        }

        /// <summary>
        /// Loads and creates image objects contained in the page.
        /// </summary>
        /// <param name="pageXml">The xml document containing the page content.</param>
        void LoadImages(XmlDocument pageXml)
        {
            Images = new List<Image>();

            foreach (XmlNode imageNode in pageXml.SelectNodes("//one:Image", Main.nsmgr))
            {
                Images.Add(new Image(imageNode, this));
            }
        }

        /// <summary>
        /// Calculates status information about the page using the loaded content.
        /// </summary>
        void SetStatus()
        {
            Active = LastModifiedTime > DateTime.Now.AddDays(-ActiveThreshold);
            Changed = LastModifiedTime > LastAssignedTime;
            HasWork = Images.Any(image => image.HasWork);
            Empty = Images.Count == 0;
        }

        /// <summary>
        /// Returns the text full report for the page.
        /// </summary>
        /// <returns>The full report for the page.</returns>
        public string FullReport()
        {
            Indenter indenter =
                new Indenter(Name.PadRight(60) + "(" + OriginalAssignmentDate.ToString("MM/dd/yyyy") + ")")
                .AppendOnSameLine(HasWork && Changed ? " (!)(modified)" : "")
                .AddIndent()
                .Append("Tag: " + TagName)
                .Append(Images.Count + " image(s):")
                .AddIndent("|   ");

            for (int i = 0; i < Images.Count; i++)
            {
                indenter.Append(i + 1 + ") ");
                indenter.AppendOnSameLine(Images[i].FullReport());
            }

            return indenter.ToString();
        }

        /// <summary>
        /// Returns the html full report for the page.
        /// </summary>
        /// <returns>The full report for the page.</returns>
        public HtmlWriter FullReportHtml()
        {
            return new HtmlWriter();
        }

        /// <summary>
        /// Returns the text status report for the page.
        /// </summary>
        /// <returns>The status report for the page.</returns>
        public string StatusReport()
        {
            return "PAGE: " +
                string.Join(" | ", new string[]
                {
                    Section.Notebook.Name.PadRight(20),
                    SectionGroup.Name.PadRight(10),
                    Section.Name.PadRight(12),
                    Name
                });
        }

        /// <summary>
        /// Returns the html status report for the page.
        /// </summary>
        /// <returns>The status report for the page.</returns>
        public HtmlWriter StatusReportHtml()
        {
            return new HtmlWriter("statusReport")
                .OpenTag("tr", "PageRow")
                    .AppendElement("td", "PageNotebook"    , Section.Notebook.Name)
                    .AppendElement("td", "PageSectionGroup", SectionGroup.Name)
                    .AppendElement("td", "PageSection"     , Section.Name)
                    .AppendElement("td", "Page"            , Name)
                    .AppendElement("td", "PageTag"         , TagName)
                .CloseTag();
        }
    }
}
