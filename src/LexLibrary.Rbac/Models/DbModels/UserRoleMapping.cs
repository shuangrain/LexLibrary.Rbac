namespace LexLibrary.Rbac.Models.DbModels
{
    public class UserRoleMapping : ValidatableObject
    {
        public long UserId { get; set; }

        public int RoleId { get; set; }

        public User User { get; set; }

        public Role Role { get; set; }
    }
}
