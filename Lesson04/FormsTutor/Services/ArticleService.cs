using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormsTutor.Models;

namespace FormsTutor.Services
{
	public interface IArticleService
	{
		Task<IEnumerable<Article>> Get();
	}

	public class ArticleService : IArticleService
	{
		int _index = 1;

		public async Task<IEnumerable<Article>> Get()
		{
			await Task.Delay(2000);
			return new List<Article> { new Article { Title = $"Article {_index++}" }, new Article { Title = $"Article {_index++}" } };
		}
	}
}
