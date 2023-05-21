using System.Collections.Generic;

namespace CYM.DevConsole.Command
{
	public class ParsedCommand
	{
		private static readonly char[] groupifiers = new char[2] { '\'', '"' };

		private const char separator = ' ';

		public string raw { get; private set; }

		public string command { get; private set; }

		public ParsedArgument[] args { get; private set; }

		public ParsedCommand(string raw)
		{
			this.raw = raw;
			GetCommand();
			GetArgs();
		}

		private void GetCommand()
		{
			string[] array = raw.Split(' ');
			command = array[0];
		}

		private void GetArgs()
		{
			string text = raw.Substring(command.Length).Trim();
			List<string> list = new List<string>();
			char? c = null;
			string text2 = string.Empty;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == ' ' && !c.HasValue)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
						text2 = string.Empty;
					}
					continue;
				}
				bool flag = false;
				for (int j = 0; j < groupifiers.Length; j++)
				{
					if (text[i] == groupifiers[j])
					{
						flag = true;
						if (!c.HasValue)
						{
							c = groupifiers[j];
						}
						else if (c == text[i])
						{
							list.Add(text2);
							text2 = string.Empty;
							c = null;
						}
					}
				}
				if (!flag)
				{
					text2 += text[i];
				}
			}
			if (text2 != string.Empty)
			{
				list.Add(text2);
			}
			args = list.ConvertAll((string x) => new ParsedArgument(x)).ToArray();
		}
	}
}