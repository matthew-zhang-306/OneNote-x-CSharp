using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OneNote_x_CSharp
{
    public class HtmlWriter
    {
        Stack<string> tags;
        Indenter body;

        public HtmlWriter()
        {
            Clear();
        }

        public HtmlWriter Clear()
        {
            tags = new Stack<string>();
            body = new Indenter();

            return this;
        }

        public HtmlWriter AddBreak()
        {
            body.Append("<br>");
            return this;
        }

        public HtmlWriter AddElement(string tagName, string className, string text)
        {
            return AddTag(tagName, className).AddText(text).CloseTag();
        }
        public HtmlWriter AddElement(string tagName, string className, HtmlWriter html)
        {
            return AddTag(tagName, className).AddHtml(html).CloseTag();
        }

        public HtmlWriter AddTag(string tagName, string className)
        {
            if (Regex.Match(tagName, @"\W").Success)
            {
                throw new ArgumentException("Tag name for HTML should only contain alphanumeric characters!");
            }

            tags.Push(tagName);
            body.Append("<" + tagName + " class='" + className.Replace("'", "") + "'>").AddIndent();

            return this;
        }

        public HtmlWriter CloseTag()
        {
            if (tags.Count == 0)
            {
                return this;
            }

            string tag = tags.Pop();
            body.RemoveIndent().Append("</" + tag + ">");

            return this;
        }

        public HtmlWriter CloseAllTags()
        {
            while (tags.Count > 0)
            {
                CloseTag();
            }

            return this;
        }

        public HtmlWriter AddText(string text)
        {
            body.Append(text.Replace("\"", "&quot;").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
            return this;
        }
        public HtmlWriter AddText(IEnumerable<string> lines)
        {
            return lines.Aggregate(this, (writer, line) => writer.AddText(line));
        }

        public HtmlWriter AddHtml(HtmlWriter html)
        {
            body.Append(html.ToString());
            return this;
        }
        public HtmlWriter AddHtml(IEnumerable<HtmlWriter> htmls)
        {
            return htmls.Aggregate(this, (writer, html) => writer.AddHtml(html));
        }

        public override string ToString()
        {
            if (tags.Count > 0)
            {
                throw new InvalidOperationException("Cannot print out an HtmlWriter with unclosed tags!");
            }

            return body.ToString();
        }
    }
}
