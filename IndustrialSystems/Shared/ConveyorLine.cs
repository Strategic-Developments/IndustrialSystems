using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class ConveyorLine : IItemConsumer, IItemProducer
    {
        public Item Item;

        public IItemProducer Start;
        public List<Vector3I> Path;
        public IItemConsumer End;
        
        bool IItemConsumer.AcceptItem(ref Item item)
        {
            Item = item;
            return true;
        }

        Item IItemProducer.GetProducedItem()
        {
            return Item;
        }
    }
}
