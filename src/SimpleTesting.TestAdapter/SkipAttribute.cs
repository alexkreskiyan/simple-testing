using System;
using System.Runtime.CompilerServices;

namespace SimpleTesting.TestAdapter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipAttribute : Attribute
    {
        public string File { get; }

        public int Line { get; }

        public SkipAttribute([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            this.File = file;
            this.Line = line;
        }
    }
}