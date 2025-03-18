using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataManagement;
using Autodesk.DataManagement.Model;
using System;
using System.Linq;

public partial class APS
{
    public async Task<IEnumerable<dynamic>> GetHubs(Tokens tokens)
    {
        DataManagementClient dataManagementClient = new DataManagementClient(_SDKManager);
        var getHubs = await dataManagementClient.GetHubsAsync(accessToken: tokens.AccessToken);
        var hubsData = getHubs.Data.Where(hub => hub.Id.StartsWith("b."));
        return hubsData;
    }

    public async Task<IEnumerable<dynamic>> GetProjects(string hubId, Tokens tokens)
    {
        DataManagementClient dataManagementClient = new DataManagementClient(_SDKManager);
        var hubProjects = await dataManagementClient.GetHubProjectsAsync(hubId: hubId, accessToken: tokens.AccessToken);
        return hubProjects.Data
        .Where(project => project.Attributes.Extension.Data.TryGetValue("projectType", out var projectType) &&
                          projectType?.ToString() == "ACC")
        .ToList();
    }

    public async Task<IEnumerable<dynamic>> GetContents(string hubId, string projectId, string folderId, Tokens tokens)
    {
        DataManagementClient dataManagementClient = new DataManagementClient(_SDKManager);
        if (string.IsNullOrEmpty(folderId))
        {
            var projectTopFolders = await dataManagementClient.GetProjectTopFoldersAsync(hubId: hubId, projectId: projectId, accessToken: tokens.AccessToken);
            List<TopFolderData> topFoldersData = projectTopFolders.Data;

            return topFoldersData;
        }

        FolderContents folderContents = await dataManagementClient.GetFolderContentsAsync(projectId: projectId, folderId: folderId, accessToken: tokens.AccessToken);
        List<IFolderContentsData> folderContentsData = folderContents.Data;

        return folderContentsData;
    }

    public async Task<IEnumerable<dynamic>> GetVersions(string hubId, string projectId, string itemId, Tokens tokens)
    {
        DataManagementClient dataManagementClient = new DataManagementClient(_SDKManager);
        Versions versions = await dataManagementClient.GetItemVersionsAsync(projectId, itemId, accessToken: tokens.AccessToken);

        return versions.Data;
    }
}