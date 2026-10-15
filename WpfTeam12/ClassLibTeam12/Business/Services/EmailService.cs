using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibTeam12.Business
{
    /// <summary>
    /// Deze klasse is verantwoordelijk voor het versturen van e-mails via de MailJet API.
    /// </summary>
    public class EmailService
    {
        // Definieer de API-endpoint en de API keys (public en private).
        private readonly string apiUrl = "https://api.mailjet.com/v3.1/send";
        private readonly string apiKeyPublic = "YOUR_MAILJET_PUBLIC_KEY";
        private readonly string apiKeyPrivate = "YOUR_MAILJET_PRIVATE_KEY";

        /// <summary>
        /// Verstuurt een e-mail asynchroon.
        /// </summary>
        /// <param name="email">Het e-mailadres van de ontvanger.</param>
        /// <param name="subject">Het onderwerp van de e-mail.</param>
        /// <param name="htmlPart">De HTML-inhoud van de e-mail.</param>
        /// <returns>True indien de e-mail succesvol verzonden is, anders false.</returns>
        public async Task<bool> SendEmailAsync(string email, string subject, string htmlPart)
        {
            try
            {
                // Maak een payload object voor de MailJet API.
                var payload = new
                {
                    Messages = new[]
                    {
                        new
                        {
                            From = new { Email = "12203839@student.pxl.be", Name = "Victory"},
                            To = new[] { new { Email = email, Name = "User" } },
                            Subject = subject,
                            //TextPart = "Dear passenger 1, welcome to Mailjet! May the delivery force be with you!",
                            HTMLPart = htmlPart
                        }
                    }
                };

                // Serialiseer de payload naar JSON.
                string jsonData = JsonConvert.SerializeObject(payload);

                // Gebruik een HttpClient om de API-aanroep te doen.
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);  // Timeout 
                    // Maak een HttpRequestMessage voor de POST-aanroep.
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                    {
                        Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
                    };

                    // Voeg de autorisatieheader toe.
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKeyPublic}:{apiKeyPrivate}")));

                    // Verstuur het request en wacht op het antwoord.
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Controleer of het antwoord succesvol is.
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("E-mail succesvol verzonden!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Fout bij verzenden: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fout: " + ex.Message);
                return false;
            }
        }
    }
}
