using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using QuickType;

/* NOTES
    * Sending a specific size does not matter. VariantId is what determines the product and its size.
*/

namespace webreq
{
    class Program
    {
        // TEST URL
        static string baseURL = "https://www.jimmyjazz.com/collections/nike-air-max-90/products/nike-air-max-90-royal-cd0881-102";
        static HttpClient client;

        static async Task Main(string[] args)
        {
            HttpClientHandler handler = new HttpClientHandler 
            {
                CookieContainer = new System.Net.CookieContainer()
            };

            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36");

            var products = await GetProducts();

            System.Console.WriteLine(products.Count);
            
            for (int i = 0; i < products.Count; i++)
            {
                if (await AddToCart(products[i].VariantId, products[i].Size) == 200)
                    System.Console.WriteLine("Success " + products[i].Size);
                else
                    System.Console.WriteLine("Failed");

                await Task.Delay(1000);
            }

            System.Console.WriteLine("Items in cart: " + await GetCartItems());

        }

        static async Task<List<ProductInfo>> GetProducts()
        {
            var products = new List<ProductInfo>();
            var response = await client.GetStringAsync(baseURL);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var titleNode = doc.DocumentNode.SelectSingleNode("//title").InnerText.Split('-')[0];
            var mainNode = doc.DocumentNode.SelectSingleNode("//select[@class='product-single__variants no-js']");
            var nodes = mainNode.SelectNodes("//option");

            foreach (var n in nodes)
            {
                if (!n.InnerText.ToLower().Contains("sold out"))
                {
                    n.InnerText.Replace("\n", string.Empty).Replace("\n", string.Empty); // removes all empty lines

                    products.Add(new ProductInfo {
                        ProductName = titleNode,
                        VariantId = Int64.Parse(n.Attributes["value"].Value.Trim()),
                        Size = double.Parse(n.InnerText.Split('-')[0].Trim())
                    });
                }
            }
            
            return products;
        }

        static async Task<int> AddToCart(long variantId, double size = 0)
        {
            string postURL = "https://www.jimmyjazz.com/cart/add.js";
            
            // SIZE IS DISREGARDED!
            var data = new {
                form_type = "product",
                utf8 = "%E2%9C%93",
                Size = size,
                id = variantId
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

            //System.Console.WriteLine(response);
            foreach (Item i in cartInfo.Items)
              Console.WriteLine("Title: {0} | Variant Id: {1} | Size: {2}\n", i.ProductTitle, i.VariantId, i.VariantTitle);

            return cartInfo.ItemCount;
        }

        static async Task<int> Checkout()
        {
            await Task.Delay(0);
            return 0;
        }
    }
}
