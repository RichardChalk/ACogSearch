using ACogSearch.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace ACogSearch.Controllers
{
    public class UploadController : Controller
    {
        private readonly DocumentUploader _uploader;

        public UploadController(DocumentUploader uploader)
        {
            _uploader = uploader;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(UploadViewModel model)
        {
            // Validera model och spara till en mapp som heter 'Uploads'
            if (ModelState.IsValid && model.File != null)
            {
                var filePath = Path.Combine("Uploads", model.File.FileName);
                Directory.CreateDirectory("Uploads");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                model.Message = "Filen har laddats upp!";
            }

            using (var reader = new StreamReader(model.File.OpenReadStream()))
            {
                var content = await reader.ReadToEndAsync();

                // Visar en förhandsgranskning av innehållet (max 200tecken)
                model.Message = "Filen har laddats upp! <br /> Förhandsgranskning: " 
                    + content.Substring(0, Math.Min(200, content.Length)) 
                    + "...";

                // Skicka till Azure Search
                var doc = new FileDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = model.File.FileName,
                    Content = content
                };

                await _uploader.UploadDocumentAsync(doc);
            }



            return View(model);
        }
    }
}
