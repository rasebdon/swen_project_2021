using MTCG.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL
{
    public class DataTable<TDatabaseObject> : IList<TDatabaseObject> where TDatabaseObject : DatabaseObject
    {
        private readonly Database _database;
        private List<TDatabaseObject> _itemsOld;
        private List<TDatabaseObject> _items;

        public DataTable(Database database) : this(database, new List<TDatabaseObject>()) { }

        public DataTable(Database database, List<TDatabaseObject> items)
        {
            _database = database;
            _items = items;
            _itemsOld = _items.Select(item => (TDatabaseObject)item.Clone()).ToList();
        }

        public TDatabaseObject this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void SaveChanges()
        {
            // Apply changes to the database
            foreach (TDatabaseObject item in _items)
            {
                var oldItem = _itemsOld.Find(i => i.Id == item.Id);

                if (oldItem != null)
                {
                    if (!item.Equals(oldItem))
                        item.Update(_database);
                }
                else
                {
                    item.Insert(_database);
                }
            }
            // Delete old items
            foreach (TDatabaseObject item in _itemsOld)
            {
                if (_items.Find(i => i.Id == item.Id) == null)
                {
                    item.Delete(_database);
                }
            }

            _itemsOld.Clear();
            _itemsOld = _items.Select(item => (TDatabaseObject)item.Clone()).ToList();
        }
        public void RevertChanges()
        {
            _items.Clear();
            _items = new List<TDatabaseObject>(_itemsOld);
            _itemsOld = _items.Select(item => (TDatabaseObject)item.Clone()).ToList();
        }

        public void Add(TDatabaseObject item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(TDatabaseObject item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(TDatabaseObject[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public TDatabaseObject Find(Predicate<TDatabaseObject> match)
        {
            return _items.Find(match);
        }

        public IEnumerator<TDatabaseObject> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(TDatabaseObject item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, TDatabaseObject item)
        {
            _items.Insert(index, item);
        }

        public bool Remove(TDatabaseObject item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
