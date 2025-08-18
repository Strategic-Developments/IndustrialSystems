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


namespace IndustrialSystems.Shared.Blocks
{
    using Material = VRage.MyTuple<MaterialDefinition, int>;
    public class Drill : ISBlock<IMyFunctionalBlock>, IItemProducer
    {
        public readonly DrillDefinition Definition;

        public bool CalculatingMaterials;

        public ResourceVector MaterialBeingMined;
        public bool HasOreSelected;

        public Material SelectedChoice;
        public readonly List<Material> UserChoices;

        public int NextBatchCounter;
        public InventoryItem OutputItem;

        public Drill(DrillDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem) : base(self, parentSystem) 
        {
            Definition = definition;
            SelectedChoice = new Material(null, 0);
            UserChoices = new List<Material>();

            HasOreSelected = false;

            Self.AppendingCustomInfo += CustomInfo;

            MyAPIGateway.Parallel.StartBackground(CalculateAvailableMaterials);
        }

        public override void Update()
        {
            if (Self.IsWorking && HasOreSelected && OutputItem.Amount < Definition.MachineInventory.MaxItemsInInventory)
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
            foreach (var choice in UserChoices)
            {
                if (choice.Item1.Base.SubtypeId == mat)
                {
                    SelectedChoice = choice;
                    HasOreSelected = true;
                    break;
                }
            }

            MaterialBeingMined = new ResourceVector(SelectedChoice.Item1.Material.MaterialProperties);
        }

        private void CustomInfo(IMyTerminalBlock block, StringBuilder builder)
        {
            builder.Append($"\nDrill Information:\n");
            if (HasOreSelected)
            {

                builder.Append($"Currently producing {SelectedChoice.Item1.Material.DisplayName} at {SelectedChoice.Item2}/s\n");
                builder.Append($"Ore Material Properties:\n");
                SelectedChoice.Item1.AppendMaterialProperties(builder);
            }
            else if (UserChoices.Count > 0)
                builder.Append($"Not producing anything - Select an option from the terminal menu.\n");
            else
                builder.Append($"Not producing anything - No voxels found.\n");
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
                for (int i = 1; i < Definition.DrillVoxelChecks.DownwardVoxelCheckSizeAmount; i++)
                {
                    Vector3 PositionToCheck = Self.WorldMatrix.Translation - Self.WorldMatrix.Down * Definition.DrillVoxelChecks.DownwardVoxelCheckSizeInterval * i;

                    GetVoxelsInBox(voxelbaseList, initialVoxelDict, PositionToCheck, Definition.DrillVoxelChecks.DownwardsVoxelCheckSize);
                }
            }
            
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
                    int miningSpeed;
                    if (!Definition.DrillBatches.OresPerBatchPerMaterial.TryGetValue(d.Base.SubtypeId, out miningSpeed))
                        miningSpeed = Definition.DrillBatches.DefaultOresPerBatch;
                    miningSpeed = (int)(Math.Floor(miningSpeed * Definition.DrillVoxelChecks.VoxelAmountMultiplier == 0 ? 1 :
                        initialVoxelDict[m] * Definition.DrillVoxelChecks.VoxelAmountMultiplier));

                    UserChoices.Add(new Material(d, miningSpeed));
                }
            }

            CalculatingMaterials = false;
        }

        private void GetVoxelsInBox(List<MyVoxelBase> detected, Dictionary<byte, int> materials, Vector3 PositionToCheck, float CheckSize)
        {
            BoundingBoxD initialVoxelCheck = new BoundingBoxD(PositionToCheck - CheckSize,
                            PositionToCheck + CheckSize);
            MyGamePruningStructure.GetAllVoxelMapsInBox(ref initialVoxelCheck, detected);
            foreach (var m in detected)
            {
                GetVoxelsInScope(PositionToCheck, CheckSize, m, materials);
            }

            detected.Clear();
        }

        private void GetVoxelsInScope(Vector3D pos, float size, MyVoxelBase map, Dictionary<byte, int> foundMaterials)
        {
            const int LOD = 0; // the trolling is immense with this one

            Vector3D posMin = pos - size;
            Vector3D posMax = pos + size;
            Vector3I voxelPosMin, voxelPosMax;
            MyVoxelCoordSystems.WorldPositionToVoxelCoord(map.PositionLeftBottomCorner, ref posMin, out voxelPosMin);
            MyVoxelCoordSystems.WorldPositionToVoxelCoord(map.PositionLeftBottomCorner, ref posMax, out voxelPosMax);

            voxelPosMin = Vector3I.Min(map.StorageMin, voxelPosMin) >> LOD;
            voxelPosMax = (Vector3I.Min(map.StorageMax, voxelPosMax) >> LOD) - 1;

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