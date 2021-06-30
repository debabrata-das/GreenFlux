namespace GreenFlux.SmartCharging.Api.CustomExceptionMiddleware.ProblemDetails
{
    public class GreenFluxProblemDetail : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public GreenFluxProblemDetail(string title, string message)
        {
            Title = title;
            Status = 400;
            Type = typeof(GreenFluxProblemDetail).ToString();
            Detail = message;
        }
    }
}
