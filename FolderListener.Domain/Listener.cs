using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace FolderListener.Domain
{
	public class Listener
	{
		private readonly ActionBlock<string> _actionBlock;

		public Listener()
		{
			_actionBlock = new ActionBlock<string>(ProcessFile, new ExecutionDataflowBlockOptions
			{
				MaxDegreeOfParallelism = 4
			});
		}
		public void Start(string sourceDirectoryPath, string destinationDirectoryPath)
		{
			var watcher = new FileSystemWatcher
			{
				Path = @"C:\Work\test",
				IncludeSubdirectories = true,
				Filter = "*.txt"
			};


			// Add event handlers.
			watcher.Created += OnAdded;


			// Begin watching.
			watcher.EnableRaisingEvents = true;
		}


		public void Stop()
		{
			
		}

		private void OnAdded(object source, FileSystemEventArgs e)
		{
			_actionBlock.Post(e.FullPath);
			Console.WriteLine(e.FullPath);
		}

		private static void ProcessFile(string path)
		{
			try
			{
				if (path == null ) return;
				using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var reader = new StreamReader(stream))
				{
					var fileName = Path.GetFileName(path);
					var charsLength = reader.ReadToEnd().Length;
					File.WriteAllText(Path.Combine(@"C:\Work\test1\", fileName), charsLength.ToString());
				}
				
				

				Console.WriteLine($"File:{path}, thread: { Thread.CurrentThread.ManagedThreadId}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}


		}


		private bool IsFileLocked(FileInfo file)
		{
			FileStream stream = null;

			try
			{
				stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException)
			{
				return true;
			}
			finally
			{
				stream?.Close();
			}

			//file is not locked
			return false;
		}
	}
}