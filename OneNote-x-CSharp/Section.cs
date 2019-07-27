using System;
using System.Collections.Generic;
using System.Xml;

namespace OneNote_x_CSharp
{
    public class Section
    {
        public string Name { get; private set; }

        public Section(XmlNode sectionNode)
        {
            Name = sectionNode.Attributes?["name"]?.Value ?? "";
        }

        public string FullReport()
        {
            return new Indenter("SECTION: " + Name)
                .AddIndent("  - ")
                .Append("This is a test")
                .AddIndent("----")
                .Append("to see if indents")
                .Append("work properly")
                .RemoveIndent()
                .Append("with many lines")
                .ToString();
        }
    }
}
