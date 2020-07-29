using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace webreq
{
    class Program
    {
        static string baseURL = "https://www.jimmyjazz.com/products/nike-air-max-90-royal-cd0881-102?variant=32686806728813";
        static HttpClient client;

        static async Task Main(string[] args)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36");

            var products = await GetProducts();

            foreach (ProductInfo p in products)
            {
                Console.Write(p.Size);
            }
        }

        static async Task<List<ProductInfo>> GetProducts()
        {
            var products = new List<ProductInfo>();
            var response = await client.GetStringAsync(baseURL);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var mainNode = doc.DocumentNode.SelectSingleNode("//select[@class='product-single__variants no-js']");
            var nodes = mainNode.SelectNodes("//option");

            foreach (var n in nodes)
            {
                if (!n.InnerText.ToLower().Contains("sold out"))
                {
                    n.InnerText.Replace("\n", string.Empty).Replace("\n", string.Empty); // removes all empty lines
                    products.Add(new ProductInfo {
                        VariantId = long.Parse(n.Attributes["value"].Value),
                        Size = n.InnerText.Split('-')[0].Trim()
                    });
                }
            }
            
            return products;
        }

        static async Task<bool> AddToCart(float size)
        {
            await Task.Delay(0);
            return true;
        }
    }
}
