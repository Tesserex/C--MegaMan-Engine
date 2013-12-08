using System;
using System.Text;
using System.IO;

namespace MegaMan.Common
{
    public class FilePath
    {
        private string basepath;
        private string relative;
        private string absolute;

        public string BasePath
        {
            get { return basepath; }
        }

        public string Absolute
        {
            get { return absolute; }
            private set
            {
                absolute = Path.GetFullPath(value);
                FindRelative();
            }
        }

        public string Relative
        {
            get { return relative; }
        }

        public static FilePath FromAbsolute(string absolute, string basepath)
        {
            if (string.IsNullOrWhiteSpace(absolute))
            {
                throw new ArgumentException("Path cannot be null or empty.", "absolute");
            }

            if (string.IsNullOrWhiteSpace(basepath))
            {
                throw new ArgumentException("Path cannot be null or empty.", "basepath");
            }

            return new FilePath(absolute, basepath);
        }

        public static FilePath FromRelative(string relative, string basepath)
        {
            if (string.IsNullOrWhiteSpace(relative))
            {
                throw new ArgumentException("Path cannot be null or empty.", "relative");
            }

            if (string.IsNullOrWhiteSpace(basepath))
            {
                throw new ArgumentException("Path cannot be null or empty.", "basepath");
            }

            FilePath fp = new FilePath
            {
                basepath = Path.GetFullPath(basepath),
                relative = relative,
                absolute = Path.GetFullPath(Path.Combine(basepath, relative))
            };
            return fp;
        }

        private FilePath()
        {
        }

        private FilePath(string absolute, string basepath)
        {
            this.basepath = Path.GetFullPath(basepath);
            Absolute = absolute;
        }

        private void FindRelative()
        {
            if (string.IsNullOrEmpty(absolute)) return;
            if (string.IsNullOrEmpty(basepath)) return;

            if (Path.HasExtension(basepath))
            {
                basepath = Path.GetDirectoryName(basepath);
            }

            // split into directories
            string[] pathdirs = absolute.Split(Path.DirectorySeparatorChar);
            string[] reldirs = basepath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            int length = Math.Min(pathdirs.Length, reldirs.Length);
            StringBuilder relativePath = new StringBuilder();

            // find where the paths differ
            int forkpoint = 0;
            while (forkpoint < length && pathdirs[forkpoint] == reldirs[forkpoint]) forkpoint++;

            // go back by the number of directories in the relativeTo path
            int dirs = reldirs.Length - forkpoint;
            for (int i = 0; i < dirs; i++) relativePath.Append("..").Append(Path.DirectorySeparatorChar);

            // append file path from that directory
            for (int i = forkpoint; i < pathdirs.Length - 1; i++) relativePath.Append(pathdirs[i]).Append(Path.DirectorySeparatorChar);
            // append file, without directory separator
            relativePath.Append(pathdirs[pathdirs.Length - 1]);

            relative = relativePath.ToString();
        }
    }
}
