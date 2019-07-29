using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace OneNote_x_CSharp
{
    public static class Helpers
    {
        public static string GetAttribute(this XmlNode xml, string name, string def = null) => xml.Attributes?[name]?.Value ?? def;

        public static string Print(this XmlDocument xml) => XDocument.Parse(xml.OuterXml).ToString();
        public static string Print(this XmlNode xml) => XDocument.Parse(xml.OuterXml).ToString();

        public static bool ContainsIgnoreCase(this string str, string inner) => str.ToLower().Contains(inner.ToLower());
        public static string Capitalized(this string str) => str.Length > 0 ? str[0].ToString().ToUpper() + str.Substring(1) : str;

        public static bool IsWeekday(string str) => new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.Contains(str.Capitalized());

        public static string Print(this IEnumerable<string> list) => "[ " + string.Join(", ", list) + " ]";
    }
}
