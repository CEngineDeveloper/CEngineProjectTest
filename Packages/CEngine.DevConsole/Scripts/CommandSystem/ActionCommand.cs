using System;

namespace CYM.DevConsole.Command
{
	public class ActionCommand : Command
	{
		public ActionCommand(Action method)
			: base(method)
		{
		}
	}
	public class ActionCommand<T1> : Command
	{
		public ActionCommand(Action<T1> method)
			: base(method)
		{
		}
	}
	public class ActionCommand<T1, T2> : Command
	{
		public ActionCommand(Action<T1, T2> method)
			: base(method)
		{
		}
	}
	public class ActionCommand<T1, T2, T3> : Command
	{
		public ActionCommand(Action<T1, T2, T3> method)
			: base(method)
		{
		}
	}
	public class ActionCommand<T1, T2, T3, T4> : Command
	{
		public ActionCommand(Action<T1, T2, T3, T4> method)
			: base(method)
		{
		}
	}
}