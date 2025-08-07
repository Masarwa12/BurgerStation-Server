namespace ShopServer.Models
{
    public class Item
    {
        public Item() { }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }

        public string Image { get; set; } 
      public string Description { get; set; }
        public override string ToString()
        {
            return Name + " " + Price + " " + Category+" "  + Description+  " " +  Image; 
        }
    }
}
