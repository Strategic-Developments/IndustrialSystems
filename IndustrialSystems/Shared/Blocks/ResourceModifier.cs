using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;
using IndustrialSystems.Shared.Interfaces;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.ModAPI;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Shared.Blocks
{
    public class ResourceModifier : ISBlock<IMyFunctionalBlock>, IItemConsumer, IItemProducer
    {
        public readonly ResourceModifierDefinition Definition;

        public InventoryItem InputItem;
        public InventoryItem OutputItem;
        private int NumberToOutputPerBatch;

        public readonly List<string> UserSelections;

        public int NextItemCounter;

        public ResourceModifier(ResourceModifierDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem) : base (self, parentSystem)
        {
            Definition = definition;
            UserSelections = new List<string>();

            InputItem = new InventoryItem(Item.CreateInvalid(), 0);
            OutputItem = new InventoryItem(Item.CreateInvalid(), 0);

            Self.AppendingCustomInfo += CustomInfo;
        }

        private void CustomInfo(IMyTerminalBlock block, StringBuilder builder)
        {
            builder.Append($"\nInput Inventory Information:\n");
            InputItem.AppendInventoryInformation(builder);
            builder.Append($"Output Inventory Information:\n");
            OutputItem.AppendInventoryInformation(builder);
        }

        public override void Close()
        {
            Self.AppendingCustomInfo -= CustomInfo;
        }
        public override void Update()
        {
            if (InputItem.Amount > Definition.BatchJob.BatchAmount)
            {
                NextItemCounter--;

                if (NextItemCounter <= 0)
                {
                    NextItemCounter = Definition.BatchJob.BatchTimeTicks;

                    OutputItem.Amount += NumberToOutputPerBatch;
                }
            }
        }

        public void RecomputeOutputItem()
        {
            ResourceVector v = InputItem.Item.Composition.Copy();
            NumberToOutputPerBatch = Definition.Modifier.ModifierFunc.Invoke(ResourceVector.Map,
                v.Vector, Definition.BatchJob.BatchAmount, UserSelections);

            OutputItem.Item = new Item(Definition.Modifier.TypeToModify, v);
        }

        bool IItemConsumer.CanAcceptItem(IIndustrialSystemMachine machineFrom, Item item)
        {
            return (InputItem.Amount == 0 && item.Type == Definition.Modifier.TypeToModify) || item.Equals(InputItem.Item);
        }

        void IItemConsumer.AcceptItem(Item item)
        {
            if (InputItem.Amount == 0 && !InputItem.Item.Equals(item))
            {
                InputItem.Item = item;
                RecomputeOutputItem();
            }
            InputItem.Amount++;
        }

        bool IItemProducer.GetNextItemFor(IIndustrialSystemMachine machine, out Item item)
        {
            if (OutputItem.Amount > 0 && !OutputItem.Item.IsInvalid())
            {
                item = OutputItem.Item;
                OutputItem.Amount--;
                return true;
            }

            item = Item.CreateInvalid();
            return false;
        }
    }
}
