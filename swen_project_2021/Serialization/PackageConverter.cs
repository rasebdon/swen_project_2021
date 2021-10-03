using MTCG.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MTCG.Serialization
{
    public static class PackageConverter
    {
        public static Package Parse(string json)
        {
            JObject jObject = JObject.Parse(json);

            JArray cardsJsonArray = (JArray)jObject.Property("Cards").Value;

            List<Card> cards = new();
            var converter = new CardConverter();

            for (int i = 0; i < cardsJsonArray.Count; i++)
            {
                var cJson = cardsJsonArray[i].ToString();
                cards.Add(JsonConvert.DeserializeObject<Card>(cJson, converter));
            }

            string name = jObject.Property("Name").ToString();
            string description = jObject.Property("Description").ToString();
            ushort cost = (ushort)(int)jObject.Property("Cost");

            Package package = new(name, description, cost, cards);

            var idObj = jObject.Property("ID").Value;
            if (idObj != null)
                package.SetID(Guid.Parse(idObj.ToString()));

            return package;
        }
    }
}
