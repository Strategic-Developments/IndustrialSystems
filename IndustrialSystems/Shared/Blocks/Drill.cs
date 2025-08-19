using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;
using IndustrialSystems.Shared.Interfaces;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game;
using VRage.Voxels;
using VRageMath;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Utils;
using VRage.ModAPI;


namespace IndustrialSystems.Shared.Blocks
{
    using Material = VRage.MyTuple<MaterialDefinition, int>;
    public class Drill : ISBlock<IMyFunctionalBlock>, IItemProducer, IUpdateable
    {
        public readonly DrillDefinition Definition;

        public bool CalculatingMaterials;

        public bool HasOreSelected;

        public Material SelectedChoice;
        public readonly List<Material> UserChoices;

        public int NextBatchCounter;
        public InventoryItem OutputItem;

        private static bool CreatedTerminalControls;

        public Drill(DrillDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem) : base(self, parentSystem) 
        {
            Definition = definition;
            SelectedChoice = new Material(null, 0);
            UserChoices = new List<Material>();

            HasOreSelected = false;

            Self.AppendingCustomInfo += CustomInfo;

            if (MyAPIGateway.Multiplayer.IsServer)
            {
                MyAPIGateway.Parallel.Start(CalculateAvailableMaterials);
            }
        }
        public override void Close()
        {
            Self.AppendingCustomInfo -= CustomInfo;
        }
        public void CreateTerminalControls()
        {
            CreatedTerminalControls = true;
            var list = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlListbox, IMyShipDrill>("IndustrialSystems_DrillListBox");
            list.Title = MyStringId.GetOrCompute("Resource to Mine:");
            //c.Tooltip = MyStringId.GetOrCompute("This does some stuff!"); // presenece of this tooltip prevents per-item tooltips
            list.SupportsMultipleBlocks = true;
            list.Visible = IsVisible;

            list.VisibleRowsCount = 3;
            list.Multiselect = false; // wether player can select muliple at once (ctrl+click, click&shift+click, etc)
            list.ListContent = (b, content, preSelect) =>
            {
                IIndustrialSystemMachine machine;
                if (IndustrialSystemManager.I.AllMachines.TryGetValue(b.EntityId, out machine) && machine is Drill)
                {
                    Drill d = (Drill)machine;

                    if (d.CalculatingMaterials)
                    {
                        content.Add(new MyTerminalControlListBoxItem(
                            text: MyStringId.GetOrCompute($"Calculating Materials, please wait"),
                            tooltip: MyStringId.GetOrCompute($"Calculating Materials, please wait"),
                            userData: ""));
                        return;
                    }

                    foreach (var choice in UserChoices)
                    {
                        var listItem = new MyTerminalControlListBoxItem(
                            text: MyStringId.GetOrCompute($"{choice.Item1.Material.DisplayName}"),
                            tooltip: MyStringId.GetOrCompute($"{((float)choice.Item2/d.Definition.DrillBatches.TimeBetweenBatches)*60:########}/s."),
                            userData: choice.Item1.Base.SubtypeId);
                        content.Add(listItem);

                        if (SelectedChoice.Item1 == null)
                            continue;

                        if (SelectedChoice.Item1 == choice.Item1)
                        {
                            preSelect.Add(listItem);
                        }
                    }
                }
            };
            list.ItemSelected = (b, selected) =>
            {
                IIndustrialSystemMachine machine;
                if (IndustrialSystemManager.I.AllMachines.TryGetValue(b.EntityId, out machine) && machine is Drill)
                {
                    Drill d = (Drill)machine;

                    if (selected.Count == 0 || !(selected[0].UserData is string))
                    {
                        d.SelectedChoice = new Material(null, 0);
                        return;
                    }

                    SelectMaterial((string)selected[0].UserData);
                }

            };
            MyAPIGateway.TerminalControls.AddControl<IMyShipDrill>(list);
        }
        public static bool IsVisible(IMyTerminalBlock b)
        {
            Definition def;
            return Config.I.BlockDefinitions.TryGetValue(b.BlockDefinition.SubtypeName, out def) && def is DrillDefinition;
        }
        public void Update()
        {
            if (!MyAPIGateway.Utilities.IsDedicated && !CreatedTerminalControls)
            {
                CreateTerminalControls();
            }

            if (Self.IsWorking && HasOreSelected && (OutputItem.Amount < Definition.MachineInventory.MaxItemsInInventory))
            {
                NextBatchCounter--;

                if (NextBatchCounter <= 0)
                {
                    NextBatchCounter = Definition.DrillBatches.TimeBetweenBatches;
                    OutputItem.Amount += SelectedChoice.Item2;
                }
            }
        }
        public void SelectMaterial(string mat)
        {
            HasOreSelected = false;
            SelectedChoice = new Material(null, 0);
            foreach (var choice in UserChoices)
            {
                if (choice.Item1.Base.SubtypeId == mat)
                {
                    if (SelectedChoice.Item1 == choice.Item1 && SelectedChoice.Item2 == choice.Item2)
                        return;

                    SelectedChoice = choice;
                    HasOreSelected = true;

                    ResourceVector vector = new ResourceVector(SelectedChoice.Item1.Material.MaterialProperties);
                    OutputItem = new InventoryItem(new Item(DefinitionConstants.ItemType.Ore, vector), 0);
                    return;
                }
            }
        }

