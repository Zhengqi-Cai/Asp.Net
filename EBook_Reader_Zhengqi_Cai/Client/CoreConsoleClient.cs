///////////////////////////////////////////////////////////////
// ConsoleClient.cs - Client for WebApi FilesController      //
//                                                           //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2019   //
///////////////////////////////////////////////////////////////
/*
 * - Based on Asp.Net Core Framework, this client project generates
 *   dynamic link library that can be hosted by Visual Studio or
 *   dotnet CLI.
 * - It provides options via its command line, e.g.:
 *   - url /fl            displays list of files in server's FileStore
 *   - url /up fileSpec   uploades fileSpec to FileStore
 *   - url /dn n          downloads nth file in FileStore
 *   - url /dl n          deletes nth file in FileStore
 */
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;  // must install Newtonsoft package from Nuget
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
//using Microsoft.AspNetCore.Http;
using Client;
using System.Threading;
using System.Reflection;

namespace ConsoleClient
{
    class CoreConsoleClient
    {
        public HttpClient client { get; set; }

        private string baseUrl_;

        public CoreConsoleClient(string url)
        {
            baseUrl_ = url;
            client = new HttpClient();
        }
        //----< upload file >--------------------------------------
        //param is an object and fileSpec maybe
        public async Task<HttpResponseMessage> CreateBook(ViewBook vBook)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            byte[] data = File.ReadAllBytes(vBook.PathUrl);
            ByteArrayContent bytes = new ByteArrayContent(data);
            string fileName = Path.GetFileName(vBook.PathUrl);
            multiContent.Add(bytes, "files", fileName);

            PropertyInfo[] properties = typeof(ViewBook).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                multiContent.Add(new StringContent(property.GetValue(vBook).ToString()), property.Name);
            }

            return await client.PostAsync(baseUrl_, multiContent);
        }
        //----< get list of files in server FileStorage >----------

        public async Task<IEnumerable<ViewBook>> GetBookList()
        {
            var Books = new List<ViewBook>();
            HttpResponseMessage resp;
            Thread.Sleep(5000);
            resp = await client.GetAsync(baseUrl_);

            //do
            //{
            //} while (resp.StatusCode != HttpStatusCode.OK);
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                {
                    ViewBook book = item.ToObject<ViewBook>();
                    Books.Add(book);

                }
            }
            return Books;
        }
        //----< download the id-th file >--------------------------

        public async Task<HttpResponseMessage> GetFile(int id)
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_ + "/" + id.ToString());
            return resp;
        }


        public async Task<HttpResponseMessage> EditBook(ViewBook vBook)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            if (!String.IsNullOrEmpty(vBook.PathUrl))
            {
                byte[] data = File.ReadAllBytes(vBook.PathUrl);
            ByteArrayContent bytes = new ByteArrayContent(data);
            string fileName = Path.GetFileName(vBook.PathUrl);
            multiContent.Add(bytes, "files", fileName);
            }

            PropertyInfo[] properties = typeof(ViewBook).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                multiContent.Add(new StringContent(property.GetValue(vBook).ToString()), property.Name);
            }

            return await client.PutAsync(baseUrl_+"/"+vBook.BookId, multiContent);

        }

        //----< delete the id-th file from FileStorage >-----------

        public async Task<HttpResponseMessage> DeleteBook(int id)
        {
            return await client.DeleteAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< usage message shown if command line invalid >------

        static void showUsage()
        {
            Console.Write("\n  Command line syntax error: expected usage:\n");
            Console.Write("\n    http[s]://machine:port /option [filespec]\n\n");
        }
        //----< validate the command line >------------------------

        static bool parseCommandLine(string[] args)
        {
            if (args.Length < 2)
            {
                showUsage();
                return false;
            }
            if (args[0].Substring(0, 4) != "http")
            {
                showUsage();
                return false;
            }
            if (args[1][0] != '/')
            {
                showUsage();
                return false;
            }
            return true;
        }
        //----< display command line arguments >-------------------

        static void showCommandLine(string[] args)
        {
            string arg0 = args[0];
            string arg1 = args[1];
            string arg2;
            if (args.Length == 3)
                arg2 = args[2];
            else
                arg2 = "";
            Console.Write("\n  CommandLine: {0} {1} {2}", arg0, arg1, arg2);
        }

        //static void Main(string[] args)
        //{
        //  Console.Write("\n  CoreConsoleClient");
        //  Console.Write("\n ===================\n");

        //  if(!parseCommandLine(args))
        //  {
        //    return;
        //  }
        //  Console.Write("Press key to start: ");
        //  Console.ReadKey();

        //  string url = args[0];
        //  CoreConsoleClient client = new CoreConsoleClient(url);

        //  showCommandLine(args);
        //  Console.Write("\n  sending request to {0}\n", url);

        //  switch(args[1])
        //  {
        //    case "/fl":
        //      Task<IEnumerable<string>> tfl = client.GetBookList();
        //      var resultfl = tfl.Result;
        //      foreach (var item in resultfl)
        //      {
        //        Console.Write("\n  {0}", item);
        //      }
        //      break;
        //    case "/up":
        //      Task<HttpResponseMessage> tup = client.CreateBook(args[2]);
        //      Console.Write(tup.Result);
        //      break;
        //    case "/dn":
        //      int id = Int32.Parse(args[2]);
        //      Task<HttpResponseMessage> tdn = client.GetFile(id);
        //      Console.Write(tdn.Result);
        //      break;
        //    case "/dl":
        //      // ToDo
        //      break;
        //  }

        //  Console.WriteLine("\n  Press Key to exit: ");
        //  Console.ReadKey();
        //}
    }
}
