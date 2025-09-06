using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;
using IndustrialSystems.Shared.Interfaces;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;

namespace IndustrialSystems.Shared.Blocks
{
    public class Smelter : Block<IMyFunctionalBlock>, IItemConsumer, IItemProducer, IUpdateable
    {
        public readonly SmelterDefinition Definition;

        public ResourceVector Mask;
        public InventoryItem InputItem;
        public InventoryItem OutputItem;

        public int NextItemCounter;
        private int OutputBatchNum;
        public Smelter(SmelterDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem) : base (self, parentSystem)
        {
            Definition = definition;

            InputItem = new InventoryItem(Item.CreateInvalid(), 0);
            OutputItem = new InventoryItem(Item.CreateInvalid(), 0);

            Mask = new ResourceVector(definition.SmelterStats.SmelterOreMultipliers, definition.SmelterStats.DefaultOreMultiplier);

            Self.AppendingCustomInfo += CustomInfo;
            NextItemCounter = Definition.BatchJob.BatchTimeTicks;
        }
        private void CustomInfo(IMyTerminalBlock block, StringBuilder builder)
        {
            builder.Append($"\nInput Inventory Information:\n");
            if (InputItem.Amount > Definition.BatchJob.BatchAmount)
            {
                builder.Append($"{Definition.BatchJob.BatchTimeTicks - NextItemCounter}/{Definition.BatchJob.BatchTimeTicks} ({(1 - (float)NextItemCounter / Definition.BatchJob.BatchTimeTicks) * 100:##.##}%) until next batch of items.\n");
            }
            InputItem.AppendInventoryInformation(builder);
            builder.Append($"\nOutput Inventory Information:\n");
            OutputItem.AppendInventoryInformation(builder);
        }

        public override void Close()
        {
            Self.AppendingCustomInfo -= CustomInfo;
        }
        public void Update()
        {
            MyAPIGateway.Utilities.ShowNotification($"smelter {NextItemCounter}", 1);
            if (InputItem.Amount >= Definition.BatchJob.BatchAmount)
            {
                NextItemCounter--;

                if (NextItemCounter <= 0)
                {
                    NextItemCounter = Definition.BatchJob.BatchTimeTicks;

                    OutputItem.Amount += OutputBatchNum;
                    InputItem.Amount -= Definition.BatchJob.BatchAmount;
                }
            }
        }

        public void RecalculateOutput()
        {
            OutputItem.Item = new Item(InputItem.Item);
            OutputItem.Item.Type = DefinitionConstants.ItemType.Ingot;

            var composition = InputItem.Item.Composition.Copy();
            MultiplyAndDiscard(composition, Mask, Definition.SmelterStats.MinimumOreRequired);
            float multiplier = composition.SumNormalize();

            OutputBatchNum = (int)(Math.Floor(Definition.BatchJob.BatchAmount * multiplier));

            OutputItem.Item.Composition = composition;
        }

        bool IItemConsumer.CanAcceptItem(IIndustrialSystemMachine machineFrom, Item item)
        {
            return (InputItem.Amount == 0 && item.Type == DefinitionConstants.ItemType.Ore) || (item.Equals(InputItem.Item) && InputItem.Amount <= Definition.MachineInventory.MaxItemsInInventory);
        }

        void IItemConsumer.AcceptItem(Item item)
        {
            if (InputItem.Amount == 0)
            {
                InputItem.Item = new Item(item);
                InputItem.Amount = 1;
                RecalculateOutput();
            }
            else
            {
                InputItem.Amount++;
            }
        }

        bool IItemProducer.GetNextItemFor(IIndustrialSystemMachine machine, out Item item)
        {
            if (OutputItem.Amount == 0)
            {
                item = Item.CreateInvalid();
                return false;
            }

            item = OutputItem.Item;
            OutputItem.Amount--;
            return true;
        }

        public static void MultiplyAndDiscard(ResourceVector resources, ResourceVector other, float cutoff)
        {
            for (int i = 0; i < ResourceVector.Map.Count; i++)
            {
                if (resources.Vector[i] > cutoff)
                {
                    resources.Vector[i] = resources.Vector[i] * other.Vector[i];
                }
                else
                {
                    resources.Vector[i] = 0;
                }
            }
        }
    }
}