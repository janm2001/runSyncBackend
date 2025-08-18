using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace runSyncBackend.Controllers;

public class PaceRequest
{
    public double Distance { get; set; }
    public double Duration { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class PaceController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PaceController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CalculatePace([FromBody] PaceRequest request)
    {
        if (request.Distance <= 0)
        {
            return BadRequest("Distance must be greater than zero.");
        }

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <Calculate xmlns=""http://www.dneonline.com/calculator.asmx"">
      <operation>divide</operation>
      <x>{request.Duration}</x>
      <y>{request.Distance}</y>
    </Calculate>
  </soap:Body>
</soap:Envelope>";

        var client = _httpClientFactory.CreateClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://www.dneonline.com/calculator.asmx")
        {
            Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
        };

        httpRequest.Headers.Add("SOAPAction", "http://www.dneonline.com/calculator.asmx/Calculate");

        try
        {
            var response = await client.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            
            var soapResponseXml = await response.Content.ReadAsStringAsync();
            
            // Return the raw XML from the SOAP service directly to the frontend
            return Content(soapResponseXml, "application/xml");
        }
        catch (HttpRequestException e)
        {
            // Log the error
            Console.WriteLine($"Error calling SOAP service: {e.Message}");
            return StatusCode(500, "An error occurred while trying to contact the calculation service.");
        }
    }
}