using System;
using System.Linq;
using System.Reactive.Linq;
using HtmlAgilityPack;

namespace FormsTutor.Services
{
	public interface IHtmlParserService
	{
		string Parse(string content, string baseUrl);
	}

    public class HtmlParserService : IHtmlParserService
    {
        public string Parse(string content, string baseUrl)
        {
            var document = new HtmlDocument();

            document.LoadHtml(content);

            ReplaceRelativeUrls(document, baseUrl);

            RemoveRedundantElements(document);

            return document.DocumentNode.OuterHtml;
        }

        static void RemoveRedundantElements(HtmlDocument document)
        {
            var body = document.DocumentNode.Descendants().First(x => x.Name == "body").ChildNodes.FirstOrDefault(x => x.Attributes.Any(y => y.Name == "class" && y.Value.Contains("site-wrapper")));

            if (body == null) return;

            body.RemoveChild(body.ChildNodes.FirstOrDefault(x => x.Name == "header"));
            body.RemoveChild(body.ChildNodes.FirstOrDefault(x => x.Name == "aside"));
            body.RemoveChild(body.ChildNodes.FirstOrDefault(x => x.Name == "footer"));
        }

        static void ReplaceRelativeUrls(HtmlDocument document, string baseUrl)
        {
            document.DocumentNode
                    .Descendants("link")
                    .ToObservable()
                    .Where(x => x.Attributes.Any(y => y.Name == "rel" && y.Value == "stylesheet") &&
                                x.Attributes.Any(y => y.Name == "href" && !y.Value.StartsWith("http", StringComparison.CurrentCulture)))
                    .Subscribe(x => ReplaceAttributeValue(x.Attributes["href"], v => $"{baseUrl}{v}"));

            document.DocumentNode
                    .Descendants("img")
                    .ToObservable()
                    .Where(x => x.Attributes.Any(y => y.Name == "src" && !y.Value.StartsWith("http", StringComparison.CurrentCulture)))
                    .Subscribe(x => ReplaceAttributeValue(x.Attributes["src"], v => $"{baseUrl}{v}"));
        }

        static void ReplaceAttributeValue(HtmlAttribute attribute, Func<string, string> replaceValueWith)
        {
            attribute.Value = replaceValueWith(attribute.Value);
        }
    }
}
