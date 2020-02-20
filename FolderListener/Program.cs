using FolderListener.Domain;

namespace FolderListener.Console
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var listener = new Listener();
			listener.Start("", "");
			System.Console.ReadKey();
			listener.Stop();

		}
	}
}
