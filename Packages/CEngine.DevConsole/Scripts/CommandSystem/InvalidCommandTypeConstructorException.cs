using System;

namespace CYM.DevConsole.Command
{
	public class InvalidCommandTypeConstructorException : CommandSystemException
	{
		private Type type;

		public override string Message => type.Name + " does not have a valid constructor. Please, review the docs on how to create new Command types";

		public InvalidCommandTypeConstructorException(Type type)
		{
			this.type = type;
		}
	}
}