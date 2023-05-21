using System.Text;

namespace CYM.DevConsole.Command
{
	public class AmbiguousCommandCallException : CommandSystemException
	{
		private ParsedCommand command;

		private Command[] matches;

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < matches.Length; i++)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append(matches[i].signature.raw);
				}
				return $"The command call '{command.raw}' is ambiguous between the following commands:{stringBuilder.ToString()}";
			}
		}

		public AmbiguousCommandCallException(ParsedCommand command, Command[] matches)
		{
			this.command = command;
			this.matches = matches;
		}
	}
}