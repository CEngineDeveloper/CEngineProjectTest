using System;

namespace CYM.DevConsole.Command
{
	public class NoValidParserFoundException : CommandSystemException
	{
		private Type type;

		public override string Message => "No valid Parser method found for type " + type.Name + ". Please, review the docs on how to create new Parser methods";

		public NoValidParserFoundException(Type type)
		{
			this.type = type;
		}
	}
}