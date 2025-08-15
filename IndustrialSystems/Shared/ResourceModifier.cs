using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.ModAPI;

namespace IndustrialSystems.Shared
{
    public class ResourceModifier : IItemProducer, IItemConsumer
    {
        public ResourceModifierDefinition Definition;

        public IMyFunctionalBlock self;

        public Item InputItem;
        public Item OutputItem;

        List<MyTerminalControlListBoxItem> UserSelections;

        public bool AcceptItem(ref Item item)
        {
            InputItem = item;

            RecalculateItem();
            return true;
        }

        public void RecalculateItem()
        {
            OutputItem = new Item(InputItem);

            OutputItem.Amount = Definition.ModifierFunc.Invoke(ResourceVector.Map, OutputItem.Composition.Vector, OutputItem.Amount, UserSelections);
        }

        Item IItemProducer.GetProducedItem()
        {
            return new Item();
        }
    }
}
