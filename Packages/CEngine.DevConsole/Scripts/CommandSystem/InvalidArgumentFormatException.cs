namespace CYM.DevConsole.Command
{
	public class InvalidArgumentFormatException<T> : CommandSystemException
	{
		private string argument;

		public override string Message => "Argument \"" + argument + "\" cannot be parsed into type " + typeof(T).Name + " because it is not in the correct format";

		public InvalidArgumentFormatException(string argument)
		{
			this.argument = argument;
		}
	}
}