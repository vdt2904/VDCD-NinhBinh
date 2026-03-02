using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using VDCD.Business.Infrastructure;

namespace VDCD.Business.Service
{
    public class AiService : IAiService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly IConfiguration _config;
		private readonly SettingService _settingService;
		public AiService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _apiKey = config["OpenAI:ApiKey"];
            _config = config;
        }

        // service genai post
        public async Task<string> GeneratePost(string topic)
        {
            var relativePath = _config["CompanyDocs:Folder"];

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                relativePath);

            if (!Directory.Exists(folderPath))
                throw new Exception("CompanyDocs folder not found");

            var knowledge = ExtractAllPdfText(folderPath);

            var prompt = $@"
                Dựa trên tài liệu công ty sau:

                {knowledge}

                Hãy viết bài Facebook marketing về:
                {topic}

                Giọng văn chuyên nghiệp, thu hút.";

            return await AskOpenAI(prompt);
        }

        private async Task<string> AskOpenAI(string prompt)
        {
			var KeyGpt = _settingService.Get("setting.openai.api_key");
            var ModelGPT = _settingService.Get("setting.openai.model") ?? "gpt-4.1-mini";
			var body = new
            {
                model = ModelGPT,
                input = prompt
            };
			
			_client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", KeyGpt);

            var res = await _client.PostAsJsonAsync(
                "https://api.openai.com/v1/responses", body);

            var raw = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception($"OpenAI error: {raw}");

            dynamic data = JsonConvert.DeserializeObject(raw);

            return data.output[0].content[0].text;
        }

        private string ExtractAllPdfText(string folderPath)
        {
            var text = "";

            var files = Directory.GetFiles(folderPath, "*.pdf");

            foreach (var file in files)
            {
                using var stream = File.OpenRead(file);
                using var pdf = PdfDocument.Open(stream);

                foreach (var page in pdf.GetPages())
                    text += page.Text + "\n";
            }

            return text;
        }
    }
}