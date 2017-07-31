using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FormsTutor.Models;
using Newtonsoft.Json;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace FormsTutor.Services
{
	public interface IArticleService
	{
        IObservable<IEnumerable<Article>> Get();
	}

	public class ArticleService : IArticleService
	{
        public IObservable<IEnumerable<Article>> Get()
		{
            var url = $"{Configuration.ApiBaseUrl}Articles.json";
            return Observable.FromAsync(() => new HttpClient().GetAsync(url))
                             .SelectMany(async x => { x.EnsureSuccessStatusCode(); return await x.Content.ReadAsStringAsync(); })
                             .Select(content => JsonConvert.DeserializeObject<Article[]>(content));
		}
	}
}
