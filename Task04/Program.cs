using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Task04
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args));

            Console.ReadLine();
        }

        static async Task MainAsync(string[] args)
        {
            string inputUrl = Console.ReadLine();

            var listOfLinkedUrls = await GetLinkedUrlsAsync(inputUrl);
            if (listOfLinkedUrls == null)
            {
                Console.WriteLine($"Input URL {inputUrl} could not load " +
                    $"or it doesn't have any linked URLs.");
                return;
            }

            foreach (var url in listOfLinkedUrls)
            {
                int size = await GetUrlSizeAsync(url);
                DisplaySize(url, size);
            }

            Console.WriteLine("All links checked.");
        }

        private static async Task<List<string>> GetLinkedUrlsAsync(string url)
        {
            string source = await ReadUrlAsync(url);

            if (source == null)
            {
                return null;
            }
            else
            {
                var listOfLinks = new List<string>();

                MatchCollection matches = Regex.Matches(source, @"href=""http(\S*)""");
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                        listOfLinks.Add(ExtractLink(match.Value));
                }

                return listOfLinks;
            }
        }

        private static async Task<string> ReadUrlAsync(string url)
        {
            string urlContent = null;

            WebRequest req = WebRequest.Create(url);
            req.Method = "GET";

            try
            {
                using (WebResponse response = await req.GetResponseAsync())
                {
                    using (StreamReader responseStream = new StreamReader(response.GetResponseStream()))
                    {
                        urlContent = await responseStream.ReadToEndAsync();
                    }
                }
            }
            catch (WebException)
            {
                //keep urlContent null
            }

            return urlContent;
        }

        private static async Task<int> GetUrlSizeAsync(string url)
        {
            string urlContent = await ReadUrlAsync(url);

            if (urlContent == null)
                return -1;

            return urlContent.Length;
        }

        private static void DisplaySize(string url, int size)
        {
            if (size == -1)
            {
                Console.WriteLine($"{url} couldn't be loaded.");
            }
            else
            {
                Console.WriteLine($"{url} has {size} symbols.");
            }
        }

        private static string ExtractLink(string match)
        {
            int subFrom = match.IndexOf("\"") + 1;
            int subTo = match.LastIndexOf("\"");
            return match.Substring(subFrom, subTo - subFrom);
        }
    }
}
