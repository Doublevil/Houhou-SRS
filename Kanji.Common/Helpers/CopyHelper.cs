using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kanji.Common.Helpers
{
    public static class CopyHelper
    {
        /// <summary>
        /// Copies all content from directory "source" to directory "destination".
        /// </summary>
        /// <param name="source">Source directory path.</param>
        /// <param name="destination">Destination directory path.</param>
        public static void CopyAllContent(string source, string destination)
        {
            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(source, "*",
                SearchOption.AllDirectories))
            {
                string newPath = dirPath.Replace(source, destination);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
            }

            // Copy all the files
            foreach (string filePath in Directory.GetFiles(source, "*",
                SearchOption.AllDirectories))
            {
                string newPath = filePath.Replace(source, destination);

                if (!File.Exists(newPath))
                {
                    File.Copy(filePath, newPath);
                }
            }
        }
    }
}
