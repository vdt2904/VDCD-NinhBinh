using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UglyToad.PdfPig;
using VDCD.Business.Infrastructure;
using VDCD.Business.Service;

namespace VDCD.Business.Service
{
    public class AiService : IAiService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly IConfiguration _config;
        private readonly SettingService _settingService;

        public AiService(HttpClient client, IConfiguration config, SettingService settingService)
        {
            _client = client;
            _apiKey = config["OpenAI:ApiKey"];
            _config = config;
            _settingService = settingService;
        }

        // service genai post - prefers fbAttachmentsList; falls back to CompanyDocs folder when attachments missing
        public async Task<string> GeneratePost(string topic, List<string>? fbAttachmentsList = null)
        {
            string knowledge;

            if (fbAttachmentsList != null && fbAttachmentsList.Any())
            {
                // Extract text from provided attachments (PDFs, DOCX, TXT). Images / other types are included as references.
                knowledge = await ExtractTextFromAttachmentsAsync(fbAttachmentsList);
                if (string.IsNullOrWhiteSpace(knowledge))
                {
                    // fallback to company docs if attachments provided but no text extracted
                    knowledge = ExtractAllPdfTextFromCompanyDocs();
                }
            }
            else
            {
                // original behavior: read company docs folder
                knowledge = ExtractAllPdfTextFromCompanyDocs();
            }

            var attachmentsText = "";
            if (fbAttachmentsList != null && fbAttachmentsList.Any())
            {
                attachmentsText = "\n\nAttached files (for reference):\n" + string.Join("\n", fbAttachmentsList);
            }

            var prompt = $@"
                Dựa trên tài liệu công ty sau:

                {knowledge}

                Hãy viết bài Facebook marketing về:
                {topic}

                Giọng văn chuyên nghiệp, thu hút.
                {attachmentsText}
            ";

            return await AskOpenAI(prompt);
        }

        private async Task<string> ExtractTextFromAttachmentsAsync(List<string> attachments)
        {
            var sb = new StringBuilder();

            foreach (var item in attachments)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    // Remote URL
                    if (item.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
                        var resp = await http.GetAsync(item);
                        if (!resp.IsSuccessStatusCode)
                        {
                            // add as reference if download fails
                            sb.AppendLine($"Reference (unavailable): {item}");
                            continue;
                        }

                        using var ms = new MemoryStream();
                        await resp.Content.CopyToAsync(ms);
                        ms.Position = 0;

                        var contentType = resp.Content.Headers.ContentType?.MediaType ?? "";
                        if (contentType.Contains("pdf") || item.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            TryExtractPdfFromStream(ms, sb);
                        }
                        else if (item.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) || contentType.Contains("officedocument.wordprocessingml"))
                        {
                            TryExtractDocxFromStream(ms, sb);
                        }
                        else if (item.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || contentType.Contains("text"))
                        {
                            ms.Position = 0;
                            using var sr = new StreamReader(ms);
                            sb.AppendLine(await sr.ReadToEndAsync());
                        }
                        else
                        {
                            // Images and other types: include as reference (OCR not implemented here)
                            sb.AppendLine($"Reference file: {item}");
                        }
                    }
                    else
                    {
                        // Local path (absolute or relative)
                        var localPath = item;
                        if (!Path.IsPathRooted(localPath))
                        {
                            // treat as relative path from content root; trim starting slashes
                            localPath = Path.Combine(Directory.GetCurrentDirectory(), localPath.TrimStart('/', '\\'));
                        }

                        if (!File.Exists(localPath))
                        {
                            sb.AppendLine($"Reference (missing): {localPath}");
                            continue;
                        }

                        if (localPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            TryExtractPdfFromFile(localPath, sb);
                        }
                        else if (localPath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                        {
                            TryExtractDocxFromFile(localPath, sb);
                        }
                        else if (localPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine(File.ReadAllText(localPath));
                        }
                        else
                        {
                            // Images and other types: include as reference (OCR not implemented here)
                            sb.AppendLine($"Reference file: {localPath}");
                        }
                    }
                }
                catch
                {
                    // best-effort: continue with others
                    continue;
                }
            }

            return sb.ToString().Trim();
        }

        private static void TryExtractPdfFromStream(Stream ms, StringBuilder sb)
        {
            try
            {
                ms.Position = 0;
                using var pdf = PdfDocument.Open(ms);
                foreach (var page in pdf.GetPages())
                    sb.AppendLine(page.Text);
            }
            catch
            {
                // ignore unreadable pdfs
            }
        }

        private static void TryExtractPdfFromFile(string path, StringBuilder sb)
        {
            try
            {
                using var stream = File.OpenRead(path);
                using var pdf = PdfDocument.Open(stream);
                foreach (var page in pdf.GetPages())
                    sb.AppendLine(page.Text);
            }
            catch
            {
                // ignore unreadable pdfs
            }
        }

        private static void TryExtractDocxFromStream(Stream ms, StringBuilder sb)
        {
            try
            {
                ms.Position = 0;
                using var zip = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: true);
                var entry = zip.GetEntry("word/document.xml");
                if (entry == null) return;

                using var entryStream = entry.Open();
                var xdoc = XDocument.Load(entryStream);
                // WordprocessingML uses namespace; collect all text nodes with local name 't'
                var texts = xdoc.Descendants().Where(x => x.Name.LocalName == "t").Select(x => (string?)x.Value).Where(v => !string.IsNullOrEmpty(v));
                foreach (var t in texts)
                    sb.AppendLine(t);
            }
            catch
            {
                // ignore unreadable docx
            }
        }

        private static void TryExtractDocxFromFile(string path, StringBuilder sb)
        {
            try
            {
                using var fs = File.OpenRead(path);
                TryExtractDocxFromStream(fs, sb);
            }
            catch
            {
                // ignore unreadable docx
            }
        }

        private string ExtractAllPdfTextFromCompanyDocs()
        {
            var relativePath = _config["CompanyDocs:Folder"];
            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                relativePath);

            if (!Directory.Exists(folderPath))
                return string.Empty;

            return ExtractAllPdfText(folderPath);
        }

        private async Task<string> AskOpenAI(string prompt)
        {
            var KeyGpt = _settingService.Get("setting.openai.api_key");
            if (string.IsNullOrWhiteSpace(KeyGpt))
                throw new Exception("OpenAI API key not configured");

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

            // Defensive null checks
            try
            {
                return data.output[0].content[0].text;
            }
            catch
            {
                return raw;
            }
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