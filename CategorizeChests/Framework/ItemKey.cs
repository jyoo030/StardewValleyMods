using Newtonsoft.Json;

namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// A key uniquely identifying a single kind of item selectable as the
    /// contents of a chest.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)] // TODO: can some of these annotations be left out?

    class ItemKey
    {
        // The type of the item
        [JsonProperty] public readonly ItemType ItemType;

        // The index of the item in the game's object information
        [JsonProperty] public readonly int ObjectIndex;

        // Constructor for the ItemKey class
        [JsonConstructor]
        public ItemKey(ItemType itemType, int parentSheetIndex)
        {
            ItemType = itemType;
            ObjectIndex = parentSheetIndex;
        }

        // Override the GetHashCode method to return a unique hash code for this object
        public override int GetHashCode()
        {
            return ((int) ItemType) * 10000 + ObjectIndex;
        }

        // Override the Equals method to compare ItemKey objects by their ItemType and ObjectIndex
        public override bool Equals(object obj)
        {
            return obj is ItemKey itemKey
                   && itemKey.ItemType == ItemType
                   && itemKey.ObjectIndex == ObjectIndex;
        }
    }
}