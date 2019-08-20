using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNote_x_CSharp
{
    /// <summary>
    /// Class <c>Indenter</c> represents a multi-line block of text with indentation.
    /// </summary>
    public class Indenter
    {
        /// <summary>
        /// The list of indents that are currently in use in the output.
        /// </summary>
        Stack<string> indents;

        /// <summary>
        /// The current indent which will preceed every line added to the output.
        /// </summary>
        string fullIndent;

        /// <summary>
        /// The multi-line indented string.
        /// </summary>
        string output;

        /// <summary>
        /// Creates an Indenter with an initial value.
        /// </summary>
        /// <param name="initialValue">The starting value of the output.</param>
        public Indenter(string initialValue = "")
        {
            Clear().Append(initialValue);
        }

        /// <summary>
        /// Removes all content from the object.
        /// </summary>
        /// <returns>Itself, after the operation.</returns>
        public Indenter Clear()
        {
            indents = new Stack<string>();
            output = "";

            return this;
        }

        /// <summary>
        /// Adds an indent to the stack.
        /// </summary>
        /// <param name="indent">The indent to be added.</param>
        /// <returns>Itself, after the operation.</returns>
        public Indenter AddIndent(string indent = "    ")
        {
            indents.Push(indent);
            fullIndent += indent;

            return this;
        }

        /// <summary>
        /// Removes the last added indent, if there are any.
        /// </summary>
        /// <returns>Itself, after the operation.</returns>
        public Indenter RemoveIndent()
        {
            if (indents.Count == 0)
            {
                return this;
            }
            
            string indent = indents.Pop();
            fullIndent = fullIndent.Substring(0, fullIndent.Length - indent.Length);

            return this;
        }

        /// <summary>
        /// Adds the given string to the end of the output, without adding a new line.
        /// </summary>
        /// <param name="lines">The string to add.</param>
        /// <returns>Itself, after the operation.</returns>
        public Indenter AppendOnSameLine(string lines)
        {
            output += lines.Replace("\n", "\n" + fullIndent);
            return this;
        }

        /// <summary>
        /// Appends the given strings to the end of the output, without adding a new line.
        /// </summary>
        /// <param name="lines">A list of strings to add.</param>
        /// <returns>Itself, after the operation.</returns>
        public Indenter AppendOnSameLine(IEnumerable<string> lines) => lines.Aggregate(this, (ind, line) => ind.AppendOnSameLine(line));

        /// <summary>
        /// Appends the given string to the end of the output starting on a new indented line.
        /// </summary>
        /// <param name="lines">The string to add.</param>
        /// <returns>Itself, after the operation.</returns>
        public Indenter Append(string lines)
        {
            if (output.Length > 0)
                output += "\n";
            output += fullIndent;

            return AppendOnSameLine(lines);
        }

        /// <summary>
        /// Appends the given strings to the end of the output starting on a new indented line.
        /// </summary>
        /// <param name="lines">A list of strings to add.</param>
        /// <returns>Itself, after the operation.</returns>
        public Indenter Append(IEnumerable<string> lines) => lines.Aggregate(this, (ind, line) => ind.Append(line));

        /// <summary>
        /// Returns whether the current output is empty.
        /// </summary>
        /// <returns>True if the output is an empty string.</returns>
        public bool IsEmpty() => output.Length == 0;

        /// <summary>
        /// Returns the output string.
        /// </summary>
        /// <returns>The value of the output.</returns>
        public override string ToString() => output;
    }
}
