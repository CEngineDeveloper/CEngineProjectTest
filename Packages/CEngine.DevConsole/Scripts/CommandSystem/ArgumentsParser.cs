using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CYM.DevConsole.Command
{

	internal class ArgumentsParser
	{
		private Dictionary<Type, MethodInfo> parsers;

		private ReflectionFinder finder;

		public bool dataLoaded { get; private set; }

		public ArgumentsParser(ReflectionFinder finder, Configuration configuration)
		{
			this.finder = finder;
			if (configuration.allowThreading)
			{
				new Thread(Load).Start();
			}
			else
			{
				Load();
			}
		}

		private void Load()
		{
			parsers = new Dictionary<Type, MethodInfo>();
			Type[] userClassesAndStructs = finder.GetUserClassesAndStructs();
			for (int i = 0; i < userClassesAndStructs.Length; i++)
			{
				MethodInfo[] methods = userClassesAndStructs[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.Length; j++)
				{
					object[] customAttributes = methods[j].GetCustomAttributes(typeof(ParserAttribute), inherit: false);
					if (customAttributes.Length != 0)
					{
						ParserAttribute parserAttribute = (ParserAttribute)customAttributes[0];
						if (!parsers.ContainsKey(parserAttribute.type))
						{
							parsers.Add(parserAttribute.type, methods[j]);
						}
						else
						{
							CommandsManager.SendException(new DuplicatedParserException(parserAttribute));
						}
					}
				}
			}
			dataLoaded = true;
			CommandsManager.SendMessage("Loaded " + parsers.Count + " parsers:\n" + string.Join("\n", parsers.ToList().ConvertAll((KeyValuePair<Type, MethodInfo> x) => x.Key.Namespace + "." + SignatureBuilder.TypeToString(x.Key)).ToArray()));
		}

		public object Parse(ParsedArgument argument, Type type)
		{
			if (argument.type != null && argument.type != type)
			{
				throw new ExplicitCastMismatchException(argument.type, type);
			}
			return Parse(argument.argument, type);
		}

		private object Parse(string value, Type type)
		{
			if (type.IsEnum)
			{
				return Enum.Parse(type, value);
			}
			if (type.IsArray)
			{
				return HandleArrayType(value, type);
			}
			if (HasParserForType(type))
			{
				return CallParser(type, value);
			}
			throw new NoValidParserFoundException(type);
		}

		private object HandleArrayType(string value, Type type)
		{
			ParsedCommand parsedCommand = new ParsedCommand("command " + value);
			Array array = (Array)Activator.CreateInstance(type, parsedCommand.args.Length);
			for (int i = 0; i < parsedCommand.args.Length; i++)
			{
				array.SetValue(Parse(parsedCommand.args[i], type.GetElementType()), i);
			}
			return array;
		}

		private bool HasParserForType(Type type)
		{
			return parsers.ContainsKey(type);
		}

		private object CallParser(Type type, string value)
		{
			return parsers[type].Invoke(null, new object[1] { value });
		}
	}
}