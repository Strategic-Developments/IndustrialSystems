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
    public class Drill : IItemProducer
    {
        public readonly DrillDefinition Definition;

        public readonly IMyFunctionalBlock Self;
        public IndustrialSystem ParentSystem;

        public IItemConsumer Consumer;
        public bool CalculatingMaterials;

        public ResourceVector MaterialBeingMined;
        public bool IsProducing;

        public Material SelectedChoice;
        public readonly List<Material> UserChoices;

        public Drill(DrillDefinition definition, IMyFunctionalBlock self, IndustrialSystem parentSystem)
        {
            Definition = definition;
            Self = self;
            ParentSystem = parentSystem;
            SelectedChoice = new Material(null, 0);
            UserChoices = new List<Material>();

            IsProducing = false;

            Self.AppendingCustomInfo += CustomInfo;

            MyAPIGateway.Parallel.StartBackground(CalculateAvailableMaterials);
        }
        public void SelectMaterial(string mat)
        {
            IsProducing = false;
            foreach (var choice in UserChoices)
            {
                if (choice.Item1.SubtypeId == mat)
                {
                    SelectedChoice = choice;
                    IsProducing = true;
                    break;
                }
            }

            MaterialBeingMined = new ResourceVector(SelectedChoice.Item1.MaterialProperties);
        }

        private void CustomInfo(IMyTerminalBlock block, StringBuilder builder)
        {
            builder.Append($"\nDrill Information:\n");
            if (IsProducing)
            {

                builder.Append($"Currently producing {SelectedChoice.Item1.DisplayName} at {SelectedChoice.Item2}/s\n");
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

            GetVoxelsInBox(voxelbaseList, initialVoxelDict, Self.WorldMatrix.Translation, Definition.InitialVoxelCheckSize);

            UserChoices.Clear();

            if (initialVoxelDict.Count > 0)
            {
                for (int i = 1; i < Definition.DownwardVoxelCheckSizeAmount; i++)
                {
                    Vector3 PositionToCheck = Self.WorldMatrix.Translation - Self.WorldMatrix.Down * Definition.DownwardVoxelCheckSizeInterval * i;

                    GetVoxelsInBox(voxelbaseList, initialVoxelDict, PositionToCheck, Definition.DownwardsVoxelCheckSize);
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
                    if (!Definition.MaterialDrillSpeed.TryGetValue(d.SubtypeId, out miningSpeed))
                        miningSpeed = Definition.DefaultDrillSpeed;
                    miningSpeed = (int)(Math.Floor(miningSpeed * Definition.VoxelAmountMultiplier == 0 ? 1 :
                        initialVoxelDict[m] * Definition.VoxelAmountMultiplier));

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
            const int LOD = 0;

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

        Item IItemProducer.GetProducedItem()
        {
            return new Item(ItemType.Ore, SelectedChoice.Item2, MaterialBeingMined);
        }
    }
}