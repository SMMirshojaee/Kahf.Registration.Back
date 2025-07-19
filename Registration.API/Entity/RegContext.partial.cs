using Microsoft.EntityFrameworkCore;

namespace Registration.API.Entity;
public partial class RegContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicantFormValue>().HasQueryFilter(e => !e.Deleted);
    }
}