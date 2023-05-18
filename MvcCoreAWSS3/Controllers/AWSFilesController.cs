using Microsoft.AspNetCore.Mvc;
using MvcCoreAWSS3.Services;

namespace MvcCoreAWSS3.Controllers
{
    public class AWSFilesController : Controller
    {
        private ServiceStorageS3 serviceStorageS3;
        private string BucketUrl;

        public AWSFilesController(ServiceStorageS3 serviceStorageS3, IConfiguration configuration)
        {
            this.serviceStorageS3 = serviceStorageS3;
            this.BucketUrl = configuration.GetValue<string>("AWS:BucketName");
        }

        /*public async Task<IActionResult> Index()
        {
            List<string> files = await this.serviceStorageS3.GetVersionsFilesAsync();
            ViewData["BUCKETURL"] = this.BucketUrl;
            return View(files);
        }*/

        public async Task<IActionResult> Index()
        {
            ViewData["BUCKETURL"] = this.BucketUrl;
            var files = await this.serviceStorageS3.GetFilesAsync();
            return View(files);
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceStorageS3.UploadFileAsync(file.FileName, stream);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FileAWS(string fileName)
        {
            Stream stream = await this.serviceStorageS3.GetFileAsync(fileName);
            return File(stream, "image/png");
        }

        public async Task<IActionResult> DeleteFile(string fileName)
        {
            await this.serviceStorageS3.DeleteFileAsync(fileName);
            return RedirectToAction("Index");
        }
    }
}
