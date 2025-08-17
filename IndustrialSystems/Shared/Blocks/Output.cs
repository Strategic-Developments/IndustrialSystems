using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;
using IndustrialSystems.Shared.Interfaces;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using KeenItem = VRage.MyTuple<VRage.MyFixedPoint, VRage.Game.MyObjectBuilder_PhysicalObject>;
using IndustrialSystems.IndustrialSystems.Definitions.Structs;

namespace IndustrialSystems.Shared.Blocks
{
    public class Output : ISBlock<IMyCargoContainer>, IItemConsumer
    {
        public readonly OutputDefinition Definition;

        public readonly IMyInventory DepositInventory;
        public readonly List<KeenItem> ItemsToAdd;
        public Output(OutputDefinition definition, IMyCargoContainer self, IndustrialSystem parentSystem) : base(self, parentSystem)
        {
            Definition = definition;

            ItemsToAdd = new List<KeenItem>();

            DepositInventory = self.GetInventory(0);
        }
        
        private void FillItemsToAdd<TObj>(ref Item item) where TObj : MyObjectBuilder_PhysicalObject, new()
        {
            foreach (var kvp in ResourceVector.Map)
            {
                ItemsToAdd.Add(new KeenItem
                {
                    Item1 = (MyFixedPoint)item.Composition.Vector[kvp.Value],
                    Item2 = MyObjectBuilderSerializer.CreateNewObject<TObj>(kvp.Key),
                });
            }
        }

        public void Update100()
        {
            foreach (var i in ItemsToAdd)
            {
                if (!DepositInventory.CanItemsBeAdded(i.Item1, i.Item2))
                {
                    DepositInventory.AddItems(i.Item1, i.Item2);
                }
            }
            ItemsToAdd.Clear();
        }
        bool IItemConsumer.CanAcceptItem(Item item)
        {
            return true;
        }

        void IItemConsumer.AcceptItem(Item item)
        {
            switch (item.Type)
            {
                case DefinitionConstants.ItemType.Ore:
                    FillItemsToAdd<MyObjectBuilder_Ore>(ref item);
                    break;
                case DefinitionConstants.ItemType.Ingot:
                    FillItemsToAdd<MyObjectBuilder_Ingot>(ref item);
                    break;
            }
        }
    }
}
