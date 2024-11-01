using StoreAgent.Models;

namespace StoreAgent.Services;

public class ShoppingCart : IShoppingCart
{
    private List<OrderItem> Items;

    public ShoppingCart() {
        Items = new List<OrderItem>();
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }

    public void EmptyCart()
    {
        Items.Clear();
    }
}
