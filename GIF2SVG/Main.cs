using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIF2SVG
{
    public class Main
    {
        public Main() { Start(); }

        private void Start()
        {
            string[] options = new string[] { "Single gif conversion", "Multiple gifs conversion" };

            int currentSelection = 0;

            Console.CursorVisible = false;

            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.WriteLine("########## GIF 2 SVG Converter ##########");
                Console.WriteLine("Select an option");

                if (currentSelection == 0)
                {
                    Console.WriteLine("> " + options[0]);
                    Console.WriteLine(options[1]);
                }
                else
                {
                    Console.WriteLine(options[0]);
                    Console.WriteLine("> " + options[1]);
                }

                key = Console.ReadKey(true).Key;

                if (key != ConsoleKey.UpArrow && key != ConsoleKey.DownArrow) continue;
                currentSelection = currentSelection == 0 ? 1 : 0;

            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            Console.Clear();
            Console.WriteLine("########## GIF 2 SVG Converter ##########");

            if (currentSelection == 0)
            {
                Console.WriteLine("Enter the absolute path of your .gif file");
                string? pathToGif = Console.ReadLine();
                
                Console.Clear();
                Console.WriteLine("########## GIF 2 SVG Converter ##########");
                Console.WriteLine("Enter the absolute path the output .svg file");
                string? pathToSvg = Console.ReadLine();

                new Converter(pathToGif, pathToSvg);
            }
            else
            {
                Console.WriteLine("Enter the absolute path of the directory your .gif files are stored in");
                string? pathToGifFiles = Console.ReadLine();

                List<string> gifFiles = new List<string>();

                string[] files = Directory.GetFiles(pathToGifFiles);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == ".gif")
                        gifFiles.Add(file);
                }

                if (gifFiles.Count <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("########## GIF 2 SVG Converter ##########");
                    Console.WriteLine("No .gif files detected in provided directory, try again");
                    Console.ReadKey();
                    Start();
                    Environment.Exit(0);
                }

                Console.Clear();
                Console.WriteLine("########## GIF 2 SVG Converter ##########");
                Console.WriteLine("Enter the absolute path of the directory your .svg files will be stored in");
                string? pathToSvgFiles = Console.ReadLine();

                if (!Directory.Exists(pathToSvgFiles))
                    Directory.CreateDirectory(pathToSvgFiles);

                foreach (string file in gifFiles)
                {
                    Converter converter = new Converter(file, pathToSvgFiles + "\\" + Path.GetFileNameWithoutExtension(file) + ".svg");
                }
            }
        }
    }
}
