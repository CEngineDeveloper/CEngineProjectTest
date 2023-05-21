using System;

namespace CYM.DevConsole.Command
{
	internal static class FlagsEnumExtensions
	{
		public static bool HasFlag(this Enum variable, Enum value)
		{
			return (Convert.ToUInt32(variable) & Convert.ToUInt32(value)) == Convert.ToUInt32(value);
		}
	}
}