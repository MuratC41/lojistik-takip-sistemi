using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Infrastructure.Context;

namespace EsparkKartur.Infrastructure.Repositories
{
	public class EfMagazaRepository : EfEntityRepositoryBase<Magaza>, IMagazaRepository
	{
		public EfMagazaRepository(EsparkKarturDbContext context) : base(context) { }
	
	}
}