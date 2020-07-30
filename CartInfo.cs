using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace webreq
{
    public partial class CartInfo
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("note")]
        public object Note { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }

        [JsonProperty("original_total_price")]
        public long OriginalTotalPrice { get; set; }

        [JsonProperty("total_price")]
        public long TotalPrice { get; set; }

        [JsonProperty("total_discount")]
        public long TotalDiscount { get; set; }

        [JsonProperty("total_weight")]
        public double TotalWeight { get; set; }

        [JsonProperty("item_count")]
        public long ItemCount { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("requires_shipping")]
        public bool RequiresShipping { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("items_subtotal_price")]
        public long ItemsSubtotalPrice { get; set; }

        [JsonProperty("cart_level_discount_applications")]
        public object[] CartLevelDiscountApplications { get; set; }
    }

    public partial class Attributes
    {
    }

    public partial class Item
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("properties")]
        public Attributes Properties { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("variant_id")]
        public long VariantId { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("original_price")]
        public long OriginalPrice { get; set; }

        [JsonProperty("discounted_price")]
        public long DiscountedPrice { get; set; }

        [JsonProperty("line_price")]
        public long LinePrice { get; set; }

        [JsonProperty("original_line_price")]
        public long OriginalLinePrice { get; set; }

        [JsonProperty("total_discount")]
        public long TotalDiscount { get; set; }

        [JsonProperty("discounts")]
        public object[] Discounts { get; set; }

        [JsonProperty("sku")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Sku { get; set; }

        [JsonProperty("grams")]
        public long Grams { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("taxable")]
        public bool Taxable { get; set; }

        [JsonProperty("product_id")]
        public long ProductId { get; set; }

        [JsonProperty("product_has_only_default_variant")]
        public bool ProductHasOnlyDefaultVariant { get; set; }

        [JsonProperty("gift_card")]
        public bool GiftCard { get; set; }

        [JsonProperty("final_price")]
        public long FinalPrice { get; set; }

        [JsonProperty("final_line_price")]
        public long FinalLinePrice { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("featured_image")]
        public FeaturedImage FeaturedImage { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("requires_shipping")]
        public bool RequiresShipping { get; set; }

        [JsonProperty("product_type")]
        public string ProductType { get; set; }

        [JsonProperty("product_title")]
        public string ProductTitle { get; set; }

        [JsonProperty("product_description")]
        public string ProductDescription { get; set; }

        [JsonProperty("variant_title")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long VariantTitle { get; set; }

        [JsonProperty("variant_options")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public long[] VariantOptions { get; set; }

        [JsonProperty("options_with_values")]
        public OptionsWithValue[] OptionsWithValues { get; set; }

        [JsonProperty("line_level_discount_allocations")]
        public object[] LineLevelDiscountAllocations { get; set; }

        [JsonProperty("line_level_total_discount")]
        public long LineLevelTotalDiscount { get; set; }
    }

    public partial class FeaturedImage
    {
        [JsonProperty("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonProperty("alt")]
        public string Alt { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }

    public partial class OptionsWithValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Value { get; set; }
    }

    public partial class CartInfo
    {
        public static CartInfo FromJson(string json) => JsonConvert.DeserializeObject<CartInfo>(json, webreq.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CartInfo self) => JsonConvert.SerializeObject(self, webreq.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) 
                return null;
            var value = serializer.Deserialize<string>(reader);
            //long l;
            if (Int64.TryParse(value, out long l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class DecodeArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long[]);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            var value = new List<long>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                var converter = ParseStringConverter.Singleton;
                var arrayItem = (long)converter.ReadJson(reader, typeof(long), null, serializer);
                value.Add(arrayItem);
                reader.Read();
            }
            return value.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (long[])untypedValue;
            writer.WriteStartArray();
            foreach (var arrayItem in value)
            {
                var converter = ParseStringConverter.Singleton;
                converter.WriteJson(writer, arrayItem, serializer);
            }
            writer.WriteEndArray();
            return;
        }

        public static readonly DecodeArrayConverter Singleton = new DecodeArrayConverter();
    }
}