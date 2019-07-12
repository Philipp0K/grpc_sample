using System.Collections.Generic;
using Google.Protobuf;

namespace Bsvt.Common
{
    public interface IProtoRepository<T> : IEnumerable<T> where T : IMessage<T>, IEntity
    {
        T Get(int key);
        int Add(T item);
        void Remove(int key);
        void Update(T item);
        bool Contains(int key);
    }
}
