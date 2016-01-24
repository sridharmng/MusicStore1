using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStore.Models
{
    public class ShoppingCart
    {
        MusicStoreEntities storeDb = new MusicStoreEntities();
        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls

        // Get Cart


        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
       //  add items to cart

        public void AddToCart(Album album)
        {
            // Get the matching cart and album instances
            var cartItem = storeDb.Carts.SingleOrDefault(c => c.CartId == ShoppingCartId && c.AlbumId == album.AlbumId);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                storeDb.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            storeDb.SaveChanges();
        }
        // Remove items from Cart

        public int RemoveFromCart(int id)
        {
            //Get the cart
            var cartItem = storeDb.Carts.SingleOrDefault(c => c.CartId == ShoppingCartId && c.RecordId == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    storeDb.Carts.Remove(cartItem);
                }
                storeDb.SaveChanges();
            }

            return itemCount;
        }

        //Emty Cart
        public void EmptyCart()
        {
            var CartItems = storeDb.Carts.Where(cart => cart.CartId == ShoppingCartId);
            foreach (var item in CartItems)
            {
                storeDb.Carts.Remove(item);
            }
        }
        public List<Cart> GetCartItems()
        {
            var cartItems = storeDb.Carts.Where(cart => cart.CartId == ShoppingCartId).ToList();
            return cartItems;
        }
        public int GetCount()
        {
            int? count = (from cartItems in storeDb.Carts where cartItems.CartId == ShoppingCartId select (int?)cartItems.Count).Sum();
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            var total = (from cartItems in storeDb.Carts where cartItems.CartId == ShoppingCartId select (int?)cartItems.Count * (decimal?)cartItems.Album.Price).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            var cartItems = GetCartItems();

            foreach (var item in cartItems)
            {

                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    Quantity = item.Count,
                    UnitPrice = item.Album.Price,
                };

                orderTotal += (item.Count * item.Album.Price);
                storeDb.OrderDetails.Add(orderDetail);
            }
            // Set the order's total to the orderTotal count

            order.Total = orderTotal;
            //Save the Order
            storeDb.SaveChanges();

            //Emptying cart
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.OrderId;
        }

        public string GetCartId(HttpContextBase context)
        {
            if ((string)context.Session[CartSessionKey] == null)
            {

                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    context.Session[CartSessionKey] = Guid.NewGuid().ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();

        }
        //Migrate Shopping Cart

        public void Migratecart(string userName)
        {
            var shoppingCart = storeDb.Carts.Where(cart => cart.CartId == ShoppingCartId);
            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDb.SaveChanges();
        }
    }
}
