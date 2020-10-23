using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		const string AddressFormat = "https://dummyimage.com/200x200/{1}/fff.jpg&text=I{0}";
		const int Count = 100;

		static readonly Random Random = new Random();

		SemaphoreSlim _sem;

		public Form1()
		{
			InitializeComponent();
			_sem = new SemaphoreSlim(20, 20);
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			//Block(() => ReadAndWriteAsync());
			//button1.Text = "Pope Francis";
			await ShowImages();
			button1.Text = "Done";
		}

		async Task ShowImages()
		{
			flowLayoutPanel1.SuspendLayout();
			flowLayoutPanel1.Controls.Clear();
			for (var i = 0; i < Count; i++)
			{
				flowLayoutPanel1.Controls.Add(new PictureBox() { Size = new Size(200, 200) });
			}
			flowLayoutPanel1.ResumeLayout();
			var tasks = new Task[Count];
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = LoadAndShowImage(i);
			}
			await Task.WhenAll(tasks);
		}

		async Task LoadAndShowImage(int index)
		{
			await _sem.WaitAsync();
			try
			{
				var rgb = $"{Random.Next(0, 255):X}{Random.Next(0, 255):X}{Random.Next(0, 255):X}";
				var data = await LoadImage(string.Format(AddressFormat, index, rgb));
				(flowLayoutPanel1.Controls[index] as PictureBox).Image = Image.FromStream(new MemoryStream(data));
			}
			finally
			{
				_sem.Release();
			}
		}

		static async Task<byte[]> LoadImage(string address)
		{
			await Task.Delay(Random.Next(500, 5000)).ConfigureAwait(false);
			using (var client = new HttpClient())
			{
				var data = await client.GetByteArrayAsync(address).ConfigureAwait(false);
				return data;
			}
		}

		//static void Block(Func<Task> t)
		//{
		//	Task.Run(t).GetAwaiter().GetResult();
		//}

		//async Task ReadAndWriteAsync()
		//{
		//	await Task.Delay(5000);
		//	var data = await Read().ConfigureAwait(false);
		//	await Write(data).ConfigureAwait(false);
		//}

		//static async Task<byte[]> Read()
		//{
		//	return await Task.FromResult(new byte[20]);
		//}

		//static async Task Write(byte[] data)
		//{
		//	await Task.CompletedTask;
		//}
	}
}
