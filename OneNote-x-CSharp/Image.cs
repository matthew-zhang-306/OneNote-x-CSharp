using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Image
    {
        public RectangleF Rect { get; private set; }

        public List<Ink> Inks { get; private set; }

        public Image(XmlNode imageNode, Page page)
        {
            Rect = Helpers.ExtractXmlRect(imageNode);

            Inks = page.Inks.Where(ink => Rect.IntersectsWith(ink.Rect)).ToList();
        }

        public string FullReport()
        {
            Indenter indenter =
                new Indenter(ToString());

            if (Inks.Count > 0)
            {
                indenter.Append(Inks.Count + " ink(s):")
                    .AddIndent("|   ")
                    .Append(Inks.Select((ink, i) => i + 1 + ") " + ink.ToString()));
            }

            return indenter.ToString();
        }

        public override string ToString() => Rect.Print();
    }
}
