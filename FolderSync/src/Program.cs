using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FolderSync
{
    class Program
    {
        static int interval = 10; // Default interval in seconds
        static bool running = true;
        static ManualResetEventSlim intervalChangedEvent = new ManualResetEventSlim(false);

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

            Console.WriteLine($"\n>> Default sync interval: {interval} seconds.");

            // Start the thread that listens for interval changes
            Thread inputThread = new Thread(new ThreadStart(WaitForUserInput));
            inputThread.Start();

            // Main synchronization loop
            while (running)
            {
                SyncFolders(sourcePath, replicaPath, logFilePath);
                WaitForInterval();
            }

            inputThread.Join(); // Wait for the input thread to finish before exiting
        }

        static bool EnsureDirectoryExists(string path, string folderName, string logFilePath)
        {
            bool created = false;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                string message = $"Created {folderName} in the program directory: {path}";
                Log(message, logFilePath);
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
            foreach (string sourceFile in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                string replicaFile = sourceFile.Replace(source, replica);
                string replicaDir = Path.GetDirectoryName(replicaFile);

                if (!Directory.Exists(replicaDir))
                {
                    Directory.CreateDirectory(replicaDir);
                    logEntry += $"\nCreated directory in replica: {replicaDir}";
                    modificationsMade = true;
                }

                // Copy the file from source to replica if it doesn't exist, or if the source file is newer, or if the contents differ
                if (!File.Exists(replicaFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(replicaFile) || !FilesAreEqual(sourceFile, replicaFile))
                {
                    File.Copy(sourceFile, replicaFile, true);
                    logEntry += $"\nCopied file from source to replica: {sourceFile} to {replicaFile}";
                    modificationsMade = true;
                }
            }

            // Remove files from replica that no longer exist in source
            foreach (string replicaFile in Directory.GetFiles(replica, "*", SearchOption.AllDirectories))
            {
                string sourceFile = replicaFile.Replace(replica, source);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(replicaFile);
                    logEntry += $"\nDeleted file from replica (no longer in source): {replicaFile}";
                    modificationsMade = true;
                }
            }

            // Remove directories from replica that no longer exist in source
            foreach (string replicaDir in Directory.GetDirectories(replica, "*", SearchOption.AllDirectories))
            {
                string sourceDir = replicaDir.Replace(replica, source);

                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(replicaDir, true);
                    logEntry += $"\nDeleted directory from replica (no longer in source): {replicaDir}";
                    modificationsMade = true;
                }
            }

            // Log the modifications if any were made
            if (modificationsMade)
            {
                Log(logEntry, logFilePath);
            }
        }

        static bool FilesAreEqual(string filePath1, string filePath2)
        {
            byte[] file1Bytes = File.ReadAllBytes(filePath1);
            byte[] file2Bytes = File.ReadAllBytes(filePath2);
            //makes sure that files with the same name modified at the replica folder will be substituted by the one in the source folder
            return file1Bytes.SequenceEqual(file2Bytes);
        }


        static void Log(string message, string logFilePath)
        {
            Console.WriteLine(message);
            File.AppendAllText(logFilePath, message + Environment.NewLine);
        }

        static void WaitForUserInput()
        {
            while (running)
            {
                Console.WriteLine("Enter a new interval in seconds, or type 'exit' to stop the program:");
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    running = false;
                    intervalChangedEvent.Set(); // Signal the event to break the wait
                    break;
                }

                if (int.TryParse(input, out int newInterval) && newInterval > 0)
                {
                    interval = newInterval;
                    Console.WriteLine($"\n>> Sync interval updated to: {interval} seconds.");
                    intervalChangedEvent.Set(); // Signal the event to break the wait
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number of seconds.");
                }
            }
        }

        static void WaitForInterval()
        {
            intervalChangedEvent.Reset(); // Reset the event before starting to wait
            bool signaled = intervalChangedEvent.Wait(TimeSpan.FromSeconds(interval));
            // If signaled is true, the interval was changed; otherwise, the full interval passed.
        }
    }
}