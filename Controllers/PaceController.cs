using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

    [HttpPost("calculate-external")]
    public async Task<ActionResult> CalculateWithExternalProcess([FromBody] PaceRequest request)
    {
        try
        {
            // Process A creates Process B
            var processInfo = new ProcessStartInfo
            {
                FileName = "python3", // or "python" on Windows
                Arguments = $"-c \"import sys; distance={request.Distance}; duration={request.Duration}; print(duration/distance if distance > 0 else 'Error'); sys.exit(0 if distance > 0 else 1)\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                return StatusCode(500, "Failed to start calculation process");
            }

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                var result = await process.StandardOutput.ReadToEndAsync();
                return Ok(new
                {
                    pace = result.Trim(),
                    message = "Process B completed successfully",
                    exitCode = process.ExitCode
                });
            }
            else
            {
                var error = await process.StandardError.ReadToEndAsync();
                return BadRequest(new
                {
                    message = "Process B failed with error",
                    error = error,
                    exitCode = process.ExitCode
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Process creation failed: {ex.Message}");
        }
    }
}