using CarDealer.Data;
using CarDealer.DTO.InputModels;
using CarDealer.DTO.OutputModels;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string supplierPath = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string partsPath = File.ReadAllText("../../../Datasets/parts.xml");
            //string carsPath = File.ReadAllText("../../../Datasets/cars.xml");
            //string customersPath = File.ReadAllText("../../../Datasets/customers.xml");
            //string salesPath = File.ReadAllText("../../../Datasets/sales.xml");

            //ImportSuppliers(context, supplierPath);
            //ImportParts(context, partsPath);
            //ImportCars(context, carsPath);
            //ImportCustomers(context, customersPath);
            //var result = ImportSales(context, salesPath);
            //Console.WriteLine(result);
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }
        //P19:
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new SaleOutputModel
                {
                    Car = new CarSalesOutputModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TraveledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    Name = x.Customer.Name,
                    Price = x.Car.PartCars.Select(c => c.Part.Price).Sum(),
                    PriceWithDiscount = x.Car.PartCars.Select(c => c.Part.Price).Sum() -
                    (x.Car.PartCars.Select(c => c.Part.Price).Sum() * x.Discount / 100)
                }).ToList();

            var result = XmlConverter.Serialize(sales, "sales");

            return result;
        }
        //P18:
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customer = context.Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(x => new CustomerOutputModel
                {
                    Name = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpendMoney = x.Sales.Select(c => c.Car)
                    .SelectMany(z => z.PartCars)
                    .Sum(a => a.Part.Price)
                }).OrderByDescending(x => x.SpendMoney)
                .ToList();

            var result = XmlConverter.Serialize(customer, "customers");
            return result;
        }
        //P17:
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new CarPartsOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TraveledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(p => new PartsOutputModel
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    }).OrderByDescending(a => a.Price)
                    .ToArray()
                }).OrderByDescending(z => z.TraveledDistance)
                .ThenBy(z => z.Model)
                .Take(5)
                .ToList();

            var result = XmlConverter.Serialize(cars, "cars");
            return result;
        }
        //P16:
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new SupliersOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                }).ToList();

            var result = XmlConverter.Serialize(suppliers, "suppliers");

            return result;
        }

        //P15:
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new BMWcarsOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TraveledDistance = x.TravelledDistance
                }).OrderBy(x => x.Model)
                .ThenByDescending(x => x.TraveledDistance)
                .ToList();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;
        }
        //P14:
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2_000_000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Select(c => new CarsOutputModel
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance
                })
                .Take(10)
                .ToArray();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;
            
        }
        //P13:
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var dtoSales = XmlConverter.Deserializer<SalesInputModel>(inputXml, "Sales");
            var carIds = context.Cars.Select(x => x.Id).ToList();
            var sales = dtoSales
                .Where(z => carIds.Contains(z.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                }).ToList();
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //P12:
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var dtoCustomers = XmlConverter.Deserializer<CustomersInputModel>(inputXml, "Customers");
            var customers = dtoCustomers.Select(x => new Customer
            {
                Name = x.Name,
                BirthDate = x.BirthDay,
                IsYoungDriver = x.IsYoungDriver
            });
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        //P11:
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer =
               new XmlSerializer(typeof(CarsInputModel[]), new XmlRootAttribute("Cars"));

            CarsInputModel[] carDtos;

            using (var reader = new StringReader(inputXml))
            {
                carDtos = (CarsInputModel[])serializer.Deserialize(reader);
            }


            List<Car> cars = new List<Car>();

            foreach (var dto in carDtos)
            {
                var distinctParts = dto.Parts
                    .Select(x => x.Id)
                    .Distinct()
                    .Where(x => context.Parts.Select(p => p.Id).Contains(x));

                Car car = new Car
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TravelledDistance = dto.TraveledDistance
                };

                foreach (var partId in distinctParts)
                {

                    PartCar partCar = new PartCar
                    {
                        PartId = partId
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //P10:
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var dtoParts = XmlConverter.Deserializer<PartsInputModel>(inputXml, "Parts");
            var supplierIds = context.Suppliers.Select(x => x.Id).ToList();

            var parts = dtoParts
                .Where(x => supplierIds.Contains(x.SupplierId))
                .Select(p => new Part
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SupplierId = p.SupplierId
                }).ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //P09:
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var dtoSuppliers = XmlConverter.Deserializer<SuppliersInputModel>(inputXml, "Suppliers");
            var suppliers = dtoSuppliers.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter
            }).ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }
    }
}