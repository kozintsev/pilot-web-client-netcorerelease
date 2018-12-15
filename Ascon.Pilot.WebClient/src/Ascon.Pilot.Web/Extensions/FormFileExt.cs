using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Ascon.Pilot.Web.Extensions
{
    public static class FormFileExt
    {
        public static string GetFileName(this IFormFile formFile)
        {
            ContentDispositionHeaderValue cd;
            ContentDispositionHeaderValue.TryParse(formFile.ContentDisposition, out cd);
            return HeaderUtilities.RemoveQuotes(cd.FileName).Value;
        }
    }
}
