using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Image
    {
        public bool IsValid { get; private set; }

        public List<Ink> Inks { get; private set; }

        public Image(XmlNode imageNode, Page page)
        {
            IsValid = true; // replace with null position/size check

            SetInks(page.Inks);
        }

        void SetInks(List<Ink> allInks)
        {
            Inks = allInks; // replace with .where intersects check
        }
    }
}
