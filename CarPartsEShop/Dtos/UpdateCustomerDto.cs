namespace CarPartsEShop.Dtos
{
    public class UpdateCustomerDto
    {
        public required string FullName { get; set; }
        public required string Phone { get; set; } = string.Empty;
        public required string Street { get; set; } = string.Empty;
        public required string City { get; set; } = string.Empty;
        public required string PostalCode { get; set; } = string.Empty;
        public required string Country { get; set; } = string.Empty;
    }
}
