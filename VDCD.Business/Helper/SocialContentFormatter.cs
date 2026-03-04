using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace VDCD.Business.Helper
{
	public static class SocialContentFormatter
	{
		public static string ToFacebookText(string? html)
		{
			if (string.IsNullOrWhiteSpace(html))
				return string.Empty;

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var builder = new StringBuilder();

			foreach (var node in doc.DocumentNode.ChildNodes)
			{
				ProcessNode(node, builder);
			}

			var result = builder.ToString();

			// Chuẩn hóa xuống dòng dư
			result = NormalizeNewLines(result);
			return result.Trim();
		}

		private static void ProcessNode(HtmlNode node, StringBuilder builder)
		{
			switch (node.Name.ToLower())
			{
				case "#text":
					builder.Append(WebUtility.HtmlDecode(node.InnerText));
					break;

				case "p":
					ProcessChildren(node, builder);
					builder.AppendLine().AppendLine();
					break;

				case "br":
					builder.AppendLine();
					break;

				case "strong":
				case "b":
					var boldText = GetInnerText(node);
					builder.Append(ConvertToUnicodeBold(boldText));
					break;

				case "ul":
					builder.AppendLine();
					ProcessChildren(node, builder);
					builder.AppendLine();
					break;

				case "ol":
					builder.AppendLine();
					int index = 1;
					foreach (var li in node.Elements("li"))
					{
						builder.Append($"{index}. ");
						ProcessChildren(li, builder);
						builder.AppendLine();
						index++;
					}
					builder.AppendLine();
					break;

				case "li":
					builder.Append("• ");
					ProcessChildren(node, builder);
					builder.AppendLine();
					break;

				default:
					ProcessChildren(node, builder);
					break;
			}
		}

		private static void ProcessChildren(HtmlNode node, StringBuilder builder)
		{
			foreach (var child in node.ChildNodes)
			{
				ProcessNode(child, builder);
			}
		}

		private static string GetInnerText(HtmlNode node)
		{
			return WebUtility.HtmlDecode(node.InnerText);
		}

		private static string NormalizeNewLines(string input)
		{
			while (input.Contains("\n\n\n"))
				input = input.Replace("\n\n\n", "\n\n");

			return input;
		}

		// Giả lập chữ in đậm bằng Unicode
		private static string ConvertToUnicodeBold(string input)
		{
			var boldMap = new Dictionary<char, string>
			{
				['a'] = "𝗮",
				['b'] = "𝗯",
				['c'] = "𝗰",
				['d'] = "𝗱",
				['e'] = "𝗲",
				['f'] = "𝗳",
				['g'] = "𝗴",
				['h'] = "𝗵",
				['i'] = "𝗶",
				['j'] = "𝗷",
				['k'] = "𝗸",
				['l'] = "𝗹",
				['m'] = "𝗺",
				['n'] = "𝗻",
				['o'] = "𝗼",
				['p'] = "𝗽",
				['q'] = "𝗾",
				['r'] = "𝗿",
				['s'] = "𝘀",
				['t'] = "𝘁",
				['u'] = "𝘂",
				['v'] = "𝘃",
				['w'] = "𝘄",
				['x'] = "𝘅",
				['y'] = "𝘆",
				['z'] = "𝘇",
				['A'] = "𝗔",
				['B'] = "𝗕",
				['C'] = "𝗖",
				['D'] = "𝗗",
				['E'] = "𝗘",
				['F'] = "𝗙",
				['G'] = "𝗚",
				['H'] = "𝗛",
				['I'] = "𝗜",
				['J'] = "𝗝",
				['K'] = "𝗞",
				['L'] = "𝗟",
				['M'] = "𝗠",
				['N'] = "𝗡",
				['O'] = "𝗢",
				['P'] = "𝗣",
				['Q'] = "𝗤",
				['R'] = "𝗥",
				['S'] = "𝗦",
				['T'] = "𝗧",
				['U'] = "𝗨",
				['V'] = "𝗩",
				['W'] = "𝗪",
				['X'] = "𝗫",
				['Y'] = "𝗬",
				['Z'] = "𝗭"
			};

			var sb = new StringBuilder();

			foreach (var c in input)
			{
				if (boldMap.ContainsKey(c))
					sb.Append(boldMap[c]);
				else
					sb.Append(c);
			}

			return sb.ToString();
		}

		// New: convert plain AI text into HTML suitable for CKEditor
		public static string ToHtmlForEditor(string? text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			var trimmed = text.Trim();

			// If there are obvious HTML tags, assume already HTML and return as-is
			if (Regex.IsMatch(trimmed, @"<\s*\/?\s*(p|br|div|ul|ol|li|strong|b|em|i|h[1-6]|a|img)[\s>]", RegexOptions.IgnoreCase))
				return trimmed;

			// Normalize new lines
			trimmed = trimmed.Replace("\r\n", "\n").Replace("\r", "\n");

			var paragraphs = trimmed.Split(new string[] { "\n\n" }, StringSplitOptions.None)
				.Select(p => p.Trim())
				.Where(p => !string.IsNullOrEmpty(p))
				.ToList();

			var sb = new StringBuilder();

			foreach (var para in paragraphs)
			{
				// Detect bullet list lines: start with -, *, • or number + dot
				var lines = para.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
				bool isBulletList = lines.All(l => Regex.IsMatch(l, @"^(\- |\* |• |\d+\.\s)"));
				if (isBulletList && lines.Count > 0)
				{
					sb.AppendLine("<ul>");
					foreach (var line in lines)
					{
						var li = Regex.Replace(line, @"^(\- |\* |• |\d+\.\s)", "").Trim();
						sb.AppendLine($"  <li>{WebUtility.HtmlEncode(li)}</li>");
					}
					sb.AppendLine("</ul>");
				}
				else
				{
					// Replace single newlines inside paragraph with <br/>
					var inner = string.Join("<br/>", lines.Select(l => WebUtility.HtmlEncode(l)));
					sb.AppendLine($"<p>{inner}</p>");
				}
			}

			return sb.ToString().Trim();
		}
	}
}
