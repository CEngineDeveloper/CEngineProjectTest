using System.Reflection;

namespace CYM.DevConsole.Command
{
	public class UnsupportedCommandDeclarationException : CommandSystemException
	{
		private MethodInfo method;

		private string methodPath => string.Concat(method.DeclaringType, ".", method.Name);

		private string header => "The command " + methodPath;

		private string tail => "Please, review the docs on how to create new Commands";

		public override string Message
		{
			get
			{
				if (!method.IsStatic)
				{
					return header + " is not static. Only static commands are supported. " + tail;
				}
				if (method.IsGenericMethod || method.IsGenericMethodDefinition)
				{
					return header + " is generic, which is not yet supported. " + tail;
				}
				return header + " declaration is unsupported. " + tail;
			}
		}

		public UnsupportedCommandDeclarationException(MethodInfo method)
		{
			this.method = method;
		}
	}
}