using System.ComponentModel.DataAnnotations;

namespace ACogSearch.Viewmodels
{
    public class UploadViewModel
    {
        [Required]
        [Display(Name = "Välj en textfil")]
        public IFormFile File { get; set; }

        public string? Message { get; set; }
    }
}
