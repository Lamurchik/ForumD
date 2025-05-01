using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.Text.Json;
using System.Text;

namespace Forum.Model.Services
{
    public class SearchService
    {
        #region лематизация строки запроса 
        string path = $"C:\\Users\\eajli\\OneDrive\\Рабочий стол\\работы в вузе\\диплом\\Forum\\lemma.py";
        //string path = @"C:\Users\eajli\PycharmProjects\PythonProject1\main.py";
        string fileName = @"C:/Users/eajli/PycharmProjects/PythonProject1/.venv/Scripts/python.exe";
        public async Task<List<string>> ProcessTextAsync(string input)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = EscapeArguments(
                    path,
                    input
                ),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                Environment =
            {
                ["PYTHONIOENCODING"] = "utf-8",
                ["PYTHONLEGACYWINDOWSSTDIO"] = "utf-8"
            }
            };

            using (var process = new Process { StartInfo = psi })
            {
                var output = new StringBuilder();
                process.OutputDataReceived += (s, e) => {
                    if (!string.IsNullOrEmpty(e.Data)) output.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                await process.WaitForExitAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                try
                {
                    return JsonSerializer.Deserialize<List<string>>(
                        output.ToString(),
                        options
                    ) ?? new List<string>();
                }
                catch
                {
                    return new List<string> { output.ToString() };
                }
            }
        }
        private string EscapeArguments(params string[] args)
        {
            return string.Join(" ", args.Select(a => $"\"{a.Replace("\"", "\\\"")}\""));
        }
        #endregion  




    }
}
