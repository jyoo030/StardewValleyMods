using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Linq;
using StardewValleyMods.CategorizeChests.Framework;

// Define the namespace
namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    // Define the ChestOverlay class, which inherits from the Widget class
    class ChestOverlay : Widget
    {
        // Declare private readonly variables
        private readonly ItemGrabMenu ItemGrabMenu;
        private readonly InventoryMenu InventoryMenu;
        private readonly InventoryMenu.highlightThisItem DefaultChestHighlighter;
        private readonly InventoryMenu.highlightThisItem DefaultInventoryHighlighter;

        private readonly Config Config;
        private readonly IChestDataManager ChestDataManager;
        private readonly IChestFiller ChestFiller;
        private readonly IItemDataManager ItemDataManager;
        private readonly ITooltipManager TooltipManager;
        private readonly Chest Chest;

        // Declare private variables
        private TextButton OpenButton;
        private TextButton StashButton;
        private CategoryMenu CategoryMenu;

        // Define the constructor for the ChestOverlay class
        public ChestOverlay(ItemGrabMenu menu,
            Chest chest,
            Config config,
            IChestDataManager chestDataManager,
            IChestFiller chestFiller,
            IItemDataManager itemDataManager,
            ITooltipManager tooltipManager)
        {
            // Initialize variables
            Config = config;
            ItemDataManager = itemDataManager;
            ChestDataManager = chestDataManager;
            ChestFiller = chestFiller;
            TooltipManager = tooltipManager;

            Chest = chest;

            ItemGrabMenu = menu;
            InventoryMenu = menu.ItemsToGrabMenu;

            DefaultChestHighlighter = ItemGrabMenu.inventory.highlightMethod;
            DefaultInventoryHighlighter = InventoryMenu.highlightMethod;

            // Add buttons to the overlay
            AddButtons();
        }

        // Define the OnParent method, which is called when the parent widget is set
        protected override void OnParent(Widget parent)
        {
            // Call the base class's OnParent method
            base.OnParent(parent);

            // If the parent widget is not null, set the width and height of this widget to match the parent's
            if (parent != null)
            {
                Width = parent.Width;
                Height = parent.Height;
            }
        }

        // Define the AddButtons method, which adds buttons to the overlay
        private void AddButtons()
        {
            // Create the OpenButton and add it to the overlay
            OpenButton = new TextButton("Categorize", Sprites.LeftProtrudingTab);
            OpenButton.OnPress += ToggleMenu;
            AddChild(OpenButton);

            // Create the StashButton and add it to the overlay
            StashButton = new TextButton(ChooseStashButtonLabel(), Sprites.LeftProtrudingTab);
            StashButton.OnPress += StashItems;
            AddChild(StashButton);

            // Position the buttons on the overlay
            PositionButtons();
        }

        // Define the PositionButtons method, which positions the buttons on the overlay
        private void PositionButtons()
        {
            // Set the width of the buttons to be the maximum of their widths
            StashButton.Width = OpenButton.Width = Math.Max(StashButton.Width, OpenButton.Width);

            // Set the position of the OpenButton
            OpenButton.Position = new Point(
                ItemGrabMenu.xPositionOnScreen + ItemGrabMenu.width / 2 - OpenButton.Width - 112 * Game1.pixelZoom,
                ItemGrabMenu.yPositionOnScreen + 22 * Game1.pixelZoom
            );

            // Set the position of the StashButton
            StashButton.Position = new Point(
                OpenButton.Position.X + OpenButton.Width - StashButton.Width,
                OpenButton.Position.Y + OpenButton.Height
            );
        }

        // Define the ChooseStashButtonLabel method, which chooses the label for the StashButton
        private string ChooseStashButtonLabel()
        {
            // Get the key for the stash action from the config
            var stashKey = Config.StashKey;

            // If the stash key is not set, return "Stash"
            if (stashKey == Keys.None)
            {
                return "Stash";
            }
            else
            {
                // If the stash key is set, return "Stash (KeyName)"
                var keyName = Enum.GetName(typeof(Keys), stashKey);
                return $"Stash ({keyName})";
            }
        }

        // Define the ToggleMenu method, which opens or closes the category menu
        private void ToggleMenu()
        {
            // If the category menu is not open, open it
            if (CategoryMenu == null)
            {
                OpenCategoryMenu();
            }
            else
            {
                // If the category menu is open, close it
                CloseCategoryMenu();
            }
        }

        // Define the OpenCategoryMenu method, which opens the category menu
        private void OpenCategoryMenu()
        {
            // Get the data for the chest
            var chestData = ChestDataManager.GetChestData(Chest);
            // Create the category menu
            CategoryMenu = new CategoryMenu(chestData, ItemDataManager, TooltipManager);
            // Set the position of the category menu
            CategoryMenu.Position = new Point(
                ItemGrabMenu.xPositionOnScreen + ItemGrabMenu.width / 2 - CategoryMenu.Width / 2 - 6 * Game1.pixelZoom,
                ItemGrabMenu.yPositionOnScreen - 10 * Game1.pixelZoom
            );
            // Set the OnClose event handler for the category menu
            CategoryMenu.OnClose += CloseCategoryMenu;
            // Add the category menu to the overlay
            AddChild(CategoryMenu);

            // Make items not clickable
            SetItemsClickable(false);
        }

        // Define the CloseCategoryMenu method, which closes the category menu
        private void CloseCategoryMenu()
        {
            // Remove the category menu from the overlay
            RemoveChild(CategoryMenu);
            // Set the category menu to null
            CategoryMenu = null;

            // Make items clickable
            SetItemsClickable(true);
        }

        // Define the StashItems method, which stashes items in the chest
        private void StashItems()
        {
            // If it's not a good time to stash items, return
            if (!GoodTimeToStash())
                return;

            // Dump items to the chest
            ChestFiller.DumpItemsToChest(Chest);
        }

        // Define the ReceiveKeyPress method, which handles key press events
        public override bool ReceiveKeyPress(Keys input)
        {
            // If the key pressed is the stash key, stash items and return true
            if (input == Config.StashKey)
            {
                StashItems();
                return true;
            }

            // Propagate the key press to the base class
            return PropagateKeyPress(input);
        }

        // Define the ReceiveLeftClick method, which handles left click events
        public override bool ReceiveLeftClick(Point point)
        {
            // Propagate the left click to the base class
            var hit = PropagateLeftClick(point);

            // If the click was not handled and the category menu is open, close the category menu
            if (!hit && CategoryMenu != null) // Are they clicking outside the menu to try to exit it?
                CloseCategoryMenu();

            // Return whether the click was handled
            return hit;
        }
        
        // Define the SetItemsClickable method, which sets whether items are clickable
        private void SetItemsClickable(bool clickable)
        {
            // If items should be clickable
            if (clickable)
            {
                // Set the highlight methods to the default highlighters
                ItemGrabMenu.inventory.highlightMethod = DefaultChestHighlighter;
                InventoryMenu.highlightMethod = DefaultInventoryHighlighter;
            }
            else
            {
                // Set the highlight methods to always return false, making items not clickable
                ItemGrabMenu.inventory.highlightMethod = item => false;
                InventoryMenu.highlightMethod = item => false;
            }
        }

        // Define the GoodTimeToStash method, which checks whether it's a good time to stash items
        private bool GoodTimeToStash()
        {
            // Return true if items are clickable and there is no held item
            return ItemsAreClickable() && ItemGrabMenu.heldItem == null;
        }

        // Define the ItemsAreClickable method, which checks whether items are clickable
        private bool ItemsAreClickable()
        {
            // Get the items in the inventory
            var items = ItemGrabMenu.inventory.actualInventory;
            // Get the highlight method
            var highlighter = ItemGrabMenu.inventory.highlightMethod;
            // Return true if any item is highlighted
            return items.Any(item => highlighter(item));
        }
    }
}