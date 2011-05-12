using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AplusCore.Types.MemoryMapped
{
    class MMAArrayIterator : IEnumerator<AType>
    {
        private int cursor;
        private AType items;

        public MMAArrayIterator(AType argument)
        {
            this.cursor = -1;
            this.items = argument;
        }

        AType IEnumerator<AType>.Current
        {
            get { return items[this.cursor]; }
        }

        void IDisposable.Dispose() { }

        object IEnumerator.Current
        {
            get { return items[this.cursor]; }
        }

        bool IEnumerator.MoveNext()
        {
            ++this.cursor;

            if (this.cursor > (this.items.Length - 1))
            {
                return false;
            }

            return true;
        }

        void IEnumerator.Reset()
        {
            this.cursor = -1;
        }
    }
}
