// EsparkKartur.Application/Repositories/ITarifeRepository.cs

using EsparkKartur.Domain.Entities;

namespace EsparkKartur.Application.Repositories
{
	// Temel IEntityRepository arayüzünden miras almalıdır.
	public interface ITarifeRepository : IEntityRepository<Tarife>
	{
		// Tarifeye özgü özel metotlar buraya gelir.
	}
}