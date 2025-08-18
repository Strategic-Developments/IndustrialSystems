using IndustrialSystems.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class ConveyorDefinition : Definition
    {
        public NameDef Base;
        public ConveyorDef Conveyors;
        public struct ConveyorDef : IPackagable
        {
            /// <summary>
            /// number of ticks for an item to travel 1 block
            /// </summary>
            public ushort ItemTravelFrequency;
            /// <summary>
            /// 1 connection = discard
            /// 2 connections = regular conveyor
            /// 3+ = merger/splitter
            /// First Vector is the outgoing connection, unless it is a splitter, in which case it is the incoming connection
            /// </summary>
            public Vector3I[] Connections;
            /// <summary>
            /// If there are 3+ connections, then have this act as a splitter instead of a merger
            /// </summary>
            public bool ConvertToSplitter;

            public bool IsConveyor => Connections.Length == 2;
            public bool IsSplitter => Connections.Length >= 3 && ConvertToSplitter;
            public bool IsMerger => Connections.Length >= 3 && !ConvertToSplitter;

            public object[] ConvertToObjectArray()
            {
                return new object[]
                    {
                        ItemTravelFrequency,
                        Connections,
                        ConvertToSplitter,
                    };
            }

            public static ConveyorDef ConvertFromObjectArray(object[] data)
            {
                return new ConveyorDef
                {
                    ItemTravelFrequency = (ushort)data[0],
                    Connections = (Vector3I[])data[1],
                    ConvertToSplitter = (bool)data[2],
                };
            }
        }
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Conveyor,
                Base.ConvertToObjectArray(),
                Conveyors.ConvertToObjectArray(),
            };
        }

        public static ConveyorDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Conveyor)
                return null;

            return new ConveyorDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
                Conveyors = ConveyorDef.ConvertFromObjectArray((object[])data[2]),
            };
        }
    }
    public class ConveyorDefinitions : Definition, IEnumerable<ConveyorDefinition>
    {
        List<ConveyorDefinition> Defs;
        public ConveyorDefinition this[int index]
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
        public ConveyorDefinitions()
        {
            Defs = new List<ConveyorDefinition>();
        }
        public override object[] ConvertToObjectArray()
        {
            object[] arr = new object[Defs.Count];
            for (int i = 0; i < Defs.Count; i++)
                arr[i] = Defs[i].ConvertToObjectArray();

            return new object[] {
                ISTypes.ConveyorArray,
                arr
            };
        }

        public static ConveyorDefinitions ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.ConveyorArray)
                return null;

            object[] dataArray = (object[])data[1];
            List<ConveyorDefinition> list = new List<ConveyorDefinition>(dataArray.Length);

            for (int i = 0; i < dataArray.Length; i++)
                list.Add(ConveyorDefinition.ConvertFromObjectArray((object[])dataArray[i]));

            return new ConveyorDefinitions()
            {
                Defs = list,
            };
        }

        public IEnumerator<ConveyorDefinition> GetEnumerator()
        {
            return ((IEnumerable<ConveyorDefinition>)Defs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Defs.GetEnumerator();
        }

        public void Add(ConveyorDefinition material)
        {
            
            Defs.Add(material);
        }
    }
}
