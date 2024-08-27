# FolderSync

FolderSync is a utility that keeps a replica folder synchronized with a source folder, ensuring the replica is always an exact copy of the source.

## Features

- **One-Way Sync**: The replica folder is always updated to match the source, overwriting any changes made in the replica.
- **Real-Time Interval Adjustment**: Change the sync interval on the fly, with changes applied immediately.
- **Automatic Folder Creation**: Creates `SourceFolder` and `ReplicaFolder` if they don't exist.
- **Logging**: Records all sync activities in `log.txt`.

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
