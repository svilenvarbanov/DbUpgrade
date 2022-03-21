using System;
using System.Diagnostics;
using System.IO;
using DbUpgrade.Abstractions;
using DbUpgrade.Helpers;
using DbUpgrade.Models;
using Microsoft.Extensions.Logging;

namespace DbUpgrade.Services
{
    public class GitRepoService : IGitRepoService, IDisposable
    {
        private bool _disposed;
        private readonly Process _gitProcess;
        private readonly string _gitRepoRoot;
        private readonly ILogger _logger;

        public GitRepoService(DbUpConfiguration dbUpSettings, ILogger<GitRepoService> logger)
        {
            _gitRepoRoot = Directory.Exists(dbUpSettings.ScriptsRootPath + "..\\..\\")
                ? dbUpSettings.ScriptsRootPath + "..\\..\\"
                : Environment.CurrentDirectory;
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "git.exe",
                CreateNoWindow = true,
                WorkingDirectory = _gitRepoRoot
            };

            _gitProcess = new Process();
            _gitProcess.StartInfo = processInfo;
            _logger = logger;
        }

        public void Pull()
        {
            if (IsGitRepository)
            {
                var result = RunCommand("pull");
            }
            else
            {
                _logger.Error("{GitRepoRoot} is not a valid Git repo.", _gitRepoRoot);
            }
        }

        private bool IsGitRepository
        {
            get
            {
                var cmd = RunCommand("log -1");

                return !String.IsNullOrWhiteSpace(cmd);
            }
        }

        private string RunCommand(string args)
        {
            _gitProcess.StartInfo.Arguments = args;
            _gitProcess.Start();
            string output = _gitProcess.StandardOutput.ReadToEnd()
                .Trim();
            _gitProcess.WaitForExit();

            return output;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _gitProcess.Dispose();
            }
        }
    }
}
