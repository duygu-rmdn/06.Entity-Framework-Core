using CarDealer.Data;
using CarDealer.DTO.InputModels;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            string supplierPath = File.ReadAllText("../../../Datasets/suppliers.xml");
            string partsPath = File.ReadAllText("../../../Datasets/parts.xml");
            string carsPath = File.ReadAllText("../../../Datasets/cars.xml");

            ImportSuppliers(context, supplierPath);
            ImportParts(context, partsPath);
            var result = ImportCars(context, carsPath);
            Console.WriteLine(result);
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