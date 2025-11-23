namespace WorkConnect.Api.Hateoas
{
    public class LinkDto
    {
        public string Rel { get; set; } = null!;
        public string Href { get; set; } = null!;
        public string Method { get; set; } = null!;
    }

    public static class HateoasHelper
    {
        public static string BuildUrl(HttpContext httpContext, string relativePath)
        {
            var request = httpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }
            return baseUrl + relativePath;
        }
    }
}
