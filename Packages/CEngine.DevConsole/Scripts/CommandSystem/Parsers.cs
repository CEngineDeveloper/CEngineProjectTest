namespace CYM.DevConsole.Command
{
	public static class Parsers
	{
		private const string nullObject = "null";

		[Parser(typeof(object))]
		private static object ParseObject(string value)
		{
			return ParseString(value);
		}

		[Parser(typeof(string))]
		private static string ParseString(string value)
		{
			return value.Equals("null") ? null : value;
		}

		[Parser(typeof(byte))]
		private static byte ParseByte(string value)
		{
			return byte.Parse(value.Trim());
		}

		[Parser(typeof(byte?))]
		private static byte? ParseNullableByte(string value)
		{
			return value.Equals("null") ? null : new byte?(ParseByte(value));
		}

		[Parser(typeof(sbyte))]
		private static sbyte ParseSbyte(string value)
		{
			return sbyte.Parse(value.Trim());
		}

		[Parser(typeof(sbyte?))]
		private static sbyte? ParseNullableSbyte(string value)
		{
			return value.Equals("null") ? null : new sbyte?(ParseSbyte(value));
		}

		[Parser(typeof(short))]
		private static short ParseShort(string value)
		{
			return short.Parse(value.Trim());
		}

		[Parser(typeof(short?))]
		private static short? ParseNullableShort(string value)
		{
			return value.Equals("null") ? null : new short?(ParseShort(value));
		}

		[Parser(typeof(ushort))]
		private static ushort ParseUshort(string value)
		{
			return ushort.Parse(value.Trim());
		}

		[Parser(typeof(ushort?))]
		private static ushort? ParseNullableUshort(string value)
		{
			return value.Equals("null") ? null : new ushort?(ParseUshort(value));
		}

		[Parser(typeof(int))]
		private static int ParseInt(string value)
		{
			return int.Parse(value.Trim());
		}

		[Parser(typeof(int?))]
		private static int? ParseNullableInt(string value)
		{
			return value.Equals("null") ? null : new int?(ParseInt(value));
		}

		[Parser(typeof(uint))]
		private static uint ParseUint(string value)
		{
			return uint.Parse(value.Trim());
		}

		[Parser(typeof(uint?))]
		private static uint? ParseNullableUint(string value)
		{
			return value.Equals("null") ? null : new uint?(ParseUint(value));
		}

		[Parser(typeof(long))]
		private static long ParseLong(string value)
		{
			return long.Parse(value.Trim());
		}

		[Parser(typeof(long?))]
		private static long? ParseNullableLong(string value)
		{
			return value.Equals("null") ? null : new long?(ParseLong(value));
		}

		[Parser(typeof(ulong))]
		private static ulong ParseUlong(string value)
		{
			return ulong.Parse(value.Trim());
		}

		[Parser(typeof(ulong?))]
		private static ulong? ParseNullableUlong(string value)
		{
			return value.Equals("null") ? null : new ulong?(ParseUlong(value));
		}

		[Parser(typeof(float))]
		private static float ParseFloat(string value)
		{
			return float.Parse(value.Trim());
		}

		[Parser(typeof(float?))]
		private static float? ParseNullableFloat(string value)
		{
			return value.Equals("null") ? null : new float?(ParseFloat(value));
		}

		[Parser(typeof(double))]
		private static double ParseDouble(string value)
		{
			return double.Parse(value.Trim());
		}

		[Parser(typeof(double?))]
		private static double? ParseNullableDouble(string value)
		{
			return value.Equals("null") ? null : new double?(ParseDouble(value));
		}

		[Parser(typeof(decimal))]
		private static decimal ParseDecimal(string value)
		{
			return decimal.Parse(value.Trim());
		}

		[Parser(typeof(decimal?))]
		private static decimal? ParseNullableDecimal(string value)
		{
			return value.Equals("null") ? null : new decimal?(ParseDecimal(value));
		}

		[Parser(typeof(bool))]
		private static bool ParseBool(string value)
		{
			bool result = false;
			if (bool.TryParse(value, out result))
			{
				return result;
			}
			if (int.TryParse(value, out var result2))
			{
				return result2 switch
				{
					1 => true,
					0 => false,
					_ => throw new InvalidArgumentFormatException<bool>(value),
				};
			}
			if (value.Equals("yes") || value.Equals("y") || value.Equals("t"))
			{
				return true;
			}
			if (value.Equals("no") || value.Equals("n") || value.Equals("f"))
			{
				return false;
			}
			throw new InvalidArgumentFormatException<bool>(value);
		}

		[Parser(typeof(bool?))]
		private static bool? ParseNullableBool(string value)
		{
			return value.Equals("null") ? null : new bool?(ParseBool(value));
		}

		[Parser(typeof(char))]
		private static char ParseChar(string value)
		{
			return char.Parse(value);
		}

		[Parser(typeof(char?))]
		private static char? ParseNullableChar(string value)
		{
			return value.Equals("null") ? null : new char?(ParseChar(value));
		}
	}
}