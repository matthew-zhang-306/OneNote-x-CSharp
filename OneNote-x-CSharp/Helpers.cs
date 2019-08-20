using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Static class <c>Helpers</c> contains methods which extend the basic C# functionality to assist methods in other classes.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Gets the value of an attribute of an xml node.
        /// </summary>
        /// <param name="xml">An xml document whose root node is to be analyzed.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="def">The default value for the attribute.</param>
        /// <returns>The value of the attribute, or the default value if it does not exist.</returns>
        public static string GetAttribute(this XmlDocument xml, string name, string def = null) => xml.DocumentElement.GetAttribute(name, def);

        /// <summary>
        /// Gets the value of an attribute of an xml node.
        /// </summary>
        /// <param name="xml">An xml node to be analyzed.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="def">The default value for the attribute.</param>
        /// <returns>The value of the attribute, or the default value if it does not exist.</returns>
        public static string GetAttribute(this XmlNode xml, string name, string def = null) => xml.Attributes?[name]?.Value ?? def;

        /// <summary>
        /// Returns a pretty-printed string of the given xml.
        /// </summary>
        /// <param name="xml">The xml node to be formatted.</param>
        /// <returns>A string containing properly indented and spaced xml.</returns>
        public static string Print(this XmlNode xml) => XDocument.Parse(xml.OuterXml).ToString();

        /// <summary>
        /// Checks whether a string contains another string, ignoring case.
        /// </summary>
        /// <param name="str">The outer string.</param>
        /// <param name="inner">The contained string.</param>
        /// <returns>True if the outer string contains the contained string, ignoring case.</returns>
        public static bool ContainsIgnoreCase(this string str, string inner) => str.ToLower().Contains(inner.ToLower());

        /// <summary>
        /// Checks whether two strings are equivalent, ignoring case.
        /// </summary>
        /// <param name="str">The first string.</param>
        /// <param name="inner">The second string.</param>
        /// <returns>True if the strings match, ignoring case.</returns>
        public static bool EqualsIgnoreCase(this string str, string inner) => str.ToLower() == inner.ToLower();

        /// <summary>
        /// Returns a capitalized version of a given string.
        /// </summary>
        /// <param name="str">A string.</param>
        /// <returns>The string with its first letter in uppercase and all other letters in lowercase.</returns>
        public static string Capitalized(this string str) => str.Length > 0 ? str[0].ToString().ToUpper() + str.Substring(1) : str;

        /// <summary>
        /// Returns whether a string is the name of a day of week.
        /// </summary>
        /// <param name="str">A string.</param>
        /// <returns>True if the string, ignoring case, is equivalent to one of the seven days of the week.</returns>
        public static bool IsWeekday(string str) => new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.Contains(str.Capitalized());

        /// <summary>
        /// Returns a printout for a list of strings.
        /// </summary>
        /// <param name="list">An enumerable object containing strings.</param>
        /// <returns>A printout of the strings.</returns>
        public static string Print(this IEnumerable<string> list) => list != null ? "[ " + string.Join(", ", list) + " ]" : "null";

        /// <summary>
        /// Returns a custom printout for a float rectangle.
        /// </summary>
        /// <param name="rect">A float rectangle.</param>
        /// <returns>A printout of the rectangle.</returns>
        public static string Print(this RectangleF rect) => "RECT [ " + rect.X + ", " + rect.Y + ", " + rect.Width + ", " + rect.Height + " ]";

        /// <summary>
        /// Determines the bounding area of a given float rectangle.
        /// </summary>
        /// <param name="rect">A float rectangle.</param>
        /// <returns>The floating point area of the rectangle.</returns>
        public static float Area(this RectangleF rect) => rect.Width * rect.Height;

        /// <summary>
        /// Returns a RectangleF object modelling a rectangle definition in OneNote xml.
        /// </summary>
        /// <param name="node">An xml node containing rectangle data.</param>
        /// <returns>A float rectangle containing the position and size specified in the xml.</returns>
        public static RectangleF ExtractXmlRect(XmlNode node)
        {
            XmlNode pos = node.SelectSingleNode("./one:Position", Main.nsmgr),
                    siz = node.SelectSingleNode("./one:Size", Main.nsmgr);

            if (pos == null || siz == null)
            {
                return RectangleF.Empty;
            }

            return new RectangleF(
                float.Parse(pos.GetAttribute("x", "0.0")),
                float.Parse(pos.GetAttribute("y", "0.0")),
                float.Parse(siz.GetAttribute("width", "0.0")),
                float.Parse(siz.GetAttribute("height", "0.0"))
            );
        }
    }
}
