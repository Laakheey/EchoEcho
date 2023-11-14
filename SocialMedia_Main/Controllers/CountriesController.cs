using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMedia_Common.UserView;
using System.Net.Http;
using System.Text;

namespace SocialMedia_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public CountriesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [AllowAnonymous]
        [Route("getCountries")]
        [HttpGet]
        public async Task<IEnumerable<Country>> GetCountries()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all");
                var content = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<Countries>>(content);

                var countriesList = countries.Select(country => new Country
                {
                    CountryName = country.Name.Common,
                    CountryCode = country.Cca2
                });

                countriesList = countriesList.OrderBy(country => country.CountryName);
                return countriesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AllowAnonymous]
        [Route("getCities")]
        [HttpPost]
        public async Task<IEnumerable<string>> GetCities(string countryName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://countriesnow.space/api/v0.1/countries/cities");
            request.Content = new StringContent($"{{ \"country\": \"{countryName}\" }}", Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);


            // var response = await _httpClient.GetAsync($"https://countriesnow.space/api/v0.1/countries/cities?&country={countryCode.Country}");
            var content = await response.Content.ReadAsStringAsync();
            var citiesResponse = JsonConvert.DeserializeObject<CitiesResponse>(content);
            if(citiesResponse.Data == null)
            {
                return null;
            }
            var cities = citiesResponse.Data.OrderBy(city => city);
            return cities;
        }




    }
}
