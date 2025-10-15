﻿namespace Pharmacy.Domain.Entities;
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Product>? products { get; set; }
}
