using System.Linq.Expressions;
using EsparkKartur.Application.Repositories;
using EsparkKartur.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EsparkKartur.Infrastructure.Repositories
{
	// TEntity: Domain Varlığı (Sefer, Kullanici vb.)
	// TContext: DbContext Sınıfımız (EsparkKarturDbContext)
	public class EfEntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
		where TEntity : class, new()
	{
		protected readonly EsparkKarturDbContext _context;

		public EfEntityRepositoryBase(EsparkKarturDbContext context)
		{
			_context = context;
		}

		// --- CRUD METOTLARI (AYNI KALDI) ---

		public async Task AddAsync(TEntity entity)
		{
			await _context.Set<TEntity>().AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(TEntity entity)
		{
			_context.Set<TEntity>().Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(TEntity entity)
		{
			_context.Set<TEntity>().Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter)
		{
			return await _context.Set<TEntity>().AnyAsync(filter);
		}

		// --- GET METOTLARI (GÜNCELLENMİŞ VERSİYONLAR) ---

		// Mevcut metot: Tek seviyeli Include'lar için
		public async Task<TEntity> GetAsync(
			Expression<Func<TEntity, bool>> filter,
			params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> query = _context.Set<TEntity>();

			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}

			// GÜNCELLEME: SingleOrDefaultAsync yerine FirstOrDefaultAsync kullanıldı.
			// Bu sayede 1-N ilişkileri yüzünden oluşan fazla satır hatası (Sequence contains more than one element) engellenir.
			return await query.FirstOrDefaultAsync(filter);
		}

		public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
		{
			return await GetAsync(filter, Array.Empty<Expression<Func<TEntity, object>>>());
		}

		// ✅ YENİ METOT UYGULAMASI: ThenInclude ve zincirleme sorgu için.
		public async Task<TEntity> GetWithIncludesAsync(
			Expression<Func<TEntity, bool>> filter,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
		{
			IQueryable<TEntity> query = _context.Set<TEntity>();

			// include (zincirleme ThenInclude'ları içeren fonksiyon) varsa uygular.
			if (include != null)
			{
				query = include(query);
			}

			// GÜNCELLEME: SingleOrDefaultAsync yerine FirstOrDefaultAsync kullanıldı.
			return await query.FirstOrDefaultAsync(filter);
		}

		// Mevcut metot: İlişkileri yükleyen GetListAsync (Raporlama için)
		public async Task<List<TEntity>> GetListAsync(
			Expression<Func<TEntity, bool>>? filter = null,
			params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> query = _context.Set<TEntity>();

			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return await query.ToListAsync();
		}

		public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null)
		{
			return await GetListAsync(filter, Array.Empty<Expression<Func<TEntity, object>>>());
		}
	}
}