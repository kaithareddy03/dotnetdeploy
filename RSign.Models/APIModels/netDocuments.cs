using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.APIModels
{
    public class netDocuments
    {
        public AdminGeneralAndSystemSettings AdminGeneralAndSystemSettings { get; set; }
        public List<CabinetsList> cabinetsList { get; set; }
        public ClientList clients { get; set; }

    }
  
    public class netDocsUserInfo
    {
        public string? displayName { get; set; }
        public string? email { get; set; }
        public string? id { get; set; }
        public string? organization { get; set; }
        public string? primaryCabinet { get; set; }
        public string? sortLookupBy { get; set; }
    }

    public class CabinetsList
    {
        public bool allowExternalUserToAccessHistory { get; set; }
        public string? id { get; set; }
        public string? isCollaborationSpacesEnabled { get; set; }
        public string? isUserExternalToCabinet { get; set; }
        public string? name { get; set; }
        public string? repositoryId { get; set; }
        public string? repositoryName { get; set; }
        public int wsAttrNum { get; set; }
        public string? wsAttrPluralName { get; set; }
        public int wsOrgAttrNum { get; set; }
    }


    public class ClientList
    {
        public string? defaulting { get; set; }
        public string? description { get; set; }
        public bool hold { get; set; }
        public string? key { get; set; }
    }

    public class Client
    {
        public List<ClientList> rows { get; set; }
    }

    public class MatterList
    {
        public string? defaulting { get; set; }
        public string? description { get; set; }
        public bool hold { get; set; }
        public string? key { get; set; }
        public string? parent { get; set; }
    }

    public class Matter
    {
        public int count { get; set; }
        public List<MatterList> rows { get; set; }
    }

    public class StandardList
    {
        public List<int> LatestVersionLabel { get; set; }
        public string? VersionLabel { get; set; }
        public string? aclStatus { get; set; }
        public DateTime created { get; set; }
        public string? createdBy { get; set; }
        public string? createdByGuid { get; set; }
        public string? envId { get; set; }
        public string? extension { get; set; }
        public string? id { get; set; }
        public int latestVersionNumber { get; set; }
        public bool locked { get; set; }
        public DateTime modified { get; set; }
        public string? modifiedBy { get; set; }
        public string? modifiedByGuid { get; set; }
        public string? name { get; set; }
        public int officialVer { get; set; }
        public int size { get; set; }
        public object syncMod { get; set; }
        public string? url { get; set; }
        public int versions { get; set; }
    }

    public class DocumentsList
    {
        public string? next { get; set; }
        public List<SortOrder> sortOrder { get; set; }
        public List<StandardList> standardList { get; set; }
        public List<CustomAttribute> customAttributes { get; set; }
        public List<CustomList> customList { get; set; }
    }
    public class CustomAttribute
    {
        public string? description { get; set; }
        public int id { get; set; }
        public string? value { get; set; }
    }
    public class CustomList
    {
        public List<CustomAttribute> customAttributes { get; set; }
        public StandardAttributes standardAttributes { get; set; }
        public StandardList standardList { get; set; }
    }
    public class Folder
    {
        public string? next { get; set; }
        public List<SortOrder> sortOrder { get; set; }
        public List<StandardList> standardList { get; set; }
        public List<CustomAttribute> customAttributes { get; set; }
        public List<CustomList> customList { get; set; }
        public List<Result> Results { get; set; }
        public int TotalFound { get; set; }
    }
    public class SortOrder
    {
        public int Field { get; set; }
        public string? SortDirection { get; set; }
        public bool UseMvpSorting { get; set; }
    }


    public class Attributes
    {
        public string? FolderId { get; set; }
        public string? DocId { get; set; }
        public string? EnvId { get; set; }
        public int DocNum { get; set; }
        public string? Name { get; set; }
        public string? Ext { get; set; }
        public int Size { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByGuid { get; set; }
        public DateTime Created { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedByGuid { get; set; }
        public DateTime Modified { get; set; }
        public string? Version { get; set; }

    }
    public class Result
    {
        public string? DocId { get; set; }
        public string? EnvId { get; set; }
        public int DocNum { get; set; }
        public Attributes Attributes { get; set; }
        public List<CustomAttribute> CustomAttributes { get; set; }
        public List<Ancestor> Ancestors { get; set; }
    }   

    public class Ancestor
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
    }

    public class StandatrdDocumentListwithAncestors
    {
        public string? DocId { get; set; }
        public string? EnvId { get; set; }
        public int DocNum { get; set; }
        public Attributes Attributes { get; set; }
        public List<Ancestor> Ancestors { get; set; }

    }
    public class Location
    {
        public string? Account { get; set; }
        public int CabOptions { get; set; }
        public string? Cabinet { get; set; }
    }

    public class Misc
    {
        public bool Approved { get; set; }
        public bool Archived { get; set; }
        public bool Autoversioning { get; set; }
        public bool Deleted { get; set; }
        public bool Echo { get; set; }
        public bool External { get; set; }
        public bool ExternalUser { get; set; }
        public bool Favorite { get; set; }
        public bool Locked { get; set; }
        public bool OfficialLocked { get; set; }
        public bool QueueForIndexing { get; set; }
        public bool Signed { get; set; }
    }

    public class Permission
    {
        public bool Administer { get; set; }
        public bool CabDefault { get; set; }
        public bool Edit { get; set; }
        public bool NoAccess { get; set; }
        public bool Share { get; set; }
        public bool View { get; set; }
    }

    public class StandardAttributes
    {
        public List<int> LatestVersionLabel { get; set; }
        public string? VersionLabel { get; set; }
        public string? AclStatus { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByGuid { get; set; }
        public string? EnvId { get; set; }
        public string? Extension { get; set; }
        public string? Id { get; set; }
        public int LatestVersionNumber { get; set; }
        public bool Locked { get; set; }
        public DateTime Modified { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedByGuid { get; set; }
        public string? Name { get; set; }
        public int OfficialVer { get; set; }
        public int Size { get; set; }
        public long SyncMod { get; set; }
        public string? Url { get; set; }
        public int Versions { get; set; }
    }

    public class DocumentsInfo
    {
        public List<CustomAttribute> CustomAttributes { get; set; }
        public Location Location { get; set; }
        public Misc Misc { get; set; }
        public List<Permission> Permissions { get; set; }
        public StandardAttributes StandardAttributes { get; set; }
        public List<Ancestor> Ancestors { get; set; }
        public List<CustomAttribute> customAttributes { get; set; }
        public StandardAttributes standardAttributes { get; set; }
    }

    public class VersionInfo
    {
        public object FlexStoreId { get; set; }
        public string? WOPISrc { get; set; }
        public DateTime created { get; set; }
        public string? createdBy { get; set; }
        public string? createdByGuid { get; set; }
        public int createdFrom { get; set; }
        public string? createdFromLabel { get; set; }
        public object createdFromName { get; set; }
        public string? description { get; set; }
        public string? extension { get; set; }
        public bool isLatest { get; set; }
        public bool locked { get; set; }
        public DateTime modified { get; set; }
        public string? modifiedBy { get; set; }
        public string? modifiedByGuid { get; set; }
        public int number { get; set; }
        public bool official { get; set; }
        public bool revoked { get; set; }
        public string? verName { get; set; }
        public string? versionLabel { get; set; }
    }


    public class netDocsAdvanceSearch
    {
        public string? Cabinet { get; set; }
        public string? Client { get; set; }
        public string? Matter { get; set; }
        public string? Folder { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public string? CreatedBy { get; set; }
        public string? DocumentId { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? SkipTokenUrl { get; set; }
    }

}
