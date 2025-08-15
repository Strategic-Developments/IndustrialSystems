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
    public class Smelter : IItemConsumer, IItemProducer
    {
        public readonly SmelterDefinition Definition;

        public readonly IMyFunctionalBlock Self;
        public IndustrialSystem ParentSystem;

        public ResourceVector Mask;
        public Item MaterialBeingSmelted;
        public Item OutputItem;

        public Smelter(SmelterDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem)
        {
            Definition = definition;
            Self = self;
            ParentSystem = parentSystem;

            Mask = new ResourceVector(definition.SmelterOreMultipliers, definition.DefaultOreMultiplier);
        }

        bool IItemConsumer.AcceptItem(ref Item item)
        {
            if (item.Amount > Definition.MaxOresSmelted)
            {
                MaterialBeingSmelted = new Item(item);
                MaterialBeingSmelted.Amount = Definition.MaxOresSmelted;
                item.Amount -= Definition.MaxOresSmelted;
                RecalculateOutput();
                return false;
            }
            
            MaterialBeingSmelted = item;
            RecalculateOutput();
            return true;
        }

        public void RecalculateOutput()
        {
            OutputItem = new Item(MaterialBeingSmelted);
            OutputItem.Type = ItemType.Ingot;

            var composition = MaterialBeingSmelted.Composition.Copy();
            composition.ElementMultiply(Mask);

            OutputItem.Composition = composition;
        }

        public Item GetProducedItem()
        {
            return OutputItem;
        }
    }
}