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
    public class Smelter : ISBlock<IMyFunctionalBlock>, IItemConsumer, IItemProducer
    {
        public readonly SmelterDefinition Definition;

        public ResourceVector Mask;
        public InventoryItem InputItem;
        public InventoryItem OutputItem;

        public int NextItemCounter;

        public Smelter(SmelterDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem) : base (self, parentSystem)
        {
            Definition = definition;

            InputItem = new InventoryItem(Item.CreateInvalid(), 0);
            OutputItem = new InventoryItem(Item.CreateInvalid(), 0);

            Mask = new ResourceVector(definition.SmelterStats.SmelterOreMultipliers, definition.SmelterStats.DefaultOreMultiplier);
        }

        public override void Update()
        {
            if (InputItem.Amount > Definition.BatchJob.BatchAmount)
            {
                NextItemCounter--;

                if (NextItemCounter <= 0)
                {
                    NextItemCounter = Definition.BatchJob.BatchTimeTicks;

                    OutputItem.Amount += Definition.BatchJob.BatchAmount;
                }
            }
        }

        public void RecalculateOutput()
        {
            OutputItem.Item = new Item(InputItem.Item);
            OutputItem.Item.Type = DefinitionConstants.ItemType.Ingot;

            var composition = InputItem.Item.Composition.Copy();
            composition.ElementMultiply(Mask);

            OutputItem.Item.Composition = composition;
        }

        bool IItemConsumer.CanAcceptItem(Item item)
        {
            return (InputItem.Amount == 0 && item.Type == DefinitionConstants.ItemType.Ore) || item.Equals(InputItem.Item);
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
    }
}