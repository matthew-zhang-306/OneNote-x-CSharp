using System;
using System.Collections.Generic;
using System.Xml;

namespace OneNote_x_CSharp
{
    class Section
    {
        public string Name { get; private set; }

        public Section(XmlNode sectionNode)
        {
            Name = sectionNode.Attributes?["name"]?.Value ?? "";
        }
    }
}
