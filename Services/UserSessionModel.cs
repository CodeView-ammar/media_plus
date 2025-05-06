 using MediaPlus.DBModels;

namespace MediaPlus.Services
{
    public class UserSessionModel
    {
        public int UserId { get; set; }
        public string UserNameAr { get; set; }
        public string UserNameEn { get; set; }
        public string UserLoginName { get; set; }
        public string UserPhoto { get; set; }
        public int UserRoleId { get; set; }
        public string RoleName {get;set;}
        public List<string>? UserPermissions { get; set; }
        public string UserCustCode { get; set; }
    }
}