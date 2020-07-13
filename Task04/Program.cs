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
            string inputUrl = Console.ReadLine();

            var handler = new UrlHandler(inputUrl);
            handler.HandleAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }
    }

    class UrlHandler
    {
        private readonly string inputUrl;

        public UrlHandler(string url)
        {
            inputUrl = url;
        }

        public async Task HandleAsync()
        {
            var listOfLinkedUrls = await GetLinkedUrlsAsync(inputUrl);
            if (listOfLinkedUrls.Count == 0)
            {
                Console.WriteLine($"Input URL {inputUrl} could not load " +
                    $"or it doesn't have any linked URLs.");
                return;
            }

            List<Task<int>> tasks = new List<Task<int>>();         
            foreach (var url in listOfLinkedUrls)
                tasks.Add(ProcessUrlAsync(url));

            await Task.WhenAll(tasks);
        }

        private async Task<int> ProcessUrlAsync(string url)
        {
            string urlContent = await ReadUrlAsync(url);
            int size = urlContent?.Length ?? -1;
            DisplaySize(url, size);
            return size;
        }

        private async Task<List<string>> GetLinkedUrlsAsync(string url)
        {
            string source = await ReadUrlAsync(url);

            var listOfLinks = new List<string>();
            if (source != null)
            {
                MatchCollection matches = Regex.Matches(source, @"<a href=""http(\S*)""");
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                        listOfLinks.Add(ExtractLink(match.Value));
                }
            }

            return listOfLinks;
        }

        private async Task<string> ReadUrlAsync(string url)
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
                Console.WriteLine($"Error while loading {url}.");
            }

            return urlContent;
        }

        private void DisplaySize(string url, int size)
        {
            string message = size == -1 ? $"{url} couldn't be loaded." : $"{url} has {size} symbols.";
            Console.WriteLine(message);
        }

        private string ExtractLink(string s)
        {
            var match = Regex.Match(s, @"http(\S*)""");
            return match.Value.TrimEnd('\"');
        }
    }
}
