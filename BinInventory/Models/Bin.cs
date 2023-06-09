namespace BinInventory.Models
{
    /// <summary>
    /// Bin - a container with a location and some number of items in it.  The bins
    /// come from the third party system SmartRack.
    /// </summary>
    public class Bin
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Item> Items { get; set; } = new List<Item>();

        /// <summary>
        /// Change the item count for a specific item in this bin
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="newCount"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AdjustItemCount(int itemId, int newCount)
        {
            if (newCount < 0)
            {
                throw new ArgumentException("New item count must be non-negative");
            }

            Item? item = GetItem(itemId);

            // Remove an item from the bin (decrement the item count) OR return an insufficient count error
            item.Quantity = newCount;
#warning this doesn't actually save the data!
        }

        /// <summary>
        /// Get a specific item from this bin
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Item GetItem(int itemId)
        {
            // find the item in the matching bin
            var item = Items.FirstOrDefault(i => i.Id.Equals(itemId));

            // If the item is not found, return a not found response
            if (item == null)
            {
                throw new KeyNotFoundException($"Item {itemId}");
            }

            return item;
        }
    }
}
