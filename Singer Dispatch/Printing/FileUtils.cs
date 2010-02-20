using System;
using System.Text;
using System.IO;

namespace SingerDispatch.Printing
{
    class FileUtils
    {
        public static string CreateTempFile(string extention)
        {
            var name = RandomString(30, true) + "." + extention;

            return Path.Combine(Path.GetTempPath(), name);
        }

        public static string CreateTempFile()
        {
            return Path.GetTempFileName();
        }

        public static void DeleteFile(string filename)
        {
            File.Delete(filename);
        }

        public static void MoveFile(string source, string destination)
        {
            File.Copy(source, destination, true);
            File.Delete(source);
        }


        private static int RandomNumber(int min, int max)
        {
            var random = new Random();

            return random.Next(min, max);
        }

        private static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            if (lowerCase)
                return builder.ToString().ToLower();

            return builder.ToString();
        }
    }
}
