using System.Xml;
using System.Xml.Linq;

namespace OneNote_x_CSharp
{
    public static class Helpers
    {
        public static string Print(this XmlDocument xml) => XDocument.Parse(xml.OuterXml).ToString();
    }
}
