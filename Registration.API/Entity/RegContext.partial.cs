using Microsoft.EntityFrameworkCore;

namespace Registration.API.Entity;

public partial class RegContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicantFormValues>().HasQueryFilter(e => !e.Deleted);
    }
}