using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Infrastructure.Context;

namespace EsparkKartur.Infrastructure.Repositories
{
	public class EfKullaniciRepository : EfEntityRepositoryBase<Kullanici>, IKullaniciRepository
	{
		public EfKullaniciRepository(EsparkKarturDbContext context) : base(context)
		{
			// Tüm motor (base sınıf) zaten hazır, içini doldurmaya gerek yok!
		}
	}
}