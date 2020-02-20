
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FolderListener.Domain
{
	public class ChannelsQueueMultiThreads
	{
		private readonly ChannelWriter<string> _writer;

		public ChannelsQueueMultiThreads(int threads)
		{
			var channel = Channel.CreateUnbounded<string>();
			var reader = channel.Reader;
			_writer = channel.Writer;
			for (var i = 0; i < threads; i++)
			{
				Task.Factory.StartNew(() =>
				{
					while (true)
					{
						reader.TryRead(out var path);
						if (!string.IsNullOrEmpty(path))
						{
							File.WriteAllText(path, File.ReadAllLines(path).Sum(s => s.Length).ToString());
							Console.WriteLine($"File:{path}, thread: { Thread.CurrentThread.ManagedThreadId}");
						}
					}
				}, CancellationToken.None);
			}
		}

		public void Enqueue(string job)
		{
			_writer.WriteAsync(job).GetAwaiter().GetResult();
		}

		public void Stop()
		{
			_writer.Complete();
		}



	}
}