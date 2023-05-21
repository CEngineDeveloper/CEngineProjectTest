using System;
using System.Collections.Generic;

namespace CYM.DevConsole.Command
{
	public class ParsedArgument
	{
		private struct CastInfo
		{
			public readonly Type type;

			public readonly string name;

			public readonly string fullName;

			public CastInfo(Type type, string name, string fullName)
			{
				this.type = type;
				this.name = name;
				this.fullName = fullName;
			}
		}

		private struct CachedCast
		{
			public readonly string cast;

			public List<CastInfo> nameMatches;

			public CachedCast(string cast)
			{
				this.cast = cast;
				nameMatches = new List<CastInfo>();
			}
		}

		private static CastInfo[] _castInfo;

		private static Dictionary<string, CachedCast> cachedCasts = new Dictionary<string, CachedCast>();

		private static CastInfo[] castInfo
		{
			get
			{
				if (_castInfo == null)
				{
					_castInfo = CreateCastInfo();
				}
				return _castInfo;
			}
		}

		public string raw { get; private set; }

		public string argument { get; private set; }

		public Type type { get; private set; }

		public bool isTypeSpecified => type != null;

		public ParsedArgument(string raw)
		{
			this.raw = raw;
			if (raw.StartsWith("("))
			{
				ParseComplex();
			}
			else
			{
				ParseSimple();
			}
		}

		private void ParseComplex()
		{
			int num = raw.IndexOf(")");
			argument = raw.Substring(num + 1);
			string text = raw.Substring(1, num - 1);
			if (!cachedCasts.ContainsKey(text))
			{
				CreateCachedCast(text);
			}
			type = GetCastType(text);
		}

		private Type GetCastType(string cast)
		{
			CachedCast cachedCast = cachedCasts[cast];
			if (cachedCast.nameMatches.Count == 0)
			{
				throw new ExplicitCastNotFoundException(cast);
			}
			if (cachedCast.nameMatches.Count > 1)
			{
				throw new AmbiguousExplicitCastException(cast, cachedCast.nameMatches.ConvertAll((CastInfo x) => x.type).ToArray());
			}
			return cachedCast.nameMatches[0].type;
		}

		private void CreateCachedCast(string cast)
		{
			CachedCast value = new CachedCast(cast);
			for (int i = 0; i < castInfo.Length; i++)
			{
				if (castInfo[i].name.Equals(cast, StringComparison.OrdinalIgnoreCase))
				{
					value.nameMatches.Add(castInfo[i]);
				}
			}
			if (value.nameMatches.Count == 0)
			{
				for (int j = 0; j < castInfo.Length; j++)
				{
					if (castInfo[j].fullName.Equals(cast, StringComparison.OrdinalIgnoreCase))
					{
						value.nameMatches.Add(castInfo[j]);
						break;
					}
				}
			}
			cachedCasts.Add(cast, value);
		}

		private void ParseSimple()
		{
			type = null;
			argument = raw;
		}

		private static CastInfo[] CreateCastInfo()
		{
			Type[] allTypes = ReflectionFinder.allTypes;
			List<CastInfo> list = new List<CastInfo>();
			foreach (Type type in allTypes)
			{
				string text = type.Name;
				string text2 = type.FullName;
				if (SignatureBuilder.aliases.ContainsKey(type))
				{
					text = (text2 = SignatureBuilder.aliases[type]);
				}
				list.Add(new CastInfo(type, text, text2));
				if (type != typeof(TypedReference) && !type.IsByRef)
				{
					list.Add(new CastInfo(type.MakeArrayType(), text + "[]", text2 + "[]"));
				}
			}
			return list.ToArray();
		}
	}
}