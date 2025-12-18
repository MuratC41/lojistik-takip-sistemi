using EsparkKartur.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Application.Repositories
{
public interface ITokenService
		{
			string CreateToken(Kullanici kullanici);
		}
}
