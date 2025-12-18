using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Infrastructure.Repositories
{
	public class EfKargoRepository : EfEntityRepositoryBase<KargoFirmasi>, IKargoRepository
	{
		public EfKargoRepository(EsparkKarturDbContext context) : base(context) { }
	}
}
