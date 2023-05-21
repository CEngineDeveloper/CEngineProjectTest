using System;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	public class CommandBuildingException : CommandSystemException
	{
		private Type type;

		private MemberInfo member;

		public override string Message => string.Concat("There was an error creating a command for ", member.MemberType, " ", member.Name, " of type ", type.Name, ". Skipping command.\nError Message: ", base.InnerException.Message, "\nStackTrace", base.InnerException.StackTrace);

		public CommandBuildingException(Type type, MemberInfo member, Exception innerException)
			: base(innerException)
		{
			this.type = type;
			this.member = member;
		}
	}
}