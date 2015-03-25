using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kanji.Common.Helpers
{
    public static class FileReadingHelper
    {
        /// <summary>
        /// Reads the file at the given path line by line.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadLineByLine(string path, Encoding encoding)
        {
            bool isFirstLine = true;
            FileStream fs = new FileStream(path, FileMode.Open);
            try
            {
                int b = 0;
                List<byte> lineBuilder = new List<byte>(256);
                while ((b = fs.ReadByte()) >= 0)
                {
                    if (b == '\n')
                    {
                        if (isFirstLine)
                        {
                            yield return encoding.GetString(lineBuilder.ToArray()).Substring(1);
                            isFirstLine = false;
                        }
                        else
                        {
                            yield return encoding.GetString(lineBuilder.ToArray());
                        }
                        lineBuilder.Clear();
                    }
                    else if (b != '\r')
                    {
                        lineBuilder.Add((byte)b);
                    }
                }
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
