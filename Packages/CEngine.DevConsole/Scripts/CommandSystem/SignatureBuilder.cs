using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CYM.DevConsole.Command.Unity;
namespace CYM.DevConsole.Command
{
	internal static class SignatureBuilder
	{
		public static readonly Dictionary<Type, string> aliases = new Dictionary<Type, string>
	{
		{
			typeof(byte),
			"byte"
		},
		{
			typeof(sbyte),
			"sbyte"
		},
		{
			typeof(short),
			"short"
		},
		{
			typeof(ushort),
			"ushort"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(uint),
			"uint"
		},
		{
			typeof(long),
			"long"
		},
		{
			typeof(ulong),
			"ulong"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(object),
			"object"
		},
		{
			typeof(bool),
			"bool"
		},
		{
			typeof(char),
			"char"
		},
		{
			typeof(string),
			"string"
		},
		{
			typeof(byte?),
			"byte?"
		},
		{
			typeof(sbyte?),
			"sbyte?"
		},
		{
			typeof(short?),
			"short?"
		},
		{
			typeof(ushort?),
			"ushort?"
		},
		{
			typeof(int?),
			"int?"
		},
		{
			typeof(uint?),
			"uint?"
		},
		{
			typeof(long?),
			"long?"
		},
		{
			typeof(ulong?),
			"ulong?"
		},
		{
			typeof(float?),
			"float?"
		},
		{
			typeof(double?),
			"double?"
		},
		{
			typeof(decimal?),
			"decimal?"
		},
		{
			typeof(bool?),
			"bool?"
		},
		{
			typeof(char?),
			"char?"
		}
	};

		public static string Build(MethodInfo method, string name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (method.ReturnType != typeof(void))
			{
				string value = TypeToString(method.ReturnType);
				stringBuilder.Append(value);
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(name);
			List<ParameterInfo> list = new List<ParameterInfo>(method.GetParameters());
			//list.RemoveAll((ParameterInfo x) => x.ParameterType == typeof(ExecutionScope));
			if (list.Count > 0)
			{
				AddParameters(stringBuilder, list.ToArray());
			}
			return stringBuilder.ToString();
		}

		private static void AddParameters(StringBuilder signature, ParameterInfo[] parameters)
		{
			signature = signature.Append('(');
			for (int i = 0; i < parameters.Length; i++)
			{
				//if (parameters[i].ParameterType != typeof(ExecutionScope))
				//{
				//	AddParameter(signature, parameters[i]);
				//	if (i != parameters.Length - 1)
				//	{
				//		signature = signature.Append(", ");
				//	}
				//}
			}
			signature = signature.Append(')');
		}

		private static void AddParameter(StringBuilder signature, ParameterInfo parameter)
		{
			signature = signature.Append(TypeToString(parameter.ParameterType));
			if (!string.IsNullOrEmpty(parameter.Name))
			{
				signature = signature.Append(" ");
				signature = signature.Append(parameter.Name);
			}
			if (parameter.IsOptional)
			{
				signature = signature.Append(" = ");
				signature = ((!(parameter.DefaultValue is string)) ? signature.Append(parameter.DefaultValue) : signature.AppendFormat("\"{0}\"", parameter.DefaultValue));
			}
		}

		public static string TypeToString(Type type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (aliases.ContainsKey(type))
			{
				stringBuilder = stringBuilder.Append(aliases[type]);
			}
			else if (type.IsArray)
			{
				stringBuilder = stringBuilder.Append(TypeToString(type.GetElementType())).Append("[]");
			}
			else if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				stringBuilder = stringBuilder.Append(type.Name.Substring(0, type.Name.IndexOf('`'))).Append("<");
				for (int i = 0; i < genericArguments.Length; i++)
				{
					stringBuilder = stringBuilder.Append(TypeToString(genericArguments[i]));
					if (i != genericArguments.Length - 1)
					{
						stringBuilder = stringBuilder.Append(", ");
					}
				}
				stringBuilder = stringBuilder.Append(">");
			}
			else
			{
				stringBuilder = stringBuilder.Append(type.Name);
			}
			return stringBuilder.ToString();
		}
	}
}