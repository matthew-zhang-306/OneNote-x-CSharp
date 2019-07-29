using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNote_x_CSharp
{

    public class Indenter
    {
        List<string> indents;
        string fullIndent;

        string output;

        public Indenter(string initialValue = "")
        {
            Clear().Append(initialValue);
        }

        public Indenter AddIndent(string indent = "    ")
        {
            indents.Add(indent);
            fullIndent += indent;

            return this;
        }

        public Indenter RemoveIndent()
        {
            fullIndent = fullIndent.Substring(0, fullIndent.Length - indents[indents.Count - 1].Length);
            indents.RemoveAt(indents.Count - 1);
            
            return this;
        }

        public Indenter AppendSameLine(string lines)
        {
            output += lines.Replace("\n", "\n" + fullIndent);
            return this;
        }

        public Indenter Append(string lines)
        {
            if (output.Length > 0)
                output += "\n";
            output += fullIndent;

            return AppendSameLine(lines);
        }

        public Indenter Append(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                Append(line);
            }

            return this;
        }

        public Indenter Clear()
        {
            indents = new List<string>();
            output = "";

            return this;
        }

        public override string ToString() => output;
    }
}
