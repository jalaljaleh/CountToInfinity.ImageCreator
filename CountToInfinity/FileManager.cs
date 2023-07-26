using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountToInfinity
{
    internal class FileManager
    {
        public static string ToResourcePath(string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $@"Resources/{path}");
        }
    }
}
