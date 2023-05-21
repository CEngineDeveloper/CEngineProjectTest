using System;
using System.Text;

namespace CYM.DevConsole.Command
{
	public class AmbiguousExplicitCastException : CommandSystemException
	{
		private string cast;

		private Type[] conflicts;

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < conflicts.Length; i++)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append(conflicts[i].FullName);
				}
				return $"The explicit cast '{cast}' is ambiguous between the following types:{stringBuilder.ToString()}\nPlease, refer to the Full Name of the type when casting again.";
			}
		}

		public AmbiguousExplicitCastException(string cast, Type[] conflicts)
		{
			this.cast = cast;
			this.conflicts = conflicts;
		}
	}
}