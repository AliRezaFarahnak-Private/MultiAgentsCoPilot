using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Net.Http;

namespace MultiAgentsCoPilot;

public class WorldWeatherPlugin 
{
    [KernelFunction, Description(@$"Describe the time for the location and weather for each day line by line, including the temperature in Celsius and any other available details.  
                                     Each line should conclude with an emoji that best represents the weather for that day.")]
    public async Task<string> WeatherAsync(
        [Description("Latitude of location you calculate this based on location")] string latitude,
        [Description("Longitude of location you calculate this based on location")] string longitude)
    {
        string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,weathercode&timezone=auto";
        string weatherJson = await new HttpClient().GetStringAsync(url);
        var currentUtcTime = $@"This is time UTC: {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}, Now calculate time for the location";
        weatherJson = $"Current UTC time: {currentUtcTime}\n{weatherJson}";
        return weatherJson;
    }

}