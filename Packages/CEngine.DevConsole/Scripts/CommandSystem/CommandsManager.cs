using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CYM.DevConsole.Command
{
	public class CommandsManager
	{
		public delegate void OnExceptionThrown(Exception exception);

		public delegate void OnMessage(string message);

		public delegate void OnCommandModified(Command command);

		private object block = new object();

		private List<Command> commands = new List<Command>();

		private readonly Configuration configuration;

		private readonly ReflectionFinder finder;

		private readonly ArgumentsParser parser;

		private readonly CommandAttributeLoader loader;

		public bool allDataLoaded => parser.dataLoaded;

		public event OnCommandModified onCommandAdded;

		public event OnCommandModified onCommandRemoved;

		public static event OnExceptionThrown onExceptionThrown;

		public static event OnMessage onMessage;

		public CommandsManager(Configuration configuration)
		{
			this.configuration = configuration;
			finder = new ReflectionFinder(configuration);
			parser = new ArgumentsParser(finder, configuration);
			loader = new CommandAttributeLoader(finder);
		}

		public void LoadCommands()
		{
			if (!configuration.allowThreading)
			{
				LoadCommandsInternal();
				return;
			}
			Thread thread = new Thread(LoadCommandsInternal);
			thread.Start();
		}

		private void LoadCommandsInternal()
		{
			Add(loader.LoadCommands());
		}

		public void Add(Command[] commands)
		{
			for (int i = 0; i < commands.Length; i++)
			{
				Add(commands[i]);
			}
		}

		public void Add(Command command)
		{
			lock (block)
			{
				if (!IsCommandAdded(command))
				{
					commands.Add(command);
					if (this.onCommandAdded != null)
					{
						this.onCommandAdded(command);
					}
				}
			}
		}

		public void Remove(Command[] commands)
		{
			for (int i = 0; i < commands.Length; i++)
			{
				Remove(commands[i]);
			}
		}

		public void Remove(Command command)
		{
			RemoveInternal((Command x) => command.Equals(x));
		}

		public void RemoveOverloads(Command[] commands)
		{
			for (int i = 0; i < commands.Length; i++)
			{
				RemoveOverloads(commands[i]);
			}
		}

		public void RemoveOverloads(Command command)
		{
			RemoveInternal((Command x) => command.IsOverloadOf(x));
		}

		public bool IsCommandAdded(Command command)
		{
			return commands.Any((Command x) => command.Equals(x));
		}

		public bool IsCommandOverloadAdded(Command command)
		{
			return commands.Any((Command x) => command.IsOverloadOf(x));
		}

		private void RemoveInternal(Predicate<Command> predicate)
		{
			for (int num = commands.Count - 1; num >= 0; num--)
			{
				if (predicate(commands[num]))
				{
					Command command = commands[num];
					commands.RemoveAt(num);
					if (this.onCommandRemoved != null)
					{
						this.onCommandRemoved(command);
					}
				}
			}
		}

		public Command[] GetCommands()
		{
			return commands.ToArray();
		}

		public object Execute(string text)
		{
			return GetCommandExecuter(text).Execute();
		}

		public CommandExecuter GetCommandExecuter(string text)
		{
			ParsedCommand parsedCommand = new ParsedCommand(text);
			return GetCommandExecuter(parsedCommand);
		}

		public CommandExecuter GetCommandExecuter(ParsedCommand parsedCommand)
		{
			return new CommandExecuter(commands, parsedCommand, parser);
		}

		public static void SendException(Exception exception)
		{
			if (CommandsManager.onExceptionThrown != null)
			{
				CommandsManager.onExceptionThrown(exception);
			}
		}

		public static void SendMessage(string message)
		{
			if (CommandsManager.onMessage != null)
			{
				CommandsManager.onMessage(message);
			}
		}
	}
}