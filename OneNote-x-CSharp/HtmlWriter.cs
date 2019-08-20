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

        public string ClassPrefix { get; private set; }

        public HtmlWriter(string classPrefix = "")
        {
            ClassPrefix = classPrefix;
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

        public HtmlWriter AddTag(string tagName, string className = "")
        {
            if (Regex.Match(tagName, @"\W").Success)
            {
                throw new ArgumentException("Tag name for HTML should only contain alphanumeric characters!");
            }

            tags.Push(tagName);
            body.Append("<" + tagName);

            if ((ClassPrefix + className).Length > 0) {
                body.AppendOnSameLine(" class='" + (ClassPrefix + className).Replace("'", "") + "'");
            }

            body.AppendOnSameLine(">").AddIndent();
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

        public HtmlWriter AppendElement(string tagName, string className, string text)                  => AddTag(tagName, className).AppendText(text).CloseTag();
        public HtmlWriter AppendElement(string tagName, string className, IEnumerable<string> text)     => AddTag(tagName, className).AppendText(text).CloseTag();
        public HtmlWriter AppendElement(string tagName, string className, HtmlWriter html)              => AddTag(tagName, className).AppendHtml(html).CloseTag();
        public HtmlWriter AppendElement(string tagName, string className, IEnumerable<HtmlWriter> html) => AddTag(tagName, className).AppendHtml(html).CloseTag();

        public HtmlWriter AppendText(string text)
        {
            body.Append(text.Replace("\"", "&quot;").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
            return this;
        }

        public HtmlWriter AppendText(IEnumerable<string> lines) => lines.Aggregate(this, (writer, line) => writer.AppendText(line));

        public HtmlWriter AppendHtml(HtmlWriter html)
        {
            body.Append(html.ToString());
            return this;
        }

        public HtmlWriter AppendHtml(IEnumerable<HtmlWriter> htmls) => htmls.Aggregate(this, (writer, html) => writer.AppendHtml(html));

        public bool IsEmpty() => body.IsEmpty();

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
