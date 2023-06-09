namespace BinInventory.Models
{
    /// <summary>
    /// Item - an item with a description and the quantity we have.  Info comes from
    /// the Items project in another solution.
    /// </summary>
    public class Item
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
