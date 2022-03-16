using GIF2SVG.Properties;

namespace GIF2SVG
{
    public class Main
    {
        public Main() { Start(); }

        private void Start()
        {
            string[] options = { Resources.Main_Start_Single_gif_conversion, Resources.Main_Start_Multiple_gifs_conversion };

            int currentSelection = 0;

            Console.CursorVisible = false;

            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.WriteLine(Resources.Main_Start_GIF_2_SVG_Converter);
                Console.WriteLine(Resources.Main_Start_Select_an_option);

                if (currentSelection == 0)
                {
                    Console.WriteLine(@"> " + options[0]);
                    Console.WriteLine(options[1]);
                }
                else
                {
                    Console.WriteLine(options[0]);
                    Console.WriteLine(@"> " + options[1]);
                }

                key = Console.ReadKey(true).Key;

                if (key != ConsoleKey.UpArrow && key != ConsoleKey.DownArrow) continue;
                currentSelection = currentSelection == 0 ? 1 : 0;

            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            Console.Clear();
            Console.WriteLine(Resources.Main_Start_GIF_2_SVG_Converter);

            if (currentSelection == 0)
            {
                Console.WriteLine(Resources.Main_Start_Enter_the_absolute_path_of_your_gif_file);
                string? pathToGif = Console.ReadLine();

                if (pathToGif != null)
                {
                    Console.Clear();
                    Console.WriteLine(Resources.Main_Start_GIF_2_SVG_Converter);
                    Console.WriteLine(Resources.Main_Start_Enter_the_absolute_path_the_output_svg_file);
                    string? pathToSvg = Console.ReadLine();

                    if (pathToSvg != null)
                    {
                        new Converter(pathToGif, pathToSvg);
                    }
                    else
                    {
                        Console.WriteLine(Resources.Main_Start_Error_Provided_path_was_null_the_program_will_now_exit_);
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine(Resources.Main_Start_Error_Provided_path_was_null_the_program_will_now_exit_);
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine(Resources.Main_Start_Enter_the_absolute_path_of_the_directory_your_gif_files_are_stored_in);
                string? pathToGifFiles = Console.ReadLine();

                if (pathToGifFiles != null)
                {
                    string[] files = Directory.GetFiles(pathToGifFiles);
                    List<string> gifFiles = files.Where(file => Path.GetExtension(file) == ".gif").ToList();

                    if (gifFiles.Count <= 0)
                    {
                        Console.Clear();
                        Console.WriteLine(Resources.Main_Start_GIF_2_SVG_Converter);
                        Console.WriteLine(Resources.Main_Start_No_gif_files_detected_in_provided_directory_try_again);
                        Console.ReadKey();
                        Start();
                        Environment.Exit(0);
                    }

                    Console.Clear();
                    Console.WriteLine(Resources.Main_Start_GIF_2_SVG_Converter);
                    Console.WriteLine(Resources.Main_Start_Enter_the_absolute_path_of_the_directory_your_svg_files_will_be_stored_in);
                    string? pathToSvgFiles = Console.ReadLine();

                    if (pathToSvgFiles != null)
                    {
                        if (!Directory.Exists(pathToSvgFiles))
                            Directory.CreateDirectory(pathToSvgFiles);

                        foreach (Converter converter in gifFiles.Select(file => new Converter(file, pathToSvgFiles + "\\" + Path.GetFileNameWithoutExtension(file) + ".svg"))) {}
                    }
                    else
                    {
                        Console.WriteLine(Resources.Main_Start_Error_Provided_path_was_null_the_program_will_now_exit_);
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine(Resources.Main_Start_Error_Provided_path_was_null_the_program_will_now_exit_);
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }
    }
}
