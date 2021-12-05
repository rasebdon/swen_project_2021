using MTCG.Interfaces;
using System.Text.Json;

namespace MTCG.Models
{
    [Serializable]
    public abstract class DataObject : IJsonSerializable
    {
        /// <summary>
        /// The unique object id
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Base constructor for DataObjects
        /// </summary>
        /// <param name="id"></param>
        protected DataObject(Guid id)
        {
            ID = id;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, this.GetType());
        }
    }
}
