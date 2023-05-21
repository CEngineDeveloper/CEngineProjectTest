using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	public class MethodInfoCommand : Command
	{
		public MethodInfoCommand(MethodInfo method)
			: base(MakeDelegate(method))
		{
		}

		private static Delegate MakeDelegate(MethodInfo method)
		{
			Type[] array = method.GetParameters().ToList().ConvertAll((ParameterInfo x) => x.ParameterType)
				.ToArray();
			Type type;
			if (method.ReturnType == typeof(void))
			{
				type = Expression.GetActionType(array);
			}
			else
			{
				array = array.Concat(new Type[1] { method.ReturnType }).ToArray();
				type = Expression.GetFuncType(array);
			}
			return Delegate.CreateDelegate(type, method);
		}
	}
}