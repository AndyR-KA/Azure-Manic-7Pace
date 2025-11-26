# .NET8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET8.0 upgrade.
3. Upgrade HttpExtensions\HttpExtensions.csproj
4. Upgrade WorkItemServices\WorkItemServices.csproj
5. Upgrade TimeTrackingService\TimeTrackingService.csproj
6. Upgrade WorkItemService.Demo\WorkItemService.Demo.csproj

## Settings

### Excluded projects

| Project name | Description |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name | Current Version | New Version | Description |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Newtonsoft.Json |13.0.1 |13.0.4 | Recommended for .NET8.0 |
| System.Net.Http |4.3.4 | | Functionality included with framework reference|
| System.Text.RegularExpressions |4.3.1 | | Functionality included with framework reference|

### Project upgrade details

#### HttpExtensions\HttpExtensions.csproj modifications

Project properties changes:
 - Target framework should be changed to `net8.0`
NuGet packages changes:
 - Newtonsoft.Json should be updated from `13.0.1` to `13.0.4` (recommended for .NET8.0)
 - System.Net.Http should be removed (functionality included with framework reference)
 - System.Text.RegularExpressions should be removed (functionality included with framework reference)

#### WorkItemServices\WorkItemServices.csproj modifications

Project properties changes:
 - Target framework should be changed to `net8.0`
NuGet packages changes:
 - Newtonsoft.Json should be updated from `13.0.1` to `13.0.4` (recommended for .NET8.0)
 - System.Net.Http should be removed (functionality included with framework reference)
 - System.Text.RegularExpressions should be removed (functionality included with framework reference)

#### TimeTrackingService\TimeTrackingService.csproj modifications

Project properties changes:
 - Target framework should be changed to `net8.0`
NuGet packages changes:
 - Newtonsoft.Json should be updated from `13.0.1` to `13.0.4` (recommended for .NET8.0)
 - System.Net.Http should be removed (functionality included with framework reference)
 - System.Text.RegularExpressions should be removed (functionality included with framework reference)

#### WorkItemService.Demo\WorkItemService.Demo.csproj modifications

Project properties changes:
 - Target framework should be changed to `net8.0`
NuGet packages changes:
 - No NuGet package changes required.
