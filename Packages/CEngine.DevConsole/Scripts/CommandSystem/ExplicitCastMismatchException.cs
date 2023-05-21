using System;

namespace CYM.DevConsole.Command
{
	public class ExplicitCastMismatchException : CommandSystemException
	{
		private Type castType;

		private Type argumentType;

		public override string Message => $"The argument needs a {argumentType.Name}, whereas the cast was made to {castType.Name}";

		public ExplicitCastMismatchException(Type castType, Type argumentType)
		{
			this.castType = castType;
			this.argumentType = argumentType;
		}
	}
}