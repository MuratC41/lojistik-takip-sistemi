using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; // ConfigurationBuilder kullanabilmek için

namespace EsparkKartur.Infrastructure.Context
{
	// Bu sınıf, EF Core araçlarının (migration'lar) DbContext'i doğru şekilde oluşturmasını sağlar.
	public class EsparkKarturDbContextFactory : IDesignTimeDbContextFactory<EsparkKarturDbContext>
	{
		public EsparkKarturDbContext CreateDbContext(string[] args)
		{
			// 1. appsettings.json dosyasını bulmak için yapılandırmayı (Configuration) kur
			IConfigurationRoot configuration = new ConfigurationBuilder()
				// API projesinin bulunduğu dizine bakar (migration komutunu nereden çalıştırdığımız önemli)
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			// 2. Bağlantı dizesini al (SQLite bağlantısı)
			var connectionString = configuration.GetConnectionString("DefaultConnection");

			// 3. DbContextOptions nesnesini oluştur
			var builder = new DbContextOptionsBuilder<EsparkKarturDbContext>();
			// UseSqlite kullanıyoruz, çünkü daha önce test amaçlı buna geçmiştik.
			builder.UseSqlite(connectionString);

			// 4. DbContext'i oluşturup döndür
			return new EsparkKarturDbContext(builder.Options);
		}
	}
}