using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IUserRepository
    {
        UserProfile GetLatestUserProfile(string emailID);
        NonRpostUser GetNonRpostUser(string? emailAddress);
        UserProfile GetUserProfileByEmailID(string? emailAddress);
        UserProfile GetUserProfileByUserID(Guid UserID);
        UserProfile GetUserProfile(Guid userId);
        UserProfile GetUserProfile(Guid userId, string EmailID, bool isAdditionalInfoRequired);
        bool Save(UserProfile userProfile);
        bool Save(NonRpostUser userDetails);
        void CheckRpostUser(Recipients recipient);
        UserProfile GetSignerUserProfileByEmail(string emailID);
        string GetUserNameByEmailid(string EmailId);
        List<UserProfile> GetUserProfileByEmailIDs(List<string> emails);
        List<NonRpostUser> GetNonRpostUserList(List<string> emails);
    }
}
