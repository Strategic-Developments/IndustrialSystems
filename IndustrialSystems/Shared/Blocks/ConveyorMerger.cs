using IndustrialSystems.Definitions;
using IndustrialSystems.Shared;
using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared.Blocks
{
    public class ConveyorMerger : ISBlock<IMyCubeBlock>, IItemConsumer, IItemProducer
    {
        public ConveyorDefinition.ConveyorDef MergerDef;
        public ConveyorLine[] Incoming;
        public ConveyorLine Outgoing;
        public Item Item;
        
        public int IncomingIndex;

        public ConveyorMerger(IMyCubeBlock self, IndustrialSystem parentSystem, ConveyorDefinition.ConveyorDef def) : base(self, parentSystem)
        {
            MergerDef = def;

            Item = Item.CreateInvalid();
            Incoming = new ConveyorLine[MergerDef.Connections.Length];
        }
        public override void Close()
        {
            
        }
        public void AcceptItem(Item item)
        {
            Item = item;
        }

        public bool CanAcceptItem(IIndustrialSystemMachine machineFrom, Item item)
        {
            if (Item.IsInvalid())
                return true;

            return false;
        }

        public bool GetNextItemFor(IIndustrialSystemMachine machine, out Item item)
        {
            item = Item;
            if (Item.IsInvalid())
            {
                return false;
            }
            Item = Item.CreateInvalid();
            return true;
        }
    }
}
