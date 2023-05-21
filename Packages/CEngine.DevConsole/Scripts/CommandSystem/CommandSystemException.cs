using System;

namespace CYM.DevConsole.Command
{
	public class CommandSystemException : Exception
	{
		public CommandSystemException()
		{
		}

		public CommandSystemException(Exception innerException)
			: base(string.Empty, innerException)
		{
		}
	}
}