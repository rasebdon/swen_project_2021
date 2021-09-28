using System.Collections.Specialized;

namespace MTCG.Models
{
    abstract class DataObject
    {
        /// <summary>
        /// The unique object id
        /// </summary>
        public uint ID { get; }
        /// <summary>
        /// Base constructor for DataObjects
        /// </summary>
        /// <param name="id"></param>
        protected DataObject(uint id)
        {
            ID = id;
        }
        /// <summary>
        /// Parse constructor for DataObjects
        /// </summary>
        /// <param name="row">The retrieved sql row</param>
        protected DataObject(OrderedDictionary row)
        {
            ID = (uint)(int)row["ID"];
        }
    }
}
