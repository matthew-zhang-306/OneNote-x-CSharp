using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>HtmlWriter</c> builds a well-formatted html document.
    /// </summary>
    public class HtmlWriter
    {
        /// <summary>
        /// The list of tags that are currently open.
        /// </summary>
        Stack<string> tags;

        /// <summary>
        /// The Indenter containing the html output.
        /// </summary>
        Indenter body;

        /// <summary>
        /// The string which preceeds the class names of the tags.
        /// </summary>
        public string ClassPrefix { get; private set; }

        /// <summary>
        /// Creates a new HtmlWriter with an optional class prefix.
        /// </summary>
        /// <param name="classPrefix">A string which will preceed class names of any added tags.</param>
        public HtmlWriter(string classPrefix = "")
        {
            ClassPrefix = classPrefix;
            Clear();
        }

        /// <summary>
        /// Removes all content from the object.
        /// </summary>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter Clear()
        {
            tags = new Stack<string>();
            body = new Indenter();

            return this;
        }

        /// <summary>
        /// Adds a single line break tag to the html.
        /// </summary>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendBreak()
        {
            body.Append("<br>");
            return this;
        }

        /// <summary>
        /// Opens a new tag in the html.
        /// </summary>
        /// <param name="tagName">The tag name to be added.</param>
        /// <param name="className">The class name for the tag.</param>
        /// <returns>Itself, after the operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the tag name contains nonalphanumeric characters or is empty.</exception>
        /// <remarks>The class name will have any apostrophes (') removed.</remarks>
        public HtmlWriter OpenTag(string tagName, string className = "")
        {
            if (!Regex.Match(tagName, @"^\w+$").Success)
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

        /// <summary>
        /// Closes the last opened tag, if there are any.
        /// </summary>
        /// <returns>Itself, after the operation.</returns>
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

        /// <summary>
        /// Closes all of the currently open tags, if any.
        /// </summary>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter CloseAllTags()
        {
            while (tags.Count > 0)
            {
                CloseTag();
            }

            return this;
        }

        /// <summary>
        /// Adds an html element to the document by opening a new tag, writing some content underneath, and then closing the tag.
        /// </summary>
        /// <param name="tagName">The tag to be added and closed.</param>
        /// <param name="className">The class name for the tag.</param>
        /// <param name="text">The text to be contained inside the tag.</param>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendElement(string tagName, string className, string text)                  => OpenTag(tagName, className).AppendText(text).CloseTag();

        /// <summary>
        /// Adds an html element to the document by opening a new tag, writing some content underneath, and then closing the tag.
        /// </summary>
        /// <param name="tagName">The tag to be added and closed.</param>
        /// <param name="className">The class name for the tag.</param>
        /// <param name="text">The lines of text to be contained inside the tag.</param>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendElement(string tagName, string className, IEnumerable<string> text)     => OpenTag(tagName, className).AppendText(text).CloseTag();

        /// <summary>
        /// Adds an html element to the document by opening a new tag, writing some content underneath, and then closing the tag.
        /// </summary>
        /// <param name="tagName">The tag to be added and closed.</param>
        /// <param name="className">The class name for the tag.</param>
        /// <param name="html">The html to be contained inside the tag.</param>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendElement(string tagName, string className, HtmlWriter html)              => OpenTag(tagName, className).AppendHtml(html).CloseTag();

        /// <summary>
        /// Adds an html element to the document by opening a new tag, writing some content underneath, and then closing the tag.
        /// </summary>
        /// <param name="tagName">The tag to be added and closed.</param>
        /// <param name="className">The class name for the tag.</param>
        /// <param name="html">The list of html to be contained inside the tag.</param>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendElement(string tagName, string className, IEnumerable<HtmlWriter> html) => OpenTag(tagName, className).AppendHtml(html).CloseTag();

        /// <summary>
        /// Adds text to the html document, with character escaping.
        /// </summary>
        /// <param name="text">The text to be added.</param>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendText(string text)
        {
            body.Append(text.Replace("\"", "&quot;").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
            return this;
        }

        /// <summary>
        /// Adds text to the html document, with character escaping.
        /// </summary>
        /// <param name="text">A list of text to be added.</param>
        /// <returns>Itself, after the operation.</returns>
        public HtmlWriter AppendText(IEnumerable<string> lines) => lines.Aggregate(this, (writer, line) => writer.AppendText(line));

        /// <summary>
        /// Adds the html output from another HtmlWriter to the document.
        /// </summary>
        /// <param name="html">The HtmlWriter containing the block to be added.</param>
        /// <returns></returns>
        public HtmlWriter AppendHtml(HtmlWriter html)
        {
            body.Append(html.ToString());
            return this;
        }

        /// <summary>
        /// Adds the html output from another HtmlWriter to the document.
        /// </summary>
        /// <param name="html">A list of HtmlWriters containing the blocks to be added.</param>
        /// <returns></returns>
        public HtmlWriter AppendHtml(IEnumerable<HtmlWriter> htmls) => htmls.Aggregate(this, (writer, html) => writer.AppendHtml(html));

        /// <summary>
        /// Returns whether the current document is empty.
        /// </summary>
        /// <returns>True if the html document is an empty string.</returns>
        public bool IsEmpty() => body.IsEmpty();

        /// <summary>
        /// Returns the html document.
        /// </summary>
        /// <returns>The full html string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if some tags are unclosed in the document.</exception>
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
