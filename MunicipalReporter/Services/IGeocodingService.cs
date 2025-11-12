using MunicipalReporter.Models;

namespace MunicipalReporter.Services
{
    public interface IGeocodingService
    {
        Task<GeocodingResult> ValidateAddressAsync(string address);
    }
}
//Reference
//Stackoverflow, 2010, The Purpose of a Service Layer and ASP.NET MVC 2. [online] Available at: https://stackoverflow.com/questions/2762978/the-purpose-of-a-service-layer-and-asp-net-mvc-2 [Accessed 1 September 2025]
