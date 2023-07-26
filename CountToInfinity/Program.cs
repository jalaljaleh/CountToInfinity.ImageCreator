using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace CountToInfinity
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("******************************************************************");
            Console.WriteLine("                         - Welcome -                              ");
            Console.WriteLine("              This Program Made by jalaljaleh                      ");
            Console.WriteLine("******************************************************************");


            while (true)
            {
                Start();
            }
        }
        public static int GetNum()
        {
            var txt1 = Console.ReadLine();

            bool fromValidation = int.TryParse(txt1, out int from);
            if (fromValidation is false)
            {
                Console.WriteLine($"Do you really think it is a number ? '{txt1}' ");
                return 0;
            }
            return from;
        }
        const string outPath = $@"out";
        public static void Start()
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("Write the number you want me to create images from:");

            int from = GetNum();

            Console.WriteLine("Write the number you want me to create images to:");

            int to = GetNum();

            var toLength = to.ToString().Length;

            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }

            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            for (int i = from; i <= to; i++)
            {
                CreateImage(i, toLength);
            }

            Log("\n\nFinished .. !");
            Console.WriteLine("Press any key to reload ..");
            Console.ReadKey();
        }
        const int numImgPixelWidth = 76;
        public static void CreateImage(int number, int length)
        {
            Log("Start working on image " + number + " !");


            using (Image parentImage = Image.Load(FileManager.ToResourcePath($@"clear.png")))
            {
                string numbers = number.ToString()/*.PadLeft(length, '0')*/;
                var items = numbers.ToCharArray();

                for (int i = 0; i < items.Length; i++)
                {
                    using (Image imgNum = Image.Load(FileManager.ToResourcePath($@"{items[i]}.png")))
                    {
                        imgNum.Mutate(x => x.Resize(numImgPixelWidth, numImgPixelWidth));

                        int width =
                           (int)((20 + (numImgPixelWidth * length)) + (i * 26) + (i * numImgPixelWidth));

                        var point = new SixLabors.ImageSharp.Point(width, 475);
                        parentImage.Mutate(x => x.DrawImage(imgNum, point, 1f));
                    }
                }

                Log("Saving image..");


                string path = $@"out/{numbers}.png";
                parentImage.SaveAsPng(path);

                Log("Image saved at " + path);
            }

        }

        public static void Log(string text)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("T") + ": \t" + text);
        }
    }
}
