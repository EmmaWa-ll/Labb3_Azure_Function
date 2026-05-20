namespace MinApi.Models
{
    public class Customer
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Name { get; set; } = "";

        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";

        public Seller Seller { get; set; } = new();


    }
}
