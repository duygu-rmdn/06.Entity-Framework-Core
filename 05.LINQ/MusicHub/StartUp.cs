namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //Console.WriteLine(ExportAlbumsInfo(context, 9));
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {

            var albums = context.Albums.
                Where(x => x.ProducerId == producerId)
                .Select(a => new
                {
                    AlbName = a.Name,
                    RelDate = a.ReleaseDate,
                    ProdName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        SongPrice = s.Price,
                        WriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(w => w.WriterName)
                    .ToList(),
                    AlbumPrice = (decimal)a.Songs.Select(p => p.Price).Sum()

                })
                .OrderByDescending(z => z.AlbumPrice)
                .ToList();
            var sb = new StringBuilder();
            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbName}");
                sb.AppendLine($"-ReleaseDate: {album.RelDate:MM/dd/yyyy}");
                sb.AppendLine($"-ProducerName: {album.ProdName}");
                sb.AppendLine($"-Songs:");

                int counter = 1;
                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{counter++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:f2}");
                    sb.AppendLine($"---Writer: {song.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .ToList()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    PerformerName = $"{s.SongPerformers.Select(p => p.Performer.FirstName).FirstOrDefault()} {s.SongPerformers.Select(p => p.Performer.LastName).FirstOrDefault()}".TrimEnd(),
                     Proc = s.Album.Producer.Name,
                    s.Duration
                })
                .OrderBy(n => n.SongName)
                .ThenBy(w => w.Writer)
                .ThenBy(p => p.PerformerName)
                .ToList();
            var sb = new StringBuilder();
            int counter = 1;
            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{counter++}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.PerformerName}");
                sb.AppendLine($"---AlbumProducer: {song.Proc}");
                sb.AppendLine($"---Duration: {song.Duration:c}");
            }

            return sb.ToString().TrimEnd();

        }
    }
}
