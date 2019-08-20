using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Ink</c> models ink drawings and ink words on pages in OneNote.
    /// </summary>
    public class Ink
    {
        /// <summary>
        /// Whether the text output of ink objects should include the rectangle.
        /// </summary>
        static bool IncludeRectInPrint = false;

        /// <summary>
        /// The formatted name of the ink mark.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The bounding rectangle of the ink mark.
        /// </summary>
        public RectangleF Rect { get; private set; }

        /// <summary>
        /// Creates a new Ink object.
        /// </summary>
        /// <param name="inkNode">The one:Ink node representing an ink mark on a page.</param>
        /// <param name="isWord">Whether the root node is a one:InkWord rather than a one:InkDrawing.</param>
        public Ink(XmlNode inkNode, bool isWord)
        {
            if (isWord)
            {
                Name = "[Text]: " + inkNode.GetAttribute("recognizedText", "");

                Rect = new RectangleF(
                    float.Parse(inkNode.GetAttribute("inkOriginX", "0.0")) * -1,
                    float.Parse(inkNode.GetAttribute("inkOriginY", "0.0")) * -1,
                    float.Parse(inkNode.GetAttribute("width"     , "0.0")),
                    float.Parse(inkNode.GetAttribute("height"    , "0.0"))
                );
            }
            else
            {
                Name = "[Drawing]";
                Rect = Helpers.ExtractXmlRect(inkNode);
            }
        }

        public override string ToString() => Name + (IncludeRectInPrint ? " " + Rect.Print() : "");
    }
}
