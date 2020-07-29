using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace webreq
{
    class Program
    {
        // TEST URL
        static string baseURL = "https://www.jimmyjazz.com/products/nike-air-max-90-royal-cd0881-102?variant=32686806728813";
        static HttpClient client;

        static async Task Main(string[] args)
        {
            HttpClientHandler handler = new HttpClientHandler 
            {
                CookieContainer = new System.Net.CookieContainer()
            };

            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36");

            await AddToCart(9);
            await AddToCart(9.5);
            await AddToCart(10);

            System.Console.WriteLine("Items in cart: " + await GetCartItems());

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
                        Size = double.Parse(n.InnerText.Split('-')[0].Trim())
                    });
                }
            }
            
            return products;
        }

        static async Task<int> AddToCart(double size)
        {
            string postURL = "https://www.jimmyjazz.com/cart/add.js";
            
            var data = new {
                form_type = "product",
                utf8 = "%E2%9C%93",
                Size = size,
                id = 32686806728813
            };

            var jsonData = JsonConvert.SerializeObject(data);
            var sData = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(postURL, sData);

            //System.Console.WriteLine(await response.Content.ReadAsStringAsync());

            return (int)response.StatusCode;
        }

        static async Task<long> GetCartItems()
        {
            string cartUrl = "https://www.jimmyjazz.com/cart.js";

            var response = await client.GetStringAsync(cartUrl);
            var cartInfo = CartInfo.FromJson(response);

            return cartInfo.ItemCount;
        }
    }
}
