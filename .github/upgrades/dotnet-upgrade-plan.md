# .NET8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET8.0 upgrade.
3. Upgrade TagPlugin.Example\TagPlugin.Example.csproj

## Settings

### Excluded projects

| Project name | Description |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

_No NuGet package changes required._

### Project upgrade details

#### TagPlugin.Example\TagPlugin.Example.csproj modifications

Project properties changes:
 - Target framework should be changed from `net451` to `net8.0-windows`
NuGet packages changes:
 - No NuGet package changes required.
