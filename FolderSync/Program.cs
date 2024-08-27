using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FolderSync
{
    class Program
    {
        static void Main(string[] args)
        {

            // Get the directory where the executable is located
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Set default folder paths
            string sourcePath = Path.Combine(baseDirectory, "SourceFolder");
            string replicaPath = Path.Combine(baseDirectory, "ReplicaFolder");

            // Log file path
            string logFilePath = Path.Combine(baseDirectory, "log.txt");

            // Ensure the folders exist and notify where they are created
            EnsureDirectoryExists(sourcePath, "SourceFolder", logFilePath);
            EnsureDirectoryExists(replicaPath, "ReplicaFolder", logFilePath);

            // Log the program initiation
            Log("\n[ FolderSync initiated ]", logFilePath);

            int interval = 10; // in seconds
            while (true)
            {
                SyncFolders(sourcePath, replicaPath, logFilePath);
                Thread.Sleep(interval * 1000); // Wait for the specified interval
            }
        }

        static bool EnsureDirectoryExists(string path, string folderName, string logFilePath)
        {
            bool created = false;
            string CheckFoldersLog;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                CheckFoldersLog = $"Created {folderName} in the program directory: {path}";
                Log(CheckFoldersLog, logFilePath);
                created = true;
            }
            else
            {
              Console.WriteLine($"{folderName} already exists at: {path}");
            }
            return created;
        }

        static void SyncFolders(string source, string replica, string logFilePath)
        {
            bool modificationsMade = false;
            string logEntry = $"\n{DateTime.Now}: ";

            // Copy new/modified files from source to replica
            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                string replicaFile = file.Replace(source, replica);
                string replicaDir = Path.GetDirectoryName(replicaFile);

                if (!Directory.Exists(replicaDir))
                {
                    Directory.CreateDirectory(replicaDir);
                    logEntry += $"\nCreated directory: {replicaDir}";
                    modificationsMade = true;
                }

                if (!File.Exists(replicaFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(replicaFile))
                {
                    File.Copy(file, replicaFile, true);
                    logEntry += $"\nCopied file: {file} to {replicaFile}";
                    modificationsMade = true;
                }
            }

            // Remove files from replica that no longer exist in source
            foreach (string file in Directory.GetFiles(replica, "*", SearchOption.AllDirectories))
            {
                string sourceFile = file.Replace(replica, source);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(file);
                    logEntry += $"\nDeleted file: {file}";
                    modificationsMade = true;
                }
            }

            // Remove directories from replica that no longer exist in source
            foreach (string dir in Directory.GetDirectories(replica, "*", SearchOption.AllDirectories))
            {
                string sourceDir = dir.Replace(replica, source);

                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(dir, true);
                    logEntry += $"\nDeleted directory: {dir}";
                    modificationsMade = true;
                }
            }

            // Log the modifications if any were made
            if (modificationsMade)
            {
                Log(logEntry, logFilePath);
            }
        }

        static void Log(string message, string logFilePath)
        {
            Console.WriteLine(message);
            File.AppendAllText(logFilePath, message + Environment.NewLine);
        }
    }
}
