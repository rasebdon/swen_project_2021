using MTCG.Interfaces;
using System;
using System.Collections.Specialized;
using System.Text.Json;

namespace MTCG.Models
{
    [Serializable]
    public abstract class DataObject : IJsonSerializable
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
        protected DataObject(Guid id)
        {
            ID = id;
        }
        /// <summary>
        /// Parse constructor for DataObjects
        /// </summary>
        /// <param name="row">The retrieved sql row</param>
        protected DataObject(OrderedDictionary row)
        {
            ID = (Guid)row["id"];
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, this.GetType());
        }
    }
}
