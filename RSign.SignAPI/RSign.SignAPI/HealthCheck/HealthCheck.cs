namespace RSign.SignAPI.HealthCheck
{
    public class HealthCheck
    {       
        /// <summary>
        /// Get application status
        /// </summary>
        /// <returns>{ Status: "OK" }</returns>
        /// <remarks>Used for application health check</remarks>
        public IResult GetStatus()
        {
            return Results.Ok(new { Status = "OK " });
        }

        /* Status API for health checks */
        public void RegisterStatusApi(WebApplication app)
        {
            app.MapGet("/api/v1/status", GetStatus);           
        }
    }
}
