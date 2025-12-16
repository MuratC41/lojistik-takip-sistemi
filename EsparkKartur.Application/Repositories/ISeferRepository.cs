using EsparkKartur.Domain.Entities;

namespace EsparkKartur.Application.Repositories
{
	public interface ISeferRepository : IEntityRepository<Sefer>
	{
		// Seferler için özel metotlar buraya gelebilir (Örn: Seferleri Tamamlanma Durumuna Göre Filtrele)
	}
}