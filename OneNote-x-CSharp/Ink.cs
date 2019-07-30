using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Ink
    {
        static bool Debug = false;

        public string Name { get; private set; }

        public RectangleF Rect { get; private set; }

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

        public override string ToString() => Name + (Debug ? " " + Rect.Print() : "");
    }
}
