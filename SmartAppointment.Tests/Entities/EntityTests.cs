using FluentAssertions;
using SmartAppointment.Domain.Entities;

namespace SmartAppointment.Tests.Entities
{
    public class PermissionTests
    {
        [Fact]
        public void Permission_Should_Have_Default_Values()
        {
            // Arrange & Act
            var permission = new Permission();

            // Assert
            permission.Id.Should().BeEmpty();
            permission.Name.Should().BeEmpty();
            permission.Description.Should().BeNull();
            permission.RolePermissions.Should().NotBeNull();
            permission.RolePermissions.Should().BeEmpty();
        }

        [Fact]
        public void Permission_Should_Set_Properties_Correctly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Appointments.Create";
            var description = "Permission to create new appointments";
            var rolePermissions = new List<RolePermission>();

            // Act
            var permission = new Permission
            {
                Id = id,
                Name = name,
                Description = description,
                RolePermissions = rolePermissions
            };

            // Assert
            permission.Id.Should().Be(id);
            permission.Name.Should().Be(name);
            permission.Description.Should().Be(description);
            permission.RolePermissions.Should().BeSameAs(rolePermissions);
        }

        [Fact]
        public void Permission_Should_Allow_RolePermissions_Assignment()
        {
            // Arrange
            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Appointments.View"
            };

            var rolePermission1 = new RolePermission
            {
                RoleId = "role1",
                PermissionId = permission.Id
            };

            var rolePermission2 = new RolePermission
            {
                RoleId = "role2",
                PermissionId = permission.Id
            };

            // Act
            permission.RolePermissions.Add(rolePermission1);
            permission.RolePermissions.Add(rolePermission2);

            // Assert
            permission.RolePermissions.Should().HaveCount(2);
            permission.RolePermissions.Should().Contain(rolePermission1);
            permission.RolePermissions.Should().Contain(rolePermission2);
        }
    }

    public class RolePermissionTests
    {
        [Fact]
        public void RolePermission_Should_Have_Default_Values()
        {
            // Arrange & Act
            var rolePermission = new RolePermission();

            // Assert
            rolePermission.RoleId.Should().BeEmpty();
            rolePermission.PermissionId.Should().BeEmpty();
            rolePermission.Permission.Should().BeNull();
        }

        [Fact]
        public void RolePermission_Should_Set_Properties_Correctly()
        {
            // Arrange
            var roleId = "admin-role";
            var permissionId = Guid.NewGuid();
            var permission = new Permission
            {
                Id = permissionId,
                Name = "Appointments.Delete"
            };

            // Act
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                Permission = permission
            };

            // Assert
            rolePermission.RoleId.Should().Be(roleId);
            rolePermission.PermissionId.Should().Be(permissionId);
            rolePermission.Permission.Should().Be(permission);
            rolePermission.Permission.Id.Should().Be(permissionId);
        }

        [Theory]
        [InlineData("user")]
        [InlineData("admin")]
        [InlineData("manager")]
        [InlineData("12345")]
        [InlineData("role-with-dashes")]
        public void RolePermission_Should_Accept_Various_RoleIds(string roleId)
        {
            // Arrange & Act
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = Guid.NewGuid()
            };

            // Assert
            rolePermission.RoleId.Should().Be(roleId);
        }

        [Fact]
        public void RolePermission_Should_Work_Without_Permission_Reference()
        {
            // Arrange & Act
            var rolePermission = new RolePermission
            {
                RoleId = "test-role",
                PermissionId = Guid.NewGuid()
                // Permission is intentionally not set
            };

            // Assert
            rolePermission.Permission.Should().BeNull();
            rolePermission.PermissionId.Should().NotBeEmpty();
            rolePermission.RoleId.Should().Be("test-role");
        }
    }
}
