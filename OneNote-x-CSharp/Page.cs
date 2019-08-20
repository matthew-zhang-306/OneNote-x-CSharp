using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Office.Interop.OneNote;

namespace OneNote_x_CSharp
{
    public class Page
    {
        public static double ActiveThreshold = 3;
        public static string DefaultTag = "none";

        public string Name { get; private set; }
        public string Subject { get { return Section.Subject; } }

        public XmlNode Tag { get; private set; }
        public string TagName { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime LastAssignedTime { get; private set; }
        public DateTime LastModifiedTime { get; private set; }
        public DateTime OriginalAssignmentDate { get; private set; }

        public bool Active { get; private set; }
        public bool Changed { get; private set; }
        public bool HasWork { get; private set; }
        public bool Empty { get; private set; }

        public Section Section { get; private set; }
        public SectionGroup SectionGroup { get { return Section.SectionGroup; } }

        public List<Image> Images { get; private set; }
        public List<Ink> Inks { get; private set; }

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

        XmlDocument GetPageXml(XmlNode pageNode)
        {
            string pageXmlStr;
            new Application().GetPageContent(pageNode.GetAttribute("ID"), out pageXmlStr, PageInfo.piBasic);

            XmlDocument pageXml = new XmlDocument();
            pageXml.LoadXml(pageXmlStr);
            return pageXml;
        }

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

        void LoadImages(XmlDocument pageXml)
        {
            Images = new List<Image>();

            foreach (XmlNode imageNode in pageXml.SelectNodes("//one:Image", Main.nsmgr))
            {
                Images.Add(new Image(imageNode, this));
            }
        }

        void SetStatus()
        {
            Active = LastModifiedTime > DateTime.Now.AddDays(-ActiveThreshold);
            Changed = LastModifiedTime > LastAssignedTime;
            HasWork = Images.Any(image => image.HasWork);
            Empty = Images.Count == 0;
        }

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
