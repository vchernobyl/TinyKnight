using Gravity.Entities;
using Gravity.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gravity.UI
{
    // TODO: A few things can be improved about command structure.
    // 1. Not every command is going to need to write an output to a console.
    // Some commands only output errors and nothing on success (like for example `clear`).
    // 2. Commands can change name, but currently these don't change automatically in
    // error messages and need to be adjusted by hand.
    // 3. Every command can accept a certain number of arguments, validation of argument
    // count can also be automated.
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
                new Command("exit", Exit),
                new Command("add", AddEntity),
                new Command("solids", ToggleSolids),
                new Command("colliders", ToggleColliders),
                new Command("history", PrintHistory),
                new Command("weapon", GiveWeapon),
                new Command("load", LoadScreen),
                new Command("timescale", SetTimeScale),
                new Command("spawn", ToggleSpawn),
            };
        }

        private string Clear(string[] args)
        {
            var console = game.Services.GetService<Console>();
            console.Clear();
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

        private string Exit(string[] args)
        {
            game.Exit();
            return "";
        }

        private static GameplayScreen? GetGameplayScreen(ScreenManager screenManager)
        {
            return (GameplayScreen?)screenManager
                .GetScreens()
                .FirstOrDefault(s => s is GameplayScreen);
        }

        private string AddEntity(string[] args)
        {
            var g = game as GravityGame;
            var screenManager = g.Services.GetService<ScreenManager>();
            var gameplayScreen = GetGameplayScreen(screenManager);
            if (gameplayScreen != null)
            {
                if (args.Length != 3)
                    return "spawn takes exactly 3 arguments";

                var name = args[0];
                Entity? entity = name switch
                {
                    "zombie" => new Zombie(gameplayScreen),
                    "bat" => new Bat(gameplayScreen),
                    "demon" => new Demon(gameplayScreen),
                    "ghost" => new Ghost(gameplayScreen),
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
            return "You have to be in GameplayScreen to spawn an entity";
        }

        public Command? Find(string name)
        {
            return commands.Find(c => c.Name == name);
        }

        private static bool? ToBoolean(string arg)
        {
            return arg switch
            {
                "on" => true,
                "off" => false,
                _ => null,
            };
        }

        private string ToggleSolids(string[] arg)
        {
            if (arg.Length != 1)
                return "solids takes exactly 1 argument";

            var toggle = ToBoolean(arg[0]);
            if (ToBoolean(arg[0]) is bool flag)
            {
                DebugInfo.ShowSolids = flag;
                return $"solids is {arg[0]}";
            }

            return $"solids: invalid argument {toggle}";
        }

        private string ToggleColliders(string[] arg)
        {
            if (arg.Length != 1)
                return "collider takes exactly 1 argument";

            var toggle = ToBoolean(arg[0]);
            if (ToBoolean(arg[0]) is bool flag)
            {
                DebugInfo.ShowEntityColliders = flag;
                return $"colliders is {arg[0]}";
            }

            return $"colliders: invalid argument {toggle}";
        }

        private string PrintHistory(string[] ars)
        {
            var console = game.Services.GetService<Console>();
            var builder = new StringBuilder();
            for (int i = 0; i < console.History.Count; i++)
            {
                builder.Append(console.History[i]);
                if (i != console.History.Count - 1)
                    builder.Append("\n");
            }
            return builder.ToString();
        }

        private string GiveWeapon(string[] args)
        {
            var screenManager = game.Services.GetService<ScreenManager>();
            var gameplayScreen = GetGameplayScreen(screenManager);
            if (gameplayScreen != null)
            {
                if (args.Length != 1)
                    return "command takes exactly 1 argument";

                var hero = gameplayScreen.Hero;
                Weapon? weapon = args[0] switch
                {
                    "axe" => new Axe(hero, gameplayScreen),
                    "crossbow" => new Crossbow(hero, gameplayScreen),
                    _ => null,
                };

                if (weapon is Weapon)
                {
                    hero.EquipWeapon(weapon);
                    return $"{args[0]} equiped";
                }

                return $"Unknown weapon {args[0]}";
            }
            return "You have to be in GameplayScreen to execute this command";
        }

        private string LoadScreen(string[] args)
        {
            if (args.Length != 1)
                return "load takes exactly 1 argument";

            var screenManager = game.Services.GetService<ScreenManager>();
            GameScreen? screen = args[0] switch
            {
                "gameplay" => new GameplayScreen(),
                "menu" => new MainMenuScreen(),
                _ => null,
            };

            foreach (var s in screenManager.GetScreens())
                s.ExitScreen();

            if (screen != null)
            {
                screenManager.AddScreen(screen);
                return $"{screen} loaded";
            }
            else
                return $"screen {screen} not found";
        }

        private string SetTimeScale(string[] args)
        {
            if (args.Length != 1)
                return "timescale takes exactly 1 argument";

            if (float.TryParse(args[0], NumberStyles.Number, 
                CultureInfo.InvariantCulture.NumberFormat, out float s))
            {
                game.TargetElapsedTime = TimeSpan.FromSeconds(0.0166667f * s);
                return $"timescale set to {s}";
            }

            return "timescale argument has to be a number";
        }

        private string ToggleSpawn(string[] args)
        {
            if (args.Length != 1)
                return "spawn takes exactly 1 argument";

            var value = args[0];
            if (ToBoolean(value) is bool on)
            {
                var gameplay = GetGameplayScreen(game.Services.GetService<ScreenManager>());
                if (gameplay == null)
                    return "gameplay screen needs to be active";

                if (on)
                    gameplay.StartEnemySpawn();
                else
                    gameplay.StopEnemySpawn();

                return $"enemy spawn {value}";
            }

            return $"invalid argument {value}";
        }
    }
}
