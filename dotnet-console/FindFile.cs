using System;
using System.IO;
using System.Linq;

namespace tools
{

    public class FindFile
    {

        public static string Up(string filename)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (Path.EndsInDirectorySeparator(path)) path = path.Substring(0, path.Length - 1);
            while (!string.IsNullOrEmpty(path))
            {
                string full = Path.Combine(path, filename);
                var info = new FileInfo(full);
                if (info.Exists) return full;
                var split = path.Split(Path.DirectorySeparatorChar);
                if (split.Length < 1) return null;
                Array.Resize(ref split, split.Length - 1);
                path = string.Join(Path.DirectorySeparatorChar, split);
            }
            return null;
        }

    }

}