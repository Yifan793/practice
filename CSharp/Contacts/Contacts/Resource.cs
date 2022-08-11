using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using System.IO;

namespace Contacts
{
    public static class Resource
    {
        public static Book LoadResource(string path)
        {
            dynamic dynamicObj = JsonConvert.DeserializeObject<Book>(File.ReadAllText(path));
            return dynamicObj;
        }
        public static void WriteResource(Book book, string path)
        {
            string JSON = JsonConvert.SerializeObject(book, Formatting.Indented);
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.WriteLine(JSON);
        }
    }
}
