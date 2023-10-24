using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SearchEngine.API.Models;

/// <summary>
/// This class is model of the content designed with Data Annotations to provide runtime validation via Type checking in backend and JQueryValidation on the frontend.
/// </summary>
public class ContentModel
{
    [Required]
    [Display(Name = "Content")]
    [DataType(DataType.Text)]
    public string? Content { get; set; }

    [Required]
    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }
}

