using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CYM.DevConsole.Command
{
	public class Command
	{
		private readonly Delegate deleg;

		public readonly MethodInfo method;

		public readonly Signature signature;

		public readonly bool isAnonymous;

		private string _className;

		private string _description;

		public bool useClassName { get; set; }

		public string alias { get; set; }

		public bool hasReturnValue => method.ReturnType != typeof(void);

		public string name
		{
			get
			{
				string text = string.Empty;
				if (useClassName)
				{
					text = className + ".";
				}
				return text + (string.IsNullOrEmpty(alias) ? method.Name : alias);
			}
		}

		public string description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public string className
		{
			get
			{
				if (string.IsNullOrEmpty(_className))
				{
					return method.DeclaringType.Name;
				}
				return _className;
			}
			set
			{
				_className = value;
				useClassName = true;
			}
		}

		public Command(Delegate _deleg)
		{
			deleg = _deleg;
			method = deleg.Method;
			isAnonymous = method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length != 0;
			signature = new Signature(this);
			description = string.Empty;
		}

		public bool IsOverloadOf(ParsedCommand parsedCommand)
		{
			return IsOverloadOf(parsedCommand.command);
		}

		public bool IsOverloadOf(Command command)
		{
			return IsOverloadOf(command.name);
		}

		public bool IsOverloadOf(string commandName)
		{
			return string.Equals(name, commandName, StringComparison.OrdinalIgnoreCase);
		}

		public object Execute(object[] args)
		{
			return deleg.DynamicInvoke(args);
		}

		public override bool Equals(object obj)
		{
			Command command = (Command)obj;
			if (command == null)
			{
				return false;
			}
			return command.deleg == deleg;
		}

		public override int GetHashCode()
		{
			return deleg.GetHashCode();
		}
	}
}