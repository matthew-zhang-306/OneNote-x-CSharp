using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class SectionGroup
    {
        /// <summary>
        /// The name of the sectiongroup.
        /// </summary>
        /// <remarks>Could be trimmed to remove numbering prefixes.</remarks>
        public string Name { get; private set; }

        /// <summary>
        /// The parent notebook in which the sectiongroup is contained.
        /// </summary>
        public Notebook Notebook { get; private set; }

        /// <summary>
        /// The list of sections contained in the sectiongroup.
        /// </summary>
        public List<Section> Sections { get; private set; }

        /// <summary>
        /// Creates a new SectionGroup object and loads the contained sections.
        /// </summary>
        /// <param name="sectionGroupNode">The one:SectionGroup node representing a sectiongroup in a student's notebook.</param>
        /// <param name="notebook">The parent notebook object.</param>
        public SectionGroup(XmlNode sectionGroupNode, Notebook notebook)
        {
            Name = sectionGroupNode.GetAttribute("name", "untitled");

            // If the name comes a form like "1) Monday", remove the 1)
            if (Regex.Match(Name, @"^\d+\W* \w+$").Success)
            {
                Name = Name.Substring(Name.LastIndexOf(' ') + 1);
            }

            Notebook = notebook;

            LoadSections(sectionGroupNode);
        }

        /// <summary>
        /// Loads and creates section objects contained in the section.
        /// </summary>
        /// <param name="sectionNode">The one:SectionGroup node.</param>
        void LoadSections(XmlNode sectionGroupNode)
        {
            Sections = new List<Section>();

            foreach (XmlNode sectionNode in sectionGroupNode.SelectNodes("./one:Section", Main.nsmgr))
            {
                Sections.Add(new Section(sectionNode, Notebook, this));
            }
        }

        /// <summary>
        /// Returns the text full report for the sectiongroup.
        /// </summary>
        /// <returns>The full report for the sectiongroup.</returns>
        public string FullReport()
        {
            return new Indenter("# SectionGroup: " + Name + " #")
                .AddIndent()
                .Append(Sections.Select(section => section.FullReport()))
                .ToString();
        }

        /// <summary>
        /// Returns the html full report for the sectiongroup.
        /// </summary>
        /// <param name="isHeader">Whether to create a header cell, rather than an item cell.</param>
        /// <returns>The full report for the sectiongroup.</returns>
        public HtmlWriter FullReportHtml(bool isHeader)
        {
            HtmlWriter htmlWriter = new HtmlWriter("fullReport");

            if (isHeader)
            {
                return htmlWriter.AppendElement("th", "SectionGroupCellHeader", Name);
            }
            else
            {
                return htmlWriter.OpenTag("td", "SectionGroupCellItem")
                    .AppendElement("div", "SectionItem", Sections.Select(section => section.FullReportHtml()))
                    .CloseTag();
            }
        }
    }
}
