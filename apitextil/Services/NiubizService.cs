    // Services/NiubizService.cs
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.Extensions.Configuration;

    namespace apitextil.Services
    {
        public class NiubizService
        {
            private readonly IConfiguration _config;
            private readonly HttpClient _httpClient;

            public NiubizService(IConfiguration config, HttpClient httpClient)
            {
                _config = config;
                _httpClient = httpClient;
            }

            // Generar token de sesión de seguridad
            public async Task<string> GenerarTokenAsync()
            {
                var url = $"{_config["Niubiz:ApiUrl"]}/api.security/v1/security";
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{_config["Niubiz:User"]}:{_config["Niubiz:Password"]}"
                ));

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al generar token: {response.StatusCode} - {result}");
                    throw new ApplicationException("No se pudo generar el token de Niubiz.");
                }

                var token = result.Trim('"').Trim();
                Console.WriteLine($"TOKEN FINAL = >>>{token}<<<");
                return token;
            }


            // Ejecutar transacción de venta
            public async Task<NiubizResponse?> EjecutarVentaAsync(NiubizVentaRequest ventaRequest, string token)
            {
                var url = $"{_config["Niubiz:ApiUrl"]}/api.authorization/v3/authorization/ecommerce/{_config["Niubiz:MerchantId"]}";

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(ventaRequest), Encoding.UTF8, "application/json")
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine($"TOKEN USADO >>> {token}");
                Console.WriteLine($"JSON ENVIADO >>> {JsonSerializer.Serialize(ventaRequest)}");

                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("RESPUESTA ERROR >>> " + result);
                    throw new ApplicationException($"Error en la transacción Niubiz: {result}");
                }

                return JsonSerializer.Deserialize<NiubizResponse>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }

        // ----------------- MODELOS ------------------

        public class NiubizVentaRequest
        {
            [JsonPropertyName("channel")]
            public string Channel { get; set; } = "web";

            [JsonPropertyName("captureType")]
            public string CaptureType { get; set; } = "manual";

            [JsonPropertyName("countable")]
            public bool Countable { get; set; } = true;

            [JsonPropertyName("order")]
            public NiubizOrder Order { get; set; } = new();

            [JsonPropertyName("dataMap")]
            public NiubizDataMap DataMap { get; set; } = new();
        }

        public class NiubizOrder
        {
            [JsonPropertyName("tokenId")]
            public string TokenId { get; set; } = "";

            [JsonPropertyName("purchaseNumber")]
            public string PurchaseNumber { get; set; } = "";

            [JsonPropertyName("amount")]
            public decimal Amount { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; } = "PEN";
        }

        public class NiubizDataMap
        {
            [JsonPropertyName("urlAddress")]
            public string UrlAddress { get; set; } = "https://tusitio.com";

            [JsonPropertyName("serviceLocationCityName")]
            public string ServiceLocationCityName { get; set; } = "Lima";

            [JsonPropertyName("serviceLocationCountrySubdivisionCode")]
            public string ServiceLocationCountrySubdivisionCode { get; set; } = "LMA";

            [JsonPropertyName("serviceLocationCountryCode")]
            public string ServiceLocationCountryCode { get; set; } = "PER";

            [JsonPropertyName("serviceLocationPostalCode")]
            public string ServiceLocationPostalCode { get; set; } = "15001";
        }

        public class NiubizResponse
        {
            [JsonPropertyName("data")]
            public string Data { get; set; }

            [JsonPropertyName("channel")]
            public string Channel { get; set; }

            [JsonPropertyName("actionCode")]
            public string ActionCode { get; set; }

            [JsonPropertyName("traceNumber")]
            public string TraceNumber { get; set; }

            [JsonPropertyName("transactionId")]
            public string TransactionId { get; set; }

            [JsonPropertyName("eci")]
            public string ECI { get; set; }

            [JsonPropertyName("brand")]
            public string Brand { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("card")]
            public string Card { get; set; }
        }
    }
