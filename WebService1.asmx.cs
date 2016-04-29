using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService2
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        private PerformanceCounter process = new PerformanceCounter("Memory", "Available Mbytes");
        public string filename = System.IO.Path.GetTempPath() + "\\filename.txt";
        static Trie newTrie = new Trie();
        [WebMethod]
        public string DownloadWiki()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("info344");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("testing.txt");
            using (var fileStream = System.IO.File.OpenWrite(filename))


            {
                blockBlob.DownloadToStream(fileStream);
            }
            return "done";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String searchTrie(String input)
        {
            String building = "";
            input = input.ToLower();
            List<string> foundWords = new List<string>();;
            if (newTrie.startsWith(input) == true)
            {
                TrieNode tn = newTrie.searchNode(input);
                newTrie.wordsFinderTraversal(tn, 0);
                foundWords = newTrie.displayFoundWords();
            }
            foreach (string item in foundWords)
            {
                building += item;
            }
           
            building = new JavaScriptSerializer().Serialize(foundWords);
            return building;
        }
        public float GetBytes()
        {
            float usage = process.NextValue();
            return usage;
        }
        [WebMethod]
        public string BuildTRIE()
        {
            int count = 0;
            Trie newTrie = new Trie();
            //string filename = System.Web.HttpContext.Current.Server.MapPath(@"C:\Users\iGuest\Desktop\wiki.txt");
            using (StreamReader sr = new StreamReader(filename))
                while (!sr.EndOfStream)
                {
                    newTrie.insert(sr.ReadLine());
                    if (count % 1000 != 0)
                    {
                        if (GetBytes() < 1000)
                        {
                            break;
                        }
                    }
                    count++;
                }
            return "done";
        }
        
    }
}
