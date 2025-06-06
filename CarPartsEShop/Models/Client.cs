﻿namespace CarPartsEShop.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public BillingAddress BillingAddress = default!;
}
