using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Api.Models.Data.Config;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn(0, 1);

        builder.Property(x => x.Name)
        .HasColumnType("VARCHAR")
        .HasMaxLength(255).IsRequired();

        builder.Property(x => x.Salary)
        .IsRequired();

        builder.Property(x => x.JopTitle)
        .HasColumnType("VARCHAR")
        .HasMaxLength(255).IsRequired();

        builder.Property(x => x.Address)
        .HasColumnType("VARCHAR")
        .HasMaxLength(255).IsRequired();

        builder.HasOne(x => x.Department)
        .WithMany(x => x.Employees)
        .HasForeignKey(x => x.DepartmentId);

        builder.ToTable("Employees");
    }
}
