using MTCG.Interfaces;
using System;
using System.Collections.Specialized;
using System.Text.Json;

namespace MTCG.Models
{
    public abstract class DataObject
    {
        /// <summary>
        /// The unique object id
        /// </summary>
        public Guid ID { get; }
        /// <summary>
        /// Base constructor for DataObjects
        /// </summary>
        /// <param name="id"></param>
        protected DataObject()
        {
            ID = Guid.NewGuid();
        }
        /// <summary>
        /// Parse constructor for DataObjects
        /// </summary>
        /// <param name="row">The retrieved sql row</param>
        protected DataObject(OrderedDictionary row)
        {
            ID = Guid.Parse(row["id"].ToString());
        }

        public string ToJson()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(this, this.GetType(), jsonOptions);
        }
    }
}
