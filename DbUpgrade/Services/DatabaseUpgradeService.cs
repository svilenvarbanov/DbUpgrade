using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbUpgrade.Abstractions;
using DbUpgrade.EF;
using DbUpgrade.Helpers;
using DbUpgrade.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DbUpgrade.Services
{
    public class DatabaseUpgradeService : IDatabaseUpgradeService
    {
        private readonly DbUpConfiguration _dbUpSettings;
        private readonly DbUpVersionContext _dbContext;
        private readonly ILogger _logger;
        private readonly IGitRepoService _gitRepoService;
        private DbVersion _dbVersionEntity;

        public DatabaseUpgradeService(
            DbUpConfiguration dbUpSettings,
            DbUpVersionContext dbContext,
            ILogger<DatabaseUpgradeService> logger,
            IGitRepoService gitRepoService)
        {
            _dbUpSettings = dbUpSettings;
            _dbContext = dbContext;
            _logger = logger;
            _gitRepoService = gitRepoService;
        }

        public void UpgradeDatabase()
        {
            _logger.Info("Starting database upgrade. Connection: {connectionString}", _dbUpSettings.ConnectionString);
            _dbVersionEntity =
                _dbContext.Versions.SingleOrDefault(v => v.Module == _dbUpSettings.Module && v.IsActive);

            if (_dbVersionEntity == null)
            {
                _logger.Error("Couldn't find module: {Module}", _dbUpSettings.Module);

                return;
            }

            if (_dbUpSettings.CheckoutLastRepoVersion)
            {
                _gitRepoService.Pull();
            }

            var currentVersion = new DbVersionModel(_dbVersionEntity.Version);
            var scriptFolders = EnumerateDatabaseUpgradesVersionsFolders()
                .Where(fn => new DbVersionModel(fn) > currentVersion)
                .ToList();

            if (scriptFolders.Count == 0)
            {
                _logger.Info("CURRENT DATABASE VERSION IS UP TO DATE.");
            }
            else
            {
                scriptFolders.ForEach(s => RunScriptsInFolder(_dbUpSettings.ScriptsRootPath, s));
            }
        }

        private IEnumerable<string> EnumerateDatabaseUpgradesVersionsFolders()
        {
            var folderInfo = new DirectoryInfo(_dbUpSettings.ScriptsRootPath);

            if (!folderInfo.Exists)
            {
                _logger.Error("Can find upgrade scripts folder: {ScriptsPath}", args: _dbUpSettings.ScriptsRootPath);

                throw new DirectoryNotFoundException(_dbUpSettings.ScriptsRootPath);
            }

            foreach (var subDir in folderInfo.GetDirectories())
            {
                yield return subDir.Name;
            }
        }

        private void RunScriptsInFolder(string scriptsRootPath, string dbVersionScript)
        {
            var folderInfo = new DirectoryInfo(scriptsRootPath + "\\" + dbVersionScript);
            var scripts = folderInfo.GetFiles("*.sql");
            foreach (var script in scripts)
            {
                try
                {
                    _logger.Info("RUNNING SCRIPT: {script} IN FOLDER: {folder}", script.Name, scriptsRootPath + "\\" + dbVersionScript);
                    string scriptText = File.ReadAllText(script.FullName);

                    var sqlConnection = new SqlConnection(_dbUpSettings.ConnectionString);
                    var server = new Server(new ServerConnection(sqlConnection));
                    int result = server.ConnectionContext.ExecuteNonQuery(scriptText);

                    _logger.Info("EXECUTION SUCCESSFUL. {count} RECORDS MODIFIED.", result > 1 ? result.ToString() : "No");
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error executing script in folder.");

                    throw;
                }
            }

            _dbVersionEntity.Version = new DbVersionModel(dbVersionScript).ToString();
            _dbContext.SaveChanges();
        }
    }
}
