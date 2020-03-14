using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Utils;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.RecognitionService
{
    public interface IRecognitionService
    {
        ResponsePython RecognitionImage(string imageString);
        Task<ResponsePython> StartRecognition(int durationBeforeStartInMinutes, int durationWhileRunningInMinutes, string rtspString);

    }
    public class RecognitionService : IRecognitionService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly MyConfiguration myConfiguration;

        public RecognitionService(IServiceScopeFactory serviceScopeFactory, MyConfiguration myConfiguration)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.myConfiguration = myConfiguration;
        }

        public async Task<ResponsePython> StartRecognition(int durationBeforeStartInMinutes, int durationWhileRunningInMinutes, string rtspString)
        {
            return await RecognitionByImageVLC(durationBeforeStartInMinutes, durationWhileRunningInMinutes, rtspString);
            //await RecognitionByOpenCV(durationStartIn, durationMinutes, rtspString);
        }
        public ResponsePython RecognitionImage(string imageString)
        {
            string imagePath = FileUtils.GetTempFilePathWithExtension(".jpg");
            File.WriteAllBytes(imagePath, Convert.FromBase64String(imageString.Split(",")[1]));

            var cmd = myConfiguration.RecognitionImageBase64Path;
            var args = string.Format(@"--imagePath {0}", imagePath);
            var startInfo = GetProcessStartInfo(cmd, args);
            var responsePython = new ResponsePython();
            using (var process = Process.Start(startInfo))
            {
                responsePython.Errors = process.StandardError.ReadToEnd();
                responsePython.Results = process.StandardOutput.ReadToEnd();
            }
            return responsePython;
        }


        private ProcessStartInfo GetProcessStartInfo(string cmd, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            var pythonFullPath = myConfiguration.PythonExeFullPath;
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            startInfo.FileName = pythonFullPath;
            startInfo.Arguments = string.Format("{0} {1}", cmd, args);
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WorkingDirectory = parentDirectory + "\\" + myConfiguration.RecognitionServiceName;
            return startInfo;
        }
        private async Task<ResponsePython> RecognitionByImageVLC(int durationStartIn, int durationMinutes, string rtspString)
        {
            // Wait until start time
            await Task.Delay(1000 * 60 * durationStartIn);

            // Get start info
            var cmd = myConfiguration.RecognitionProgramPathVLC;
            var args = "";
            args += string.Format(@"--rtsp {0}", rtspString);
            var startInfo = GetProcessStartInfo(cmd, args);

            // Start process
            ResponsePython responsePython = new ResponsePython();
            using (var myProcess = Process.Start(startInfo))
            {
                await Task.Delay(1000 * 60 * durationMinutes);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                    await recordService.UpdateRecordsAfterEndSession();
                }
                myProcess.Kill();
                responsePython.Errors = myProcess.StandardError.ReadToEnd();
                responsePython.Results = myProcess.StandardOutput.ReadToEnd();
            }
            return responsePython;
        }
        private async Task<ResponsePython> RecognitionByOpenCV(int durationStartIn, int durationMinutes, string rtspString)
        {
            await Task.Delay(1000 * 60 * durationStartIn);

            // Get start info
            var cmd = myConfiguration.RecognitionProgramPathOpenCV;
            var args = "";
            args += string.Format(@"--rtsp {0}", rtspString);
            var startInfo = GetProcessStartInfo(cmd, args);

            // Start process
            ResponsePython responsePython = new ResponsePython();
            using (var myProcess = Process.Start(startInfo))
            {
                await Task.Delay(1000 * 60 * durationMinutes);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                    await recordService.UpdateRecordsAfterEndSession();
                }
                myProcess.Kill();
                responsePython.Errors = myProcess.StandardError.ReadToEnd();
                responsePython.Results = myProcess.StandardOutput.ReadToEnd();
            }
            return responsePython;
        }
    }
}
