using LexLibrary.Rbac.Models.Consts;
using LexLibrary.Rbac.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace LexLibrary.Rbac.Repositories
{
    public class LexLibraryRbacDbContext : DbContext
    {
        public LexLibraryRbacDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            registerEntity(modelBuilder);
            registerMapping(modelBuilder);
            addDefaultData(modelBuilder);
        }

        private void registerEntity(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var entityTypes = assembly.DefinedTypes.Where(x => x.FullName.StartsWith("LexLibrary.Rbac.Models.DbModels"))
                                                   .Select(x => x.AsType());

            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }

        private void registerMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.Account).IsUnique();
                builder.ToTable("LexLibraryRbac_User");
            });

            modelBuilder.Entity<UserToken>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasOne(x => x.User).WithMany(x => x.UserTokens).HasForeignKey(x => x.UserId);
                builder.ToTable("LexLibraryRbac_UserToken");
            });

            modelBuilder.Entity<Role>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.ToTable("LexLibraryRbac_Role");
            });

            modelBuilder.Entity<UserRoleMapping>(builder =>
            {
                builder.HasKey(x => new { x.UserId, x.RoleId });
                builder.HasOne(x => x.Role).WithMany(x => x.UserRoleMappings).HasForeignKey(x => x.RoleId);
                builder.HasOne(x => x.User).WithMany(x => x.UserRoleMappings).HasForeignKey(x => x.UserId);
                builder.ToTable("LexLibraryRbac_UserRoleMapping");
            });

            modelBuilder.Entity<Function>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasOne(x => x.ChildFunction).WithMany(x => x.ChildFunctions).HasForeignKey(x => x.ParentId);
                builder.ToTable("LexLibraryRbac_Function");
            });

            modelBuilder.Entity<RoleFunctionMapping>(builder =>
            {
                builder.HasKey(x => new { x.RoleId, x.FunctionId });
                builder.HasOne(x => x.Function).WithMany(x => x.RoleFunctionMappings).HasForeignKey(x => x.FunctionId);
                builder.HasOne(x => x.Role).WithMany(x => x.RoleFunctionMappings).HasForeignKey(x => x.RoleId);
                builder.ToTable("LexLibraryRbac_RoleFunctionMapping");
            });
        }

        private void addDefaultData(ModelBuilder modelBuilder)
        {
            // Admin
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Account = "admin",
                Password = string.Empty,
                DisplayName = "系統管理員",
                Email = "admin@exfast.me",
                IsEmailConfirmed = true
            });

            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = ApplicationConst.AdminRoleId,
                Name = "管理員"
            });

            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = ApplicationConst.DefaultRoleId,
                Name = "一般會員"
            });

            modelBuilder.Entity<UserRoleMapping>().HasData(new UserRoleMapping
            {
                UserId = 1,
                RoleId = 1,
            });

            modelBuilder.Entity<Function>().HasData(new Function
            {
                Id = 1,
                Name = "權限模組",
                IconClass = "fas fa-fw fa-folder"
            });
        }
    }
}
