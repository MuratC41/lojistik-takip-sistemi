using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Infrastructure.Context;

namespace EsparkKartur.Infrastructure.Repositories
{
	public class EfSevkFisiRepository : EfEntityRepositoryBase<SevkFisi>, ISevkFisiRepository
	{
		public EfSevkFisiRepository(EsparkKarturDbContext context) : base(context)
		{
		}
	}
}