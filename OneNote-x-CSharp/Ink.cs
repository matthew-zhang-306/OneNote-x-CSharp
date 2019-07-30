using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Ink
    {
        public string Name { get; private set; }

        public Ink(XmlNode inkNode, bool isWord)
        {
            if (isWord)
            {
                Name = "[Text]"; // replace with text recognition
            }
            else
            {
                Name = "[Drawing]";
            }
        }
    }
}
