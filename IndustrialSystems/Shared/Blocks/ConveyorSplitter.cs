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
    public class ConveyorSplitter : ISBlock<IMyCubeBlock>, IItemConsumer, IItemProducer, IConveyorJunction
    {
        public ConveyorDefinition.ConveyorDef SplitterDef;
        public ConveyorLine Incoming;
        public ConveyorLine[] Outgoing;
        public Item[] Items;
        
        public int OutgoingIndex;

        public ConveyorSplitter(IMyCubeBlock self, IndustrialSystem parentSystem, ConveyorDefinition.ConveyorDef def) : base(self, parentSystem)
        {
            SplitterDef = def;
            Items = new Item[SplitterDef.Connections.Length];
            Outgoing = new ConveyorLine[SplitterDef.Connections.Length];
        }
        public override void Close()
        {

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

        public bool CanAcceptItem(IIndustrialSystemMachine machineFrom, Item item)
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

        public bool Link(ConveyorLine line, IMyCubeBlock connector, bool isIncommingConnection)
        {
            Vector3I diff = (connector.Min - Self.Min + connector.Max - Self.Max) / 2;

            Vector3I transformedVector = Self.InverseTransformVector(diff);
            int connectionVecIndex = Array.IndexOf(SplitterDef.Connections, transformedVector);
            if (connectionVecIndex == -1)
                return false;

            if (connectionVecIndex == 0 && isIncommingConnection)
            {
                Incoming = line;
                return true;
            }
            else if (!isIncommingConnection)
            {
                Outgoing[connectionVecIndex] = line;
                return true;
            }
            return false;
        }
    }
}
