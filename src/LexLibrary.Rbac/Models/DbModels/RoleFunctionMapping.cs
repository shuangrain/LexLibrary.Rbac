namespace LexLibrary.Rbac.Models.DbModels
{
    public class RoleFunctionMapping : ValidatableObject
    {
        public int RoleId { get; set; }

        public int FunctionId { get; set; }

        public Role Role { get; set; }

        public Function Function { get; set; }
    }
}
