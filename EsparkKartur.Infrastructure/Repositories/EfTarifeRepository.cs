// EfTarifeRepository.cs

using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using EsparkKartur.Infrastructure.Repositories;

public class EfTarifeRepository : EfEntityRepositoryBase<Tarife>, ITarifeRepository
{
	// Özel alan: DbContext'i burada tutacağız.
	private readonly EsparkKarturDbContext _context;

	// Kurucu metot: DbContext'i hem base sınıfa gönderir hem de burada tutarız.
	public EfTarifeRepository(EsparkKarturDbContext context) : base(context)
	{
		// _context alanı şimdi EfTarifeRepository içinde kullanılabilir.
		_context = context;
	}

	// ITarifeRepository'deki GetTarifeByMagazaId metodunun implementasyonu
	public async Task<Tarife> GetTarifeByMagazaId(int magazaId)
	{
		// Şimdi Context yerine _context'i kullanıyoruz:
		return await _context.Set<Tarife>()
							.FirstOrDefaultAsync(t => t.MagazaId == magazaId);
	}
}