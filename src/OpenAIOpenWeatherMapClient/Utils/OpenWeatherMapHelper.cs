using OpenWeatherMapSharp;
using OpenWeatherMapSharp.Models;
using OpenWeatherMapSharp.Models.Enums;

namespace OpenAIOpenWeatherMapClient.Utils;

/// <summary>
///     Helper class for fetching weather information from OpenWeatherMap.
/// </summary>
internal static class OpenWeatherMapHelper
{
    /// <summary>
    ///     Fetches weather information from OpenWeatherMap based on the city.
    /// </summary>
    /// <param name="openWeatherMapService">The OpenWeatherMap service instance.</param>
    /// <param name="city">The name of the city.</param>
    /// <returns>The weather information as a string.</returns>
    public static async Task<string> GetWeatherFromOpenWeatherAsync(
        OpenWeatherMapService openWeatherMapService,
        string city)
    {
        try
        {
            // Get city location information from OpenWeatherMap
            OpenWeatherMapServiceResponse<List<GeocodeInfo>>? location
                = await openWeatherMapService.GetLocationByNameAsync(city);

            if (location?.Response is null || location.Response.Count == 0)
            {
                return "No location found for the specified city.";
            }

            // Get weather data for the location
            var weather = await openWeatherMapService.GetWeatherAsync(
                location.Response[0].Latitude,
                location.Response[0].Longitude,
                LanguageCode.EN,
                Unit.Metric);

            if (weather?.Response is null)
            {
                return "No weather data available.";
            }

            // Format and return weather data
            return $"Current weather in {city}. " +
                $"Temperature: {weather.Response.MainWeather.Temperature}°C. " +
                $"Condition: {weather.Response.WeatherInfos[0].Description}.";
        }
        catch (Exception ex)
        {
            return $"Error retrieving weather data: {ex.Message}";
        }
    }
}
