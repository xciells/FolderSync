# FolderSync

FolderSync is a utility that keeps a replica folder synchronized with a source folder, ensuring the replica is always an exact copy of the source.

## Features

- **One-Way Sync**: The replica folder is always updated to match the source, overwriting any changes made in the replica.
- **Real-Time Interval Adjustment**: Change the sync interval on the fly, with changes applied immediately.
- **Automatic Folder Creation**: Creates `SourceFolder` and `ReplicaFolder` if they don't exist.
- **Logging**: Records all sync activities in `log.txt`.

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) (if building from source)

### Installation

You can either download the latest release or build the project from source.

#### Option 1: Download the Latest Release

1. Go to the [Releases](https://github.com/xciells/FolderSync/releases) section of this repository.
2. Download the latest release ZIP file (`FolderSync_v1.0.zip`).
3. Extract the ZIP file to your desired location.
4. Run the executable inside the extracted folder to start the application.

#### Option 2: Build from Source

1. **Clone the repository**:
   ```bash
   git clone https://github.com/xciells/FolderSync.git
   cd FolderSync
1. **Build and run the project**:
   dotnet build --configuration Release
   dotnet run

