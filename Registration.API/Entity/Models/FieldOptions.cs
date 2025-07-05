using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class FieldOptions
{
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public int FieldId { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ApplicantFormValues> ApplicantFormValues { get; set; } = new List<ApplicantFormValues>();
}
