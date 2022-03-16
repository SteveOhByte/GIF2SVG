using ImageProcessor;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using Xabe.FFmpeg;

namespace GIF2SVG
{
    public class Converter
    {
        private readonly string tempFolder = AppDomain.CurrentDomain.BaseDirectory + "\\temp";

        private string pathToGif, pathToSvg, newPath;
        private string ffmpegOutput = string.Empty;
        private int numFrames = 0;
        private float ffps = 0f;
        private float speed = 0f;

        public Converter(string pathToGif, string pathToSvg)
        {
            this.pathToGif = pathToGif;
            this.pathToSvg = pathToSvg;

            Console.Clear();
            Console.WriteLine("########## GIF 2 SVG Converter ##########");
            Console.WriteLine("Converting...");

            Convert(pathToGif, pathToSvg);
        }

        public void Convert(string? pathToGif, string? pathToSvg)
        {
            this.pathToGif = pathToGif;
            this.pathToSvg = pathToSvg;

            Directory.CreateDirectory(tempFolder);
            // Extract ffmpeg resources
            File.WriteAllBytes(tempFolder + "\\ffmpeg.exe", Properties.Resources.ffmpeg);
            File.WriteAllBytes(tempFolder + "\\ffplay.exe", Properties.Resources.ffplay);
            File.WriteAllBytes(tempFolder + "\\ffprobe.exe", Properties.Resources.ffprobe);

            newPath = tempFolder + "\\" + Path.GetFileNameWithoutExtension(pathToGif) + ".gif";

            ExtractFramesAsync().Wait();

            Console.WriteLine("Getting data...");

            List<string> frames = Directory.GetFiles(tempFolder + "\\" + Path.GetFileNameWithoutExtension(newPath)).ToList();
            numFrames = frames.Count;

            List<string> splitString = ffmpegOutput.Split("\n").ToList();
            List<string> pieces = new List<string>();
            foreach (string line in splitString)
            {
                int i = splitString.IndexOf(line);
                if (line.StartsWith("Input #")) 
                {
                    string streamLine = splitString[i + 2];
                    pieces = streamLine.Split(",").ToList();

                    break;
                }
            }

            string fps = string.Empty;
            foreach (string piece in pieces)
            {
                if (piece.Contains("fps"))
                {
                    fps = piece.Split(" ").ToList()[1];
                    break;
                }
            }

            ffps = float.Parse(fps, CultureInfo.InvariantCulture.NumberFormat);
            speed = numFrames / ffps;

            BuildSVG(frames);

            Cleanup();
        }

        private async Task ExtractFramesAsync()
        {
            Console.WriteLine("Extracting frames...");
            File.Copy(pathToGif, newPath);

            string filenameNoExt = Path.GetFileNameWithoutExtension(newPath);
            string filenameExt = Path.GetFileName(newPath);

            Directory.CreateDirectory(tempFolder + "\\" + filenameNoExt);

            // Extract the .gif to the tempfolder
            FFmpeg.SetExecutablesPath(tempFolder);
            Func<string, string> outputFileNameBuilder = (number) => { return "\"" + tempFolder + "\\" + filenameNoExt + "\\" + "frame" + number + ".png\""; };
            IMediaInfo info = await FFmpeg.GetMediaInfo(pathToGif).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.png);

            IConversion conversion = FFmpeg.Conversions.New();
            conversion.OnDataReceived += Conversion_OnDataReceived;
            conversion.AddStream(videoStream);
            conversion.ExtractEveryNthFrame(1, outputFileNameBuilder);

            IConversionResult conversionResult = await conversion.Start();
        }

        private void Conversion_OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            ffmpegOutput += e.Data + "\n";
        }

        private void BuildSVG(List<string> frames)
        {
            Image img = Image.FromFile(tempFolder + "\\" + Path.GetFileNameWithoutExtension(newPath) + "\\frame_001.png");
            int width = img.Width;
            int height = img.Height;

            string framesFolder = Path.GetFileNameWithoutExtension(pathToGif) + "Frames";

            Console.WriteLine("Building SVG...");
            StreamWriter sw = File.CreateText(pathToSvg);
            sw.WriteLine("<svg version=\"1.1\" baseProfile=\"tiny\" id=\"svg-root\" width=\"" + width + "\" height=\"" + height + "\" viewBox=\"0 0 " + width + " " + height + "\"");
            sw.WriteLine("    xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
            sw.WriteLine("");
            sw.WriteLine("    <image width=\"" + width + "\" height=\"" + height + "\" xlink:href=\"" + framesFolder + "/frame_001.png\">");
            sw.WriteLine("        <animate attributeName=\"xlink:href\"");
            sw.Write("            values=\"");
            foreach (string frame in frames)
                sw.Write(framesFolder + "/" + Path.GetFileNameWithoutExtension(frame) + ".png" + ";");
            sw.WriteLine("\" begin=\"0s\"");
            sw.WriteLine("            repeatCount=\"indefinite\" dur=\"" + speed + "s\" />");
            sw.WriteLine("    </image>");
            sw.WriteLine("");
            sw.WriteLine("</svg>");

            sw.Close();
        }

        private void Cleanup()
        {
            GC.Collect();

            DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(pathToSvg) + "\\" + Path.GetFileNameWithoutExtension(pathToGif) + "Frames");
            foreach (string file in Directory.GetFiles(tempFolder + "\\" + Path.GetFileNameWithoutExtension(newPath)).ToList())
            {
                File.Copy(file, di.FullName + "\\" + Path.GetFileName(file));
            }

            Directory.Delete(tempFolder, true);
            Console.WriteLine("Conversion Complete!");
        }
    }
}