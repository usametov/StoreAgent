using StoreAgent.Models;

namespace StoreAgent.Services;

public interface IShoppingCart {
    public void AddItem(OrderItem item);
    public void EmptyCart();
}
