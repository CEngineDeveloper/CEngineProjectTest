using System;
using System.Collections.Generic;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	public class CommandExecuter
	{
		public struct Match
		{
			public readonly Command command;

			public readonly object[] parameters;

			public Match(Command command, object[] parameters)
			{
				this.command = command;
				this.parameters = parameters;
			}
		}

		private readonly List<Command> commands;

		private readonly ParsedCommand parsedCommand;

		private List<Command> overloads = new List<Command>();

		private List<Match> matches = new List<Match>();

		public bool isValidCommand => overloads.Count >= 1;

		public bool canBeExecuted => matches.Count == 1;

		public bool hasReturnValue => canBeExecuted && matches[0].command.hasReturnValue;

		internal CommandExecuter(List<Command> commands, ParsedCommand parsedCommand, ArgumentsParser parser)
		{
			this.commands = commands;
			this.parsedCommand = parsedCommand;
			FilterOverloads();
			FilterMatches(parser);
		}

		private void FilterOverloads()
		{
			for (int i = 0; i < commands.Count; i++)
			{
				if (commands[i].IsOverloadOf(parsedCommand))
				{
					overloads.Add(commands[i]);
				}
			}
		}

		private void FilterMatches(ArgumentsParser parser)
		{
			for (int i = 0; i < overloads.Count; i++)
			{
				try
				{
					if (overloads[i].signature.Matches(parsedCommand.args))
					{
						object[] parameters = overloads[i].signature.Convert(parsedCommand.args, parser);
						matches.Add(new Match(overloads[i], parameters));
					}
				}
				catch (TargetInvocationException)
				{
				}
				catch (CommandSystemException)
				{
				}
			}
		}

		public object Execute()
		{
			try
			{
				Match match = GetMatch();
				return match.command.Execute(match.parameters);
			}
			catch (Exception exception)
			{
				CommandsManager.SendException(exception);
				return null;
			}
		}

		public Command[] GetOverloads()
		{
			return overloads.ToArray();
		}

		public Match GetMatch()
		{
			if (!isValidCommand)
			{
				throw new CommandNotFoundException(parsedCommand);
			}
			if (matches.Count == 0)
			{
				throw new MatchNotFoundException(parsedCommand, overloads.ToArray());
			}
			if (matches.Count > 1)
			{
				throw new AmbiguousCommandCallException(parsedCommand, matches.ConvertAll((Match x) => x.command).ToArray());
			}
			return matches[0];
		}
	}
}