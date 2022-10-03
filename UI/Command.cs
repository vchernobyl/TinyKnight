using System;

namespace Gravity.UI
{
    public class Command
    {
        public readonly string Name;
        public readonly Func<string[], string> Procedure;

        public Command(string name, Func<string[], string> procedure)
        {
            Name = name;
            Procedure = procedure;
        }
    }
}
