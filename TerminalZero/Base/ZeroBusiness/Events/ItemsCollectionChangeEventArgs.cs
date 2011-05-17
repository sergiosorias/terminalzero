using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroBusiness.Events
{
    public class ItemsCollectionChangeEventArgs<TItem> : EventArgs
    {
        public TItem Item { get; private set; }
        public ItemsCollectionChangeEventArgs(TItem item)
        {
            Item = item;
        }
    }
}
