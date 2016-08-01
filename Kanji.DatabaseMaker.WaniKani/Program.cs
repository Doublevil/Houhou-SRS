using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using WaniKaniApi;
using WaniKaniApi.Models;

namespace Kanji.DatabaseMaker.WaniKani
{
    /// <summary>
    /// Given a WaniKani API key, produces wanikani data files usable by Houhou.Etl.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string key = ConfigurationManager.AppSettings["ApiKey"];
            Console.WriteLine("To pull data from WaniKani, the application needs an API key from a subscribed user account.");
            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine("No key could be read from the App.config file.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"The key \"{key}\" has been read from the App.config file. Initializing the client.");
            WaniKaniClient client = new WaniKaniClient(key);
            Console.WriteLine("Retrieving kanji...");
            List<WaniKaniKanjiItem> kanji = client.GetKanji();
            Console.WriteLine("Retrieving vocab...");
            List<WaniKaniVocabularyItem> vocab = client.GetVocabulary();

            Console.WriteLine("Writing files...");

            File.WriteAllLines("WaniKaniKanjiList.txt", kanji.Select(k => $"{k.Character}|{k.Level}"));

            List<string> vocabLines = new List<string>();
            foreach (var v in vocab)
            {
                // For each vocab, get each different kana reading.
                foreach (string reading in v.KanaReadings)
                {
                    // For each reading, write a line.
                    vocabLines.Add($"{v.Character}|{reading}|{v.Level}");

                    // Handle the する verb case: WaniKani sometimes teaches only the する verb version of a noun
                    // and it isn't necessarily in the dictionary, so we add another line without the する.
                    if (v.Character.EndsWith("する") && reading.EndsWith("する"))
                        vocabLines.Add($"{v.Character.Substring(0, v.Character.Length - 2)}|{reading.Substring(0, reading.Length - 2)}|{v.Level}");
                }
            }
            File.WriteAllLines("WaniKaniVocabList.txt", vocabLines);

            Console.WriteLine("Done.");
        }
    }
}
