using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildTool
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			var command = args[0];

			switch (command)
			{
				case "pick-folder":
					var folders = Directory.GetDirectories(args[1]);
					for (var i = 0; i < folders.Length; i++)
					{
						Console.WriteLine((char) ('A' + i) + ": " + Path.GetFileName(folders[i]));
					}

					var sb = new StringBuilder();
					for (var i = 3; i < args.Length; i++)
						sb.Append(args[i]);
					var remainingArgs = sb.ToString();

					while (true)
					{
						Console.WriteLine(remainingArgs);
						var key = Console.ReadKey();
						Console.WriteLine();

						if (char.IsLetter(key.KeyChar))
						{
							var index = key.KeyChar.ToString().ToUpper()[0] - 'A';
							File.WriteAllText(args[2], folders[index]);
							break;
						}
					}

					return 0;

				case "prompt":
					var characters = args[1].ToUpper().ToList();
					while (true)
					{
						Console.WriteLine($"[{string.Join(", ", characters)}]?");
						var key = char.ToUpper(Console.ReadKey().KeyChar);
						Console.WriteLine();
						if (characters.Contains(key))
						{
							return characters.IndexOf(key);
						}
					}

				default:
					Console.WriteLine($"Invalid command: {command}");
					return 255;
			}
		}
	}
}
