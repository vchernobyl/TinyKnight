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
                new Command("exit", Exit),
            };
        }

        private string Clear(string[] args)
        {
            var console = game.Services.GetService<Console>();
            console.ClearHistory();
            return "";
        }

        private string AvailableCommands(string[] args)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < commands.Count; i++)
            {
                builder.Append(commands[i].Name);
                if (i != commands.Count - 1)
                    builder.Append("\n");
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

        private string Exit(string[] args)
        {
            game.Exit();
            return "";
        }

        public Command? Find(string name)
        {
            return commands.Find(c => c.Name == name);
        }
    }
}
