using System;
using System.IO;
using FolderListener.Domain;

namespace FolderListener.ConsoleApp
{
	public class Program
	{
		private static readonly Listener Listener = new Listener();
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;


			// запрашиваем путь к папке с исходными документами в формате TXT
			var sourcePath = AskValidPath("enter the path of directory for listening:");

			// запрашиваем путь к папке для результатов
			var destinationPath = AskValidPath("enter the path of directory for results saving:");


			// запуск наблюдения за папкой
			Listener.Start(sourcePath, destinationPath);
			Console.WriteLine("listener started. press any key to stop");
			Console.ReadKey();

		}

		private static string AskValidPath(string message)
		{
			Console.WriteLine(message);
			bool pathExists;
			string path;
			do
			{
				path = Console.ReadLine();
				pathExists = Directory.Exists(path);
				if (!pathExists)
				{
					Console.WriteLine("directory does not exists. Please enter valid path:");
				}
			} while (!pathExists);

			return path;
		}


		/// <summary>
		/// Метод для обработки события о завершении
		/// </summary>
		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			Console.WriteLine("listener stopped. Waiting for the end of processing.");
			Listener.Dispose();
		}
	}
}
