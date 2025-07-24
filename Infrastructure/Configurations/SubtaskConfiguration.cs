using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SubtaskConfiguration : IEntityTypeConfiguration<Subtask>
    {
        public void Configure(EntityTypeBuilder<Subtask> builder)
        {
            builder.HasKey(st => st.Id);

            builder.Property(st => st.Title)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(st => st.Description)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(st => st.IsCompleted)
                .IsRequired();

            //Configure relationship with Project
            builder.HasOne(st => st.Project)
                .WithMany(p => p.Subtasks)
                .HasForeignKey(st => st.ProjectId)
                .IsRequired();

            builder.HasData(
                 new Subtask
                 {
                     Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                     Title = "Subtask 1 for Project Alpha",
                     Description = "Description for Subtask 1",
                     IsCompleted = false,
                     ProjectId = new Guid("80abbca8-664d-4b20-b5de-024705497d4a")
                 },
                 new Subtask
                 {
                     Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                     Title = "Subtask 2 for Project Alpha",
                     Description = "Description for Subtask 2",
                     IsCompleted = true,
                     ProjectId = new Guid("80abbca8-664d-4b20-b5de-024705497d4a")
                 },
                 new Subtask
                 {
                     Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                     Title = "Subtask 1 for Project Beta",
                     Description = "Description for Subtask 1",
                     IsCompleted = false,
                     ProjectId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
                 }
            );
        }
    }
}
