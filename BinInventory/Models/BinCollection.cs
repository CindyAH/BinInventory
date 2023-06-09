namespace BinInventory.Models
{
    /// <summary>
    /// A collection of Bins.  The bins come from the third party system SmartRack.
    /// </summary>
    public class BinCollection
    {
        /// <summary>
        /// List of all the Bins.  This is the Repository, for Bins stored in the database
        /// </summary>
        public List<Bin> Bins
        {
            get
            {
                // ToDo Do this with lazy load and explicit backing variables
                return GetBinsFromSmartRack();
            }
            set { }
        }

        /// <summary>
        /// Get the bin with the specified Id, or null if that bin doesn't exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bin GetById(int id)
        {
            return Bins.FirstOrDefault(x => x.Id == id);
        }

        private static List<Bin> GetBinsFromSmartRack()
        {
            // ToDo What is the SmartRack API?
            return new List<Bin>()
            {
                new Bin() { Id = 1, Description = "Warehouse A", Items = new List<Item>()
                {
                    new Item() { Id = 1, Description = "Screwdriver", Quantity = 10 },
                    new Item() { Id = 2, Description = "Hammer", Quantity = 5 }
                }},
                new Bin() { Id = 2, Description = "Warehouse B", Items = new List<Item>()
                {
                    new Item() { Id = 3, Description = "Drill", Quantity = 3 },
                    new Item() { Id = 4, Description = "Saw", Quantity = 7 }
                }}
            };
        }
    }
}
