using System;
using System.Runtime.CompilerServices;

namespace SimpleTesting.TestAdapter
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FactAttribute : Attribute
    {
        public string File { get; }

        public int Line { get; }

        public FactAttribute([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            this.File = file;
            this.Line = line;
        }
    }
}