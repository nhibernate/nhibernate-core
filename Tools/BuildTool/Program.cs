using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildTool
{
    class Program
    {
        static int Main(string[] args)
        {
            string command = args[0];

            if (command == "pick-folder")
            {
                string[] folders = Directory.GetDirectories(args[1]);
                for (int i = 0; i < folders.Length; i++)
                {
                    Console.WriteLine((char)('A' + i) + ": " + Path.GetFileName(folders[i]));
                }
                while (true)
                {
                    Console.WriteLine(args[3]);
                    var key = Console.ReadKey();
                    Console.WriteLine();
                    if (key.KeyChar >= 'A' && key.KeyChar <= 'Z' || key.KeyChar >= 'a' && key.KeyChar <= 'z')
                    {
                        int index = key.KeyChar.ToString().ToUpper()[0] - 'A';
                        File.WriteAllText(args[2], folders[index]);
                        break;
                    }
                }

                return 0;
            }
            else if (command == "prompt")
            {
                while(true)
                {
                    Console.WriteLine("[" + string.Join(", ", args[1].ToCharArray().Select(c => c.ToString()).ToArray()) + "]?");
                    char[] characters = args[1].ToLower().ToCharArray();
                    var key = char.ToLower(Console.ReadKey().KeyChar);
                    Console.WriteLine();
                    if (characters.Contains(key))
                        return characters.ToList().IndexOf(key);
                }
            }
            else
            {
                Console.WriteLine("Invalid command: " + command);
                return 255;
            }
        }
    }
}
