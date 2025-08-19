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
using KeenItem = VRage.MyTuple<VRage.Game.MyObjectBuilder_PhysicalObject, VRage.MyFixedPoint>;

namespace IndustrialSystems.Shared.Blocks
{

    public class OutputCargo : ISBlock<IMyCargoContainer>, IItemConsumer, IUpdateable
    {
        public readonly OutputCargoDefinition Definition;

        public readonly IMyInventory DepositInventory;
        internal readonly Dictionary<MyDefinitionId, KeenItem> ItemsToAdd;

        private int TickOffset;
        public OutputCargo(OutputCargoDefinition definition, IMyCargoContainer self, IndustrialSystem parentSystem) : base(self, parentSystem)
        {
            Definition = definition;

            ItemsToAdd = new Dictionary<MyDefinitionId, KeenItem>();

            TickOffset = MyAPIGateway.Session.GameplayFrameCounter % 100;

            DepositInventory = self.GetInventory(0);
        }
        public override void Close()
        {

        }
        private void FillItemsToAdd<TObj>(ref Item item) where TObj : MyObjectBuilder_PhysicalObject, new()
        {
            foreach (var kvp in ResourceVector.Map)
            {
                var defId = MyDefinitionId.Parse($"{typeof(TObj)}/{kvp.Key}");
                KeenItem it;
                if (!ItemsToAdd.TryGetValue(defId, out it))
                {
                    ItemsToAdd.Add(defId, new KeenItem
                    {
                        Item1 = MyObjectBuilderSerializer.CreateNewObject<TObj>(kvp.Key),
                        Item2 = (MyFixedPoint)item.Composition.Vector[kvp.Value],
                    });
                }
                else
                {
                    it.Item2 += (MyFixedPoint)item.Composition.Vector[kvp.Value];
                    ItemsToAdd[defId] = it;
                }
            }
        }

        public void Update()
        {
            if (MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Session.GameplayFrameCounter % 100 == TickOffset)
            {
                foreach (var i in ItemsToAdd.Values)
                {
                    if (!DepositInventory.CanItemsBeAdded(i.Item2, i.Item1))
                    {
                        DepositInventory.AddItems(i.Item2, i.Item1);
                    }
                }
                ItemsToAdd.Clear();
            }
        }
        bool IItemConsumer.CanAcceptItem(IIndustrialSystemMachine machineFrom, Item item)
        {
            return true;
        }

        void IItemConsumer.AcceptItem(Item item)
        {
            if (!MyAPIGateway.Multiplayer.IsServer)
            {
                return;
            }


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
