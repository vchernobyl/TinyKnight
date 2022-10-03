using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace Gravity.UI
{
    public class CommandRegistry
    {
        private readonly Game game;
        private readonly List<Command> commands;

        public CommandRegistry(Game game)
        {
            this.game = game;
            this.commands = new List<Command>
            {
                new Command("clear", Clear),
                new Command("commands", AvailableCommands),
                new Command("add", Add),
            };
        }

        private string Clear(string[] args)
        {
            var console = game.Services.GetService<Console>();
            console.ClearHistory();
            return "";
        }

        // BUG: Multiline output is not printed correctly in the console.
        private string AvailableCommands(string[] args)
        {
            var builder = new StringBuilder();
            foreach (var command in commands)
            {
                builder.Append($"{command.Name}\n");
            }
            return builder.ToString();
        }

        private string Add(string[] args)
        {
            if (args.Length != 2)
                return "`add` requires exactly 2 arguments";

            if (int.TryParse(args[0], out int a) && int.TryParse(args[1], out int b))
                return $"{a + b}";
            else
                return "Arguments have to be integers";
        }

        public Command? Find(string name)
        {
            return commands.Find(c => c.Name == name);
        }
    }
}
