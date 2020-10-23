using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace NETDDAsync
{
	class Program
	{
		static async Task Main(string[] args)
		{
#if false
			//for (var i = 0; i < 5000; i++)
			//{
			//	var t = new Thread(o =>
			//	{
			//		//Console.WriteLine("Test");
			//		Thread.Sleep(-1);
			//	});
			//	t.Start(null);
			//	Console.WriteLine(i);
			//}

			////for (var i = 0; i < 4000; i++)
			//{
			//	ThreadPool.QueueUserWorkItem(o =>
			//	{
			//		//Thread.Sleep(-1);
			//		Console.WriteLine("Test");
			//	});
			//}

			//var arr = Enumerable.Range(1, 64).ToArray();
			//var sw = Stopwatch.StartNew();
			//var tasks = new Task[arr.Length];
			//for (var i = 0; i < arr.Length; i++)
			//{
			//	var j = i;
			//	tasks[j] = Task.Run(() => arr[j] = Sum(arr[j], arr[j]));
			//}
			//Task.WaitAll(tasks);
			//Console.WriteLine(sw.Elapsed);
#endif

			ServicePointManager.DefaultConnectionLimit = int.MaxValue;

			//for (var i = 0; i < 1000; i++)
			//{
			//	Console.WriteLine($"Starting: {i}");
			//	var request = WebRequest.CreateHttp("https://www.tabsoverspaces.com");
			//	request.BeginGetResponse(ar =>
			//	{
			//		//using (var response = request.EndGetResponse(ar))
			//		//{
			//		//	Console.WriteLine($"{i}: {response.ContentLength}");
			//		//}
			//		var response = request.EndGetResponse(ar);
			//		var stream = response.GetResponseStream();
			//		ReadStream(stream,
			//			() =>
			//			{
			//				Console.WriteLine("Done");
			//				stream.Dispose();
			//				response.Dispose();
			//			},
			//			ex =>
			//			{
			//				Console.WriteLine($"ERROR: {ex}");
			//				stream.Dispose();
			//				response.Dispose();
			//			});
			//	}, null);
			//}

			//Console.ReadLine();
			//var tasks = new Task[1000];
			//for (var i = 0; i < 1000; i++)
			//{
			//	tasks[i] = DoRequestAsync(i);
			//}
			//await Task.WhenAll(tasks);
			//Console.WriteLine("All done.");

			//var request = WebRequest.CreateHttp("https://www.tabsoverspaces.com");
			//var data = await GetResponseAsync2(request);

			//var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
			//Console.CancelKeyPress += (sender, e) => tcs.SetResult(null);
			//await tcs.Task;

			//await using (var foo = new Foo())
			//{

			//}

			//await foreach (var item in new Foo())
			//{

			//}

			Console.ReadLine();
		}

		static async Task Bar()
		{
			Console.WriteLine("A");
			await Task.Run(() => 10);
			Console.WriteLine("B");
		}

		class Foo : IAsyncDisposable, IAsyncEnumerable<Foo>
		{
			public ValueTask DisposeAsync()
			{
				throw new NotImplementedException();
			}

			public IAsyncEnumerator<Foo> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			{
				throw new NotImplementedException();
			}
		}

		static Task<WebResponse> GetResponseAsync2(HttpWebRequest request)
		{
			return Task.Factory.FromAsync(
				request.BeginGetResponse,
				request.EndGetResponse,
				null);
		}

		static async Task DoRequestAsync(int i)
		{
			var request = WebRequest.CreateHttp("https://www.tabsoverspaces.com");
			using (var response = await request.GetResponseAsync())
			{
				using (var stream = response.GetResponseStream())
				{
					var buffer = new byte[32 * 1024];
					var length = 0;
					while (true)
					{
						var read = await stream.ReadAsync(buffer, 0, buffer.Length);
						if (read == 0)
							break;
						length += read;
					}
					Console.WriteLine($"{i}: {response.ContentLength} | {length}");
				}
			}
		}

		static void ReadStream(Stream stream, Action onDone, Action<Exception> onError)
		{
			var buffer = new byte[32 * 1024];
			stream.BeginRead(buffer, 0, buffer.Length, ar =>
			{
				int read;
				try
				{
					read = stream.EndRead(ar);
				}
				catch (Exception ex)
				{
					onError(ex);
					return;
				}
				if (read == 0)
				{
					onDone();
					return;
				}
				ReadStream(stream, onDone, onError);
			}, null);
		}

		async Task Test()
		{
			Console.WriteLine("A");
			await Task.CompletedTask;
			Console.WriteLine("A");
			await Task.CompletedTask;
			Console.WriteLine("A");
			await Task.CompletedTask;
			Console.WriteLine("A");
		}

		static int Sum(int a, int b)
		{
			Thread.SpinWait(2999999);
			return a + b;
		}
	}

	static class Ext
	{
		public static Task WaitForExitAsync(this Process p)
		{
			p.EnableRaisingEvents = true;
			var tcs = new TaskCompletionSource<object>();
			p.Exited += (s, e) => tcs.TrySetResult(null);
			if (p.HasExited)
				tcs.TrySetResult(null);
			return tcs.Task;
		}
	}
}
