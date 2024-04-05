using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Generic;

namespace ETCRegionManagementSimulator.Collections
{
    public class ETCDataFormatCollection<T> where T : IDataFormat
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);
        public T Get(int index) => items[index];
        public int Count => items.Count;
        public IEnumerable<T> GetAll() => items.AsReadOnly();

    }
}
