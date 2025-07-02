using ACogSearch.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace ACogSearch.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new SearchViewModel
            {
                UploadedFiles = await _searchService
                .GetAllUploadedFilesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                model.Results = await _searchService.SearchDocumentsAsync(model.Query);
            }

            model.UploadedFiles = await _searchService
                .GetAllUploadedFilesAsync();
            return View(model);
        }
    }

}
