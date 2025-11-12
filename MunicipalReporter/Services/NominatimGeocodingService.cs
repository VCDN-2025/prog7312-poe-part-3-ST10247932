using MunicipalReporter.Models;
using System.Text.Json;
using System.Globalization; 

namespace MunicipalReporter.Services
{
    public class NominatimGeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NominatimGeocodingService> _logger;

        // HttpClient is injected and configured in Program.cs
        public NominatimGeocodingService(HttpClient httpClient, ILogger<NominatimGeocodingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Nominatim requires a descriptive User-Agent. Use your app name & contact.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MunicipalReporterApp/1.0 (your@email.com)");
        }

        public async Task<GeocodingResult> ValidateAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return new GeocodingResult { IsValid = false };
            }

            try
            {
                // URL encode the address and build the request
                var encodedAddress = Uri.EscapeDataString(address);
                // The 'format=json' and 'limit=1' parameters make the response small and fast.
                var requestUrl = $"https://nominatim.openstreetmap.org/search?format=json&q={encodedAddress}&limit=1";

                var response = await _httpClient.GetAsync(requestUrl);

                // If the API is down or returns an error, we don't want to block the user.
                // We'll log the error and return true (assuming the address is valid).
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Nominatim API error: {StatusCode}", response.StatusCode);
                    return new GeocodingResult { IsValid = true }; // Fail-open strategy
                }

                var json = await response.Content.ReadAsStringAsync();
                var places = JsonSerializer.Deserialize<JsonElement>(json);

                // If the array has items, the address was found and is valid.
                if (places.ValueKind == JsonValueKind.Array && places.GetArrayLength() > 0)
                {
                    var firstResult = places[0];
                    return new GeocodingResult
                    {
                        IsValid = true,
                        FormattedAddress = firstResult.GetProperty("display_name").GetString(),
                        Latitude = double.Parse(firstResult.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture),
                        Longitude = double.Parse(firstResult.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture)
                    };
                }

                // The array was empty, meaning no results were found.
                return new GeocodingResult { IsValid = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating address with Nominatim for '{Address}'", address);
                // Fail-open: if the service errors out, don't punish the user.
                return new GeocodingResult { IsValid = true };
            }
        }
    }
}
//Reference
//Stackoverflow, 2010, The Purpose of a Service Layer and ASP.NET MVC 2. [online] Available at: https://stackoverflow.com/questions/2762978/the-purpose-of-a-service-layer-and-asp-net-mvc-2 [Accessed 1 September 2025]
