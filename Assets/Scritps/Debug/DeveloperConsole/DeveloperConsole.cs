using System;
using System.Collections.Generic;
using System.Linq;

namespace Baks
{
    public class DeveloperConsole
    {
        readonly string prefix;
        readonly IEnumerable<IConsoleCommand> commands;

        public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix)) return;

            inputValue = inputValue.Remove(0, prefix.Length);
            var inputSplit = inputValue.Split(' ');

            var commandInput = inputSplit[0];
            var args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }

        void ProcessCommand(string commandInput, string[] args)
        {
            foreach (var command in commands)
            {
                if (!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (command.Process(args))
                    return;
            }
        }
    }
}
