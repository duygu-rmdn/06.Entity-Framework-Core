using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string suppliersPath = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsPath = File.ReadAllText("../../../Datasets/parts.json");
            //string carsPath = File.ReadAllText("../../../Datasets/cars.json");
            //string customersPath = File.ReadAllText("../../../Datasets/customers.json");
            //string salesPath = File.ReadAllText("../../../Datasets/sales.json");

            //ImportSuppliers(context, suppliersPath);
            //ImportParts(context, partsPath);
            //ImportCars(context, carsPath);
            //ImportCustomers(context, customersPath);
            //Console.WriteLine(ImportSales(context, salesPath));
            //Console.WriteLine(GetOrderedCustomers(context));
            //Console.WriteLine(GetCarsFromMakeToyota(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //Console.WriteLine(GetTotalSalesByCustomer(context));
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }
        //P11:
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                 .Select(x => new
                 {
                     car = new
                     {
                         Make = x.Car.Make,
                         Model = x.Car.Model,
                         TravelledDistance = x.Car.TravelledDistance
                     },
                     customerName = x.Customer.Name,
                     Discount = x.Discount.ToString("F2"),
                     price = x.Car.PartCars.Select(a => a.Part.Price).Sum().ToString("F2"),
                     priceWithDiscount = (x.Car.PartCars.Select(a => a.Part.Price).Sum() -
                     (x.Car.PartCars.Select(pc => pc.Part.Price).Sum() * (x.Discount / 100))).ToString("F2")
                 }).Take(10)
                 .ToList();
            var result = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return result;

        }

        //P10:
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count,
                    spentMoney = x.Sales
                    .Select(y => y.Car.PartCars.Select(z => z.Part.Price).Sum()).Sum()
                })
                .OrderByDescending(a => a.spentMoney)
                .ThenByDescending(z => z.boughtCars)
                .ToArray();
            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }

        //P09:
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars.Select(a => new
                    {
                        Name = a.Part.Name,
                        Price = a.Part.Price.ToString("F2")
                    })
                }).ToList();
            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        //P08:
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    PartsCount = c.Parts.Count()
                }).ToList();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return result;
        }
        //P07:
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(s => s.TravelledDistance)
                .ToList();
            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        //P06:
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();
            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }

        //P05:
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InisalizeMapping();
            var dtoSales = JsonConvert.DeserializeObject<IEnumerable<SalesInputModel>>(inputJson);
            var sales = mapper.Map<IEnumerable<Sale>>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        //P04:
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InisalizeMapping();
            var customerDto = JsonConvert.DeserializeObject<IEnumerable<CustomersInputModel>>(inputJson);

            var customers = mapper.Map<IEnumerable<Customer>>(customerDto);

            context.AddRange(customers);
            context.SaveChanges();

            return  $"Successfully imported {customers.Count()}.";

        }

        //P03:
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InisalizeMapping();
            var carsDto = JsonConvert.DeserializeObject<List<CarInputModel>>(inputJson);

            var cars = new List<Car>();

            foreach (var car in carsDto)
            {
                var currentCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                foreach (var partId in car.PartsId.Distinct())
                {
                    currentCar.PartCars.Add(new PartCar { PartId = partId });
                }

                cars.Add(currentCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }
        //P02:
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InisalizeMapping();
            var sup = context.Suppliers.Select(s => s.Id).ToArray();
            var dtoParts = JsonConvert.DeserializeObject<IEnumerable<PartsInputMadel>>(inputJson)
                .Where(x => sup.Contains(x.SupplierId));
            var parts = mapper.Map<IEnumerable<Part>>(dtoParts);
            
            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        //P01:
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InisalizeMapping();
            var dtoSuppliers = JsonConvert.DeserializeObject<IEnumerable<SuppliersInputModel>>(inputJson);
            var suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSuppliers);
            
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        private static void InisalizeMapping()
        {
            MapperConfiguration config = new MapperConfiguration(c => 
            { 
                c.AddProfile<CarDealerProfile>(); 
            });
            mapper = config.CreateMapper();
        }
    }
}
 
 