﻿using ETicaretAPI.Application.Repositories.Basket;
using ETicaretAPI.Application.Repositories.BasketItem;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.BasketItem
{
    public class BasketItemWriteRepository : WriteRepository<Domain.Entities.BasketItem>, IBasketItemWriteRepository
    {
        public BasketItemWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
