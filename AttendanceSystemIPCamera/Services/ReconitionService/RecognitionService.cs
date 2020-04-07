using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Utils;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.RecognitionService
{
    public interface IRecognitionService
    {
        ResponsePython RecognitionImage(string imageString);
        Task<ResponsePython> StartRecognition(int durationBeforeStartInMinutes, int durationWhileRunningInMinutes, string rtspString);
        public Task<ResponsePython> StartRecognitionMultiple(string rtspString);

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
            return await RecognitionByVideo(durationBeforeStartInMinutes, durationWhileRunningInMinutes, rtspString);
        }
        public async Task<ResponsePython> StartRecognitionMultiple(string rtspString)
        {
            return await RecognitionByVideoMultiple(rtspString);
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
        private async Task<ResponsePython> RecognitionByVideo(int durationStartIn, int durationMinutes, string rtspString)
        {
            try
            {
                await Task.Delay(1000 * 60 * durationStartIn);

                // Get start info
                var cmd = myConfiguration.RecognitionProgramPathVLC;
                var args = "";

                // rtsp string of camera
                args += string.Format(@"--rtsp {0}", rtspString);

                // num of people for recognition
                args += string.Format(@" --num {0}", 1);

                // duration for recognition
                args += string.Format(@" --time {0}", 1000 * 60 * durationMinutes);

                // set flag for checking attendance
                args += string.Format(@" --attendance {0}", "True");
                var startInfo = GetProcessStartInfo(cmd, args);
                // Handle outputs and errors
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            error.AppendLine(e.Data);
                        }
                    };
                    //Start process 
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait for exit
                    var exitWait = process.WaitForExit(1000 * 60 * (durationMinutes + 1));
                    if (exitWait)
                    {
                        // ok
                        if (process.ExitCode == 0)
                        {
                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                                await recordService.UpdateRecordsAfterEndSession();
                            }
                        }
                        // exception
                        else if (process.ExitCode == 1)
                        {
                            throw new AppException(HttpStatusCode.InternalServerError, error.ToString());
                        }
                    }
                    else
                    {
                        process.Kill();
                        throw new AppException(HttpStatusCode.InternalServerError, error.ToString());
                    }
                }
                return new ResponsePython()
                {
                    Errors = error.ToString(),
                    Results = output.ToString()
                };

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
                    sessionRepository.RemoveActiveSession();
                }
                throw new AppException(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private async Task<ResponsePython> RecognitionByVideoMultiple(string rtspString)
        {
            try
            {
                // Get start info
                var cmd = myConfiguration.RecognitionProgramMultiplePath;
                var args = "";

                // rtsp string of camera
                args += string.Format(@"--rtsp {0}", rtspString);

                // set flag for checking attendance
                args += string.Format(@" --attendance {0}", "True");
                var startInfo = GetProcessStartInfo(cmd, args);
                // Handle outputs and errors
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            error.AppendLine(e.Data);
                        }
                    };
                    //Start process 
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait for exit
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                        await recordService.UpdateRecordsAfterEndSession();
                    }
                    // exception
                    else if (process.ExitCode == 1)
                    {
                        throw new AppException(HttpStatusCode.InternalServerError, error.ToString());
                    }
                }
                return new ResponsePython()
                {
                    Errors = error.ToString(),
                    Results = output.ToString()
                };
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
                    sessionRepository.RemoveActiveSession();
                }
                throw new AppException(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
