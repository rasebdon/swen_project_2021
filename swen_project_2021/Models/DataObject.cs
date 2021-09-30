using MTCG.Interfaces;
using System;
using System.Collections.Specialized;
using System.Text.Json;

namespace MTCG.Models
{
    [Serializable]
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
        public DataObject()
        {
            ID = Guid.NewGuid();
        }
        public DataObject(Guid id)
        {
            ID = id;
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
            return JsonSerializer.Serialize(this, this.GetType());
        }
    }
}
