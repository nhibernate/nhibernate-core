using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

namespace BuildTool
{
	public class Program
	{
		public static int Main(string[] args)
		{
			string command = args[0];
			switch (command)
			{
				case "pick-folder":
					string[] folders = Directory.GetDirectories(args[1]);
					for (int i = 0; i < folders.Length; i++)
					{
						WriteLine((char) ('A' + i) + ": " + Path.GetFileName(folders[i]));
					}

					var sb = new StringBuilder();
					for (int i = 3; i < args.Length; i++)
						sb.Append(args[i]);
					while (true)
					{
						WriteLine(sb.ToString());
						ConsoleKeyInfo key = ReadKey();
						WriteLine();

						if (char.IsLetter(key.KeyChar))
						{
							int index = key.KeyChar.ToString().ToUpper()[0] - 'A';
							File.WriteAllText(args[2], folders[index]);
							break;
						}
					}

					return 0;
				case "prompt":
					List<char> characters = args[1].ToUpper().ToCharArray().ToList();
					while (true)
					{
						WriteLine($"[{string.Join(", ", characters).ToUpper()}]?");
						char key = char.ToUpper(ReadKey().KeyChar);
						WriteLine();
						if (characters.Contains(key))
						{
							return characters.IndexOf(key);
						}
					}
				default:
					WriteLine($"Invalid command: {command}");
					return 255;
			}
		}
	}
}
