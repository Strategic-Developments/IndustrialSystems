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

namespace IndustrialSystems.Shared.Blocks
{
    public class Output : IItemConsumer
    {
        public readonly OutputDefinition Definition;

        public readonly IMyFunctionalBlock Self;
        public IndustrialSystem ParentSystem;

        public readonly IMyInventory DepositInventory;
        public readonly List<KeenItem> ItemsToAdd;
        public Output(OutputDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem)
        {
            Definition = definition;
            Self = self;
            ParentSystem = parentSystem;

            ItemsToAdd = new List<KeenItem>();

            DepositInventory = self.GetInventory(0);
        }


        bool IItemConsumer.AcceptItem(ref Item item)
        {
            ItemsToAdd.Clear();

            switch (item.Type)
            {
                case ItemType.Ore:
                    FillItemsToAdd<MyObjectBuilder_Ore>(ref item);
                    break;
                case ItemType.Ingot:
                    FillItemsToAdd<MyObjectBuilder_Ingot>(ref item);
                    break;
            }

            return true;
        }
        
        private void FillItemsToAdd<TObj>(ref Item item) where TObj : MyObjectBuilder_PhysicalObject, new()
        {
            foreach (var kvp in ResourceVector.Map)
            {
                ItemsToAdd.Add(new KeenItem
                {
                    Item1 = (MyFixedPoint)(item.Composition.Vector[kvp.Value] * item.Amount),
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
            
        }
    }
}
