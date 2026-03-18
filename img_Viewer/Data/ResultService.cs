using img_Viewer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static img_Viewer.Models.TagResult;

namespace img_Viewer.Data
{
    public static class ResultService
    {
        public static async Task<string> RunPythonScript(
        string scriptPath,
        string venvPython,
        string folderPath,
        string jsonPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = venvPython, 
                Arguments = $"\"{scriptPath}\" \"{folderPath}\" \"{jsonPath}\"",
                WorkingDirectory = Path.GetDirectoryName(scriptPath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process();
            process.StartInfo = psi;

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return output;
        }
    }
    
}
