﻿using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Services.RecordService;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.RecognitionService
{
    public class RecognitionService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly MyConfiguration myConfiguration;

        public RecognitionService(IServiceScopeFactory serviceScopeFactory, MyConfiguration myConfiguration)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.myConfiguration = myConfiguration;
        }

        public async Task StartRecognition(double timeDifferenceMilliseconds, int durationMinutes, string rtspString)
        {
            // Wait until start time
            await Task.Delay(Convert.ToInt32(timeDifferenceMilliseconds));

            // Start process on python
            ProcessStartInfo startInfo = new ProcessStartInfo();
            var pythonFullPath = myConfiguration.PythonExeFullPath;
            var currentDirectory = Environment.CurrentDirectory;
            var cmd = string.Format(@"{0}\{1}", currentDirectory, myConfiguration.RecognizerProgramPath);
            var args = "";
            args += string.Format(@"--recognizer {0}\{1}", currentDirectory, myConfiguration.RecognizerPath);
            args += string.Format(@" --le {0}\{1}", currentDirectory, myConfiguration.LePath);
            startInfo.FileName = pythonFullPath;
            startInfo.Arguments = string.Format("{0} {1}", cmd, args);
            startInfo.UseShellExecute = true;
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = false;
            Process myProcess = new Process();
            myProcess.StartInfo = startInfo;
            myProcess.Start();

            // Wait until done recognition process and update remain record
            await Task.Delay(1000 * 60 * durationMinutes);
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();
                await recordService.UpdateRecordsAfterEndSession();
            }
            myProcess.Kill();
            
        }
    }
}