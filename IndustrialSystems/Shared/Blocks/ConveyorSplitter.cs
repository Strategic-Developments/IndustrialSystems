using IndustrialSystems.IndustrialSystems.Definitions.Structs;
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
    public class ConveyorSplitter : IItemConsumer, IItemProducer
    {
        public IMyCubeBlock Self;
        public IndustrialSystem ParentSystem;

        public ConveyorLine Incoming;
        public ConveyorLine[] Outgoing;
        public Item[] Items;
        
        public int OutgoingIndex;

        public ConveyorSplitter(IMyCubeBlock self, IndustrialSystem parentSystem)
        {
            Self = self;
            ParentSystem = parentSystem;

            Items = new Item[5];
            Outgoing = new ConveyorLine[5];
        }

        public void AcceptItem(Item item)
        {
            for (int i = OutgoingIndex; i < Items.Length + OutgoingIndex; i++)
            {
                if (Items[i % Items.Length].Type != DefinitionConstants.ItemType.None)
                    continue;

                Items[i % Items.Length] = item;
                OutgoingIndex = i % Items.Length;
                break;
            }
        }

        public bool CanAcceptItem(Item item)
        {
            foreach (var i in Items)
            {
                if (i.Type == DefinitionConstants.ItemType.None)
                    return true;
            }

            return false;
        }

        public bool GetNextItemFor(IIndustrialSystemMachine machine, out Item item)
        {
            for (int i = 0; i < Outgoing.Length; i++)
            {
                if (Outgoing[i] == machine)
                {
                    item = Items[i];

                    Items[i].Type = DefinitionConstants.ItemType.None;
                    return true;
                }
            }

            item = Item.CreateInvalid();
            return false;
        }
    }
}
