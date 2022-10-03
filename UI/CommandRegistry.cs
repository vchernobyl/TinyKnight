using System.Collections.Generic;

namespace Gravity.UI
{
    public class CommandRegistry
    {
        private readonly List<Command> commands;

        public CommandRegistry()
        {
            commands = new List<Command>
            {
                new Command("clear", (string[] args) => { return "Clear command was invoked"; })
            };
        }

        public Command? Find(string name)
        {
            return commands.Find(c => c.Name == name);
        }
    }
}
