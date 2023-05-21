using System;

namespace CYM.DevConsole.Command
{
	public class FuncCommand<TResult> : Command
	{
		public FuncCommand(Func<TResult> method)
			: base(method)
		{
		}
	}
	public class FuncCommand<T1, TResult> : Command
	{
		public FuncCommand(Func<T1, TResult> method)
			: base(method)
		{
		}
	}
	public class FuncCommand<T1, T2, TResult> : Command
	{
		public FuncCommand(Func<T1, T2, TResult> method)
			: base(method)
		{
		}
	}
	public class FuncCommand<T1, T2, T3, TResult> : Command
	{
		public FuncCommand(Func<T1, T2, T3, TResult> method)
			: base(method)
		{
		}
	}
	public class FuncCommand<T1, T2, T3, T4, TResult> : Command
	{
		public FuncCommand(Func<T1, T2, T3, T4, TResult> method)
			: base(method)
		{
		}
	}
}