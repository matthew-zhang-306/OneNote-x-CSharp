using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace OneNote_x_CSharp
{
    public static class Helpers
    {
        public static string Print(this XmlDocument xml) => XDocument.Parse(xml.OuterXml).ToString();
        public static string Print(this XmlNode xml) => XDocument.Parse(xml.OuterXml).ToString();

        public static string Capitalized(this string str) => str.Length > 0 ? str[0].ToString().ToUpper() + str.Substring(1) : str;

        public static bool IsWeekday(string str) => new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.Contains(str.Capitalized());
        public static bool IsSubject(string str) => Notebook.AllSubjects.Contains(str.Capitalized());

        public static string ToString(this IEnumerable<string> list) => "[ " + string.Join(", ", list) + " ]";
    }
}
