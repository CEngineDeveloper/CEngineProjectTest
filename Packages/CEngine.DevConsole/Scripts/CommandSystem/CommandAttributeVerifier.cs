using System;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	internal class CommandAttributeVerifier
	{
		private MethodInfo method;

		private CommandAttribute attribute;

		public bool hasCommandAttribute => attribute != null;

		public bool isDeclarationSupported => method.IsStatic && !method.IsGenericMethod && !method.IsGenericMethodDefinition;

		public CommandAttributeVerifier(MethodInfo method)
		{
			this.method = method;
			object[] customAttributes = method.GetCustomAttributes(typeof(CommandAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				attribute = customAttributes[0] as CommandAttribute;
			}
		}

		public Command ExtractCommand()
		{
			if (!isDeclarationSupported)
			{
				throw new UnsupportedCommandDeclarationException(method);
			}
			Command command = (Command)Activator.CreateInstance(typeof(MethodInfoCommand), method);
			command.alias = attribute.alias;
			command.description = attribute.description;
			command.className = attribute.className;
			command.useClassName = attribute.useClassName || !string.IsNullOrEmpty(attribute.className);
			return command;
		}
	}
}