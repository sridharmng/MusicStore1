using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using MusicStore.Models;
using MusicStore.ViewModel;
namespace MusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        MusicStoreEntities storeDb = new MusicStoreEntities();
        // GET: ShoppingCart

        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var viewModel = new ShoppingCartViewModel
            {
             cartItems = cart.GetCartItems(),
              cartTotal = cart.GetTotal()
            };         
            return View(viewModel);
        }   

        public ActionResult AddToCart(int id)
        {
            //Retrieve the album from Database
            var Cart = ShoppingCart.GetCart(this.HttpContext);
            var addedAlbum = storeDb.Albums.Single(a => a.AlbumId == id);
            Cart.AddToCart(addedAlbum);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart= ShoppingCart.GetCart(this.HttpContext);
        
            //get the album Name for confirmation
             
            string albumName = storeDb.Carts.Single(item => item.RecordId==id).Album.Title;
           int itemCount = cart.RemoveFromCart(id);

            ///Display the confirmation message
            var results= new ShoppingCartRemoveViewModel{
               Message=  Server.HtmlEncode(albumName)+"has been removed from your shopping cart.",
               CartCount= cart.GetCount(),
               CartTotal= cart.GetTotal(),
               ItemCount= itemCount,
               DeleteId= id
            };

            return Json(results);
        }
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}

