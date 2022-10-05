﻿using Gravity.Entities;
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
                new Command("add_entity", AddEntity),
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
                return "add requires exactly 2 arguments";

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

        private string AddEntity(string[] args)
        {
            var g = game as GravityGame;
            var screenManager = g.Services.GetService<ScreenManager>();
            if (screenManager.CurrentScreen is GameplayScreen gameplayScreen)
            {
                if (args.Length != 3)
                    return "add_entity takes exactly 3 arguments";

                var name = args[0];
                Entity? entity = name switch
                {
                    "zombie" => new Zombie(gameplayScreen),
                    "bat" => new Bat(gameplayScreen),
                    _ => null,
                };

                if (entity is Entity)
                {
                    if (!int.TryParse(args[1], out int x))
                        return "Second argument (x) has to be an integer";

                    if (!int.TryParse(args[2], out int y))
                        return "Third argument (y) has to be an integer";

                    entity.Position = new Vector2(x, y);
                    gameplayScreen.AddEntity(entity);
                    return $"Added {name} at x={x}, y={y}";
                }
                else
                    return $"Entity {name} not recognized";

            }
            return "You have to be in GameplayScreen to add an entity";
        }

        public Command? Find(string name)
        {
            return commands.Find(c => c.Name == name);
        }
    }
}