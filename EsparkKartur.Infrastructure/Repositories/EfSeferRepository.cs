using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Infrastructure.Context;

namespace EsparkKartur.Infrastructure.Repositories
{
	// ISeferRepository arayüzünü uygular ve temel CRUD işlemlerini EfEntityRepositoryBase'den miras alır.
	public class EfSeferRepository : EfEntityRepositoryBase<Sefer>, ISeferRepository
	{
		public EfSeferRepository(EsparkKarturDbContext context) : base(context)
		{
		}

		// Buraya sadece ISeferRepository arayüzünde tanımlanan özel metotlar eklenecektir.
	}
}