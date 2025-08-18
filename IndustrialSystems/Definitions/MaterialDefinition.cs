using IndustrialSystems.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class MaterialDefinition : Definition
    {
        public NameDef Base;
        public MaterialDef Material;
        public struct MaterialDef : IPackagable
        {
            /// <summary>
            /// Name players will see for materials shown in terminals
            /// </summary>
            public string DisplayName;
            /// <summary>
            /// If true, has this definition point to the voxel's MinedOre instead of its subtype.
            /// </summary>
            public bool IsMinedOre;

            /// <summary>
            /// Key: Ore name (keen Ore subtype like Iron, Nickel, etc)
            /// Value: Percentage per ore
            /// "None" for empty space
            /// Values must sum to 1, if they aren't then they will be scaled to have the sum = 1
            /// </summary>
            public Dictionary<string, float> MaterialProperties;

            public object[] ConvertToObjectArray()
            {
                return new object[]
                {
                    DisplayName,
                    IsMinedOre,
                    MaterialProperties,
                };
            }

            public static MaterialDef ConvertFromObjectArray(object[] data)
            {
                return new MaterialDef
                {
                    DisplayName = (string)data[0],
                    IsMinedOre = (bool)data[1],
                    MaterialProperties = (Dictionary<string, float>)data[2],
                };
            }
        }
        public void AppendMaterialProperties(StringBuilder sb)
        {
            foreach (var kvp in Material.MaterialProperties)
            {
                sb.Append($"{kvp.Key}: {kvp.Value*100:##.####}%\n");
            }
        }

        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Material,
                Base.ConvertToObjectArray(),
                Material.ConvertToObjectArray(),
            };
        }

        public static MaterialDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Material)
                return null;

            return new MaterialDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
                Material = MaterialDef.ConvertFromObjectArray((object[])data[2]),
            };
        }
    }
    public class MaterialDefinitions : Definition, IEnumerable<MaterialDefinition>
    {
        List<MaterialDefinition> Defs;
        public MaterialDefinition this[int index]
        {
            get
            {
                return Defs[index];
            }
            set
            {
                Defs[index] = value;
            }
        }
        public MaterialDefinitions()
        {
            Defs = new List<MaterialDefinition>();
        }
        public override object[] ConvertToObjectArray()
        {
            object[] arr = new object[Defs.Count];
            for (int i = 0; i < Defs.Count; i++)
                arr[i] = Defs[i].ConvertToObjectArray();

            return new object[] {
                ISTypes.MaterialArray,
                arr
            };
        }

        public static MaterialDefinitions ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.MaterialArray)
                return null;

            object[] dataArray = (object[])data[1];
            List<MaterialDefinition> list = new List<MaterialDefinition>(dataArray.Length);

            for (int i = 0; i < dataArray.Length; i++)
                list.Add(MaterialDefinition.ConvertFromObjectArray((object[])dataArray[i]));

            return new MaterialDefinitions()
            {
                Defs = list,
            };
        }

        public IEnumerator<MaterialDefinition> GetEnumerator()
        {
            return ((IEnumerable<MaterialDefinition>)Defs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Defs.GetEnumerator();
        }

        public void Add(MaterialDefinition material)
        {
            Defs.Add(material);
        }
    }
}
