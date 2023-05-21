using System.Text;

namespace CYM.DevConsole.Command
{
	public class MatchNotFoundException : CommandSystemException
	{
		private ParsedCommand command;

		private Command[] overloads;

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < overloads.Length; i++)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append(overloads[i].signature.raw);
				}
				return $"No match found between command '{command.raw}' and any of its overloads:{stringBuilder.ToString()}";
			}
		}

		public MatchNotFoundException(ParsedCommand command, Command[] overloads)
		{
			this.command = command;
			this.overloads = overloads;
		}
	}
}