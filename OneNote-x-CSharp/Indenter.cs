using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNote_x_CSharp
{

    public class Indenter
    {
        Stack<string> indents;
        string fullIndent;

        string output;

        public Indenter(string initialValue = "")
        {
            Clear().Append(initialValue);
        }

        public Indenter Clear()
        {
            indents = new Stack<string>();
            output = "";

            return this;
        }

        public Indenter AddIndent(string indent = "    ")
        {
            indents.Push(indent);
            fullIndent += indent;

            return this;
        }

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

        public Indenter AppendOnSameLine(string lines)
        {
            output += lines.Replace("\n", "\n" + fullIndent);
            return this;
        }

        public Indenter Append(string lines)
        {
            if (output.Length > 0)
                output += "\n";
            output += fullIndent;

            return AppendOnSameLine(lines);
        }

        public Indenter Append(IEnumerable<string> lines)
        {
            return lines.Aggregate(this, (ind, line) => ind.Append(line));
        }

        public override string ToString() => output;
    }
}
