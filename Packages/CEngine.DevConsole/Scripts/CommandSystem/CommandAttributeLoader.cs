using System;
using System.Collections.Generic;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	internal class CommandAttributeLoader
	{
		private List<Command> commands = new List<Command>();

		private Type[] types;

		public CommandAttributeLoader(ReflectionFinder finder)
		{
			types = finder.GetUserClassesAndStructs();
		}

		public Command[] LoadCommands()
		{
			for (int i = 0; i < types.Length; i++)
			{
				commands.AddRange(LoadCommandsInType(types[i]));
			}
			return commands.ToArray();
		}

		private Command[] LoadCommandsInType(Type type)
		{
			List<Command> list = new List<Command>();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.Length; i++)
			{
				CommandAttributeVerifier commandAttributeVerifier = new CommandAttributeVerifier(methods[i]);
				if (commandAttributeVerifier.hasCommandAttribute)
				{
					if (!commandAttributeVerifier.isDeclarationSupported)
					{
						CommandsManager.SendException(new UnsupportedCommandDeclarationException(methods[i]));
						continue;
					}
					Command item = commandAttributeVerifier.ExtractCommand();
					list.Add(item);
				}
			}
			return list.ToArray();
		}
	}
}