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
using VRage.Utils;
using Ingame = VRage.Game.ModAPI.Ingame;

namespace IndustrialSystems.Shared.Blocks
{

    public class OutputCargo : Block<IMyCargoContainer>, IItemConsumer, IUpdateable
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
        private void FillItemsToAdd<TObj>(ref Item item, bool onlyAddMost) where TObj : MyObjectBuilder_PhysicalObject, new()
        {
            if (onlyAddMost)
            {
                string largestKey = null;
                float largestValue = 0;

                foreach (var kvp in ResourceVector.Map)
                {
                    if (item.Composition.Vector[kvp.Value] > largestValue)
                    {
                        largestValue = item.Composition.Vector[kvp.Value];
                        largestKey = kvp.Key;
                    }
                }
                if (largestKey != null)
                {
                    AddItem<TObj>(largestKey, largestValue);
                }
            }
            else
            {
                foreach (var kvp in ResourceVector.Map)
                {
                    AddItem<TObj>(kvp.Key, item.Composition.Vector[kvp.Value]);
                }
            }
        }

        private void AddItem<TObj>(string item, float amount) where TObj : MyObjectBuilder_PhysicalObject, new()
        {
            if (amount <= 0)
                return;

            var defId = MyDefinitionId.Parse($"{typeof(TObj).Name}/{item}");
            KeenItem it;
            if (!ItemsToAdd.TryGetValue(defId, out it))
            {
                ItemsToAdd.Add(defId, new KeenItem
                {
                    Item1 = MyObjectBuilderSerializer.CreateNewObject<TObj>(item),
                    Item2 = (MyFixedPoint)amount,
                });
            }
            else
            {
                it.Item2 += (MyFixedPoint)amount;
                ItemsToAdd[defId] = it;
            }
        }

        public void Update()
        {
            if (MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Session.GameplayFrameCounter % 100 == TickOffset)
            {
                foreach (var i in ItemsToAdd.Values)
                {
                    if (DepositInventory.CanItemsBeAdded(i.Item2, i.Item1))
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
                    FillItemsToAdd<MyObjectBuilder_Ore>(ref item, true);
                    break;
                case DefinitionConstants.ItemType.Ingot:
                    FillItemsToAdd<MyObjectBuilder_Ingot>(ref item, false);
                    break;
            }
        }
    }
}
