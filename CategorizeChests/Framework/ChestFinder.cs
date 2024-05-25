using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Linq;
using StardewValleyMods.CategorizeChests.Framework.Persistence;

// Namespace for the mod's framework
namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// Class that finds chests in the game world.
    /// </summary>
    class ChestFinder : IChestFinder
    {
        /// <summary>
        /// Get a chest by its address.
        /// </summary>
        /// <param name="address">The address of the chest.</param>
        /// <returns>The chest at the given address.</returns>
        public Chest GetChestByAddress(ChestAddress address)
        {
            // If the location type of the address is a refrigerator
            if (address.LocationType == ChestLocationType.Refrigerator)
            {
                // Find the farmhouse location
                var farmHouse = (FarmHouse) Game1.locations.First(l => l is FarmHouse);

                // If the player's house upgrade level is less than 1, throw an exception
                if (Game1.player.HouseUpgradeLevel < 1)
                    throw new InvalidSaveDataException(
                        "Chest save data contains refrigerator data but no refrigerator exists");

                // Return the fridge in the farmhouse
                return farmHouse.fridge;
            }
            else
            {
                // Get the location from the address
                var location = GetLocationFromAddress(address);

                // If the location contains a chest at the address's tile
                if (location.objects.ContainsKey(address.Tile) && location.objects[address.Tile] is Chest chest)
                {
                    // Return the chest
                    return location.objects[address.Tile] as Chest;
                }
                else
                {
                    // If no chest is found, throw an exception
                    throw new InvalidSaveDataException($"Can't find chest in {location.Name} at {address.Tile}");
                }
            }
        }

        /// <summary>
        /// Get a game location from a chest address.
        /// </summary>
        /// <param name="address">The address of the chest.</param>
        /// <returns>The game location of the chest.</returns>
        private GameLocation GetLocationFromAddress(ChestAddress address)
        {
            // Find the first location that matches the location name in the address
            var location = Game1.locations.FirstOrDefault(l => l.Name == address.LocationName);

            // If no location is found, throw an exception
            if (location == null)
                throw new InvalidSaveDataException($"Can't find location named {address.LocationName}");

            // If the location type of the address is a building
            if (address.LocationType == ChestLocationType.Building)
            {
                // If the location is a buildable location
                if (location is BuildableGameLocation buildableLocation)
                {
                    // Find the first building that matches the building name in the address
                    var building = buildableLocation.buildings // TODO: check
                        .FirstOrDefault(b => b.nameOfIndoors == address.BuildingName);

                    // If no building is found, throw an exception
                    if (building == null)
                        throw new InvalidSaveDataException(
                            $"Can't find building named {address.BuildingName} in location named {location.Name}");

                    // Return the indoors of the building
                    return building.indoors;
                }
                else
                {
                    // If the location is not a buildable location, throw an exception
                    throw new InvalidSaveDataException($"Can't find any buildings in location named {location.Name}");
                }
            }
            else
            {
                // If the location type of the address is not a building, return the location
                return location;
            }
        }
    }
}