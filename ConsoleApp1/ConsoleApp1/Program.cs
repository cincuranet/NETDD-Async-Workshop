using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace ConsoleApp1
{
	class Program
	{
		static async Task Main(string[] args)
		{
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

			//for (var i = 0; i < 4000; i++)
			//{
			//	ThreadPool.QueueUserWorkItem(o =>
			//	{
			//		Thread.Sleep(-1);
			//	}); 
			//}

			var tasks = new Task[1];
			for (var i = 0; i < 1; i++)
			{
				tasks[i] = DoRequestAsync(i);
			}
			await Task.WhenAll(tasks);
			Console.WriteLine("All done.");

			Console.ReadLine();
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
	}
}
