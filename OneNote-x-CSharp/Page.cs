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
            if (Name == "Algebra 1 Evaluating Simple Expressions 1-2")
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
            TagName = DefaultTag; // replace with actual tag logic
        }

        void LoadDates(XmlDocument pageXml)
        {

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

        }

        public string FullReport()
        {
            Indenter indenter =
                new Indenter(Name.PadRight(40) + "(date)")
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
    }
}
