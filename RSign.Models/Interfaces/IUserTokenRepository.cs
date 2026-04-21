using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IUserTokenRepository
    {
        string GetUserEmailByToken(string token);
        Guid GetUserProfileIDByEmail(string Email);
        Guid GetUserProfileUserIDByID(Guid ID);
        UserProfile GetUserProfileByToken(string AuthToken);
        UserProfile GetUserProfileByEmail(string Email);
    }
}
