using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace FormsTutor.Extensions
{
	public static class ObservableExtensions
	{
		public static IObservable<T> RetryWithDelay<T>(
			this IObservable<T> source,
            int retryCount,
			IScheduler scheduler = null)
		{
			var attempt = 0;

			return Observable.Defer(() =>
			{
                var delay = TimeSpan.FromSeconds(Math.Pow(attempt++, 2));
                var observable = attempt == 1 ? source : Observable.Timer(delay).SelectMany(_ => source);

                return observable.Select(item => item);
			})
			.Retry(retryCount)
			.SelectMany(x => Observable.Return(x));
		}
	}
}
