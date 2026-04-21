using System.Text;

namespace RSign.SendAPI.API
{
    public class ApiEndpoints
    {
        private readonly IConfiguration _configuration;

        public ApiEndpoints(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void UsefullLinksAPI(WebApplication app)
        {
            app.MapGet("/api/v1/UsefullLinks", GeUsefullLinks);
        }

        public async Task<IResult> GeUsefullLinks()
        {
            string template = Convert.ToString(_configuration["TemplatePath"]) + "Links.html";

            byte[] file = Encoding.UTF8.GetBytes(template);
            string filename = $"UsefullLinks.html";
            return Results.File(file, "text/html", filename, false);
        }
    }
}
