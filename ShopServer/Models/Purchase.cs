namespace ShopServer.Models
{
    public class Purchase
    {
        public string UserEmail { get; set; }
        public string ItemName { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
