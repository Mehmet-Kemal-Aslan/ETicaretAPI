using ETicaretAPI.Application.Features.Queries.Basket.GetBasketItems;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Basket.AddItemToBasket
{
    public class AddItemToBasketCommandRequest : IRequest<AddItemToBasketCommandResponse>
    { 
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
