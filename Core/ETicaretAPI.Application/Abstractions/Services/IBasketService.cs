using ETicaretAPI.Application.ViewModels.Basket;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IBasketService
    {
        public Task<List<BasketItem>> GetBasketItemsAsync();
        //public Task<BasketItem> GetBasketItemByIdAsync(int id);
        public Task AddItemToBasketAsync(VM_Create_BasketItem basketItem);
        //public Task<List<BasketItem>> RemoveBasketItemsAsync();
        public Task RemoveBasketItemByIdAsync(int id);
        public Task UpdateQuantityAsync(VM_Update_BasketItem basketItem);
    }
}
