using Bsvt.Common;

namespace Bsvt.Stroibat
{
    public partial class Violation : IEntity
    {
        public int GetKey()
        {
            return this.id_;
        }

        public void SetKey(int value)
        {
            this.Id = value;
        }
    }
}
