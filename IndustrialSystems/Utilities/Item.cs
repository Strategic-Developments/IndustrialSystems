using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using IndustrialSystems.Utilities;
using static IndustrialSystems.IndustrialSystems.Definitions.Structs.DefinitionConstants;

namespace IndustrialSystems.Utilities
{
    public struct Item : IEquatable<Item>
    {
        private static readonly Dictionary<ushort, ResourceVector> _vectorMap = new Dictionary<ushort, ResourceVector>();
        private static readonly Dictionary<ResourceVector, ushort> _idMap = new Dictionary<ResourceVector, ushort>();
        
        // internally store the item composition as a ushort which maps to a ResourceVector
        // might be overly complex, needs testing
        private ushort _composition;
        public ItemType Type;
        public ResourceVector Composition
        {
            get
            {
                if (_composition == 0)
                    return null;

                return _vectorMap[_composition];
            }  
            set
            {
                if (!_idMap.TryGetValue(value, out _composition))
                {
                    _composition = (ushort)(_idMap.Count+1);
                    _idMap[value] = _composition;
                    _vectorMap[_composition] = value;
                }
            }
        }
        public Item(ItemType Type, ushort Composition = 0)
        {
            this.Type = Type;
            this._composition = Composition;
        }
        public Item(Item toCopy)
        {
            this.Type = toCopy.Type;
            this._composition = toCopy._composition;

        }
        public Item(ItemType Type, ResourceVector vec)
        {
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
            return $"{Type} w/ Composition {_composition}:\n{Composition}";
        }

        bool IEquatable<Item>.Equals(Item other)
        {
            return other.Type == Type && other._composition == _composition;
        }

        public static Item CreateInvalid()
        {
            return new Item
            {
                _composition = 0,
                Type = ItemType.None,
            };
        }

        public bool IsInvalid()
        {
            return _composition != 0 && Type != ItemType.None;
        }
    }

    public struct ConveyorItem : IEquatable<ConveyorItem>
    {
        public Item Item;
        public ushort Distance;

        public ConveyorItem(Item item, ushort dist)
        {
            Item = item;
            Distance = dist;
        }

        bool IEquatable<ConveyorItem>.Equals(ConveyorItem other)
        {
            return Item.Equals(other.Item);
        }
    }

    public struct InventoryItem : IEquatable<InventoryItem>
    {
        public Item Item;
        public int Amount;

        public InventoryItem(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }
        bool IEquatable<InventoryItem>.Equals(InventoryItem other)
        {
            return Amount == other.Amount && Item.Equals(other.Item);
        }
    }
}
