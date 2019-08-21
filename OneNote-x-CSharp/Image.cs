using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Image</c> models pictures placed in OneNote pages which represent worksheeets assigned to a student.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The ink area to image area ratio which must be met for the work to be considered adequate.
        /// </summary>
        public static double PageFillConstant = 0.005;

        /// <summary>
        /// The amount of inks which must be on the image for the work to be considered adequate.
        /// </summary>
        public static int MinimumInks = 5;

        /// <summary>
        /// The bounding rectangle of the image.
        /// </summary>
        public RectangleF Rect { get; private set; }

        /// <summary>
        /// The ink marks that overlap the image.
        /// </summary>
        public List<Ink> Inks { get; private set; }

        /// <summary>
        /// Whether or not the page contains an adequate amount of work.
        /// </summary>
        public bool HasWork { get; private set; }

        /// <summary>
        /// Creates a new Image object.
        /// </summary>
        /// <param name="imageNode">The one:Image node representing a worksheet image on a page.</param>
        /// <param name="page">The parent page which contains the image.</param>
        /// <remarks>Requires inks to be loaded into the parent page.</remarks>
        public Image(XmlNode imageNode, Page page)
        {
            Rect = Helpers.ExtractXmlRect(imageNode);

            Inks = page.Inks.Where(ink => Rect.IntersectsWith(ink.Rect)).ToList();
            CheckForWork();
        }

        /// <summary>
        /// Determines whether the image contains enough ink to qualify as having work.
        /// </summary>
        void CheckForWork()
        {
            HasWork = Inks.Count >= MinimumInks && Inks.Sum(ink => ink.Rect.Area()) / Rect.Area() >= PageFillConstant;
        }

        /// <summary>
        /// Returns the text full report for the image.
        /// </summary>
        /// <returns>The full report for the image.</returns>
        public string FullReport()
        {
            Indenter indenter =
                new Indenter(ToString())
                .AppendOnSameLine(HasWork ? " (!)(has work)" : "");

            if (Inks.Count > 0)
            {
                indenter.Append(Inks.Count + " ink(s):")
                    .AddIndent("|   ")
                    .Append(Inks.Select((ink, i) => i + 1 + ") " + ink.ToString()));
            }

            return indenter.ToString();
        }

        /// <summary>
        /// Returns the html full report for the image.
        /// </summary>
        /// <returns>The full report for the image.</returns>
        public HtmlWriter FullReportHtml()
        {
            return new HtmlWriter("fullReport")
                .OpenTag("li", "ImageItem")
                    .AppendElement("p", "ImageSubheader", Inks.Count + " mark" + (Inks.Count == 1 ? "" : "s"))
                .CloseTag();
        }

        public override string ToString() => Rect.Print();
    }
}
