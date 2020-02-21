using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace TestFilesProducer
{
	/// <summary>
	/// Програма для генерации тестовых документов
	/// </summary>
	public class Program
	{
		public static void Main(string[] args)
		{
			var t = new Timer(GenerateFile, null, 0, 1000);
			Console.ReadKey();

		}

		/// <summary>
		/// Генерация тестовых документов
		/// </summary>
		/// <param name="state"></param>
		private static void GenerateFile(object state)
		{
			var random = new Random();

			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var randomString = new string(Enumerable.Repeat(chars, random.Next(100))
				.Select(s => s[random.Next(s.Length)]).ToArray());

			File.WriteAllText(Path.Combine(@"C:\Work\test\", $"{DateTime.Now.Ticks}.txt"), randomString);
		}
	}
}
