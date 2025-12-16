namespace EsparkKartur.Application
{
	// Transaction yönetimi için genel arayüz
	public interface IUnitOfWork : IDisposable
	{
		Task BeginTransactionAsync();
		Task CommitAsync();
		Task RollbackAsync();
		Task<int> SaveChangesAsync();
	}
}