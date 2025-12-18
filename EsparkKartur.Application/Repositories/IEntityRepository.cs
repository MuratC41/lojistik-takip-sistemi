using System.Linq.Expressions;
using EsparkKartur.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace EsparkKartur.Application.Repositories
{
	// T: Domain katmanındaki herhangi bir varlık (entity) olmalıdır.
	public interface IEntityRepository<T> where T : class
	{
		// CRUD Operasyonları:

		// MEVCUT: Tek bir kayıt getir (Filtre isteğe bağlıdır)
		Task<T> GetAsync(Expression<Func<T, bool>> filter);

		// MEVCUT: Tek bir kayıt getir + İlişkileri Yükle (Eager Loading)
		Task<T> GetAsync(
			Expression<Func<T, bool>> filter,
			params Expression<Func<T, object>>[] includeProperties
		);

		// ThenInclude (zincirleme) yüklemeye izin verir.
		Task<T> GetWithIncludesAsync(
			Expression<Func<T, bool>> filter,
			Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
		);

		// MEVCUT: Tüm kayıtları getir (Filtre isteğe bağlıdır, List<T> döner)
		Task<List<T>> GetListAsync(Expression<Func<T, bool>>? filter = null);

		// MEVCUT: Tüm kayıtları getir + İlişkileri Yükle (Rapor için de gerekli)
		Task<List<T>> GetListAsync(
			Expression<Func<T, bool>>? filter = null,
			params Expression<Func<T, object>>[] includeProperties
		);
		// Liste olarak getir + Zincirleme İlişkileri Yükle (ThenInclude desteği için)
		Task<List<T>> GetListWithIncludesAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
		);

		// Yeni kayıt ekle
		Task AddAsync(T entity);

		// Kaydı güncelle
		Task UpdateAsync(T entity);

		// Kaydı sil (Soft Delete için T'de Durum/AktifMi alanı olmalıdır)
		Task DeleteAsync(T entity);

		// Kayıt var mı kontrol et
		Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
	}
}