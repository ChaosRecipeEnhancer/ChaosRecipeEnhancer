using System.Text.Json;
using ChaosRecipeEnhancer.API.Helpers;
using ChaosRecipeEnhancer.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChaosRecipeEnhancer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _secret;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AuthenticationController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AuthenticationController> logger)
        {
            _secret = configuration.GetSection("Settings")["Secret"];
            _httpClientFactory = httpClientFactory;
            _clientId = configuration.GetSection("OAuth2")["ClientId"];
            _clientSecret = configuration.GetSection("OAuth2")["ClientSecret"];
            _logger = logger;
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromBody]CreTokenRequest creTokenRequest)
        {
            var accountValid = await ValidateAccount(creTokenRequest.ClientId, creTokenRequest.AccessToken);
            if (!accountValid)
            {
                _logger.LogError($"[Account: {creTokenRequest.Name}] - Token has expired for account with Id: {creTokenRequest.Name}");
                return BadRequest("error:token_expired");
            }
            
            var userAgent = Request.Headers["User-Agent"].ToString();
            var version = HeadersHelper.VersionFromUserAgent(userAgent);

            creTokenRequest.Version = version;

            var token = AuthHelper.GenerateToken(_secret, creTokenRequest);

            creTokenRequest.AccessToken = token;

            return Ok(creTokenRequest);
        }

        [HttpGet]
        [Route("oauth2")]
        public async Task<IActionResult> OAuth2(string code)
        {
            var uri = "https://www.pathofexile.com/oauth/token";

            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "chaosrecipeenhancer");

                var data = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
                });
                var response = await client.PostAsync(uri, data);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(content);
                }

                _logger.LogError($"Something went wrong fetching token from code: {code}, reason: {response.ReasonPhrase}");
                return BadRequest(content);
            }
        }
        
        [HttpGet]
        [Route("redirect")]
        public IActionResult Redirect(string code, string state)
        {
            return Redirect($"chaosrecipeenhancer://?code={code}&state={state}");
        }
        
        private async Task<bool> ValidateAccount(string accountName, string accessToken)
        {
            string uri = "https://www.pathofexile.com/api/profile";

            try
            {

                using (var client = _httpClientFactory.CreateClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    client.DefaultRequestHeaders.Add("User-Agent", "chaosrecipeenhancer");


                    var response = await client.GetAsync(uri);
                    var content = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var model = JsonSerializer.Deserialize<ProfileEndpointModel>(content, options);

                    if (response.IsSuccessStatusCode)
                    {
                        if (model.Name == accountName)
                        {
                            return true;
                        }
                        else
                        {
                            _logger.LogError($"Mismatch between said accountName: {accountName} and accountName fetched from GGG: {model.Name}.");
                        }
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong trying to validate account: {accountName} with token: {accessToken}, reason: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception when trying to validate account: {accountName} with token: {accessToken}, message: {e.Message}");
                return false;
            }

            return false;

        }

    }
}