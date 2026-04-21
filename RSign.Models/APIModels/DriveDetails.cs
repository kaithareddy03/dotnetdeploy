using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSign.Web.Models
{
    public class DriveFileDetails
    {
        public string? FileName { get; set; }
        public string? FileSource { get; set; }
        public int FileSize { get; set; }
    }
    public class UploadDriveFiles
    {
        public Guid EnvelopeID { get; set; }
        public string? Stage { get; set; }
        public bool IsStaticTemplate { get; set; }
        public string? GAuthToken { get; set; }
        public List<DriveFileDetails> DriveFiles { get; set; }
        public string? UpdatedDocumentId { get; set; }
        public Guid EnvelopeType { get; set; }
        public string? UploadDocDriveType { get; set; }
        public string? IsEnableFileReview { get; set; }
    }

    public class UploadDriveFilesForAttachments
    {
        public Guid EnvelopeID { get; set; }
        public Guid RecepientID { get; set; }
        public bool IsStaticTemplate { get; set; }
        public string? GAuthToken { get; set; }
        public List<DriveFileDetails> DriveFiles { get; set; }
        public int uploadsAttachmentID { get; set; }
        public Guid EnvelopeType { get; set; }
        public string? UploadDocDriveType { get; set; }
        public string? Stage { get; set; }
        public string? RecepientEmail { get; set; }
    }

    public class UploadiManageFiles
    {
        public Guid EnvelopeID { get; set; }
        public string? Stage { get; set; }
        public bool IsStaticTemplate { get; set; }
        public string? UpdatedDocumentId { get; set; }
        public Guid EnvelopeType { get; set; }
        public string? UploadDocDriveType { get; set; }
        public string? customerid { get; set; }
        public string? oauthToken { get; set; }

        public string? AppliedEpicUser { get; set; }
        public string? AppliedEpicEntityId { get; set; }
        public string? EntityType { get; set; }
        public string? IsEnableFileReview { get; set; }

        public List<iManageDocumentslist> iManageDocslist { get; set; }
        public List<RSign.Models.APIModels.APIEnvelopeRecipientRequest> Recipients { get; set; }
        public string? IntegrationType { get; set; }

    }
    public class iManageDocumentslist
    {
        public string? DocumentId { get; set; }
        public string? Workspace_Id { get; set; }
        public string? Workspace_Name { get; set; }
        public string? Folder_Id { get; set; }
        public int DocumentSize { get; set; }
        public string? DocumentName { get; set; }
        public string? Iwl { get; set; }
        public string? DocumentType { get; set; }
        public string? Version { get; set; }
    }
    public partial class iManageDocumentPathDetails
    {
        public iManageDocumentPath[][] Data { get; set; }
    }

    public partial class iManageDocumentPath
    {
        public string? Id { get; set; }

        public string? Name { get; set; }
    }

    public class iManageLoginResponse
    {
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }

        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? userName { get; set; }
        public string? issued { get; set; }
        public string? expires { get; set; }
        public string? BhRestToken { get; set; }
        public string? restUrl { get; set; }
    }
    public class BHTokenDetails
    {
        public string? BhRestToken { get; set; }
        public string? restUrl { get; set; }
    }
    public class Address
    {
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? city { get; set; }
        public string? countryCode { get; set; }
        public int countryID { get; set; }
        public string? countryName { get; set; }
        public string? state { get; set; }
        public string? timezone { get; set; }
        public string? zip { get; set; }
    }

    public class EntityFields
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? name { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? fax { get; set; }
        public Address address { get; set; }
        public int id { get; set; }
        public string? title { get; set; }
        public ClientContact clientContact { get; set; }
        public bool isDeleted { get; set; }
        public string? employmentType { get; set; }
        public string? description { get; set; }
        public long startDate { get; set; }
        public bool _editable { get; set; }
        public string? _subtype { get; set; }

        public object linkedPerson { get; set; }
        public ClientCorporation clientCorporation { get; set; }
        public string? status { get; set; }
        public Owner owner { get; set; }
        public SecondaryOwners secondaryOwners { get; set; }
        public string? occupation { get; set; }
    }
    public class SecondaryOwners
    {
        public int total { get; set; }
        public List<object> data { get; set; }
    }
    public class EntityResponse
    {
        public EntityFields data { get; set; }
    }
    public class EntityFile
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public object fileType { get; set; }
        public string? description { get; set; }
        public string? contentType { get; set; }
        public string? contentSubType { get; set; }
        public string? externalID { get; set; }
        public object dateAdded { get; set; }
        public bool isPrivate { get; set; }
        public string? distribution { get; set; }
        public bool isExternal { get; set; }
        public string? fileExtension { get; set; }
        public bool isDeleted { get; set; }
        public string? fileUrl { get; set; }
    }

    public class AssociatedEntity
    {
        public string? entity { get; set; }
        public string? entityMetaUrl { get; set; }
        public string? label { get; set; }
        public string? dateLastModified { get; set; }
        public List<Field> fields { get; set; }
    }

    public class EntityFiles
    {
        public int id { get; set; }
        public object dateAdded { get; set; }
        public string? name { get; set; }
        public object type { get; set; }
        public bool isPrivate { get; set; }
        public int fileSize { get; set; }
        public string? fileExtension { get; set; }
        public string? directory { get; set; }
        public string? description { get; set; }
        public UsersSharedWith usersSharedWith { get; set; }
        public bool isEncrypted { get; set; }
        public string? distribution { get; set; }
        public string? fileUrl { get; set; }

    }

    public class Field
    {
        public string? name { get; set; }
        public string? type { get; set; }
        public string? dataType { get; set; }
        public bool optional { get; set; }
        public string? label { get; set; }
        public string? dataSpecialization { get; set; }
        public bool? confidential { get; set; }
        public bool? required { get; set; }
        public bool? readOnly { get; set; }
        public bool? multiValue { get; set; }
        public bool? hideFromSearch { get; set; }
        public int? sortOrder { get; set; }
        public string? hint { get; set; }
        public string? description { get; set; }
        public bool? systemRequired { get; set; }
        public bool? shouldAddCustomEntityLabel { get; set; }
        public long? maxLength { get; set; }
        public List<Option> options { get; set; }
        public string? optionsType { get; set; }
        public string? optionsUrl { get; set; }
        public AssociatedEntity associatedEntity { get; set; }
    }

    public class Message
    {
        public string? detailMessage { get; set; }
        public string? severity { get; set; }
        public string? type { get; set; }
    }
    public class ClientContact
    {
        public int id { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
    }

    public class Option
    {
        public string? value { get; set; }
        public string? label { get; set; }
    }

   

    public class UsersSharedWith
    {
        public int total { get; set; }
        public List<object> data { get; set; }
    }



    public class EntityFileResponse
    {
        public int total { get; set; }
        public int start { get; set; }
        public int count { get; set; }
        public List<EntityFiles> data { get; set; }
        public List<Message> messages { get; set; }
        public List<EntityFile> EntityFile { get; set; }
    }


    public class ClientCorporation
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? phone { get; set; }
        public string? status { get; set; }

    }

    public class ClientContacts
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? occupation { get; set; }
        public ClientCorporation clientCorporation { get; set; }
        public object phone { get; set; }
        public string? status { get; set; }
        public string? type { get; set; }
        public object dateLastVisit { get; set; }
        public object dateLastComment { get; set; }
        public object dateAdded { get; set; }
        public object source { get; set; }
        public Owner owner { get; set; }
        public string? email { get; set; }
        public double _score { get; set; }
    }

    public class Owner
    {
        public int id { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
    }

    public class clientCorporationContactResponse
    {
        public int total { get; set; }
        public int start { get; set; }
        public int count { get; set; }
        public List<ClientContacts> data { get; set; }
    }

    public class FileContent
    {
        public string? contentType { get; set; }
        public string? fileContent { get; set; }
        public string? name { get; set; }
    }

    public class FileResponse
    {
        public FileContent File { get; set; } 
    }

    public class UploadSignerDriveFiles
    {
        public Guid EnvelopeID { get; set; }
        public string? Stage { get; set; }
        public bool IsStaticTemplate { get; set; }
        public string? GAuthToken { get; set; }
        public List<DriveFileDetails> DriveFiles { get; set; }
        public string? UpdatedDocumentId { get; set; }
        public Guid EnvelopeType { get; set; }
        public string? UploadDocDriveType { get; set; }
        public string? IsEnableFileReview { get; set; }

        public int UploadAttachmentID { get; set; }
        public string? NameSiA { get; set; }

        public string? DescriptionSiA { get; set; }

        public string? AdditionalInfoSiA { get; set; }
        public string? recipientEmailSiA { get; set; }
    }

    public class UploadSignerImageDriveFiles
    {
        public Guid EnvelopeID { get; set; } 
        public string? GAuthToken { get; set; }
        public List<DriveFileDetails> DriveFiles { get; set; }
        public string? recipientEmailSiA { get; set; }
    }
}