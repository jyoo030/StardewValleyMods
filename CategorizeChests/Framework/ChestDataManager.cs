using StardewModdingAPI;
using StardewValley.Objects;
using System.Runtime.CompilerServices;

// Namespace for the mod's framework
namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// Class that manages the data associated with chests.
    /// </summary>
    class ChestDataManager : IChestDataManager
    {
        // Private fields for managing item data and logging
        private readonly IItemDataManager ItemDataManager;
        private readonly IMonitor Monitor;

        // A table that maps Chest objects to their associated ChestData
        private ConditionalWeakTable<Chest, ChestData> Table = new ConditionalWeakTable<Chest, ChestData>();

        /// <summary>
        /// Constructor that initializes the item data manager and monitor.
        /// </summary>
        public ChestDataManager(IItemDataManager itemDataManager, IMonitor monitor)
        {
            ItemDataManager = itemDataManager;
            Monitor = monitor;
        }

        /// <summary>
        /// Get the data associated with a given chest.
        /// </summary>
        /// <param name="chest">The chest to get the data for.</param>
        /// <returns>The data associated with the chest.</returns>
        public ChestData GetChestData(Chest chest)
        {
            return Table.GetValue(chest, c => new ChestData(c, ItemDataManager));
        }
    }
}