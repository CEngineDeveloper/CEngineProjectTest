namespace CYM.DevConsole.Command
{
	public class ExplicitCastNotFoundException : CommandSystemException
	{
		private string cast;

		public override string Message => $"There is no suitable Type for the explicit cast '{cast}'";

		public ExplicitCastNotFoundException(string cast)
		{
			this.cast = cast;
		}
	}
}