using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace FolderListener.Domain
{
	public class Listener: IDisposable
	{
		private ActionBlock<string> _actionBlock;
		private FileSystemWatcher _watcher;

		/// <summary>
		/// Запуск наблюдения за директорией 
		/// </summary>
		/// <param name="sourceDirectoryPath">Путь к папке с исходными документами в формате TXT</param>
		/// <param name="destinationDirectoryPath">Путь к папке для результатов</param>
		/// <param name="maxDegreeOfParallelism">Максимально допумтимое количество потоков для обработки файлов</param>
		public void Start(string sourceDirectoryPath, string destinationDirectoryPath, int maxDegreeOfParallelism = 4)
		{
			_actionBlock = new ActionBlock<string>(p => ProcessFile(p, destinationDirectoryPath), new ExecutionDataflowBlockOptions
			{
				MaxDegreeOfParallelism = maxDegreeOfParallelism,
				
			});


			_watcher = new FileSystemWatcher
			{
				Path = sourceDirectoryPath,
				Filter = "*.txt"
			};

			_watcher.Created += OnAdded;
			_watcher.EnableRaisingEvents = true;
		}


		/// <summary>
		/// Метод обработки события появления нового файла. 
		/// </summary>
		private void OnAdded(object source, FileSystemEventArgs e)
		{
			// добавляем путь к файлу в очередь
			_actionBlock.Post(e.FullPath);
			Console.WriteLine($"File:{e.FullPath} added");
		}

		/// <summary>
		/// Подсчет символов и сохранение файла в директорию результатов
		/// </summary>
		/// <param name="filePath">Путь к исходному файлу</param>
		/// <param name="destinationDirectoryPath">Путь к папке с результатами</param>
		/// <param name="numberOfRetries">Количество попыток для прочтения файла</param>
		/// <param name="delayOnRetry">Время ожидания перед повторной попыткой, при ошибке прочтения файла</param>
		private static void ProcessFile(string filePath, string destinationDirectoryPath, int numberOfRetries = 3, int delayOnRetry = 1000)
		{
			Console.WriteLine($"File:{filePath} processing started");
			Thread.Sleep(5000);

			for (var i = 1; i <= numberOfRetries; ++i)
			{
				try
				{
					using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					using (var reader = new StreamReader(stream))
					{
						var fileName = Path.GetFileName(filePath);
						var charsLength = reader.ReadToEnd().Length;
						File.WriteAllText(Path.Combine(destinationDirectoryPath, fileName), charsLength.ToString());
						Console.WriteLine($"File:{filePath} was processed successfully");
					}
					break;
				}
				catch (Exception) 
				{
					Thread.Sleep(delayOnRetry);
					if (numberOfRetries < i)
					{
						Console.WriteLine($"File:{filePath} processing failed");
					}
				}
			}
		}

		public void Dispose()
		{
			_watcher?.Dispose();
			_actionBlock?.Complete();

			_actionBlock?.Completion.Wait();
		}
	}
}