namespace CYM.DevConsole.Command
{
	public class CommandNotFoundException : CommandSystemException
	{
		private ParsedCommand command;

		public override string Message => "No command found with name '" + command.command + "'";

		public CommandNotFoundException(ParsedCommand command)
		{
			this.command = command;
		}
	}
}