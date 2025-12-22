using System;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Application.Repositories
{
	public interface IAuditService
	{
		int GetCurrentKullaniciId(); // JWT'den kullanıcı ID'sini çeker
	}
}