        private void CustomInfo(IMyTerminalBlock block, StringBuilder builder)
        {
            builder.Append($"\nDrill Information:\n");

            if (CalculatingMaterials)
            {
                builder.Append($"Scanning.\n");
                return;
            }
            if (Self.IsWorking && HasOreSelected && (OutputItem.Amount < Definition.MachineInventory.MaxItemsInInventory))
            {
                builder.Append($"{Definition.DrillBatches.TimeBetweenBatches-NextBatchCounter}/{Definition.DrillBatches.TimeBetweenBatches} ({(1-(float)NextBatchCounter/Definition.DrillBatches.TimeBetweenBatches)*100:##.##}%) until next batch of {SelectedChoice.Item2} items.\n");
            }

            if (HasOreSelected)
            {
                SelectedChoice.Item1.AppendMaterialProperties(builder);
            }
            builder.Append($"\nOutput Item Information:\n");
            OutputItem.AppendInventoryInformation(builder);
        }
        public void CalculateAvailableMaterials()
        {
            CalculatingMaterials = true;

            List<MyVoxelBase> voxelbaseList = new List<MyVoxelBase>();
            Dictionary<byte, int> initialVoxelDict = new Dictionary<byte, int>();

            GetVoxelsInBox(voxelbaseList, initialVoxelDict, Self.WorldMatrix.Translation, Definition.DrillVoxelChecks.InitialVoxelCheckSize);

            UserChoices.Clear();

            if (initialVoxelDict.Count > 0)
            {
                for (int i = 0; i < Definition.DrillVoxelChecks.DownwardVoxelCheckSizeAmount; i++)
                {
                    Vector3 PositionToCheck = Self.WorldMatrix.Translation - Self.WorldMatrix.Down * Definition.DrillVoxelChecks.DownwardVoxelCheckSizeInterval * i;
                    GetVoxelsInBox(voxelbaseList, initialVoxelDict, PositionToCheck, Definition.DrillVoxelChecks.DownwardsVoxelCheckSize);
                }
            }
            Dictionary<MaterialDefinition, int> Materials = new Dictionary<MaterialDefinition, int>();
            foreach (var m in initialVoxelDict.Keys)
            {
                var def = MyDefinitionManager.Static.GetVoxelMaterialDefinition(m);
                if (def != null)
                {

                    MaterialDefinition d;
                    if (!Config.I.MaterialVoxelDefinitions.TryGetValue(m, out d) &&
                        !Config.I.MaterialOreDefinitions.TryGetValue(def.MinedOre, out d))
                    {
                        continue;
                    }

                    if (!Materials.ContainsKey(d))
                        Materials.Add(d, 0);

                    Materials[d] += initialVoxelDict[m];
                }
            }

            foreach (var material in Materials)
            {
                int miningSpeed;
                if (!Definition.DrillBatches.OresPerBatchPerMaterial.TryGetValue(material.Key.Base.SubtypeId, out miningSpeed))
                    miningSpeed = Definition.DrillBatches.DefaultOresPerBatch;

                miningSpeed = (int)Math.Floor(miningSpeed *
                    (Definition.DrillBatches.VoxelAmountMultiplier == 0 ? 1 :
                    material.Value * Definition.DrillBatches.VoxelAmountMultiplier));

                if (miningSpeed > 0)
                {
                    UserChoices.Add(new Material(material.Key, miningSpeed));
                }
            }
            

            CalculatingMaterials = false;
        }

        private void GetVoxelsInBox(List<MyVoxelBase> detected, Dictionary<byte, int> materials, Vector3 PositionToCheck, float CheckSize)
        {
            BoundingBoxD initialVoxelCheck = new BoundingBoxD(PositionToCheck - CheckSize, PositionToCheck + CheckSize);
            MyGamePruningStructure.GetAllVoxelMapsInBox(ref initialVoxelCheck, detected);
            foreach (var m in detected)
            {
                GetVoxelsInScope(PositionToCheck, CheckSize, m, materials);
            }
            detected.Clear();
        }

        private void GetVoxelsInScope(Vector3D pos, float size, MyVoxelBase map, Dictionary<byte, int> foundMaterials)
        {
            const int LOD = 2; // the trolling is immense with this one

            Vector3D posMin = pos - size;
            Vector3D posMax = pos + size;
            Vector3I voxelPosMin, voxelPosMax;
            MyVoxelCoordSystems.WorldPositionToVoxelCoord(map.PositionLeftBottomCorner, ref posMin, out voxelPosMin);
            MyVoxelCoordSystems.WorldPositionToVoxelCoord(map.PositionLeftBottomCorner, ref posMax, out voxelPosMax);

            voxelPosMin = Vector3I.Max(map.StorageMin, voxelPosMin) >> LOD;
            voxelPosMax = Vector3I.Max((Vector3I.Min(map.StorageMax, voxelPosMax) >> LOD) - 1, voxelPosMin);

            MyStorageData cache = new MyStorageData(MyStorageDataTypeFlags.ContentAndMaterial);
            cache.Resize(voxelPosMin, voxelPosMax);

            map.Storage.ReadRange(cache, MyStorageDataTypeFlags.ContentAndMaterial, LOD, voxelPosMin, voxelPosMax);


            for (int i = 0; i < cache[MyStorageDataTypeEnum.Content].Length; i++)
            {
                if (cache.Content(i) > MyVoxelConstants.VOXEL_ISO_LEVEL)
                {
                    if (foundMaterials.ContainsKey(cache.Material(i)))
                    {
                        foundMaterials[cache.Material(i)] += cache.Content(i);
                    }
                    else
                    {
                        foundMaterials.Add(cache.Material(i), cache.Content(i));
                    }
                }
            }
        }

        public bool GetNextItemFor(IIndustrialSystemMachine machine, out Item item)
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