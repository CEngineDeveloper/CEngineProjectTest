namespace CYM.DevConsole.Command
{
	public class DuplicatedParserException : CommandSystemException
	{
		private ParserAttribute parser;

		public override string Message => string.Concat("More than one Parser was specified for type ", parser.type, ".Please, note that most common types already have a built-in Parser");

		public DuplicatedParserException(ParserAttribute parser)
		{
			this.parser = parser;
		}
	}
}