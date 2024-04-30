using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using TierListApp.Server.Models;

namespace TierListApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private static readonly string[] regions = [
            "apac",
            "americas",
            "emea",
            "china"
        ];


        private static readonly Dictionary<string, string> urls = new Dictionary<string, string>
        {
            { "apac", "https://www.vlr.gg/event/2002/champions-tour-2024-pacific-stage-1/regular-season" },
            { "americas", "https://www.vlr.gg/event/2004/champions-tour-2024-americas-stage-1/regular-season" },
            { "emea", "https://www.vlr.gg/event/1998/champions-tour-2024-emea-stage-1/regular-season" },
            { "china", "https://www.vlr.gg/event/2006/champions-tour-2024-china-stage-1/regular-season" },
        };


        [HttpGet]
        public async Task<TeamModel[]> Get()
        {
            List<TeamModel> items = new List<TeamModel>();
            // for each urls
            // do the same thing and add it to items
            try
            {
                foreach (var region in regions)
                {
                    var testUrl = urls[region];
                    var httpClient = new HttpClient();
                    var html = await httpClient.GetStringAsync(testUrl);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    var divsHtml = new List<string>();
                    var elements = htmlDocument.DocumentNode.SelectNodes("//a[contains(@style, 'display: flex; align-items: center; padding-right: 2px; padding-left: 0;')]");

                    if (elements == null)
                    {
                        Console.WriteLine($"Cannot find matching content on {region}");
                        continue;
                    }

                    foreach (var element in elements)
                    {
                        var href = element.GetAttributeValue("href", string.Empty);
                        var img = element.SelectSingleNode(".//img");
                        var imgSrc = img != null ? img.GetAttributeValue("src", string.Empty) : string.Empty;
                        var teamName = element.SelectSingleNode(".//div[contains(@class, 'event-group-team text-of')]");
                        var teamNameText = string.Empty;
                        if (teamName != null)
                        {
                            var firstTextNode = teamName.ChildNodes.FirstOrDefault(n => n.NodeType == HtmlNodeType.Text);
                            teamNameText = firstTextNode != null ? firstTextNode.InnerText.Trim() : string.Empty;
                        }
                        var country = element.SelectSingleNode(".//div[contains(@class, 'ge-text-light')]");
                        var countryText = country != null ? country.InnerText.Trim() : string.Empty;

                        // Construct new Item and push 
                        var baseUrl = "https://www.vlr.gg/";
                        items.Add(new TeamModel { Id = (items.Count + 1), Title = teamNameText, ImageUrl = $"https:{imgSrc}", Url = $"{baseUrl}{href}", Origin = countryText });
                    }
                }

                // for each item in items console log them
                foreach (var item in items)
                {
                    Console.WriteLine($"Id: {item.Id}");
                    Console.WriteLine($"Title: {item.Title}");
                    Console.WriteLine($"ImageUrl: {item.ImageUrl}");
                    Console.WriteLine($"Url: {item.Url}");
                    Console.WriteLine($"Origin: {item.Origin}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured: {ex.Message}");
            }

            return items.ToArray();
        }
    }
}
