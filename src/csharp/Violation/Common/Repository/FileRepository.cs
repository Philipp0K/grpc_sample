using System;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Collections;

namespace Bsvt.Common
{
    public class FileRepository<T> : IProtoRepository<T>, IDisposable where T : IMessage<T>, IEntity, new()
    {
        protected IPathProvider provider;
        readonly protected Dictionary<int, T> data = new Dictionary<int, T>();
        public FileRepository(IPathProvider provider)
        {
            var parser = new MessageParser<T>(() => { return new T(); });
            this.provider = provider;
            using (var stream = new FileStream(provider.GetPath(), FileMode.OpenOrCreate))
            {
                while (stream.Position != stream.Length)
                {
                    var item = parser.ParseDelimitedFrom(stream);
                    data.Add(item.GetKey(), item);
                    if (stream.Position > stream.Length)
                        throw new Exception($"broken repository at path: {provider.GetPath()}");
                }
            }
        }
        ~FileRepository()
        {
            Dispose(false);
        }

        public virtual int Add(T item)
        {
            int id = 0;
            if (data.Count > 0)
                id = data[data.Count - 1].GetKey() + 1;
            item.SetKey(id);
            data.Add(id, item);
            using (var stream = new FileStream(provider.GetPath(), FileMode.Append))
                item.WriteDelimitedTo(stream);
            return id;
        }

        public bool Contains(int key)
        {
            return data.ContainsKey(key);
        }

        public T Get(int key)
        {
            return data[key];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)(data.Values)).GetEnumerator();
        }

        public void Update(T item)
        {
            data[item.GetKey()] = item;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool isDisposing)
        {
            Save();
        }

        public void Save()
        {
            using (var fs = new FileStream(provider.GetPath(), FileMode.OpenOrCreate))
                foreach (var d in data.Values)
                    d.WriteDelimitedTo(fs);
        }

        public void Remove(int key)
        {
            if (!data.Remove(key)) return;
            Save();
        }
    }
}
