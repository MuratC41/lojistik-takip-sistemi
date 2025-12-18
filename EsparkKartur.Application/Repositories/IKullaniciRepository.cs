using System;
using System.Collections.Generic;
using System.Text;
using EsparkKartur.Domain.Entities;

namespace EsparkKartur.Application.Repositories
{
	// IEntityRepository'den türeterek tüm o GetAsync, AddAsync metotlarını miras alıyoruz.
	public interface IKullaniciRepository : IEntityRepository<Kullanici>
	{
		// Kullanıcıya özel ekstra metodun olursa buraya yazarsın (Örn: GetByRole)
	}
}