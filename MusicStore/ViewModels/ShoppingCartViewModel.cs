using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicStore.Models;

namespace MusicStore.ViewModel
{
    public class  ShoppingCartViewModel
    {
        public List<Cart> cartItems { get; set; }
        public decimal cartTotal { get; set; }
    }
}