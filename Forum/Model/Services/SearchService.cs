using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;

namespace Forum.Model.Services
{
    public class SearchService
    {

        public string Tets(string input)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python", // Убедись, что путь до python в переменной окружения PATH
                Arguments = $"\"C:\\Users\\eajli\\OneDrive\\Рабочий стол\\работы в вузе\\диплом\\Forum\\lemma.py\" \"{input}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Trim();
        }

    }
}
