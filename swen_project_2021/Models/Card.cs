using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MTCG.Models
{
    public enum CardType
    {
        Monster,
        Spell
    }

    public enum Element
    {
        Normal,
        Fire,
        Water
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    //public class CardConverter : JsonConverter<Card>
    //{
    //    public override Card Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        while (reader.Read())
    //        {
    //            switch (reader.TokenType)
    //            {
    //                case JsonTokenType.StartObject:
    //                case JsonTokenType.EndObject:
    //                case JsonTokenType.StartArray:
    //                case JsonTokenType.EndArray:
    //                    Console.WriteLine($"TokenType={reader.TokenType}");
    //                    break;
    //                case JsonTokenType.String:
    //                    Console.WriteLine($"TokenType=String Value={reader.GetString()}");
    //                    break;
    //                case JsonTokenType.Number:
    //                    Console.WriteLine($"TokenType=Number Value={reader.GetInt32()}");
    //                    break;
    //                case JsonTokenType.PropertyName:
    //                    Console.WriteLine($"TokenType=PropertyName Value={reader.GetString()}");
    //                    break;
    //            }
    //        }
    //        return null;
    //    }

    //    public override void Write(Utf8JsonWriter writer, Card value, JsonSerializerOptions options)
    //    {
    //        writer.WriteStringValue(value.ID);
    //        writer.WriteStringValue(value.Name);
    //        writer.WriteStringValue(value.Description);
    //        writer.WriteStringValue(value.Damage.ToString());
    //        writer.WriteStringValue(((int)value.Element).ToString());
    //        writer.WriteStringValue(((int)value.Rarity).ToString());
    //        writer.WriteStringValue(((int)value.CardType).ToString());
    //    }
    //}

    /// <summary>
    /// A card is the abstract representation of the cards, the user can obtain
    /// and fight with. It splits into multiple subcategories like the MonsterCard
    /// or the SpellCard
    /// </summary>
    [Serializable]
    public abstract class Card : DataObject
    {
        /// <summary>
        /// The unique name of the card
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The damage of the card
        /// </summary>
        public int Damage { get; }

        public static Card ParseFromDatabase(OrderedDictionary row)
        {
            CardType t = (CardType)(int)row["type"];

            switch (t)
            {
                case CardType.Monster:
                    return new MonsterCard(row);
                case CardType.Spell:
                    return new SpellCard(row);
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// The type of the card should be set in 
        /// the derived constructors
        /// </summary>
        public CardType CardType { get; }
        /// <summary>
        /// The element of the card
        /// </summary>
        public Element Element { get; }

        public Rarity Rarity { get; }

        public string Description { get; }

        protected Card(OrderedDictionary row) : base(row)
        {
            Name = row["name"].ToString();
            Description = row["description"].ToString();
            CardType = (CardType)(int)row["type"];
            Damage = (int)row["damage"];
            Element = (Element)(int)row["element"];
            Rarity = (Rarity)(int)row["rarity"];
        }

        [JsonConstructor]
        public Card(string name, string description, int damage, CardType cardType, Element element, Rarity rarity) : base()
        {
            CardType = cardType;
            Name = name;
            Description = description;
            Damage = damage;
            Element = element;
            Rarity = rarity;
        }


        public override bool Equals(object obj)
        {
            return obj is Card c &&
                c.ID == this.ID &&
                c.Name == this.Name &&
                c.Description == this.Description &&
                c.Rarity == this.Rarity &&
                c.CardType == this.CardType &&
                c.Damage == this.Damage &&
                c.Element == this.Element;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Damage, CardType, Element, Rarity, Description);
        }
    }
}
