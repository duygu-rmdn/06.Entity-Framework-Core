namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();
			var dtoGame = JsonConvert.DeserializeObject<IEnumerable<GamesInputModel>>(jsonString);
            foreach (var game in dtoGame)
            {
                if (!IsValid(game) || game.Tags.Count() == 0)
                {
					sb.AppendLine("Invalid Data");
					continue;
                }
				var dev = context.Developers.FirstOrDefault(x => x.Name == game.Developer)
					?? new Developer { Name = game.Developer};

				var genre = context.Genres.FirstOrDefault(x => x.Name == game.Genre)
					?? new Genre { Name = game.Genre };
				var resultGame = new Game
				{
					Name = game.Name,
					Price = game.Price,
					ReleaseDate = game.ReleaseDate.Value,
					Developer = dev,
					Genre = genre,
				};
                foreach (var tagDto in game.Tags)
                {
					var tag = context.Tags.FirstOrDefault(x => x.Name == tagDto)
						?? new Tag { Name = tagDto };
					resultGame.GameTags.Add(new GameTag { Tag = tag});
                }
				sb.AppendLine($"Added {resultGame.Name} ({resultGame.Genre.Name}) with {resultGame.GameTags.Count} tags");
				context.Games.Add(resultGame);
				context.SaveChanges();
            }
			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(jsonString);
			var sb = new StringBuilder();

            foreach (var dtoUser in dtoUsers)
            {
				if (!IsValid(dtoUser) || !dtoUser.Cards.All(IsValid))
                {
					sb.AppendLine("Invalid Data");
					continue;
				}
				var user = new User
				{
					FullName = dtoUser.FullName,
					Username = dtoUser.Username,
					Email = dtoUser.Email,
					Age = dtoUser.Age,
					Cards = dtoUser.Cards.Select(c => new Card
					{
						Cvc = c.CVC,
						Number = c.Number,
						Type = c.Type.Value
					}).ToList()
				};
				context.Users.Add(user);
				sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }
			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var dtoPurchases = XmlConverter.Deserializer<PurchaseInputModel>(xmlString, "Purchases");
			var sb = new StringBuilder();

            foreach (var pruch in dtoPurchases)
            {
                if (!IsValid(pruch))
				{
					sb.AppendLine("Invalid Data");
					continue;
                }
				var result = new Purchase
				{
					ProductKey = pruch.Key,
					Type = pruch.Type.Value,
					Card = context.Cards.FirstOrDefault(x => x.Number == pruch.Card),
					Date = DateTime.ParseExact(pruch.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
					Game = context.Games.FirstOrDefault(x => x.Name == pruch.Title)
				};
				context.Purchases.Add(result);
				var user = context.Users.FirstOrDefault(x => x.Username == result.Card.User.Username);
				sb.AppendLine($"Imported {result.Game.Name} for {user.Username}");
				context.SaveChanges();
            }

			return sb.ToString().TrimEnd();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}