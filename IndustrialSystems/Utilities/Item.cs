using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;

namespace IndustrialSystems.Utilities
{
    public struct Item
    {
        private static readonly Dictionary<ushort, ResourceVector> _vectorMap = new Dictionary<ushort, ResourceVector>();
        private static readonly Dictionary<ResourceVector, ushort> _idMap = new Dictionary<ResourceVector, ushort>();

        public uint Amount;
        private ushort _composition;
        public ItemType Type;
        public ResourceVector Composition
        {
            get
            {
                return _vectorMap[_composition];
            }
            set
            {
                if (!_idMap.TryGetValue(value, out _composition))
                {
                    _composition = (ushort)_idMap.Count;
                    _idMap[value] = _composition;
                    _vectorMap[_composition] = value;
                }
            }
        }
        public Item(ItemType Type, uint Amount = 0, ushort Composition = 0)
        {
            this.Amount = Amount;
            this.Type = Type;
            this._composition = Composition;
        }
        public Item(Item toCopy)
        {
            this.Amount = toCopy.Amount;
            this.Type = toCopy.Type;
            this._composition = toCopy._composition;

        }
        public Item(ItemType Type, uint Amount, ResourceVector vec)
        {
            this.Amount = Amount;
            this.Type = Type;

            if (!_idMap.TryGetValue(vec, out _composition))
            {
                _composition = (ushort)_idMap.Count;
                _idMap[vec] = _composition;

                _vectorMap[_composition] = vec;
            }
        }

        public override string ToString()
        {
            return $"{Amount}kg {Type} w/ Composition {_composition}:\n{Composition}";
        }
    }
    public struct Fluid
    {
        public string GasName;
        public uint Amount;

        public override string ToString()
        {
            return $"{Amount}L of gas {GasName}";
        }
    }
}
