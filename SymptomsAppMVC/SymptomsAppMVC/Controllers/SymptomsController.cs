using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SymptomsAppMVC.Models;
using SymptomsAppMVC.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

namespace SymptomsAppMVC.Controllers
{
    public class SymptomsController : Controller
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();
        private readonly CacheService _cacheService;
        private const string CacheAllSymptomsKey = "AllSymptoms";
        private const string CacheSymptomKeyPrefix = "Symptom_";

        Symptom _symptom = new Symptom();
        List <Symptom> _symptomsList = new List <Symptom> ();

        public SymptomsController(CacheService cacheService)
        {
            _cacheService = cacheService;
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            { return true; };
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<List<Symptom>> GetAllSymptoms()
        {
            var cacheDuration = TimeSpan.FromMinutes(5);
    List<Symptom> symptomsList = await _cacheService.GetOrSetAsync(CacheAllSymptomsKey, async () =>
    {
        using (var httpClient = new HttpClient(_clientHandler))
        {
            var response = await httpClient.GetAsync("http://localhost:5158/api/Symptoms");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Symptom>>(apiResponse);
            }
            return new List<Symptom>(); // return an empty list in case of API failure
        }
    }, cacheDuration);

    return View(symptomsList);

            //_symptomsList = new List<Symptom>();

            //using (var httpClient = new HttpClient(_clientHandler))
            //{
            //    using (var response = await httpClient.GetAsync("http://localhost:5158/api/Symptoms"))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        _symptomsList = JsonConvert.DeserializeObject<List<Symptom>>(apiResponse);
            //    }
            //}
            //return _symptomsList;
        }

        [HttpGet]
        public async Task<Symptom> GetSymptomById(string symptomId)
        {
            _symptom = new Symptom();

            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.GetAsync("http://localhost:5158/api/Symptoms" + symptomId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    _symptom = JsonConvert.DeserializeObject<Symptom>(apiResponse);
                }
            }
            return _symptom;
        }

        [HttpPost]
        public async Task<Symptom> AddSymptom(Symptom symptom)
        {
            _symptom = new Symptom();

            using (var httpClient = new HttpClient(_clientHandler))
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(symptom), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("http://localhost:5158/api/Symptoms", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    _symptom = JsonConvert.DeserializeObject<Symptom>(apiResponse);
                }
            }
            return _symptom;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            using (var httpClient = new HttpClient(_clientHandler))
            {
                var response = await httpClient.GetAsync($"http://localhost:5158/api/Symptoms/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var symptom = await response.Content.ReadAsAsync<Symptom>();
                    ViewBag.SymptomId = symptom.Id;
                    ViewBag.PainType = symptom.PainType;
                    ViewBag.SeverityScale = symptom.SeverityScale;
                    ViewBag.SymptomDurationHours = symptom.SymptomDurationHours;
                    return View("EditSymptom");
                }
                return NotFound();
            }
               
        }
        //public async Task<IActionResult> EditSymptom(string id, Symptom updatedSymptom)
        //{
        //    // Validate the incoming updatedSymptom object
        //    if (updatedSymptom == null)
        //    {
        //        return BadRequest("Updated symptom data is null.");
        //    }

        //    // Make sure the ID in the URL matches the ID in the body (if ID is part of the body)
        //    if (updatedSymptom.Id != id)
        //    {
        //        return BadRequest("Symptom ID mismatch.");
        //    }

        //    using (var httpClient = new HttpClient())
        //    {
        //        // Serialize the updatedSymptom object to JSON
        //        var content = new StringContent(JsonConvert.SerializeObject(updatedSymptom), Encoding.UTF8, "application/json");

        //        // Send the PUT request to the external API
        //        using (var response = await httpClient.PutAsync($"http://localhost:5158/api/Symptoms/{id}", content))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                // If the update is successful, you can return the updated symptom or simply a 204 No Content status
        //                return NoContent(); // 204 No Content indicates success without returning data
        //            }
        //            else
        //            {
        //                // Handle the case where the API call fails
        //                return StatusCode((int)response.StatusCode, "Error while updating the symptom.");
        //            }
        //        }
        //    }
        //}

        [HttpDelete]
        public async Task<string> DeleteSymptom(string symptomId)
        {
            string message = "";

            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.DeleteAsync("http://localhost:5158/api/Symptoms" + symptomId))
                {
                    message = await response.Content.ReadAsStringAsync();
                }
            }
            return message;
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveSymptom(Symptom symptom)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (string.IsNullOrEmpty(symptom.Id))
        //        {
        //            // Add new symptom
        //            await AddSymptom(symptom);
        //        }
        //        else
        //        {
        //            // Update existing symptom
        //            await EditSymptom(symptom.Id, symptom);
        //        }
        //        return RedirectToAction("Index", "Symptoms"); // Redirect to the symptom list page after saving
        //    }
        //    return View("EditSymptom", symptom); // Return the view with the current symptom model if validation fails
        //}

    }
}
