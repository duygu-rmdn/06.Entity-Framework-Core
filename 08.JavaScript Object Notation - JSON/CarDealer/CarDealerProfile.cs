using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<SuppliersInputModel, Supplier>();
            this.CreateMap<PartsInputMadel, Part>();
            this.CreateMap<CarInputModel, Car>();
            this.CreateMap<CustomersInputModel, Customer>();
            this.CreateMap<SalesInputModel, Sale>();

        }
    }
}
