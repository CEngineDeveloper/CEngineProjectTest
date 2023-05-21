using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	internal class ReflectionFinder
	{
		private Configuration configuration;

		private Type[] userTypes;

		private static Type[] _allTypes;

		public static Type[] allTypes
		{
			get
			{
				if (_allTypes == null)
				{
					_allTypes = LoadAllTypes().ToArray();
				}
				return _allTypes;
			}
		}

		public static Type[] enumTypes => allTypes.Where((Type x) => x.IsEnum).ToArray();

		public ReflectionFinder(Configuration configuration)
		{
			this.configuration = configuration;
			userTypes = LoadUserTypes().ToArray();
		}

		private static List<Type> LoadAllTypes()
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				list.AddRange(assemblies[i].GetTypes());
			}
			return list;
		}

		private List<Type> LoadUserTypes()
		{
			List<Type> list = new List<Type>();
			Assembly[] assembliesWithCommands = GetAssembliesWithCommands();
			CommandsManager.SendMessage("Loading CommandSystem data from: " + string.Join(", ", assembliesWithCommands.ToList().ConvertAll(delegate (Assembly x)
			{
				AssemblyName name = x.GetName();
				string codeBase = name.CodeBase;
				string text = codeBase.Substring(codeBase.LastIndexOf('.'));
				return name.Name + text;
			}).ToArray()) + ".");
			for (int i = 0; i < assembliesWithCommands.Length; i++)
			{
				list.AddRange(assembliesWithCommands[i].GetTypes());
			}
			return list;
		}

		public Type[] GetUserClassesAndStructs()
		{
			return userTypes.Where((Type x) => x.IsClass || (x.IsValueType && !x.IsEnum)).ToArray();
		}

		private Assembly[] GetAssembliesWithCommands()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			string[] registeredAssemblies = configuration.registeredAssemblies;
			for (int i = 0; i < registeredAssemblies.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < assemblies.Length; j++)
				{
					if (assemblies[j].GetName().Name == registeredAssemblies[i])
					{
						flag = true;
						list.Add(assemblies[j]);
						break;
					}
				}
				if (!flag)
				{
					try
					{
						Assembly item = Assembly.Load(new AssemblyName(registeredAssemblies[i]));
						list.Add(item);
					}
					catch
					{
						CommandsManager.SendMessage("Assembly with name '" + registeredAssemblies[i] + "' could not be found. Please, make sure the assembly is properly loaded");
					}
				}
			}
			return list.ToArray();
		}
	}
}