using System;
using System.Collections.Generic;

namespace CYM.DevConsole.Command
{
	internal static class BuiltInCommands
	{
		[Command]
		private static string GetEnumValues(string enumName)
		{
			string text = string.Empty;
			Type[] enumTypes = ReflectionFinder.enumTypes;
			List<Type> list = new List<Type>();
			for (int i = 0; i < enumTypes.Length; i++)
			{
				if (enumTypes[i].Name.Equals(enumName, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(enumTypes[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				string[] names = Enum.GetNames(list[j]);
				text = text + list[j].FullName.Replace('+', '.') + ":\n";
				for (int k = 0; k < names.Length; k++)
				{
					text = text + "\t" + names[k] + "\n";
				}
			}
			return text;
		}
	}
}