using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	public class Signature
	{
		private Command command;

		private string _raw;

		public ParameterInfo[] parameters { get; private set; }

		public string raw
		{
			get
			{
				if (_raw == null)
				{
					_raw = SignatureBuilder.Build(command.method, command.name);
				}
				return _raw;
			}
		}

		internal Signature(Command command)
		{
			this.command = command;
			List<ParameterInfo> list = new List<ParameterInfo>(command.method.GetParameters());
			//list.RemoveAll((ParameterInfo x) => x.ParameterType == typeof(excu));
			parameters = list.ToArray();
		}

		internal bool Matches(ParsedArgument[] args)
		{
			return args.Length >= parameters.Count((ParameterInfo x) => !x.IsOptional) && args.Length <= parameters.Length;
		}

		internal object[] Convert(ParsedArgument[] args, ArgumentsParser parser)
		{
			object[] array = new object[parameters.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (args.Length > i)
				{
					array[i] = parser.Parse(args[i], parameters[i].ParameterType);
				}
				else
				{
					array[i] = parameters[i].DefaultValue;
				}
			}
			return array;
		}
	}
}