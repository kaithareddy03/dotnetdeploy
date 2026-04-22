using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common.Helpers
{
    public static class Constants
    {
        public static class RecipientType
        {
            public static readonly Guid CC = new Guid("{63EA73C2-4B64-4974-A7D5-0312B49D29D0}");
            public static readonly Guid Signer = new Guid("{C20C350E-1C6D-4C03-9154-2CC688C099CB}");
            public static readonly Guid Sender = new Guid("{26E35C91-5EE1-4ABF-B421-3B631A34F677}");
            public static readonly Guid Prefill = new Guid("{712f1a0b-1ac6-4013-8d74-aac4a9bf5568}");  // V2 Team Prefill Change
        }

        public static class StatusCode
        {
            public static class Envelope
            {
                public static readonly Guid Incomplete_and_Expired = new Guid("{2A8C3F8D-B512-43A6-8579-00BFF4EFE546}");
                public static readonly Guid Terminated = new Guid("{EB0BB5BD-DADA-4C37-AD2F-704CE4992E1C}");
                public static readonly Guid Completed = new Guid("{C9596319-6F6B-4840-B49D-85010206C7C7}");
                public static readonly Guid Waiting_For_Signature = new Guid("{63E17398-AFDC-48C7-B9EC-90582CFEB562}");
                public static readonly Guid CancelledTransaction = new Guid("{3AC65996-8529-4415-B895-54F27B3CC609}");
                public static readonly Guid TemplateKey = new Guid("{547CFB99-E2CA-457D-A0D2-86D2FA12C86E}");

            }

            public static class AttachmentRequestUploadStatus
            {
                public static readonly Guid Uploaded = new Guid("{E7C648E8-1323-468D-ACD3-4A40CC325467}");
                public static readonly Guid NotUploaded = new Guid("{CAAAC14A-D4BF-4DBA-8542-73B9E8505B65}");
            }

            public static class Signer
            {
                public static readonly Guid Sent = new Guid("{1B5D8208-B1BF-40E1-9FBE-1D18F8A87B44}");
                public static readonly Guid Viewed = new Guid("{D30DBB92-E6D6-4729-93E2-BD02F56CEBE1}");
                public static readonly Guid Rejected = new Guid("{B03B5387-50BB-49BE-92A0-0745C1136092}");
                public static readonly Guid Signed = new Guid("{4F564EA5-009C-4F52-A3DE-C6D0AC598617}");
                public static readonly Guid Pending = new Guid("{20D49160-82E6-48F2-93AF-CE78F86177D2}");
                public static readonly Guid Delegated = new Guid("{3B229C22-16AE-48BB-AA06-CF05C78A60D7}");
                public static readonly Guid Incomplete_and_Expired = new Guid("{468DF264-DD7A-41BE-A0AF-D0EC9B95001F}");
                public static readonly Guid Finish_Later_Selected = new Guid("{D5ADF209-18AC-4D7E-985E-ECAE72307F2F}"); // Changed status to Finish_Later_Selected from Saved_As_Draft as per ticketID:S3-74
                public static readonly Guid AwaitingConfirmation = new Guid("51E85610-7671-4407-A4DE-2A2AA5851E89");
                public static readonly Guid Update_And_Resend = new Guid("B656B331-D44C-4489-9CB8-BB8DACB56682");
            }

            public static class Recipients
            {
                public static readonly Guid Edit = new Guid("{EFEB4B3C-D039-4677-9D7D-758AD8ACCBCC}");
                public static readonly Guid Resend = new Guid("{7659B486-EBE3-4708-A351-BB12ED66DD6A}");
                public static readonly Guid Transferred = new Guid("{CDAEFC53-DC16-44C7-838D-007145831E6F}");
            }
        }

        public static class ExpiryType
        {
            public static readonly Guid One_Day = new Guid("{25B259E0-5C43-4724-B215-406AAD6000C1}");
            public static readonly Guid Two_Days = new Guid("{2BD241A0-7196-45E7-A980-F887E823FC63}");
            public static readonly Guid Three_Days = new Guid("{9C7A85D8-2BC1-487D-93F6-3356D670C867}");
            public static readonly Guid Four_Days = new Guid("{A6B7B576-46BF-4CFD-95E1-030179FA89AD}");
            public static readonly Guid Five_Days = new Guid("{B1451DE4-CC89-46CE-8098-41E2C9D3618F}");
            public static readonly Guid Six_Days = new Guid("{CABC31E9-FCF1-45BB-B2F3-AB0BD0DF46E1}");
            public static readonly Guid One_Weeks = new Guid("{44E03B8A-2E73-473C-B453-B8643D09910B}");
            public static readonly Guid Two_Weeks = new Guid("{5A6AD6BF-564B-4306-93E7-1563DA6DB649}");
            public static readonly Guid Three_Months = new Guid("{8DFDA0D8-5FDF-41B8-8455-6A63791E4130}");
            public static readonly Guid Thirty_Days = new Guid("{EE01FD0A-B72E-4F62-B434-7081DB5BB1DB}");
            public static readonly Guid Sixty_Days = new Guid("{67CDB85F-8CD3-4D92-AB9A-8BC7DFEA28A7}");
            public static readonly Guid Ten_Days = new Guid("{C0F07AAE-47DC-4530-AE38-0B5A0F7DACC8}");
        }

        public static class DateFormat
        {
            public static readonly Guid US_mm_dd_yyyy_slash = new Guid("{E3DAD8D9-E16F-40F5-B112-BBA204538136}");
            public static readonly Guid US_mm_dd_yyyy_colan = new Guid("{9FC73C2B-34D7-42A1-B2A6-702ED2FD312B}");
            public static readonly Guid US_mm_dd_yyyy_dots = new Guid("{BDA0023F-AFC1-46E5-A134-884EDCA48799}");
            public static readonly Guid Europe_mm_dd_yyyy_slash = new Guid("{577D1738-6891-45DE-8481-E3353EB6A963}");
            public static readonly Guid Europe_mm_dd_yyyy_colan = new Guid("{6685D1ED-60D2-4028-94E9-BC875C2E551D}");
            public static readonly Guid Europe_mm_dd_yyyy_dots = new Guid("{374FE10E-4313-4541-B3CB-627310A14499}");
            public static readonly Guid Europe_yyyy_mm_dd_dots = new Guid("{8FAC284C-AB19-456C-BC73-1CE0D66D7220}");
            public static readonly Guid US_dd_mmm_yyyy_colan = new Guid("{7F472F00-CD12-443E-B38D-085F8872115F}");
        }
        public static class DateFormatString
        {
            public static readonly string US_mm_dd_yyyy_slash = "mm/dd/yyyy";
            public static readonly string US_mm_dd_yyyy_colan = "mm-dd-yyyy";
            public static readonly string US_mm_dd_yyyy_dots = "mm.dd.yyyy";
            public static readonly string Europe_mm_dd_yyyy_slash = "dd/mm/yyyy";
            public static readonly string Europe_mm_dd_yyyy_colan = "dd-mm-yyyy";
            public static readonly string Europe_mm_dd_yyyy_dots = "dd.mm.yyyy";
            public static readonly string Europe_yyyy_mm_dd_dots = "yyyy.mm.dd.";
            public static readonly string US_dd_mmm_yyyy_colan = "dd-mmm-yyyy";
        }

        public static class DateFormatStringNew
        {
            public static readonly string US_mm_dd_yyyy_slash = "MM/dd/yyyy";
            public static readonly string US_mm_dd_yyyy_colan = "MM-dd-yyyy";
            public static readonly string US_mm_dd_yyyy_dots = "MM.dd.yyyy";
            public static readonly string Europe_mm_dd_yyyy_slash = "dd/MM/yyyy";
            public static readonly string Europe_mm_dd_yyyy_colan = "dd-MM-yyyy";
            public static readonly string Europe_mm_dd_yyyy_dots = "dd.MM.yyyy";
            public static readonly string Europe_yyyy_mm_dd_dots = "yyyy.MM.dd.";
            public static readonly string US_dd_mmm_yyyy_colan = "dd-MMM-yyyy";
        }

        public static class Control
        {
            public static readonly Guid Checkbox = new Guid("{2CE0EF43-5736-44B4-AC5C-08B2008DBEB9}");
            public static readonly Guid Company = new Guid("{7EFDBC3E-9F56-4532-9B73-16934C5CD0DD}");
            public static readonly Guid Date = new Guid("{27A278AE-F886-4420-B0D1-0DCBAC62A867}");
            public static readonly Guid Initials = new Guid("{7731B804-2771-4DCA-82DC-9BBA03B61A2B}");
            public static readonly Guid NewInitials = new Guid("{BBB41561-0426-4D1F-9887-15D7DFC7C5A2}");
            public static readonly Guid Label = new Guid("{9A0B1230-CDC4-49E7-AEEA-990F415FB89E}");
            public static readonly Guid Name = new Guid("{582F9ACF-353F-4B99-90F4-333189D2D5C9}");
            public static readonly Guid Email = new Guid("{BB801D80-4EC8-4506-B080-A9980CE0AB1A}");
            public static readonly Guid Signature = new Guid("{E294C207-13FD-4508-95FC-90C5D9C555FA}");
            public static readonly Guid Text = new Guid("{76597505-F0DE-47EE-AF59-F721BE198242}");
            public static readonly Guid Title = new Guid("{82ECB881-E947-473C-BC93-E7D378928975}");
            public static readonly Guid DropDown = new Guid("{DE56E81A-89AE-4D1B-8194-37D9A68A5191}");
            public static readonly Guid Radio = new Guid("{2B02B483-B876-4483-92E4-63E6A5740A37}");
            public static readonly Guid DateTimeStamp = new Guid("{6AFCF199-51CB-4938-B106-AF8F24674C99}");
            public static readonly Guid Hyperlink = new Guid("{B731C98D-74C3-4983-BC06-A0DEF7C306F7}");

        }
        public static class ControlConditionRule
        {
            public static class Signature
            {
                public static readonly Guid Signed = new Guid("{B7101FBE-F33B-4FCD-A65F-9A44E5A8A354}");
                public static readonly Guid Unsigned = new Guid("{11B6EDCC-70A4-4EC9-AA57-BD7C59ABF9F8}");
            }
            public static class Initials
            {
                public static readonly Guid Initialed = new Guid("{2380C0B0-4A55-4B33-9E22-A93C55A78CC3}");
                public static readonly Guid NotInitialed = new Guid("{7B108AA4-B5F9-489D-930B-2AEB90811518}");
            }
            public static class Text
            {
                public static readonly Guid AnyText = new Guid("{E8351007-1A7E-449C-B3FD-AFE619CFFA8D}");
                public static readonly Guid SpecificText = new Guid("{8E2B3EC5-6CD1-4F3E-8025-4FDC7D6E35E3}");
                public static readonly Guid ContainsText = new Guid("{68569338-7AEA-481F-988C-B70D13607E81}");
                public static readonly Guid Empty = new Guid("{0B069009-E271-4534-8C8B-21D876CC31DE}");
            }
            public static class Checkbox
            {
                public static readonly Guid Unchecked = new Guid("{E6FC0258-FBD1-4B11-99A7-138193AF2E64}");
                public static readonly Guid Checked = new Guid("{1765014E-6611-4C9A-9268-833E7D853D1A}");
            }
        }

        public static class MailTemplate
        {
            public static readonly Guid Accept = new Guid("{85EBA5A5-1635-4348-B6D8-E61BD9E77DDC}");
            public static readonly Guid RejectRecipient = new Guid("{F8735CAF-6502-4E1F-8F05-E6BD3BB8A778}");
            public static readonly Guid CCRecipient = new Guid("{2907D4F0-0432-4097-ADFE-1C7BD1BF4E58}");
            public static readonly Guid RejectSender = new Guid("{9846AF0E-AD06-48DA-8D66-96E845D228EB}");
            public static readonly Guid DelegateSender = new Guid("{27424DBB-263A-48F3-8280-82EF3C8C10B5}");
            public static readonly Guid DelegatedTo = new Guid("{A797ED1E-8DF8-4200-A6C7-8963A8E4DE45}");
            public static readonly Guid Reminder = new Guid("{B78E1DBA-99EB-4559-80B3-36BC4CF4A39C}");
            public static readonly Guid SigningCertificate = new Guid("{26D30BB1-0BCB-4C4F-B306-04D657B83DD5}");
            public static readonly Guid PasswordOpen = new Guid("{8F3952A0-3D6A-4ABA-8568-06ABEBBE40B9}");
            public static readonly Guid SendReminderFromManage = new Guid("{91388B3F-3EE8-4B99-A372-35B82F6DC6B2}");
            public static readonly Guid PasswordSign = new Guid("{861F8E53-3E79-4EF3-9F82-554CCFB25827}");
            public static readonly Guid SendEnvelope = new Guid("{E03EA5D3-F780-4B83-851A-F6BF3DB5C903}");
            public static readonly Guid SendEnvelopeSpanish = new Guid("{0B413E11-B409-4AA3-B3A9-57900ACEC9FF}");
            public static readonly Guid CCRecipientSpanish = new Guid("{0DEB18BF-5E9F-46EA-9CCA-8E95C3BBF9F0}");
            public static readonly Guid AcceptSpanish = new Guid("{E5B71635-D92B-4DA7-A0BB-526C8335F446}");
            public static readonly Guid PasswordSignSpanish = new Guid("{18C4E2B6-CE8D-4866-B8DE-4357E03BAEC4}");
            public static readonly Guid PasswordOpenSpanish = new Guid("{703881AA-780E-4F9B-9C39-74331C88AB2E}");
            public static readonly Guid SendReminderFromManageSpanish = new Guid("{2FB3BB6E-CFE6-4378-904F-79DF2CE1B83E}");
            public static readonly Guid SigningCertificateSpanish = new Guid("{8AFCDD42-5E11-4C8C-BFF2-E5964A797D3B}");
            public static readonly Guid RejectSenderSpanish = new Guid("{8C387E18-ECF3-483B-A0FC-3C3C5EF53B07}");
            public static readonly Guid RejectRecipientSpanish = new Guid("{58361079-FD0D-4C4D-8003-CC56CD78FD51}");
            public static readonly Guid DelegateSenderSpanish = new Guid("{EB48BC04-1B8E-4DB1-9578-B9CB65330CA5}");
            public static readonly Guid DelegatedToSpanish = new Guid("{6B828039-1F38-4FAD-BFDA-05CCD870ADDA}");
            public static readonly Guid SigningCertificateStaticDoc = new Guid("{307C82D7-5CD8-422C-B7F6-BA1330E32007}");
            public static readonly Guid SendingConfirmation = new Guid("{307C82D7-5CD8-422C-B7F6-BA1330E32007}");
        }

        public static class String
        {
            public static readonly string DocumentTempFolderPath = "DocumentTempFolderPath";
            public static readonly string DocumentFolderPath = "DocumentFolderPath";
            public static readonly string TemplatesPath = "TemplatesPath";
            public static readonly string ExpirySoonInDays = "ExpirySoonInDays";
            public static readonly string MessageTextThreeVerticalSpace = "<div><br></div><div><br></div><div><br></div>";
            public static readonly string SignatureControlName = "Signature";
            public static readonly string NewInitialsControlName = "NewInitials";
            public static class RSignStage
            {
                //Envelope
                public static readonly string InitializeEnvelope = "InitializeEnvelope";
                public static readonly string PrepareEnvelope = "PrepareEnvelope";
                //Draft
                public static readonly string InitializeDraft = "InitializeDraft";
                public static readonly string PrepareDraft = "PrepareDraft";
                //TemplateCreate
                public static readonly string InitializeTemplate = "InitializeTemplate";
                public static readonly string PrepareTemplate = "PrepareTemplate";

                //RuleCreate
                public static readonly string InitializeRule = "InitializeRule";
                public static readonly string PrepareRule = "PrepareRule";

                //TemplateEdit
                public static readonly string InitializeEditTemplate = "InitializeEditTemplate";
                public static readonly string PrepareEditTemplate = "PrepareEditTemplate";

                //RuleEdit
                public static readonly string InitializeEditRule = "InitializeEditRule";
                public static readonly string PrepareEditRule = "PrepareEditRule";

                //TemplateUse
                public static readonly string InitializeUseTemplate = "InitializeUseTemplate";
                public static readonly string PrepareUseTemplate = "PrepareUseTemplate";

                //RuleUse
                public static readonly string InitializeUseRule = "InitializeUseRule";
                public static readonly string PrepareUseRule = "PrepareUseRule";

                //Signing
                public static readonly string SignEnvelope = "SignEnvelope";

                public static readonly string UpdateAndResend = "UpdateAndResend";

                public static readonly string ProcessGroup = "ProcessGroup";

            }

            public static class Envelope
            {
                public static readonly string EnvelopeCreated = "Created";
                public static readonly string EnvelopeCompleted = "Completed";
                public static readonly string EnvelopeTerminated = "Terminated";
                public static readonly string EnvelopeSigned = "Signed";
                public static readonly string EnvelopeRejected = "Rejected";
                public static readonly string WaitingForSignature = "Waiting for Signature";
                public static readonly string CancelledTrans = "Transaction Cancelled";
                public static readonly string ArchivingSoon = "Archiving Soon";

            }

            public static class MailTemplateName
            {
                public static readonly string Accept = "Accept";
                public static readonly string CCRecipient = "CCRecipient";
                public static readonly string DelegatedTo = "DelegatedTo";
                public static readonly string DelegateSender = "DelegateSender";
                public static readonly string PasswordOpen = "PasswordOpen";
                public static readonly string PasswordSign = "PasswordSign";
                public static readonly string RejectRecipient = "RejectRecipient";
                public static readonly string RejectSender = "RejectSender";
                public static readonly string SendEnvelope = "SendEnvelope";
                public static readonly string SendReminderFromManage = "SendReminderFromManage";
                public static readonly string SigningCertificate = "SigningCertificate";
                public static readonly string SigningCertificateN = "SigningCertificateN";
                public static readonly string CurrentStatus = "CurrentStatus";
                public static readonly string CompanyCreate = "CompanyCreate";
                public static readonly string NewUserRegistration = "NewUserRegistration";
                public static readonly string UserActivation = "UserActivation";
                public static readonly string UserDeactivation = "UserDeactivation";
                public static readonly string UserTypeAdminToUser = "UserTypeAdminToUser";
                public static readonly string UserTypeUserToAdmin = "UserTypeUserToAdmin";
                public static readonly string UserTypeChangeNotification = "UserTypeChangeNotification";
                public static readonly string CompanyActivation = "CompanyActivation";
                public static readonly string CompanyDeactivation = "CompanyDeactivation";
                public static readonly string PwOpenForCC = "PwOpenForCC";
                public static readonly string SigningCertificateTransparency = "SigningCertificateTransparency";
                public static readonly string SigningCertificateTransparencyN = "SignedCertificateTransparencyN";
                public static readonly string ReminderTemplate = "ReminderTemplate";
                public static readonly string SendStaticTemplateConfirm = "SendStaticTemplateConfirm";
                public static readonly string FinalContractHeader = "FinalContractHeader";
                public static readonly string FinalContractFooter = "FinalContractFooter";
                public static readonly string FinalContractLink = "FinalContractLink";
                public static readonly string TransparencyLink = "TransparencyLink";
                public static readonly string EnvelopeXMLLink = "EnvelopeXMLLink";
                public static readonly string SignerAttachmentLink = "SignerAttachmentLink";
                public static readonly string DocumentSizeMsg = "DocumentSizeMsg";
                public static readonly string FinalContractContentNew = "FinalContractContentN";
                public static readonly string TransparencyContentNew = "TransparencyContentN";
                public static readonly string EnvelopeXMLContentNew = "EnvelopeXMLContentN";
                public static readonly string SignerAttachmentContentNew = "SignerAttachmentContentN";
                public static readonly string FinalContractContent = "FinalContractContent";
                public static readonly string TransparencyContent = "TransparencyContent";
                public static readonly string EnvelopeXMLContent = "EnvelopeXMLContent";
                public static readonly string SignerAttachmentContent = "SignerAttachmentContent";
                public static readonly string ErrorNotification = "ErrorNotification";
                public static readonly string SigningCertificateStaticDoc = "SigningCertificateStaticDoc";
                public static readonly string SigningCertificateStaticDocN = "SigningCertificateStaticDocN";
                public static readonly string FinalContractStaticDocContentNew = "FinalContractStaticDocContentNew";
                public static readonly string FinalContractStaticDocContent = "FinalContractStaticDocContent";
                public static readonly string HeaderTemplate = "HeaderTemplate";
                public static readonly string StaticHeaderTemplate = "StaticHeaderTemplate";
                public static readonly string SendingConfirmation = "SendingConfirmation";
                public static readonly string PwOpenForConfirmationEmail = "PwOpenForConfirmationEmail";
                public static readonly string ReminderTemplateOld = "ReminderTemplateOld";
                public static readonly string SigningCertificateRecepLogIn = "SigningCertificateRecepLogIn";
                public static readonly string RejectSenderLogIn = "RejectSenderLogIn";
                public static readonly string DeliveryStatusNotification = "DeliveryStatusNotification";
                public static readonly string EnvelopeFileReviewLink = "EnvelopeFileReviewLink";
                public static readonly string AuthenticateSigner = "AuthenticateSigner";
                public static readonly string VerificationCode = "VerificationCode";
                public static readonly string HeaderTemplateCC = "HeaderTemplateCC";
                public static readonly string ExpirationReminder = "ExpirationReminder";
                public static readonly string ElectronicSignatureNCC = "ElectronicSignatureNCC";
                public static readonly string ElectronicSignatureUNSCC = "ElectronicSignatureUNSCC";
                public static readonly string FinalDocumentCC = "FinalDocumentCC";
                public static readonly string SigningFooter = "SigningFooter";
                public static readonly string SigningFooterEmpty = "SigningFooterEmpty";
                public static readonly string FinalContractFooterEmpty = "FinalContractFooterEmpty";
                public static readonly string FinalSubmitVerificationCode = "FinalSubmitVerificationCode";
                public static readonly string FinishLaterReminderTemplate = "FinishLaterReminderTemplate";
            }

            public static class SignatureFontStyle
            {
                public static readonly string HomemadeApple = "HomemadeApple";
                public static readonly string DawningofaNewDay = "DawningofaNewDay";
                public static readonly string Kristi = "Kristi";
                public static readonly string LaBelleAurore = "LaBelleAurore";
                public static readonly string OvertheRainbow = "OvertheRainbow";
                public static readonly string Brush = "Brush";
            }

            public static class UserType
            {
                public static readonly string Superuser = "Superuser";
                public static readonly string Admin = "Admin";
                public static readonly string RSignSupport = "RSignSupport";
                public static readonly string User = "User";
            }
            public static class UserTypeDescription
            {
                public static readonly string RPostSuperAdmin = "RPostSuperAdmin";
                public static readonly string CustomerAdmin = "CustomerAdmin";
                public static readonly string RSignSupport = "RSignSupport";
                public static readonly string User = "User";
            }
            public static class Notations
            {
                public static readonly string Prefill = "(P)";
            }

            public static class SettingsType
            {
                public static readonly string Company = "Company";
                public static readonly string User = "User";
            }

            public static class languageKeyType
            {
                public static readonly string Label = "Label";
                public static readonly string Validation = "Validation";
                public static readonly string Both = "Both";
            }

            public static class EmailOperation
            {
                public static readonly string Send = "InitialOffer";
                public static readonly string Resend = "Resend";
                public static readonly string SignDocument = "SignDocument";
                public static readonly string Delegate = "Delegate";
                public static readonly string Reject = "Reject";
                public static readonly string CurrentStatus = "CurrentStatus";
                public static readonly string Accept = "Accept";
                public static readonly string SignStaticDocument = "SignStaticDocument";
                public static readonly string SignRecipient = "SignRecipient";
                public static readonly string SendCC = "SendCC";
                public static readonly string SignRecipientCC = "SignRecipientCC";
                public static readonly string InitialOfferStatic = "InitialOfferStatic";
                public static readonly string UserAccount = "UserAccount";
                public static readonly string UserCompany = "UserCompany";
                public static readonly string PasswordToSign = "PasswordToSign";
                public static readonly string PasswordToOpen = "PasswordToOpen";
                public static readonly string SendingConfirmation = "SendingConfirmation";
                public static readonly string EmailDeliveryStatus = "EmailDeliveryStatus";
                public static readonly string VerificationCode = "VerificationCode";
                public static readonly string SeviceEmailFail = "SeviceEmailFail";
            }
        }
        public static class SettingsKeyConfig
        {
            public static readonly Guid ExpiresIn = new Guid("{EBD069DA-E175-4D57-B3E3-04572FB8B283}");
            public static readonly Guid ThenSendReminderInDropdownOption = new Guid("{C2424CC4-098E-41F1-AB99-1196F9770C91}");
            public static readonly Guid StorageDriveGoogle = new Guid("{1A3BE4C3-4281-46A1-A5F2-298D4E9E6C43}");
            public static readonly Guid StorageDriveLocal = new Guid("{EC6BB052-167E-492D-831D-29AF3485A22A}");
            public static readonly Guid ThenSendReminderIn = new Guid("{106C3610-549B-4DB0-9E45-39365EAF0473}");
            public static readonly Guid StorageDriveDropbox = new Guid("{9DDD49F2-54AE-4E70-80DF-44116E8B1D9F}");
            public static readonly Guid StorageiManage = new Guid("{B3D431BE-5B5F-4D62-966B-5E7FB846A205}");
            public static readonly Guid Storagenetdocuments = new Guid("{BDBBAB60-A0E8-4ADF-83C2-A1788C556AD2}");
            public static readonly Guid StorageAppliedEpic = new Guid("{B7700C59-846B-4D44-9ABE-798B485F9565}");
            public static readonly Guid StorageBullhorn = new Guid("{FFD0C65B-A3EF-4038-B4CA-4B1DCD33E5B4}");
            public static readonly Guid StorageVincere = new Guid("{238EFA7E-5600-4FC7-A9FF-0B0B0C07AC75}"); //ramya
            public static readonly Guid SendReminderInDropdownOption = new Guid("{6968B8A7-98BE-4737-B71A-470A5271AF0F}");
            public static readonly Guid StoredSignedPDF = new Guid("{F2B2C7C7-1666-40EB-972F-4768DC57916F}");
            public static readonly Guid IncludeSignedCertificateOnSignedPDF = new Guid("{48758234-B794-42AD-AE69-62502FF8E663}");
            public static readonly Guid AccessCodeRequiredToOpenSignedDoc = new Guid("{C6B8EF2B-510C-495E-A279-739F96C693F3}");
            public static readonly Guid DateFormat = new Guid("{5E2810F6-B946-4978-B96F-7455429DBBA2}");
            public static readonly Guid TimeZone = new Guid("{051EABE4-494C-4F13-A0BE-9C8150CA90AD}");
            public static readonly Guid OverrideUserSettings = new Guid("{50352B2D-AA02-4ECB-84CC-A6ED7BE666B4}");
            public static readonly Guid SignInSequence = new Guid("{6D11EAAB-037B-40C6-90B1-A996DF8F82E8}");
            public static readonly Guid StorageDriveSkydrive = new Guid("{C99F72DC-7CBC-4697-8F5C-B534134018B6}");
            public static readonly Guid SignatureCaptureTypeSign = new Guid("{786D4233-7ECD-47C5-85EB-C04003D86C2D}");
            public static readonly Guid SignatureCaptureHandDrawn = new Guid("{F817820D-0566-4FA4-8C87-C561225BABCA}");
            public static readonly Guid IncludeTransparencyDocument = new Guid("{3169BE57-C18D-4020-BEDB-D495D3219BEB}");
            public static readonly Guid AccessCodeRequiredToSign = new Guid("{C2A8F6C5-7B4B-4B85-9F2A-ED2EEB2EAB5C}");
            public static readonly Guid ShowSettingsTabSelected = new Guid("{3E1C344F-6299-4C78-9C8A-F115B0EE2350}");
            public static readonly Guid SendReminderIn = new Guid("{AAAD8A21-D1EB-4D1A-9440-F1693AB15574}");
            public static readonly Guid Disclaimer = new Guid("{59C44C00-EDFA-435F-BB8B-BB4C8E3FA2F1}");
            public static readonly Guid IsDisclaimerInCertificate = new Guid("{72BCF7B1-4F73-41CE-A895-2921E813BB5B}");
            public static readonly Guid IsDeleteSignedContracts = new Guid("{6CC589DF-9F78-48E6-92C3-FB374B1E6487}");
            public static readonly Guid IsCreateRules = new Guid("{D13EB35E-991E-4C74-8FEC-A5804F5BEF31}");
            public static readonly Guid IncludeSignerAttachFile = new Guid("{F5A413FF-D6AF-45C3-8D6B-57C6EE1E8B37}");
            public static readonly Guid AllowRecipientToAttachFileWhileSigning = new Guid("{8679C104-C1C0-4004-8BF2-8FE31E1F3BFD}");
            public static readonly Guid FormFieldAlignment = new Guid("{8B5C9BB5-C1AB-4019-9E49-90CC0CD302CF}");
            public static readonly Guid CreateStaticLink = new Guid("{C20A5A85-F9EA-413C-B6EF-74C3C9004377}");
            public static readonly Guid AttachXML = new Guid("{8B33812A-09B6-4676-A6E2-779907F04C35}");
            public static readonly Guid SeparateMultipleDocumentsAfterSigning = new Guid("{2CF8C629-BCAE-4921-A887-D696B75B41D6}");
            public static readonly Guid IsShareTemplateRule = new Guid("{051AA71A-3DF1-4504-89B3-96CF706ECBDD}");
            public static readonly Guid AccessAuthentication = new Guid("{477844A3-4733-455F-8FBC-B7A00E5A818F}");
            public static readonly Guid AccessPassword = new Guid("{FFCD1492-ADFF-4ADC-ACC8-972FBA174EBC}");
            public static readonly Guid IsAccessCodeSendToSignerEnabled = new Guid("{4CB81B29-F903-4E0C-8E39-ED1CC8F5B3D4}");
            public static readonly Guid StoreEmailBody = new Guid("{9179F14D-9C5C-411B-BF0D-64A2B86F34D7}");
            public static readonly Guid AllowUserToDeleteEmailBody = new Guid("{6093FCBE-6D18-4863-B1A0-F2E12CFA8196}");
            public static readonly Guid FinalContractOptionSetting = new Guid("{2EBC257A-95F2-476A-BF67-33FEB4E6A6D7}");
            public static readonly Guid SignatureControlRequired = new Guid("{635E88CF-2618-4D7C-8E8A-6720849B2591}");
            public static readonly Guid EsignMailCopyAddress = new Guid("{A78B0973-2F7B-46A7-BB10-BB4DB7144E5F}");
            public static readonly Guid EsignMailRerouteAddress = new Guid("{8E989F58-B331-477A-9C55-220F0512466D}");
            public static readonly Guid ReceiveSendingEmailConfirmation = new Guid("{AE8E7190-49C7-4FB2-8B03-64B7102C43A1}");
            public static readonly Guid AttachSignedPDFOptionSetting = new Guid("{FE3CAD58-1B4D-4777-A38F-B2CEBFFAA9FB}");
            public static readonly Guid HeaderFooterOptionSettings = new Guid("{A3093F88-1BEB-435C-81F4-A11226A452F0}");
            public static readonly Guid IsPostSigningLandingPage = new Guid("{8CCB6FD5-04B9-46BE-91DC-8BFB9066B60D}");
            public static readonly Guid IsIncludeEnvelopeXmlData = new Guid("{B21049DD-EE6D-47F5-A254-BDA76A85B3FB}");
            public static readonly Guid AllowTemplateEditing = new Guid("{DE94EE53-49F0-40C8-9CA4-59CA339C93A8}");
            public static readonly Guid EnablePostSigningLoginPopup = new Guid("{E3F18D74-ED94-4EF8-B310-4B32CB7DFE87}");
            public static readonly Guid IsCreateMessageTemplate = new Guid("{800e8f46-2046-41b4-9def-6e6dd561f749}");
            public static readonly Guid EmailDisclaimer = new Guid("{9F42AA62-25F7-412B-B7EF-F017E5F3C5B3}");
            public static readonly Guid SendIndividualSignatureNotifications = new Guid("{38aae858-181f-41ed-8aef-908fe4ce5ba7}");
            public static readonly Guid AddDatetoSignedDocumentNameOptionSettings = new Guid("{208C0496-0204-42BE-815C-AF667D871243}");
            public static readonly Guid SendFinalReminderBeforeExp = new Guid("{E6A1444E-7D54-4540-BAD1-592DF3CB32C3}");
            public static readonly Guid SendFinalReminderBeforeExpInDropdownOption = new Guid("{F48FDC11-ECB8-4053-A1F3-3CC3ECBD39FA}");
            public static readonly Guid IsDefaultSignatureRequiredForStaticTemplate = new Guid("{E42BE481-0E1D-4119-8D94-B2BD686BC7B1}");
            public static readonly Guid ControlPanelPinnedPosition = new Guid("{5E4230EF-3573-470E-B676-61546F318090}");
            public static readonly Guid EnableOutOfOfficeMode = new Guid("{935019C6-452B-4823-A82E-946A647BFFE9}");
            public static readonly Guid OOFDateRangeFirstDay = new Guid("{890C0EF4-7DE7-43E7-B5AD-554004FDB729}");
            public static readonly Guid OOFDateRangeLastDay = new Guid("{B69782FB-8963-4E4A-ABE4-1BDECE855712}");
            public static readonly Guid OOFCopyEmailAddr = new Guid("{A88BCDC8-CDD9-4D34-BCB2-C32EED4490B5}");
            public static readonly Guid OOFRerouteEmailAddr = new Guid("{B26FDBB2-77C7-40FB-BFBA-A3106291A74A}");
            public static readonly Guid EnableDependenciesFeature = new Guid("{B0D10D44-E578-4F2B-ADFE-CF6CA27B46F3}");
            public static readonly Guid ReferenceCodeOptionSetting = new Guid("{389CC7D6-4DFE-433D-AE54-F55D4829E7DF}");
            public static readonly Guid SignMultipleDocumentIndependently = new Guid("{6E8FB657-EC5F-49F0-AD7A-6B06B070DCBB}");
            public static readonly Guid SendInvitationEmailToSigner = new Guid("{039ED0DB-E6D1-4C0F-82D6-7B4768F2A541}");
            public static readonly Guid EnableIntegrationAccess = new Guid("{80730F23-3142-4A7F-9BC0-C8F3187A4E6D}");
            public static readonly Guid DocumentPaperSize = new Guid("{391D4BDB-BB4D-414F-AFE1-424F23803BB3}");
            public static readonly Guid StampOnSignerCopySetting = new Guid("{C55D2433-F634-472B-B6B2-DF6A5A945B88}");
            public static readonly Guid StampOnSignerCopyAuthrozieText = new Guid("{56FDCF80-6BD1-4B38-AA5F-F48D467325E2}");
            public static readonly Guid StampOnSignerCopyWatermarkText = new Guid("{E12F4508-80D1-49FA-8545-EBACD3B658DD}");
            public static readonly Guid ElectronicSignIndicationSetting = new Guid("{5C57C305-E1FA-47B3-A39B-9E7CBE9946E0}");
            public static readonly Guid DeclineTemplateReasonsSettings = new Guid("{DF1DB446-6434-4000-B5AD-E307D227AD83}");
            public static readonly Guid SignatureRequestReplyAddress = new Guid("{4BE6593A-283A-48CD-9054-FDC921C659D5}");
            public static readonly Guid ShowRSignLogo = new Guid("{6EC2B879-F803-411D-B432-C963F4E4558C}");
            public static readonly Guid ShowCompanyLogo = new Guid("{25A5B369-0CC0-44FA-B64A-4534B759A16D}");
            public static readonly Guid EmailBannerBackgroundColor = new Guid("{C42155C1-0B84-4164-A369-99306C19B7D8}");
            public static readonly Guid EmailBannerFontColor = new Guid("{98FF4DA4-7404-467A-8937-48B65E0366C3}");
            public static readonly Guid CompanyLogoImage = new Guid("{B2BEB875-7E3F-422F-8FCB-13FB29EF5B9E}");
            public static readonly Guid PrivateMode = new Guid("{C0F8819E-0DB6-442C-9573-2235B68F44E7}");
            public static readonly Guid StoreOriginalDocument = new Guid("{A8947605-505A-4D84-9B92-EAB2BE302143}");
            public static readonly Guid StoreSignatureCertificate = new Guid("{0502EA3F-E65C-4B77-AFD6-EEA39C6949F6}");
            public static readonly Guid DeleteOriginalDocument = new Guid("{32AE3066-DB28-463F-82B5-9CB811B21E73}");
            public static readonly Guid AllowBulkSending = new Guid("{3EC04D10-6FBB-4A5E-838F-42BAA26743C5}");
            public static readonly Guid PostSigningPageUrl = new Guid("{28A12BE4-329A-4194-9C21-EC0AF0EEA02E}");
            public static readonly Guid IsEnvelopePostSigningPage = new Guid("{38BE1569-BE9B-49C7-A0D1-EC0289395AF4}");
            public static readonly Guid EnvelopePostSigningPageUrl = new Guid("{58147781-58AF-4B8B-B251-2EBE92D2E625}");
            public static readonly Guid EnableFileReview = new Guid("{6ECF22FC-A5ED-4939-8B00-B82446A26D52}");
            public static readonly Guid ShowControlID = new Guid("{D2ED8D63-4F67-4CC4-8B0B-4C90BFCCF8CB}");
            public static readonly Guid AllowRuleEditing = new Guid("{081CC546-F49A-4551-8E29-B8E97E674CCF}");
            public static readonly Guid AllowRuleUse = new Guid("{BB090397-10FC-4E3C-8F41-77EE47509691}");
            public static readonly Guid AllowMessageTemplate = new Guid("{B4E450FE-61CB-4BB0-A55A-978856E2DB3A}");
            public static readonly Guid EnableClickToSign = new Guid("{1B42E765-3882-4D6D-BDB1-9D3040FE3209}");
            public static readonly Guid PostSendingNavigationPage = new Guid("{4D39ADF5-6027-4D02-9CCC-BD5E6EC19AC5}");
            public static readonly Guid DefaultControlStyle = new Guid("{02E3C230-7F06-4ED3-8B03-F9EB40D6F80B}");

            public static readonly Guid EnableAutoFillTextControls = new Guid("{CC6E15A2-42F3-4B01-90CB-FCAD145FC636}");
            public static readonly Guid EnableWebhook = new Guid("{52BA8352-0C43-4ED5-B78B-A00E4F7BEEAE}");
            public static readonly Guid TypographySize = new Guid("{82349480-2C34-4D20-A1AB-9A76A3796462}");
            public static readonly Guid UploadSignature = new Guid("{54119165-12A1-4328-B3BB-12CEC2AFB312}");
            public static readonly Guid EnvelopeExpirationRemindertoSender = new Guid("{991CB9B0-6B2B-41FF-B397-7CF34C7CEF79}");
            public static readonly Guid SendReminderTillExpiration = new Guid("{1CAC5BC9-DB27-4A44-AD21-8C638A2D2AB9}");

            public static readonly Guid AllowMultiSigners = new Guid("{0FFE4738-8A86-4B57-964C-D59A3D514045}");
            public static readonly Guid SendConfirmationEmail = new Guid("{2AAD1D73-2493-492B-9B75-2EBDC5A28EC1}");
            public static readonly Guid DigitalCertificate = new Guid("{93C9F319-7535-4D70-8E39-4150F7AAB499}");
            public static readonly Guid AppKey = new Guid("{3766C4A8-66CD-4D6D-A38C-C81945EFFE72}");
            public static readonly Guid EnableCcOptions = new Guid("{A3AA2F33-97AA-4BD4-909B-ABE81A9BE5E2}");

            public static readonly Guid DisclaimerLocation = new Guid("{BC014CC1-4CD3-4F37-A63C-1DBF50155704}");
            public static readonly Guid DefaultLandingPage = new Guid("{62DA36E8-1D2D-4554-A702-BF191517EB47}");

            public static readonly Guid EnableRecipientLanguageSelection = new Guid("{3aa17446-330a-488e-ab8a-74964b69aa06}");

            public static readonly Guid RegisteredUserDefaultLandingPage = new Guid("{28184C63-DF3E-4F6A-9C30-7353B51CA9EE}");
            public static readonly Guid NonRegisteredUserDefaultLandingPage = new Guid("{BFE4028E-C1DD-4661-B65B-5710FE201262}");

            public static readonly Guid IsEmailListReasonsforDeclining = new Guid("{379022D9-550B-43B4-AC1C-0BB49AF0A59D}");
            public static readonly Guid DeclineReportSendTo = new Guid("{796340EF-12F9-4033-81C3-3817DC4232F1}");
            public static readonly Guid DeclineEmailSendingFrequency = new Guid("{2D3D14CF-F6E8-4CF0-B0DD-1DB8B9FC3834}");
            public static readonly Guid DisableDownloadOptionOnSignersPage = new Guid("{A60C10F5-E38D-4A16-B980-D0400671B490}");

            public static readonly Guid EnableSendingMessagesToMobile = new Guid("{85F73220-621A-4338-9D89-B31EA9EF4EB7}");
            public static readonly Guid DefaultCountryCode = new Guid("{10B657DD-DBAE-4D1F-B2AE-805A3EF2640C}");
            public static readonly Guid DefaultDeliveryMode = new Guid("{C8F0D382-4857-4FF6-82E5-DE93BFC73754}");
            public static readonly Guid RestrictRecipientsToContactListonly = new Guid("{FA557E7F-A6CA-4924-8780-AB3DB6E87174}");

            public static readonly Guid RequiresSignersConfirmationonFinalSubmit = new Guid("{DC6E46D4-EB79-4C24-9EED-07AC2E627767}");
            public static readonly Guid IncludeStaticTemplates = new Guid("{AB1E8432-6D5D-41B2-AD56-6B8558394559}");
            public static readonly Guid SMSProvider = new Guid("{C1414FAF-C14F-4DAF-9909-C3A318B22DBA}");
            public static readonly Guid IsAllowSignerstoDownloadFinalContract = new Guid("{6E1D8FBA-72E1-41C4-A0F5-233B974DAE24}");

            public static readonly Guid RenameFileToMode = new Guid("{459E1390-9372-4151-B45F-227B4A908250}");
            public static readonly Guid EnableMultipleAttachmentsCustomizable = new Guid("{C14D9873-20B3-4985-87D7-DD53F9144662}");
            public static readonly Guid AddPrefixToFileNameMode = new Guid("{64E4ED0A-56C7-4894-9DE5-479C846CD72A}");
            public static readonly Guid AddSuffixFileToMode = new Guid("{50323272-448A-4564-9917-969D592640BF}");
            public static readonly Guid DateTimeStampForMultipleDocSettingsForPrefix = new Guid("{97186854-5EC1-4D7E-918D-C74A6764581F}");
            public static readonly Guid DateTimeStampForMultipleDocSettingsForSufix = new Guid("{917B2EE9-2486-408E-8003-3981D4393757}");
            public static readonly Guid PrefixCustomName = new Guid("{8B35B73F-4B8F-497F-9BDE-B31465A60D79}");
            public static readonly Guid SuffixCustomName = new Guid("{C91E45C1-A269-44B2-9C16-FF0FEF85A5F1}");


            public static readonly Guid DisableFinishLaterOption = new Guid("{0F2AE946-F03E-4FB1-94C1-E0194B91241C}");
            public static readonly Guid DisableDeclineOption = new Guid("{00435995-6C35-4F5F-8108-03042072BA4E}");
            public static readonly Guid DisableChangeSigner = new Guid("{1B6ABBA6-0CF5-4FE0-A56B-5AEC7CA0312A}");
        }

        public static class SignatureType
        {
            public static readonly Guid Auto = new Guid("{05280C40-3DC8-479E-BD58-AE2FE0D62A97}");
            public static readonly Guid Hand = new Guid("{AEB3D4A0-9DCF-4441-98A8-8ECDB7169936}");
            public static readonly Guid UploadSignature = new Guid("{8D122A62-56FB-4118-9140-FCFF5FD66AC5}");
        }

        public static class ReminderDropdownOptions
        {
            public static readonly Guid Days = new Guid("{23821383-563D-4EE4-8F34-3595E8A469D1}");
            public static readonly Guid Weeks = new Guid("{08E957DB-5CA5-4F1D-AC65-EEABBB7CE6FD}");
        }

        public static class SizeDropdownOptions
        {
            public const string Small = "Small";
            public const string Standard = "Standard";
            public const string Large = "Large";
            public const string ExtraLarge = "ExtraLarge";

            public const double StandardWidth = 18;
            public const double StandardHeight = 18;
            public const double SmallWidth = 16;
            public const double SmallHeight = 16;
            public const double LargeWidth = 20;
            public const double LargeHeight = 20;
            public const double ExtraLargeWidth = 22;
            public const double ExtraLargeHeight = 22;
        }

        public static class SettingsAccessOptions
        {
            public static readonly Guid Yes = new Guid("{3EBDEF9D-DED7-4E9D-9D91-6D51B8524A44}");
            public static readonly Guid No = new Guid("{3C0F73FE-04B9-4FF0-9287-6EB8678FE343}");
            public static readonly Guid ViewOnly = new Guid("{BA6FFE11-F1BD-4319-B14D-61F7790AC9D7}");
        }
        public static class SettingsElectronicSignIndicationOption
        {
            public static readonly int Disable = 1;
            public static readonly int EnablewithEnvelopeID = 2;
            public static readonly int Enablewithtimedatestamp = 3;
        }
        public static class UserType
        {
            public static readonly Guid ADMIN = new Guid("{8718FB86-F7C3-4398-A4AC-024DB0F5B410}");
            public static readonly Guid SUPERUSER = new Guid("{40F2E21E-BE61-48BE-9452-07EC73EE21E5}");
            public static readonly Guid RSignSupport = new Guid("{CC4D13B1-06EA-4C1A-A2FB-D4E2B63A0BC0}");
            public static readonly Guid USER = new Guid("{5C9BD2AC-B92B-4BDB-B6D6-2ED65FE9F420}");
            public static readonly Guid NewUser = new Guid("{213F9E1C-649C-40B2-85BE-A69AD0B5F097}");
            public static readonly Guid MigratedUser = new Guid("{0BCABEA6-C684-479C-B84D-93349639D809}");
            public static readonly Guid Review = new Guid("{EA8C0A39-E258-4B77-A60B-01873DEBCF02}");
            public static readonly Guid MigrateCompany = new Guid("{6C8228A8-A789-443A-B4CC-F1DE78224C4E}");
        }
        public static class UserRoleCode
        {
            public static readonly int SuperUser = 1001;
            public static readonly int RSignSupport = 1002;
            public static readonly int Admin = 1003;
            public static readonly int User = 1004;
            public static readonly int ProfessionalServicesRep = 4;
            public static readonly int IntegrationsManager = 5;
        }

        public static class MaxLength
        {
            public static readonly Guid Ten = new Guid("{37136A24-7456-42F3-B153-232E98D09112}");
            public static readonly Guid Twenty = new Guid("{65009F0A-CD91-4033-8D1A-2D09A38B9BCF}");
            public static readonly Guid Thirty = new Guid("{E96D0ABD-37FC-4660-BC42-A6FE41DF6D7C}");
            public static readonly Guid Fourty = new Guid("{95507D6E-807D-46BC-8E2B-73DDE86F1A87}");
            public static readonly Guid Fifty = new Guid("{CD02DA0C-BAAD-432C-A7BB-FC2B9D9B27CA}");
            public static readonly Guid MaxLimit = new Guid("{3AC3281B-6F6B-4C0A-BD1D-8E87FABF3130}");

        }

        public static class TextType
        {
            public static readonly Guid Text_EN = new Guid("{26C0ACEA-3CC8-43D6-A255-A870A8524A77}");
            public static readonly Guid Text_ES = new Guid("{CBAF463C-8287-4C04-B90C-C6E2F1EC5299}");
            public static readonly Guid Text_PT = new Guid("{126AF3B7-409E-425E-A9C3-A313254ACB03}");
            public static readonly Guid Text_DE = new Guid("{F01331D9-3413-466A-9821-2670A8D9F3EE}");
            public static readonly Guid Text_FR = new Guid("{F690C267-D10F-40AD-A487-D2035D9C3858}");

            public static readonly Guid Date_EN = new Guid("{8348E5CD-59EA-4A77-8436-298553D286BD}");
            public static readonly Guid SSN_EN = new Guid("{DCBBE75C-FDEC-472C-AE25-2C42ADFB3F5D}");
            public static readonly Guid Zip_EN = new Guid("{5121246A-D9AB-49F4-8717-4EF4CAAB927B}");
            public static readonly Guid Email_EN = new Guid("{1AD2D4EC-4593-435E-AFDD-F8A90426DE96}");
            //public static readonly Guid Custom_EN = new Guid("{81EAA78E-55FC-4843-8593-D1D671D6ACD7}");

            public static readonly Guid AlphaNumeric_EN = new Guid("{B0443A47-89C3-4826-BECC-378D81738D03}");
            public static readonly Guid AlphaNumeric_ES = new Guid("{D6FBBFC2-C907-4290-929F-175EB437AA81}");
            public static readonly Guid AlphaNumeric_PT = new Guid("{73C17C33-F255-474F-9F46-248542ADDACC}");
            public static readonly Guid AlphaNumeric_DE = new Guid("{C175A449-3A22-4FE0-A009-C3F76F612510}");
            public static readonly Guid AlphaNumeric_FR = new Guid("{D1409FCF-5683-4921-A62B-2F635F4E49B7}");
        }

        public static class UserConstants
        {
            public static readonly string Superuser = "Superuser";
            public static readonly string Admin = "Admin";
            public static readonly string RSignSupport = "RSignSupport";
            public static readonly string User = "User";
            public static readonly string ComanyNameRepite = "companyNameRepite";
            public static readonly string AdminNotFound = "AdminNotFound";
            public static readonly string CompanyDomainValidation = "Provide atlest one domain for company.";
            public static readonly string CompanyRegistrationMessage = "Registration is successful.";
            public static readonly string CompanyNotBlank = "CompanyNotBlank";
            public static readonly string AlreadyDomain = "AlreadyDomain";
        }

        public static class EnvelopeType
        {
            public static readonly Guid Envelope = new Guid("{E6F16AED-8544-4DCC-AEB4-639478761F4A}");
            public static readonly Guid Draft = new Guid("{E6D863B3-846D-4B05-9005-67A9DEF17BF6}");
            public static readonly Guid Template = new Guid("{3420959A-BAC9-4912-A9D2-5472BC08C27D}");
            public static readonly Guid TemplateRule = new Guid("{6B3650FA-D25F-4E31-AB0A-11AE71FEDFC1}");
            public static readonly Guid MessageTemplate = new Guid("{ad63ab58-c067-4de9-ac14-c84c284e48b8}");
            public static readonly Guid DocumentGroup = new Guid("{37EE6ADC-78C5-44D5-BF6A-2A468DBBA531}");
        }

        public static class ConfigurationalProperties
        {
            public static class PasswordProperties
            {
                public static readonly string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                public static readonly int Length = 10;
                public static readonly int passwordKeySize = 128;
                public static readonly string completeEncodedKey = "NnlIQ3JGMWo3dXVRMi9pd01FR2JhUT09LDU2aTVpNzVYSXZoWTNkblNZZHZNd3c9PQ==";
            }

            public static class SessionValues
            {
                public static readonly string ContinueUsingScroll = "UserContinueTheSessionByScroll";
            }

            public static class PasswordType
            {
                public static readonly Guid Select = new Guid("{3702FE94-D7DB-45F4-86D7-8CC4791F7677}");
                public static readonly Guid Endtoend = new Guid("{FC14F65B-6FE9-4211-90FB-A03F241A55BE}");
                public static readonly Guid RequiredToOpenSigned = new Guid("{5D8162DA-C7A7-40BE-9CF1-442FB2524CA3}");
                public static readonly Guid SignerIdentity = new Guid("{6F95605F-AC41-4423-99EB-4FA9B1C24EDE}");
            }
        }
        public static class Resourcekey
        {
            public static readonly Guid RsignFeatureReminderLabel = new Guid("{C2A4AFA8-AD47-4F3F-A705-0012A06EBF24}");
            public static readonly Guid AddRecipients = new Guid("{816D417A-B3B0-4A54-A738-00BD7B8C99EE}");
            public static readonly Guid DeleteSignedContracts = new Guid("{6B4027A8-072C-4529-BB82-02315F27CACE}");
            public static readonly Guid SessionExpiredMessage = new Guid("{B9D51C87-87BB-4A0A-BB01-02360567E29F}");
            public static readonly Guid AccessCodeRequiredToOpenSignedDocument = new Guid("{F50FFA81-3992-43CE-9D80-0273F965BFF9}");
            public static readonly Guid Updatedby = new Guid("{FF8B9EE8-AD55-4C68-8453-02CD7A3055C4}");
            public static readonly Guid NoFileUploaded = new Guid("{363945CD-0687-4D74-87C0-02E5EC5FFE67}");
            public static readonly Guid eSignFeatureList = new Guid("{EBBF75FC-C885-4340-A373-037927C184C7}");
            public static readonly Guid Reject = new Guid("{C344955E-874A-4B46-9ED0-03897DCC67FE}");
            public static readonly Guid RejectedBy = new Guid("{71CA5FEB-C532-4A43-8869-03B36FDFECC3}");
            public static readonly Guid TextModalHeaderP = new Guid("{E626AEFB-4D77-4D23-96FF-04BF184C6979}");
            public static readonly Guid Messagesignaturetext = new Guid("{A0BC5FEC-B337-4B15-B8EB-04CAC4F163A9}");
            public static readonly Guid FALSE = new Guid("{1372287A-F6DD-49BA-AB90-06CAD1EB6BC0}");
            public static readonly Guid RoleHeading = new Guid("{6EC5A6B7-84E9-475E-A3C1-06D3B9CD6625}");
            public static readonly Guid Subject_EnvelopeRejected = new Guid("{4E9E254A-22AC-499C-B14A-06F5B531A9E4}");
            public static readonly Guid AdminEmailResponse = new Guid("{381076FE-7A67-4287-A510-078020E0C6C7}");
            public static readonly Guid lang_cntrlingField = new Guid("{72D31789-BCB4-49B5-8FCA-07E952011C97}");
            public static readonly Guid SrNo = new Guid("{A1F67F2A-C9AC-4342-BD58-087A0148D2B9}");
            public static readonly Guid Terminated = new Guid("{4FAA67B6-3847-4B02-A122-08DAF97BA966}");
            public static readonly Guid Open = new Guid("{C875780A-B5D9-4F60-87A1-0933B179FE09}");
            public static readonly Guid Numeric = new Guid("{A4539C35-C491-4435-BA4C-09891FEF2E77}");
            public static readonly Guid CreateStaticLinkforTemplate = new Guid("{F20ED49A-F9CE-4A38-9F88-0A481BC1B501}");
            public static readonly Guid PasswordInvalid = new Guid("{CC76B3B1-9BFB-4690-8614-0BB32FA307CB}");
            public static readonly Guid FileFormat = new Guid("{02DB7641-A475-433B-BB88-0C3712A07D35}");
            public static readonly Guid ProfileSaveSuccess = new Guid("{4A6AAA43-3279-4082-B2FB-0C5BD8C16F40}");
            public static readonly Guid EmailSuccessfullySent = new Guid("{FD33F192-FEE5-485D-8389-0D019A761989}");
            public static readonly Guid SuccessInitializeEnvelope = new Guid("{F09D694F-C854-47DF-B5D9-0D23D1839507}");
            public static readonly Guid EnvelopeData = new Guid("{66E777DF-365D-4DE4-9188-0DA8EEF29108}");
            public static readonly Guid SelectedLanguage = new Guid("{779B7FB8-A247-47D2-8B08-0DDD81FD2D37}");
            public static readonly Guid NoDraft = new Guid("{5D53E5CE-AD61-49B8-BB24-0E9A317CF638}");
            public static readonly Guid UnitsRemaining = new Guid("{786FA05C-0018-4F4C-B6B4-0F0DFAF25B2E}");
            public static readonly Guid SignedDoc = new Guid("{1FA50861-CA0E-41DD-8A6F-0F3ECE2458E3}");
            public static readonly Guid Name = new Guid("{391FF1F4-6A14-4782-A7C9-0FA49F36A104}");
            public static readonly Guid Checkbox = new Guid("{B5D83695-8B5B-4475-BAE0-0FC873C77DA1}");
            public static readonly Guid Templates = new Guid("{4A8C6D07-F4CA-41B5-BCD1-102CAE6615AE}");
            public static readonly Guid Width = new Guid("{22B062B3-E204-47B5-B179-10AE944565D7}");
            public static readonly Guid AwaitingMySignature = new Guid("{3B1099DB-A54B-4DB9-89C7-13020AD13EE1}");
            public static readonly Guid ContainsText = new Guid("{F4B6AC3B-006C-48D7-A085-13098A8818B3}");
            public static readonly Guid Address = new Guid("{C1841288-A024-42C4-868C-134A33C92B2C}");
            public static readonly Guid TypeSignature = new Guid("{72C42EB0-F203-4456-83DB-14EDB767DBDC}");
            public static readonly Guid MuchMore = new Guid("{F2A4C744-BEB9-48ED-9C41-16BA46502B11}");
            public static readonly Guid EnvelopeDeletedSuccess = new Guid("{87C71D7A-888D-43B4-9389-171CE36213F7}");
            public static readonly Guid title_DeleteselectedControls = new Guid("{E937C40F-4572-40F3-B261-1736689CBC09}");
            public static readonly Guid FinalContractContentN = new Guid("{AD1D84CE-4ADC-4B80-A8D0-17583CBF6F47}");
            public static readonly Guid Submit = new Guid("{CBD33E2D-8F50-4A92-B934-183D03D0EE34}");
            public static readonly Guid ReminderEmail = new Guid("{2B7F2DDE-F94E-45FE-A795-1903DF9A3912}");
            public static readonly Guid RadioButton = new Guid("{8C91184D-447F-443F-9914-194F3CE03E42}");
            public static readonly Guid SignerName = new Guid("{E440BF9C-E8FA-4727-B5CA-19B9D463BBD0}");
            public static readonly Guid EnvelopeDraftSuccess = new Guid("{FCAED700-04B8-41B9-ABA3-19D2CEF2265F}");
            public static readonly Guid title_pasteselectedControl = new Guid("{B0F7717E-B627-4AAD-9D5C-1A8359BF494F}");
            public static readonly Guid DefaultSignature = new Guid("{F10734C1-189F-4545-A2F0-1A8848AB1056}");
            public static readonly Guid Iagree = new Guid("{F54DEA9B-E74E-4280-B543-1AC466C16A79}");
            public static readonly Guid SignDocument = new Guid("{553133C6-3D00-4DE6-BC40-1BA68EF06D52}");
            public static readonly Guid lang_DeclineEnvelopeProgress = new Guid("{C2CB8495-BFBA-4F89-AAAC-1BB67A4E5207}");
            public static readonly Guid ResendEmail = new Guid("{181670B1-BA07-48E9-A5B7-1BBC585C96CA}");
            public static readonly Guid PendingBy = new Guid("{ACC3D802-EC47-42A3-B049-1C47DE76D80F}");
            public static readonly Guid title_sortEnvelopeSubject = new Guid("{C0AEBA7A-84A8-4CBC-8965-1CA33208B22A}");
            public static readonly Guid EnvelopeXMLContentN = new Guid("{65777238-DD71-4453-9D32-1D673B15DD5A}");
            public static readonly Guid viewDoc = new Guid("{B838349C-BD18-4933-9EA6-1DEA92617D95}");
            public static readonly Guid CompanyName = new Guid("{0FD9DABD-D58F-4E22-B4DA-1E85ABF2E760}");
            public static readonly Guid DateFormatStr = new Guid("{113E2BA8-F40A-47CC-9384-1F1E8CB58368}");
            public static readonly Guid AddMe = new Guid("{345A6F27-05AC-4B5C-9651-1F2070D39740}");
            public static readonly Guid TimeoutRefresh = new Guid("{351645D0-B7BA-4880-B5F2-1F2C26F4A6FC}");
            public static readonly Guid ApplicationError = new Guid("{FAAD2ADA-0E04-4D3C-ACDA-1F4FB62B3C48}");
            public static readonly Guid migrateNewUser = new Guid("{8510EC25-A8A9-4979-AE5C-1F76A05EAE60}");
            public static readonly Guid UPGRADE = new Guid("{79810927-83B8-4198-A038-1F8BD0303471}");
            public static readonly Guid alreadySigned = new Guid("{B899BF6C-2BF4-4856-97C0-200DE7DBBC35}");
            public static readonly Guid TypeYourName = new Guid("{85965CF8-F230-42FA-B2E6-20518AA6C115}");
            public static readonly Guid Myplan = new Guid("{E28556C2-E755-48F8-9C28-20845C302701}");
            public static readonly Guid uplaodLogoImg = new Guid("{B582C532-41D5-4FFE-AFAE-2086701CA87F}");
            public static readonly Guid SharedTemplatesandRules = new Guid("{3BFF00AB-8523-4167-BF3F-209ABFDACDD2}");
            public static readonly Guid AddMessage = new Guid("{96E9EA7D-13C9-47A7-9ECA-214B7FBF662D}");
            public static readonly Guid GrpNameMessage = new Guid("{033B5F51-E04D-4316-83D3-217B612C573D}");
            public static readonly Guid Link = new Guid("{A02A4EDC-13FF-4DA2-B400-226F998F4434}");
            public static readonly Guid signatureImage = new Guid("{FDA3A303-39EB-4569-B10D-22CBB3E53EC5}");
            public static readonly Guid Expand = new Guid("{4C13C666-B884-446B-B887-231095A6846A}");
            public static readonly Guid DownloadData = new Guid("{E9CC051A-29F2-4EF9-B3CE-234AFEFD8DDE}");
            public static readonly Guid LoadingDashboard = new Guid("{2465B554-9531-43E7-A63D-23B0125A9ED3}");
            public static readonly Guid MadeSimpleLabel = new Guid("{D4BE3E6C-1AA3-4A6C-91A9-2468420FFED4}");
            public static readonly Guid DelegatedBy = new Guid("{38562864-FAD1-410E-A54E-24B88D26CF1D}");
            public static readonly Guid AlreadyExistValue = new Guid("{E6326EFE-E8FE-495B-9D0F-24C80AD83117}");
            public static readonly Guid InvalidFile = new Guid("{0AAF35C9-B823-4504-B203-25065AF7EE4B}");
            public static readonly Guid Hi = new Guid("{EE0583CE-968E-4BCC-B32A-25252C31E152}");
            public static readonly Guid UserDelegatedPartOfEnvelope = new Guid("{8153A5AA-62E4-4E1E-A3AF-252FED2A800F}");
            public static readonly Guid DocumentSent = new Guid("{33C8D394-B5A9-4D8C-8032-259F4FCF99D0}");
            public static readonly Guid EnvelopeTerminated = new Guid("{DD2A2D57-CBF0-419F-8E48-25D449E313ED}");
            public static readonly Guid Draft = new Guid("{F0E1DC46-F352-4D69-A2FE-26BC4740C96F}");
            public static readonly Guid SessionLogOut = new Guid("{E1D5B882-A547-498C-AC7D-2872178EDE78}");
            public static readonly Guid LegalNotice = new Guid("{66C9D9F0-EC72-4AEC-8886-28F5701AF892}");
            public static readonly Guid Please = new Guid("{78F254F2-6687-4729-8F50-2A2AF57EE482}");
            public static readonly Guid migrateCompanyReview = new Guid("{B4158B5A-2B68-4D1C-944D-2A9E9318E0CC}");
            public static readonly Guid TextModalApprLength = new Guid("{44CDC1E6-1E05-439D-A106-2BF11201A1CC}");
            public static readonly Guid FromDate = new Guid("{9CF517C6-EB48-496B-88AE-2C5E9AFE85D1}");
            public static readonly Guid ThenSendReminderEvery = new Guid("{5036967F-6516-43A2-91DD-2C7BBB6950F5}");
            public static readonly Guid DocumentHash = new Guid("{806105C9-4D67-4A32-9862-2C93F44B17DA}");
            public static readonly Guid GENERALSETTINGS = new Guid("{1DFC1FA9-FC98-4ED1-B421-2D1C291BDFB6}");
            public static readonly Guid CompanyNameBlank = new Guid("{05295426-130F-47E5-8D53-2D661B3B803F}");
            public static readonly Guid DocumentDelegated = new Guid("{90FDBCFB-15EE-4AE7-A2C6-2DB3AB0A5C30}");
            public static readonly Guid DomainNameBlank = new Guid("{C9B03242-F53C-406B-8CDB-2DFBE1B8496F}");
            public static readonly Guid AdminEmail = new Guid("{9C07343E-11A8-4DFB-9C68-2E1D6334DFFB}");
            public static readonly Guid TRUE = new Guid("{438F4CC9-47D8-40D6-898A-2E8F1F9E1F38}");
            public static readonly Guid RPostTechnologies = new Guid("{A264B5C8-53C9-482C-8DFF-2FED0FD88E87}");
            public static readonly Guid Visitusat = new Guid("{AC16B6A3-A21D-4510-A23A-300A066A809B}");
            public static readonly Guid CreateRule = new Guid("{09EB67BA-4FF7-49AA-8728-3091428E6757}");
            public static readonly Guid Report = new Guid("{013710AB-5EB8-488D-AE07-319854850F67}");
            public static readonly Guid ReSend = new Guid("{74C263DE-AF18-4CFE-88BF-31F0552BAB0D}");
            public static readonly Guid PostSignerLandingPage = new Guid("{7DDA3A1D-BA8A-455F-A368-320EBEE6E0C0}");
            public static readonly Guid EnableContentEditing = new Guid("{0BA50979-6531-4B41-A524-3255E32E5336}");
            public static readonly Guid TemplateCreated = new Guid("{08530191-8CA2-464A-9B45-32C7116FCC6E}");
            public static readonly Guid Home = new Guid("{B77C9677-7217-4E21-9422-330C0CA983A1}");
            public static readonly Guid EnvelopeCancelled = new Guid("{E0BB7985-04E1-4366-B7C9-339B2C45DCF9}");
            public static readonly Guid SentFor = new Guid("{562FDEC3-33CF-4985-BBE4-343C85E8EC43}");
            public static readonly Guid AwaitingMy = new Guid("{815F46BC-7ABC-4EE8-A6F2-34574D29A769}");
            public static readonly Guid Company = new Guid("{78348D80-2C99-4C2A-9A19-3483966A5AD6}");
            public static readonly Guid title_Alignselectedcontrolstoright = new Guid("{45359ABC-23FC-4B40-8883-34DBA307C58B}");
            public static readonly Guid Date = new Guid("{0EAB0BE2-0420-4D2D-AC8E-34FF54A73A3A}");
            public static readonly Guid getStarted = new Guid("{A91F0BA8-4711-438A-ABAD-35012E00958D}");
            public static readonly Guid enterEmail = new Guid("{93668F73-7023-4E26-ABE7-356B620DA9F5}");
            public static readonly Guid supportedFormat = new Guid("{11C4F434-45E4-4534-8DA8-35E28187E4C5}");
            public static readonly Guid Documents = new Guid("{B8EE7347-F40B-4C57-87F5-369E44565ADB}");
            public static readonly Guid TermsServiceOncertificate = new Guid("{D296AC02-7D15-48C8-830E-36BF5B24ED91}");
            public static readonly Guid title_redoselectedControl = new Guid("{274FDD6E-8CAF-43AC-8E4E-399E5A17851C}");
            public static readonly Guid Changepaswd = new Guid("{394BE551-6883-4AD6-9067-39DD88731473}");
            public static readonly Guid resendToThis = new Guid("{9ED8DB87-8B5B-40AB-A3A9-3A0233A1FD2C}");
            public static readonly Guid CurrentUser = new Guid("{B0E1080C-4B0B-4FFB-9D22-3A05DA1FF594}");
            public static readonly Guid DuplicateFile = new Guid("{557033B5-3AD6-4179-9A46-3AA61346B535}");
            public static readonly Guid ResendToall = new Guid("{FFE08E94-4AF3-4F70-B147-3EB0AC2F52A5}");
            public static readonly Guid TransparencyContent = new Guid("{79F83826-BA58-45F7-8CA2-3ED169722984}");
            public static readonly Guid SessionKeepSign = new Guid("{C4F375B8-E46C-410D-A774-3F1C8B72A2E7}");
            public static readonly Guid CancelledDoc = new Guid("{69D32A88-F79C-4F51-AC8E-3F69F40799B1}");
            public static readonly Guid DefaultValue = new Guid("{1006CB52-2CDA-405B-9D2D-3FB854E6DA7A}");
            public static readonly Guid Prefill = new Guid("{712F1A0B-1AC6-4013-8D74-AAC4A9BF5568}");
            public static readonly Guid RsignNextLevelLabel = new Guid("{7DE04548-F7A4-4791-B4F1-41C4F57773C5}");
            public static readonly Guid EditEmailAndUserName = new Guid("{82E38999-A032-4BE8-B707-428D2744E763}");
            public static readonly Guid ManageDomain = new Guid("{E09856FB-3D73-4EF6-BD6A-42D55561FEB7}");
            public static readonly Guid AllSignersSigned = new Guid("{D44A9528-9621-407E-8B33-430970ED6501}");
            public static readonly Guid TermsOfService = new Guid("{626C7F9E-B167-424F-B195-43C99C4BCC3E}");
            public static readonly Guid Sessionpopupsigninbtn = new Guid("{C0B4BB8D-7DBC-4173-ABEA-43DA1F178862}");
            public static readonly Guid EnvelopeRejected = new Guid("{5E901B5B-588B-4FDE-B373-44DE1B02AA86}");
            public static readonly Guid DraftSuccessfullySaved = new Guid("{5F3D08A4-5384-4D56-B094-452171BF8EB5}");
            public static readonly Guid FinalContractContent = new Guid("{A7A9192B-FFA8-4E97-9964-45E7F3B2E295}");
            public static readonly Guid Type = new Guid("{1911745D-DB10-41E9-A6A3-465DE82553C6}");
            public static readonly Guid LastName = new Guid("{AF83E84B-291D-4855-AD1B-4689F23F8754}");
            public static readonly Guid Sender = new Guid("{26E35C91-5EE1-4ABF-B421-3B631A34F677}");
            public static readonly Guid NoEnvelope = new Guid("{C78D921D-C6FB-48A6-B931-46C17299DD78}");
            public static readonly Guid NoEnvelopeSearchMessage1 = new Guid("{3AB2CFC4-DDE0-4757-A263-AF3CF5BCF5A4}");
            public static readonly Guid NoEnvelopeSearchMessage2 = new Guid("{1AF71911-9DC1-427C-8801-88B9B2FDB017}");
            public static readonly Guid UserAuthentication = new Guid("{FC06967E-6DAC-4AF6-A0FF-46D05533F872}");
            public static readonly Guid ActivationMessage = new Guid("{A03D776E-5520-4D09-8C3C-470974D0B395}");
            public static readonly Guid Envelope = new Guid("{53384CC3-1AD6-4E75-8C1E-47121AE47491}");
            public static readonly Guid TemplateData = new Guid("{8A2879EE-E0FF-44D7-AFD9-4748CDFE5ABF}");
            public static readonly Guid title_viewAll = new Guid("{433B4C60-BE01-4C88-BF09-474EC1499D2B}");
            public static readonly Guid AllFieldsMessage = new Guid("{7B4DC04C-F3E3-4260-83FB-47AD03A27E46}");
            public static readonly Guid Myprofile = new Guid("{72D9CAC8-2414-48E9-AA3E-48BD66C5663E}");
            public static readonly Guid title_Alignselectedcontrolstoleft = new Guid("{BABF8DC9-A66B-45DA-85E7-48C2BCE32175}");
            public static readonly Guid DomainIDProvide = new Guid("{133A6E20-BB14-4B49-990E-492D8D6283FA}");
            public static readonly Guid EnvelopeTransferred = new Guid("{969A143F-1637-40A4-9D60-4942C443A0EE}");
            public static readonly Guid staticLinkEnabled = new Guid("{B396310A-B977-4664-98E2-4A232C80277A}");
            public static readonly Guid invalidSize = new Guid("{76BEE30E-F1E4-4850-B22C-4AAA66A06FE9}");
            public static readonly Guid Recipient = new Guid("{32128ED5-531A-4AB1-ACE6-4B8953C70D13}");
            public static readonly Guid Sessionpopupminutes = new Guid("{2EADAFC9-0378-4849-85F0-4D20C9C87210}");
            public static readonly Guid Signed = new Guid("{CA4F7024-9FA0-46B3-91B5-4D494877046B}");
            public static readonly Guid StatusDate = new Guid("{810E7ABD-0325-4238-96D0-4E12F08B40D1}");
            public static readonly Guid ViewPDF = new Guid("{5A1DE24B-ACD1-41BD-A300-4E646D5DECE8}");
            public static readonly Guid DownloadLinkOnManage = new Guid("{AECAE4AC-3DFF-476C-84F7-4EA9160D18E3}");
            public static readonly Guid AccessCodeToOpenSignedDocument = new Guid("{2E4DF021-1399-4531-9302-4EF4FE08D204}");
            public static readonly Guid Completed = new Guid("{9574A292-D570-43E5-9EF9-4F1ADCBF800C}");
            public static readonly Guid EnvelopeSubmitted = new Guid("{CA117F9E-0D73-4F67-8E5A-4F298D857E72}");
            public static readonly Guid Created = new Guid("{56B5F353-1CDB-4CF0-B782-4F4DB88EF84D}");
            public static readonly Guid Logout = new Guid("{7DCB707A-56AA-49B6-9CB7-4FE9CB4412B4}");
            public static readonly Guid Local = new Guid("{756661BB-2461-45C2-8113-5201EB58EBE5}");
            public static readonly Guid TemplateCode = new Guid("{C607560E-54F0-48F1-91ED-52D032B57962}");
            public static readonly Guid DraftIt = new Guid("{3BCA6EA3-E26B-412D-A50A-52F3C14EBEE5}");
            public static readonly Guid Sosionpopupsignoutbtn = new Guid("{E115E028-FDF2-4121-833A-52F784F561B2}");
            public static readonly Guid Expired = new Guid("{9473F326-C53F-407C-B7ED-5349C3469F79}");
            public static readonly Guid Yes = new Guid("{F0E0417A-2482-45FD-89B0-54269409999E}");
            public static readonly Guid lang_dependentField = new Guid("{E948C454-B0A1-47B6-9F5E-56E02F18E69A}");
            public static readonly Guid Staticlinktemplate = new Guid("{0C9B679E-7165-4FCC-85F2-578BB264872A}");
            public static readonly Guid ChooseFile = new Guid("{DFEEC95D-9BDD-4E49-B710-57AF70707015}");
            public static readonly Guid SessionKeepOut = new Guid("{01C61630-ABF3-4E28-BB0E-57E7F869C7BC}");
            public static readonly Guid Password = new Guid("{BE2EABAF-9096-4027-BE83-5802B8F3A50B}");
            public static readonly Guid FooterTextLegalNoticeLink = new Guid("{00CBE5D0-1393-46E6-AE4A-5806B45B0B1C}");
            public static readonly Guid RejectionReason = new Guid("{A99D3BFF-CF0E-4ADD-AF44-58B5CB29D9DB}");
            public static readonly Guid Sequence = new Guid("{3747D0FE-1021-4BCF-B22A-590779C19799}");
            public static readonly Guid Checklist = new Guid("{05985821-7ED3-4581-B982-5973D3CB8F01}");
            public static readonly Guid IsActive = new Guid("{E2C10EC8-06B9-4CEA-B6EA-59BAE0B9BD63}");
            public static readonly Guid Label = new Guid("{735BF0E9-FC01-40A6-82A3-5A037A3FF9B5}");
            public static readonly Guid ConatctSender = new Guid("{411CECE9-2884-423D-A83B-5B02AAD11ED7}");
            public static readonly Guid Signature = new Guid("{CC8F7E49-E9CF-41A2-9D7B-5BBC4759D9CA}");
            public static readonly Guid AlreadySignedUp = new Guid("{95106BFF-97C8-4DF4-88BE-5CD8E4928095}");
            public static readonly Guid Localmachine = new Guid("{7FD82CF2-6413-418A-B3A7-5D2400CE1808}");
            public static readonly Guid EnvelopeCompleted = new Guid("{117327B1-C2B6-4D7B-8700-5D252D9D1822}");
            public static readonly Guid Download = new Guid("{C4BF6ECB-D741-4F1F-B1CC-5D80A066F2F8}");
            public static readonly Guid ResendAndEditUpdatedBy = new Guid("{2BB3AC8D-00A0-4FA1-9832-5E0466D86BE8}");
            public static readonly Guid AvgTimeOfCompletion = new Guid("{BAD02A17-381E-40D9-A35E-5E37D2BB0F3B}");
            public static readonly Guid Dimensions = new Guid("{A1E86660-4235-451E-8914-5E9F6D8CC702}");
            public static readonly Guid EditProfile = new Guid("{52AE55CE-AECA-4537-AE1C-602D22855A67}");
            public static readonly Guid ToDate = new Guid("{A4E832B8-D0D8-4C7B-9A47-603B2E0FAFB5}");
            public static readonly Guid EmailAddress = new Guid("{04F496B6-4135-42F9-B2C0-604C569B0D0C}");
            public static readonly Guid Comments = new Guid("{9BAC9CFB-7EC4-4C2B-88E1-9DCC65D430BE}");
            public static readonly Guid EmailAccessCode = new Guid("{FE1C80C7-E56B-4EF3-A15A-607E63D9652F}");
            public static readonly Guid AccessCodeRequiredToSign = new Guid("{38AE42E3-11A1-41A6-8414-61162BC635DD}");
            public static readonly Guid freeTrial = new Guid("{A6FD5C43-BF20-49C5-A321-616DD7ADC006}");
            public static readonly Guid TimeZone = new Guid("{7D5EBF97-49DD-4A70-8D2B-620EE152EDA9}");
            public static readonly Guid EnvelopeCancelledFailed = new Guid("{53F81332-AEB1-4E3C-B885-621DBEFBB105}");
            public static readonly Guid EnvelopeContentNotFouund = new Guid("{F756A843-EA16-4585-B403-62B207136A0B}");
            public static readonly Guid Pagesize = new Guid("{1B627C82-5E57-4267-9DE1-6306206B7676}");
            public static readonly Guid DeleteFinalContractSuccess = new Guid("{C5A535B8-2B64-4206-878F-635081599076}");
            public static readonly Guid UpdateUserSetting = new Guid("{BFAD21AD-512C-4C06-BFE8-643E52FEC8A2}");
            public static readonly Guid VisibleRequired = new Guid("{8EA7ED1B-ECB1-4CAB-B85E-6499C455381F}");
            public static readonly Guid SigningDocumentCompleted = new Guid("{E474057C-CEB4-427A-A3CF-64D77C90193E}");
            public static readonly Guid SignUp = new Guid("{0A2A4C06-A2BF-446F-97D9-655B281B63CB}");
            public static readonly Guid Lang_SignUpCamelCase = new Guid("{A21C17B5-64BB-4DCF-8B5A-612D5CEF828A}");
            public static readonly Guid CompanyURL = new Guid("{C09E3C0B-3272-4E15-9DF9-659DA6A89F2F}");
            public static readonly Guid ExpiringSoon = new Guid("{30B08A89-259F-4927-9888-660FDA431095}");
            public static readonly Guid FinishLater = new Guid("{B50BBC95-D622-4C6C-94FF-664629F1C524}");
            public static readonly Guid EnvelopeAlreadyDelegate = new Guid("{7B6B87A6-491F-4FFC-AA28-665897019D10}");
            public static readonly Guid EnvelopeExpired = new Guid("{76BE755F-A451-42B7-8417-665D55867920}");
            public static readonly Guid PrintTS = new Guid("{CD5A23DF-9FCF-49CA-816A-66B2E7A0E422}");
            public static readonly Guid Rejected = new Guid("{2E4496F4-677B-4B24-9B8C-66CABDDC4CED}");
            public static readonly Guid title_resendtoAllButton = new Guid("{E42114A8-1D3F-4BA7-BAD6-676401A8C257}");
            public static readonly Guid Height = new Guid("{54010589-9302-42BF-888E-677D763AC631}");
            public static readonly Guid Signer = new Guid("{C20C350E-1C6D-4C03-9154-2CC688C099CB}");
            public static readonly Guid EnvelopeXMLContent = new Guid("{3E37FE99-B960-46EA-AA54-680414DE400B}");
            public static readonly Guid IncompleteAndExpiredDoc = new Guid("{7BFCBD48-0A4B-4DE1-B4B9-688E96EDA2AE}");
            public static readonly Guid CurrentEmail = new Guid("{5B375A19-7224-4877-895D-68C6B0FA5B33}");
            public static readonly Guid ExpiresIn = new Guid("{ACE132A6-3F58-4B3B-BE82-691446C0A0B8}");
            public static readonly Guid UploadNew = new Guid("{D27CA932-6095-4F9A-8918-694729C1F9EA}");
            public static readonly Guid DocumentEvents = new Guid("{71CCE528-6FC1-45AF-95AE-6980862D14E7}");
            public static readonly Guid lang_SavingControlDetailsProgress = new Guid("{CF1B69FC-6E9A-4267-A1D1-6A494F393781}");
            public static readonly Guid PersonalTemplates = new Guid("{2D0F6F50-EECB-4A84-A76A-6A58C82FC66E}");
            public static readonly Guid Patented = new Guid("{09D88CDF-7416-45E1-B626-6AAE7FDA9EDC}");
            public static readonly Guid Prepare = new Guid("{D291245F-5C98-46B1-BB29-6AC823A3BAB4}");
            public static readonly Guid Sign = new Guid("{D6BA71C2-91A4-41CE-ABE8-6AD9CD5F757A}");
            public static readonly Guid Remove = new Guid("{1342D97C-743C-4F29-AD78-F12A16743009}");
            public static readonly Guid SeparateMultipleDocumentsAfterSigning = new Guid("{95266884-7C92-46FC-8A32-6C31D078EF6D}");
            public static readonly Guid Recomemded = new Guid("{04380F78-9009-4573-96E4-6CF3FFD72D10}");
            public static readonly Guid ShowSettingstab = new Guid("{BF384411-1B3D-4E3F-A5B5-6CFBBEB34C64}");
            public static readonly Guid SpecifyaTimeZone = new Guid("{15B93935-8750-4F3F-8333-6D2B1C31252B}");
            public static readonly Guid RecordUpdate = new Guid("{59DC22BF-21A1-42B9-BBD1-6D700686AC2E}");
            public static readonly Guid Initialed = new Guid("{C50DDA04-9E80-46CA-A9BD-6DF4953F5F53}");
            public static readonly Guid Required = new Guid("{93F9CE7C-3295-4B61-AA5B-6E116BC5F078}");
            public static readonly Guid PendingDocuments = new Guid("{6EB1298A-FAB8-43C9-B8B8-6E17934A09B5}");
            public static readonly Guid RememberPwd = new Guid("{CB9AF93A-A8B3-46CC-8C16-6E5799A71C3A}");
            public static readonly Guid Initials = new Guid("{0F001FA1-C71F-490B-93CF-6E5A55ABAAD5}");
            public static readonly Guid Subject_DocumentDelegated = new Guid("{9E9E0E40-B6A3-414A-888E-6EA320E58DAA}");
            public static readonly Guid XMLCreateError = new Guid("{2BA77314-AD49-40DB-8781-6EB7F76EAF8B}");
            public static readonly Guid WrongDelegatedUser = new Guid("{64C1C12A-4EDF-4A32-97B4-6F0EA29B8DC7}");
            public static readonly Guid CompanyDescription = new Guid("{B7743F69-C687-4473-8C61-6F463DC94BEC}");
            public static readonly Guid rangeBetween = new Guid("{061EE9E3-0439-41A2-98BA-7015FA726271}");
            public static readonly Guid VerifyDimension = new Guid("{4F3F27D4-7715-478F-941E-70EFFAFB9960}");
            public static readonly Guid JobTitle = new Guid("{4AF0FEFD-8EE9-44E0-B240-717D59072231}");
            public static readonly Guid ChooseSignature = new Guid("{4DA35121-888A-488C-A466-71F7C8D8B22A}");
            public static readonly Guid RsignFeatureTrackingLabel = new Guid("{4F164C4D-9408-4BDB-8CED-72FBE60A4EFB}");
            public static readonly Guid RuleData = new Guid("{10E71507-EFAF-4DBD-B1A6-73454AB9DD19}");
            public static readonly Guid preSelected = new Guid("{5AD8935E-32C4-4C0A-AE16-738053932F3E}");
            public static readonly Guid NoFileSelected = new Guid("{765D0E22-BFEE-469B-B7EF-75133E8BF292}");
            public static readonly Guid PhoneNumber = new Guid("{DB5FAE8A-C60A-4C74-89D6-7531063262BC}");
            public static readonly Guid Save = new Guid("{99ACCB6E-0617-46DA-A539-75AB0F047ED6}");
            public static readonly Guid lang_processing = new Guid("{9ECF474A-193C-45D5-B1DD-760B5D1C32A8}");
            public static readonly Guid Add = new Guid("{06547D5E-8427-4956-A8A3-76211E98B1C7}");
            public static readonly Guid title_Alignselectedcontrolstocenter = new Guid("{7DE4AB94-982C-4AFF-8A56-7649333D80C4}");
            public static readonly Guid title_viewEmailBody = new Guid("{A70BFF36-2625-47AA-99AF-767C629F84F6}");
            public static readonly Guid EnvelopeDelegatedTo = new Guid("{ECBAC9EB-37A9-4D64-95B7-7731C258BB6D}");
            public static readonly Guid Sync = new Guid("{3D4277E0-0CEE-4D03-9E9D-789264C89CD0}");
            public static readonly Guid NoUsersForThisCompany = new Guid("{34F77774-B946-4C25-9D34-790EB961F2F7}");
            public static readonly Guid responseFinishLater = new Guid("{C8C1D755-0ABF-40C4-BE27-79EF8DDF5C08}");
            public static readonly Guid lang_Actions = new Guid("{4C70C532-730C-4C21-871B-79FEFA3F1D06}");
            public static readonly Guid RsignFeatureShareTemplateLabel = new Guid("{FDEEA25E-55E9-4CA9-A0CC-7A4340221794}");
            public static readonly Guid TemplateEditable = new Guid("{5B0E5841-7C67-40D0-98E7-7B501F1EDC48}");
            public static readonly Guid RsignFeaturePrefillLabel = new Guid("{A5B5C9CB-A383-4540-B909-7B7D52ECBF9C}");
            public static readonly Guid Sent = new Guid("{CEA7D4C5-14C4-44B6-9B42-7BE91A40FF61}");
            public static readonly Guid Enable = new Guid("{6FB06C1E-0291-418A-A668-7BED3373E3E5}");
            public static readonly Guid RuleCreatedUpdateSuccess = new Guid("{16406FEC-E895-4C5E-B740-7C234C383FAD}");
            public static readonly Guid TerminatedDoc = new Guid("{B4C7CDE2-E31B-4EA5-8875-7C2F2666D8B7}");
            public static readonly Guid NoTemplateRule = new Guid("{70303EB9-3E6E-4884-AE85-7C671C11157D}");
            public static readonly Guid RsignFeatureReportsLabel = new Guid("{E589EFC1-51E7-47B0-870C-7CC37F975948}");
            public static readonly Guid RejectionComment = new Guid("{DE49519B-C69E-4742-91C8-7D013FBFFA9B}");
            public static readonly Guid EnvelopeID = new Guid("{9C2A99C8-DF21-4FCF-9968-7E2F5A8629BC}");
            public static readonly Guid enterValid = new Guid("{981497D4-EEE6-4730-BB9D-7E52E723D313}");
            public static readonly Guid CompanyIDProvide = new Guid("{AC6AE96B-ED23-44F2-9F60-7E765ADDFE28}");
            public static readonly Guid TextType = new Guid("{6E90B110-5AC8-4B23-A086-7EEAC8F41154}");
            public static readonly Guid Share = new Guid("{C2D79D51-E607-489C-8CA3-802068DEA247}");
            public static readonly Guid RuleCode = new Guid("{78F7E70A-5F8A-4DF9-9F90-805CC8E20634}");
            public static readonly Guid No = new Guid("{0E9CDDC4-25BF-4BF0-AD53-80D8C89E185D}");
            public static readonly Guid CreateCompany = new Guid("{27833374-F166-4C5E-862C-8136B7DCF23F}");
            public static readonly Guid title_copyselectedControl = new Guid("{E48BEFC1-E152-44AB-8EBB-82223C148A42}");
            public static readonly Guid ToLearnMoreAboutRPostServices = new Guid("{2FA3250E-7C3E-4319-910C-83BA77D7BB23}");
            public static readonly Guid Subject_EnvelopeSubmitted = new Guid("{90B2BD0A-18B5-466C-B80D-8453CE4067F7}");
            public static readonly Guid ExportToPDF = new Guid("{E4807FAA-76C7-4487-ADF2-84D34263DD23}");
            public static readonly Guid DeleteFinalContractFailed = new Guid("{3F4386B1-91CA-4D6A-BA91-8570FD1904C5}");
            public static readonly Guid ResetPaswd = new Guid("{D4F33217-9AC7-4DD4-83DC-860230BB9B79}");
            public static readonly Guid DownloadTS = new Guid("{7C89B3BB-9097-4815-BAAE-868010FAFCA4}");
            public static readonly Guid DelegatedTo = new Guid("{A5F1954D-C956-4D28-8A41-86D9A78DAF49}");
            public static readonly Guid SessionAboutExpire = new Guid("{04C148F5-E4B8-4EE7-A7B8-87152953E8AA}");
            public static readonly Guid lang_accessAuthPswdTitle = new Guid("{BC9FC450-934E-4941-860E-877CE083BBBA}");
            public static readonly Guid AddSignatureCertificate = new Guid("{700361D4-36E6-434A-8925-87B68710395C}");
            public static readonly Guid CancelledTrans = new Guid("{41B582AE-B4F0-46C8-8AD4-8942973557AF}");
            public static readonly Guid sampleDoc = new Guid("{BD55D09E-EAFD-4D84-9CFF-899263BF1253}");
            public static readonly Guid Template = new Guid("{0924B054-2E88-4E0B-9F26-8A3DBD2121AD}");
            public static readonly Guid EnvelopeAlreadyDelegated = new Guid("{80A13FCC-4900-44C2-94E2-8A840733FEAA}");
            public static readonly Guid ForgotPassword = new Guid("{E4FCD335-539F-4A13-AE60-8B4F98D1CAA3}");
            public static readonly Guid Prev = new Guid("{5F2140EB-6923-43F2-8ED4-8B5CB3A0164E}");
            public static readonly Guid OldUser = new Guid("{2B501A77-5A69-4DD2-85F3-8B8B87EB5217}");
            public static readonly Guid All = new Guid("{F874B17E-F1E9-4195-8687-8C4D0C6067F5}");
            public static readonly Guid EnvelopeSessionNotSet = new Guid("{10EA367D-6A8F-4ADC-8179-8C70CE26385B}");
            public static readonly Guid Document = new Guid("{23930417-0123-4AAB-8B7F-8CD783FDD770}");
            public static readonly Guid lang_DelegaeEnvelopeProgress = new Guid("{66736C91-24D1-4B1A-BFEB-8DB3095505A8}");
            public static readonly Guid title_sortEnvelopeDatetime = new Guid("{BE3F22BF-D20D-4F5C-897B-8DC348556BC4}");
            public static readonly Guid UploadImgError = new Guid("{155A9150-3948-4721-B623-8E01621F3F93}");
            public static readonly Guid anApplicationErrorContact = new Guid("{91DF60FD-C9BC-4FEF-9331-8E372675F310}");
            public static readonly Guid TimeoutRedirect = new Guid("{9687C9A5-A28F-41CC-9370-8F1FA33FC359}");
            public static readonly Guid Sessionexpiredialogheader = new Guid("{FD82ADD2-72EA-4CD9-8484-8FCD0B32A7F8}");
            public static readonly Guid EmailAdminLoinSub = new Guid("{7D9E2F1C-B048-44DB-9C07-9014E2D54EC4}");
            public static readonly Guid EmailSendingFail = new Guid("{288C5B6C-146B-4661-858D-903CA09E859B}");
            public static readonly Guid SentDocuments = new Guid("{17286B4C-8888-4E8E-9939-91C0D7750E5F}");
            public static readonly Guid Dropdown = new Guid("{81C0A7D6-88B4-4BB2-85C8-91F88022BB71}");
            public static readonly Guid AttachXML = new Guid("{65347815-79BA-4E57-9AC5-921289EE098D}");
            public static readonly Guid DocumentHistory = new Guid("{06FAA940-4879-47C3-AFCE-927894DEE691}");
            public static readonly Guid title_otherOwnerTemplate = new Guid("{771F1CED-6B1D-4D45-9416-92FC4603C738}");
            public static readonly Guid ds = new Guid("{2E96F0A1-1BB4-4D27-A8DE-934BC0BEE1C6}");
            public static readonly Guid CompanyProfile = new Guid("{5680ED93-3666-4994-B8E2-947245B06A69}");
            public static readonly Guid ViewedBy = new Guid("{AEC45EA8-8E51-414C-98FF-951D43243526}");
            public static readonly Guid DropdownOptionList = new Guid("{814FEEF9-B6DC-4BB7-81ED-964E46EE93CB}");
            public static readonly Guid RecipientAttachment = new Guid("{A09C93B2-837F-494D-A953-9714B05DFFB6}");
            public static readonly Guid SignerAttachmentOptions = new Guid("{9E6663BD-92E9-4194-8822-4E61AF07FACA}");
            public static readonly Guid AllowRecipienttoAttachFileswhileSigning = new Guid("{B10F7B58-2044-4E49-B32A-C0FE7951E29A}");
            public static readonly Guid NoTemplate = new Guid("{25E5FF6E-2387-4D1E-BB34-97584799FEB5}");
            public static readonly Guid Sessionpopupsigninmsg = new Guid("{2450F2B5-C3AB-4F3F-BD69-986D677CFFCF}");
            public static readonly Guid Filename = new Guid("{53E5EC53-BDB9-412E-84AE-987AFB08BA48}");
            public static readonly Guid Title = new Guid("{A17FC9D6-CA82-4E80-9F6B-9974AA1709F7}");
            public static readonly Guid PreviewDocument = new Guid("{B50DE0F3-9048-4B8C-B780-9BA1DF610654}");
            public static readonly Guid DocumentDetails = new Guid("{BDC91145-B75D-4C12-9D2D-9C6FBCB5694A}");
            public static readonly Guid DocumentRetrieval = new Guid("{82933370-3865-4411-8719-9C73450BE77C}");
            public static readonly Guid AddTempSignatureCertificate = new Guid("{164CBAFE-659D-4201-BF1A-9CC7A00F2095}");
            public static readonly Guid SignedDocuments = new Guid("{95F2EBC1-008F-442A-BA40-9DC8FE2CBBDA}");
            public static readonly Guid SupportedDoc = new Guid("{711E4774-9989-4739-BF6D-9E43FA5902B7}");
            public static readonly Guid NoTemplateorRules = new Guid("{5B0E58D1-7966-4D31-AF13-9E4AA2648974}");
            public static readonly Guid DownloadTransparency = new Guid("{BC625635-7C50-4B37-81B5-9E9DCB9B6505}");
            public static readonly Guid IncorrectURL = new Guid("{CDCBF7EA-367C-4496-A27F-A009C505B864}");
            public static readonly Guid Disable = new Guid("{473E44BC-133C-4924-8844-A04A6F1D3161}");
            public static readonly Guid CarbonCopyEvents = new Guid("{2270CF18-A94B-4300-A00F-A0ABF9BF299D}");
            public static readonly Guid RsignFeatureRecipientLabel = new Guid("{4ED695FC-6D5B-4AA0-9A58-A0DF8A4F18B7}");
            public static readonly Guid Rejection = new Guid("{2435AC30-718E-44B0-AB84-A0FB5EF7835D}");
            public static readonly Guid CompanyLastAdminActive = new Guid("{A447AA98-BD0B-460D-A851-A1B7ED82AED5}");
            public static readonly Guid RuleEditable = new Guid("{8357F427-9FD2-48CD-9735-A1E4B9D89DB0}");
            public static readonly Guid DocumentSubimittedSuccessfully = new Guid("{0D302E16-0448-46B4-9FB5-A229AE9B726E}");
            public static readonly Guid DropdownList = new Guid("{D4BBFB45-6956-4993-9B99-A22B63ACE089}");
            public static readonly Guid DeleteEmailBody = new Guid("{BFB9B9BF-D468-49C4-BC41-A3D0447F0AE4}");
            public static readonly Guid CompanyLogo = new Guid("{A7C8CACF-995C-4920-ABF0-A435DE4B007D}");
            public static readonly Guid lang_ErrorSomethingWrong = new Guid("{41F007D6-5931-439E-BE5B-A4FA9A63F535}");
            public static readonly Guid confirmPassword = new Guid("{3F29D0D7-C911-4574-B37A-A50DFE75DC99}");
            public static readonly Guid ExpiresOn = new Guid("{918CCBED-C91C-49BC-A90A-A61F8DC72550}");
            public static readonly Guid SpecificText = new Guid("{879FCB96-AEEA-4D67-97D9-A6A961D56B7C}");
            public static readonly Guid EmailResent = new Guid("{9AFC556B-ADF1-4362-8D00-A6CD4F695B94}");
            public static readonly Guid title_sortEnvelopeCode = new Guid("{B80C9381-14E6-49C4-9671-A7BE05531D08}");
            public static readonly Guid Unitsent = new Guid("{C2B717C8-1D91-436E-B325-A7C59EF68884}");
            public static readonly Guid newSubscription = new Guid("{6FBDA172-1C83-448D-8614-A7D88C3DAE19}");
            public static readonly Guid Event = new Guid("{E1B30131-0D14-4374-9640-A805693899A8}");
            public static readonly Guid title_sortEnvelopecurrentStatus = new Guid("{032B231F-3A83-4061-B726-A85046A17329}");
            public static readonly Guid AccessAuthentication = new Guid("{4FC99518-EF53-44F3-8BB5-A893C574C7DD}");
            public static readonly Guid Step = new Guid("{1CB5E303-3853-4F7B-A7D1-A8F7C26DCB8D}");
            public static readonly Guid SignedBy = new Guid("{F2E6C452-6515-493D-8ACA-AA2A6E77994F}");
            public static readonly Guid AddDomainName = new Guid("{179855DD-AD43-4652-AB0C-AA7DB53C130B}");
            public static readonly Guid Cancel = new Guid("{66B1DC00-482B-4980-8710-AAFC3DADA3C3}");
            public static readonly Guid Personal = new Guid("{97736E5F-2913-41D2-8B10-AB2B8CCEC8ED}");
            public static readonly Guid Update = new Guid("{57A8EA2B-246A-4491-944B-ACC90CE67CE6}");
            public static readonly Guid Planname = new Guid("{D04991CF-D54B-404C-8878-AD2768F180DE}");
            public static readonly Guid IndividualPlanType = new Guid("{77814EBA-65D7-4BD1-B892-AD3A99BDD74F}");
            public static readonly Guid Sessionpopupseconds = new Guid("{3367D21B-93C0-48F7-B299-AD51327DD48C}");
            public static readonly Guid title_undoselectedControl = new Guid("{083BE54A-C836-4794-965D-AD6853DA5DBD}");
            public static readonly Guid Soon = new Guid("{B5B17ABD-C6FA-46E9-B203-AD92AD9F8627}");
            public static readonly Guid DateFormatEU = new Guid("{D0E30CFA-955E-430C-BB21-ADE421FC934B}");
            public static readonly Guid SendReminderIn = new Guid("{1C6B0B1B-205C-490F-A4CD-ADE9A1C65D5A}");
            public static readonly Guid FooterTextParantedLink = new Guid("{FE18BFAB-9DA2-4905-81A6-ADF74687B91B}");
            public static readonly Guid Notinitialed = new Guid("{D1B91C22-F004-494D-89D5-AE05B255734B}");
            public static readonly Guid eSignServiceFeature = new Guid("{C88C2043-E52F-40CD-9DCB-AE4188766703}");
            public static readonly Guid DateFormat = new Guid("{DB6ECFE0-ABE6-44E2-AA3B-AE8819F0C5C3}");
            public static readonly Guid RsignFeatureLabel = new Guid("{5AC9D2C0-B471-4F9C-AB57-AEAB6D9D43C3}");
            public static readonly Guid emailBody = new Guid("{9D45ADB4-DF15-4140-AA93-AED0F568B862}");
            public static readonly Guid OutFor = new Guid("{786EF75C-6DA6-47C8-910F-AF7F18CB2EA8}");
            public static readonly Guid AnyText = new Guid("{6475D52A-43A3-4071-8B2B-AFA43C212E6D}");
            public static readonly Guid Option = new Guid("{41036A50-AAA3-410C-8723-B0057F037609}");
            public static readonly Guid AdminEmailWrong = new Guid("{948B60EA-096F-4E35-BCEA-B088D4DD3817}");
            public static readonly Guid Expiring = new Guid("{B1FECDFE-AE54-41A5-ADE8-B0AC06EB5B06}");
            public static readonly Guid Status = new Guid("{0EBF7088-E4B7-4692-8DBE-B14A6100850C}");
            public static readonly Guid TimezoneHelp = new Guid("{5C7F6C25-C0BE-488B-BF09-B1E77BED56C0}");
            public static readonly Guid WaitingForSignature = new Guid("{E380FC43-4AB6-444B-96B3-B1EDAFAC36B7}");
            public static readonly Guid ProfilePicture = new Guid("{86B264B3-1045-42A8-BB41-B25F87EABD08}");
            public static readonly Guid Return = new Guid("{9D85A8DE-E9A7-4EDC-BAE1-B29D383CB641}");
            public static readonly Guid Code = new Guid("{7EA5E590-FC03-4D80-91B3-B312177DD486}");
            public static readonly Guid FirstName = new Guid("{157F519D-455A-4C71-A8BF-B3217AD074F4}");
            public static readonly Guid DateTimeStamp = new Guid("{A022376B-9495-4EAA-8CFE-B3293618BEBE}");
            public static readonly Guid Drives = new Guid("{1269B6E7-E2DA-4CFE-B458-B330CB279E21}");
            public static readonly Guid lang_resendActivationLink = new Guid("{D1766F62-E2EA-42A9-AD89-B3C14B9784CD}");
            public static readonly Guid Continue = new Guid("{38F7EC6A-02D4-4D13-8498-B3F089AB057E}");
            public static readonly Guid FormFieldAlignment = new Guid("{A2AB70D9-CBC9-4F21-A110-B46DEC8D4F77}");
            public static readonly Guid RsignFeatureAdminMonitoringLabel = new Guid("{D2C1E19C-AE7F-4A4E-87A6-B5216C55ADA5}");
            public static readonly Guid CompanyAlready = new Guid("{D3D9BC71-671C-45D2-BAEC-B5308A0794EA}");
            public static readonly Guid Email = new Guid("{982A95AB-6FE0-4764-A8E1-B5760121CCBC}");
            public static readonly Guid Subject = new Guid("{A521C3A5-5AEF-4E26-BFBD-B5D6B3E1054D}");
            public static readonly Guid TemplateCreatedUpdateSuccess = new Guid("{2794A634-47A4-4E52-9844-B607C2CB38D6}");
            public static readonly Guid AccessCode = new Guid("{D052E338-5447-4619-B07A-B61D6F31EA25}");
            public static readonly Guid User = new Guid("{C6661AE6-8E62-4165-92C4-B650EE2ACCB1}");
            public static readonly Guid SignatureBlank = new Guid("{95483640-1100-4E46-BEB2-B6FCE404357E}");
            public static readonly Guid HandDrawn = new Guid("{191DDB81-C976-4381-B40B-B76D42A506BE}");
            public static readonly Guid AttachFiles = new Guid("{9ED198C0-A1CD-4855-A6BB-B883EA3D4153}");
            public static readonly Guid AlternateEmailAddress = new Guid("{E794F8DB-7D20-43C4-B541-B990E40B81EC}");
            public static readonly Guid PageNotSet = new Guid("{DE66E7B7-C2F3-45B0-8513-B99BEBE53ECF}");
            public static readonly Guid Recipients = new Guid("{C5F1A5C7-11A2-424C-9186-B9D23A1CBFA6}");
            public static readonly Guid SignerSignatures = new Guid("{0F24BB34-8862-484C-ABEC-BA8E2AEDB0BF}");
            public static readonly Guid CompanyNot = new Guid("{470A616F-09E8-4F47-B65B-BAA7E04F0A9B}");
            public static readonly Guid IncludeTransparencyDocument = new Guid("{EA0FF30F-6545-4D0D-A66B-BB96A99BCA4B}");
            public static readonly Guid VisibleNoRequired = new Guid("{A1D2A72B-F130-4DA4-9E99-BB9D7A70AB0A}");
            public static readonly Guid Hello = new Guid("{A2B9797E-4120-4EBB-B602-BBA2D1F25F00}");
            public static readonly Guid Unshare = new Guid("{3B1F67ED-ADED-4A83-A4DF-BCD9D4D0ED90}");
            public static readonly Guid Accepted = new Guid("{D9AA6F04-E337-4332-9D81-C0A1DDF0F635}");
            public static readonly Guid CancelTrans = new Guid("{2BDC9E02-047E-4626-B6FD-C12CC5FC12E9}");
            public static readonly Guid DefaultSettings = new Guid("{0D0E1A54-CDB5-464F-A04D-C2A278839C08}");
            public static readonly Guid Options = new Guid("{61F98795-6DFC-4A84-8155-C34EC1112738}");
            public static readonly Guid newPassword = new Guid("{1B176C04-2ADD-4A4B-8C68-C3AC4722C8FD}");
            public static readonly Guid IPAddress = new Guid("{09122DE2-BF2F-46A3-A89B-C3B0F3093A55}");
            public static readonly Guid SYSTEMSETTINGS = new Guid("{117692A5-B579-4D1F-9AD9-C3F34101FCF5}");
            public static readonly Guid Empty = new Guid("{87274578-D3DB-4098-AF4D-C416FE8DCD03}");
            public static readonly Guid NewUserRegistration = new Guid("{D0BA0856-2FAE-4312-81F4-C4B58389F94B}");
            public static readonly Guid DomainInvalid = new Guid("{FA7F87EE-809A-44CB-8EC2-C4EA3617AB35}");
            public static readonly Guid pleaseContact = new Guid("{088813D1-491A-4B9C-A353-C53028D57C89}");
            public static readonly Guid IncompleteAndExpired = new Guid("{BD971750-67E3-4D58-8AF2-C5CE6CBB426B}");
            public static readonly Guid CreateNewAccount = new Guid("{D33216CB-BB76-4523-9C8A-C61D1EA7E937}");
            public static readonly Guid AttachXMLDocument = new Guid("{4AAE5631-B7A0-4659-95DF-C7FAAB6B680A}");
            public static readonly Guid RSignAutomatedWorkflowLabel = new Guid("{065C7475-82AD-423E-8E8C-C8A1F9683F72}");
            public static readonly Guid Unchecked = new Guid("{EA688CD4-86A5-4E82-A6EE-C9C88A15FBEE}");
            public static readonly Guid Radio = new Guid("{582A741D-F1E4-4E2C-A81A-CA3D1407F0CF}");
            public static readonly Guid SignIn = new Guid("{87262D2C-DFD5-4471-B07E-CA98AF4B1BCE}");
            public static readonly Guid lang_conditions = new Guid("{56083321-92BA-44E0-96AB-CAD3B5792B1C}");
            public static readonly Guid UserIsNotValid = new Guid("{F149D06A-059A-4E06-9E0E-CADBD270FC09}");
            public static readonly Guid DependencyKey = new Guid("{E11CE2B1-C308-46CC-A977-CB485C603F9E}");
            public static readonly Guid DocumentSubmitConfirmation = new Guid("{D2DBF537-3143-464D-BC9B-CB7291B20CAA}");
            public static readonly Guid Rules = new Guid("{6F52E523-3ED4-4DA6-BD06-CD081E474B7A}");
            public static readonly Guid Attachment = new Guid("{D1D63F89-FAAD-43C2-9764-CE115FC7441D}");
            public static readonly Guid Footer = new Guid("{9609B8D6-4743-4C9C-927E-CEC2A531FAB2}");
            public static readonly Guid NoSubject = new Guid("{2DA77939-C4CA-4A8C-AB1F-CF2BFBF031FC}");
            public static readonly Guid Allowedunits = new Guid("{6359FF55-24F8-490D-BA8A-D06522A6FD80}");
            public static readonly Guid Page = new Guid("{1F1728BC-8711-4ECD-B62E-D08C4D4B5A94}");
            public static readonly Guid ErrorOccurred = new Guid("{9525C4B8-2324-43C0-8415-D17EF42F23D8}");
            public static readonly Guid staticLinkDisabled = new Guid("{919C7AE5-4D31-48EC-B43E-D2AB4F87E583}");
            public static readonly Guid Text = new Guid("{CB5F06CB-FFD6-4E4B-AD47-D2DE3944DFDB}");
            public static readonly Guid Welcome = new Guid("{F7BB5F89-0DF4-4AC9-8754-D2FAC65C140B}");
            public static readonly Guid RsignFeatureCreateTemplateLabel = new Guid("{B30DD448-89C7-4660-B69A-D33A8FE6A091}");
            public static readonly Guid Oldpassword = new Guid("{A16B1AEE-39EC-427E-9B82-D3D9742F81E9}");
            public static readonly Guid Admin = new Guid("{65ED1BC3-818C-480F-838D-D3F7363295F2}");
            public static readonly Guid AccessCodeToSign = new Guid("{BCD4D6D2-430C-48E6-B718-D542BA475A9C}");
            public static readonly Guid Discard = new Guid("{2245B299-51D3-4877-9699-D58B994A24DF}");
            public static readonly Guid AcceptTS = new Guid("{0D3E0743-51DC-4C14-A9CC-D68B9EEA8FB2}");
            public static readonly Guid PersonalTemplatesRules = new Guid("{27587AE1-6894-4AA2-80FE-D6BD8888EFA2}");
            public static readonly Guid AlreadyConfirmed = new Guid("{BDD3E3C1-4E27-4B46-A1F7-D819A9203EB9}");
            public static readonly Guid IsSequential = new Guid("{0AFF8DB3-873D-47C8-A41D-D89249CE09E7}");
            public static readonly Guid CoordinatedUniversalTime = new Guid("{989B5B6D-D6C2-41D1-9DDB-D93E1D9A17F8}");
            public static readonly Guid EnvelopeAccepted = new Guid("{9C978EE2-D034-4D94-9577-D99A0640073F}");
            public static readonly Guid Days = new Guid("{096DD340-6003-4770-90CB-DAC0A0001541}");
            public static readonly Guid Weeks = new Guid("{e3f79a7b-469d-456a-9a1e-76f2ea11af35}");
            public static readonly Guid CurrentStatus = new Guid("{BD13AAD1-5DC9-4751-9A2A-DC042951456D}");
            public static readonly Guid Select = new Guid("{D0565066-0ABD-40DA-A9F8-DC769E6D4F48}");
            public static readonly Guid Copy = new Guid("{380EF85A-E330-4516-8675-DC7DFE285257}");
            public static readonly Guid CompanyValidation = new Guid("{F6F51558-DD59-401D-ADA4-DDBFE5325EF1}");
            public static readonly Guid SignerAttachmentContent = new Guid("{4DACC35E-CF9C-4D28-B91F-DDCE0E78831A}");
            public static readonly Guid CreatedDate = new Guid("{96858DAF-3F15-4E1F-AAF5-DE1287218C4F}");
            public static readonly Guid CreateRoles = new Guid("{7741B070-1F28-4AF2-87E0-DE8AAA628F6A}");
            public static readonly Guid Order = new Guid("{FC9AA510-7ABC-449D-B260-DE9F1A81B634}");
            public static readonly Guid AccessAuthEmailSendFlag = new Guid("{15323032-AF08-49C7-932D-DF700ADB7C56}");
            public static readonly Guid title_loadEnvelopeHistory = new Guid("{5EAAA101-EEB9-4DF2-8BCA-DF920E0DDEF4}");
            public static readonly Guid Reset = new Guid("{39214709-9E4D-46DE-B68D-DFBAE0A6940C}");
            public static readonly Guid EnvelopeCode = new Guid("{862FAD9D-9101-4CDC-A4C7-E0A55C6613AB}");
            public static readonly Guid sessionPopUplogoutwarn = new Guid("{E590BA40-0E59-43D9-8917-E126576F0767}");
            public static readonly Guid CreateTemplate = new Guid("{A47B2CE1-BEE1-4BD2-9F3C-E19C686CA617}");
            public static readonly Guid OldEmail = new Guid("{869FAF5C-FC11-4632-BE95-E37A7AA3A937}");
            public static readonly Guid AddRole = new Guid("{3591F8F3-99E5-45F5-BD4D-E47B0FD07852}");
            public static readonly Guid Unsigned = new Guid("{FB5DA801-4C4A-48C9-ABC4-E526C3ADA39B}");
            public static readonly Guid StoreEmailBody = new Guid("{2E3D2D8A-89E7-4C11-A244-E553E66F26E0}");
            public static readonly Guid OverrideUserSettings = new Guid("{BFE5E953-D6AF-41E7-B8EB-E67F0C847485}");
            public static readonly Guid lang_DeleteFileProgress = new Guid("{455933D4-CB43-4FD8-9EE1-E6FBF8061A27}");
            public static readonly Guid ReTypePwd = new Guid("{18ECAAFD-E4C7-4DCE-9386-E81548D76DA7}");
            public static readonly Guid Settings = new Guid("{9A76FB60-3DE2-4C4E-9326-E81EB4CE9292}");
            public static readonly Guid lang_DocumentDeletedMsg = new Guid("{24DC9E44-CDCC-4CAF-B504-E83A3919C609}");
            public static readonly Guid Checked = new Guid("{ED57F769-EA90-473F-9DFE-E87773385026}");
            public static readonly Guid Delete = new Guid("{F42DF6C3-B200-4264-A9EF-E8F343161071}");
            public static readonly Guid title_Close = new Guid("{548A19E7-DE15-40DD-B461-E96F4FDA5A49}");
            public static readonly Guid Decline = new Guid("{784FC921-B967-4D4B-974F-EB19E69422A3}");
            public static readonly Guid Refresh = new Guid("{90349BED-5015-4CEF-BECE-EB34B44FA29F}");
            public static readonly Guid title_removeEmailBody = new Guid("{BB02FE46-0F26-4679-BF85-EB50129DCB71}");
            public static readonly Guid EnvelopeRejectedWithMail = new Guid("{A9B4453E-FD57-4826-AA4D-EB9CE3555C39}");
            public static readonly Guid CreateTemplateorRule = new Guid("{C3C02FE6-8605-4FC9-A053-EBB08C215D5A}");
            public static readonly Guid Send = new Guid("{1CE1860B-143A-4958-99A4-EC0967B21FFE}");
            public static readonly Guid Delegated = new Guid("{23E7AF66-C0FA-4708-841A-EC2796766F96}");
            public static readonly Guid TemplateRuleList = new Guid("{FECC825B-91EC-4A8B-B8BB-ECA44BE44A63}");
            public static readonly Guid ExpiresToday = new Guid("{D67BBB5A-3EE7-43BE-A868-ECA81BA5B140}");
            public static readonly Guid Description = new Guid("{8E9D6844-5904-4C08-83CE-ECC621B36DAF}");
            public static readonly Guid Character = new Guid("{913128DE-1D41-4E8E-898C-ED8F51E51FD6}");
            public static readonly Guid NotValidUrl = new Guid("{2804A8CC-82D1-4648-B2BD-EDEED06DB630}");
            public static readonly Guid RememberMe = new Guid("{9025BF59-D718-4FF2-8E48-EED1842EBDF3}");
            public static readonly Guid DomainName = new Guid("{87A93899-D79A-441A-9946-EEF365E28B2F}");
            public static readonly Guid SendDocumentEncrypted = new Guid("{B8056141-D4DE-4CB3-8849-EF22AD6199EA}");
            public static readonly Guid Storagedrivesavailable = new Guid("{D2F93CC2-5D17-4636-A110-EF44B4E8345B}");
            public static readonly Guid Viewed = new Guid("{7564AA3C-97B7-4B26-9FC6-EFCDF17DD41C}");
            public static readonly Guid SaveAs = new Guid("{D4254E74-AA24-47CC-B363-EFDB4F9DE2CE}");
            public static readonly Guid ShareTemplateRule = new Guid("{8D142C1B-8171-41F6-9312-F0CE8D26F106}");
            public static readonly Guid Roles = new Guid("{C46815DA-40EA-49A6-83DD-F1787793BE58}");
            public static readonly Guid Settinghelpicon = new Guid("{0FEFC2EA-A363-422B-8FDD-F228E384A44C}");
            public static readonly Guid GroupName = new Guid("{2C91AA05-3BDB-4F22-9698-F26263CE318B}");
            public static readonly Guid Next = new Guid("{1DDC6AB4-C61A-4617-8D88-F27A5BFEABC1}");
            public static readonly Guid SucccesfullySubmitted = new Guid("{56587859-D747-4B5A-89BA-F27C09027B16}");
            public static readonly Guid SharedTemplateList = new Guid("{809EE023-7906-4AD9-9232-F2A13C24190E}");
            public static readonly Guid RequiredConfirmation = new Guid("{2E57F2F2-5B10-4C6D-82AC-F2B2DA70D0E4}");
            public static readonly Guid ChangeSignerName = new Guid("{659FFD2E-F36C-4A3B-8FEA-F2C152DFCAB0}");
            public static readonly Guid TransparencyContentN = new Guid("{E3C35303-4894-4871-B18F-F36071C46A35}");
            public static readonly Guid ADMINSETTINGS = new Guid("{91886562-AF78-481C-9B80-F3FCE23B53AC}");
            public static readonly Guid ADVANCEDSETTINGS = new Guid("{C374F9B7-D23D-4E69-985D-C561B345EADA}");
            public static readonly Guid DocumentSizeMsg = new Guid("{207F5702-5817-4380-9358-F470F77AB4F0}");
            public static readonly Guid reviewpageAnyQuestionText = new Guid("{80793863-AA6E-40F6-95F6-F47BD7719036}");
            public static readonly Guid TextFormatting = new Guid("{96A2B245-E49C-44A9-9B2C-F4BA2D9BA080}");
            public static readonly Guid RecipientRole = new Guid("{18CE629B-E294-4237-8533-F585F0CE088F}");
            public static readonly Guid Edit = new Guid("{6927A498-E2FE-498D-A904-F5BD36DE64C7}");
            public static readonly Guid PasswordWindow = new Guid("{CE950E8D-4BB0-48B1-93E7-F5C68D073F34}");
            public static readonly Guid DeclineMsg = new Guid("{BFCBE8F1-D5D2-42ED-80EF-F6815784EECB}");
            public static readonly Guid Manage = new Guid("{619550AA-5AB1-4EE2-95DF-F752EEF08460}");
            public static readonly Guid SignatureCapture = new Guid("{E67C9564-988D-4A6E-A4A4-F75C7A06D448}");
            public static readonly Guid ManageAdmin = new Guid("{5EE6B225-D6AC-443A-9471-F7ADF62D7B37}");
            public static readonly Guid lang_addRole = new Guid("{5849E15F-88FF-4F33-BE52-F7D3EF977FAF}");
            public static readonly Guid emailConfirmation = new Guid("{5878154D-6A20-4E41-AC52-F94801406D7D}");
            public static readonly Guid CopyLink = new Guid("{4A835791-060B-4CA5-9581-F9CEB759A433}");
            public static readonly Guid SessionStayIn = new Guid("{BFEFEC19-5256-4A37-A408-FA547AF5B89B}");
            public static readonly Guid FooterTextRpostTechnologyLink = new Guid("{1091A590-02C1-4419-BA35-FCA75367C64B}");
            public static readonly Guid title_accessAuthPwd = new Guid("{E390A331-46A5-4013-A7ED-FCAFBB49A6CC}");
            public static readonly Guid SignerVerificationPlaceHolder = new Guid("{DB1298EE-5FCC-4C5A-AF15-2D671015D45B}");
            public static readonly Guid Stats = new Guid("{D86AD7F7-68D1-46E4-9BCD-FCB1157C0D45}");
            public static readonly Guid Role = new Guid("{5243E6A3-1860-4C27-B718-FCF44F4C5933}");
            public static readonly Guid EditRecipients = new Guid("{022A044A-6D1C-4409-A477-FD5578CC4C05}");
            public static readonly Guid ChangeSigner = new Guid("{6598064B-33FD-450D-A982-FDACA94B1F57}");
            public static readonly Guid Length = new Guid("{DBA33721-35C8-405E-99B5-FDFAA0914E3A}");
            public static readonly Guid lang_confirmation = new Guid("{A6EBEF12-7AEF-4082-BC87-FE29042430D7}");
            public static readonly Guid Content = new Guid("{3CB8114B-F89E-4823-84A2-FE30E3F9CA77}");
            public static readonly Guid Clear = new Guid("{81938C0A-696C-493A-9B31-FE8C81EFBD18}");
            public static readonly Guid DeleteFinalContractDisplayMsg = new Guid("{F7E5511F-DED4-41BA-A668-FE947218FD8A}");
            public static readonly Guid NoCompany = new Guid("{88ADD6E5-816B-43A3-9795-FEF20CB22737}");
            public static readonly Guid Search = new Guid("{E5BD2380-F8E2-4EE3-B171-FF02E3F60FC3}");
            public static readonly Guid SignerAttachmentContentN = new Guid("{E45075FF-B234-48C9-B753-FFF5428541DC}");
            public static readonly Guid lang_pleaseWait = new Guid("{15ABAE70-E2CB-4EDF-99DD-8B5B3228C8AE}");
            public static readonly Guid WaitingMsgTopForStatic = new Guid("{3606EF5A-DAA6-423E-828D-B586AEFA3AA5}");
            public static readonly Guid WaitingMsgBottomForStatic = new Guid("{D6340425-388D-4C58-9550-EA50963DEFA2}");
            public static readonly Guid lang_attention = new Guid("{683C2554-3FF9-4E5A-BBC0-BC5259ECAE4C}");
            public static readonly Guid waitingMsgAddressStatic = new Guid("{79D71697-27E3-46A9-862E-CE9280F28AC2}");
            public static readonly Guid waitingMsgSignStatic = new Guid("{F643AFF9-00EF-4975-9D45-466AB068286A}");
            public static readonly Guid waitingMsgTryAgainStatic = new Guid("{3728ABE7-3343-4049-8887-7867B198E31C}");
            public static readonly Guid waitingMsgProceedStatic = new Guid("{074490B9-972A-41C6-974F-28D9AAB5583D}");
            public static readonly Guid Approx = new Guid("{56EFEADD-1124-4F24-834A-FD51AE6C39B7}");
            public static readonly Guid lang_Group = new Guid("{200B9721-E350-46C7-B31B-23D5E8D94DBF}");
            public static readonly Guid lang_yourNameImg = new Guid("{FFD64B83-8E49-464F-90EB-33A2D0FBF31F}");
            public static readonly Guid lang_SelectTemplate = new Guid("{230AB9F8-52EE-4F8B-924F-25839D9D4A4C}");
            public static readonly Guid lang_SelectRule = new Guid("{7CD34AE9-E97C-48CE-A131-8DED64FE2ED8}");
            public static readonly Guid FirstLineBlank = new Guid("{BFEBE496-868F-4823-B03C-92727C1007D0}");
            public static readonly Guid UIFileConvertorOptions = new Guid("{2EBC257A-95F2-476A-BF67-33FEB4E6A6D7}");
            public static readonly Guid SignatureControlDefaultRequired = new Guid("{DC0D8411-06EB-4F0D-8D1D-F98F2CB6C78A}");
            public static readonly Guid EsignedEmailCopyAddress = new Guid("{79761B5A-BA3D-4FC4-B997-12A554EB53D2}");
            public static readonly Guid EsignedEmailRerouteAddress = new Guid("{8AEEA3A2-3183-4E26-8ED3-0F3D45750148}");
            public static readonly Guid Day = new Guid("{B3EA7CD5-2FE0-458E-83E0-4692BE68E5A6}");
            public static readonly Guid RadioName = new Guid("{8A8A0B45-6E8D-4B4A-B7CC-A46D0A179823}");
            public static readonly Guid RsignSupport = new Guid("{6C374C58-5EAD-4BA4-AD53-5FD5E0CC6C80}");
            public static readonly Guid SendingConfirmationEmail = new Guid("{13AF4ECE-C831-4E7B-A89D-518D58F0CA8F}");
            public static readonly Guid HeaderFooterSettings = new Guid("{3A271DB4-C2F1-43A8-81E7-A7EF5BC220B7}");
            public static readonly Guid lang_UpdateResend = new Guid("{42CC063D-71B7-4543-82D9-43594431ECE1}");
            public static readonly Guid EnvelopeUpdating = new Guid("{B352717B-2B1A-4017-A69E-F8A23ECF9B6C}");
            public static readonly Guid EnvelopeTerminatedForRecipient = new Guid("{6F1AE2D4-8120-4A04-982D-AC3278B135F8}");
            public static readonly Guid EnvelopeOutOfdateResend = new Guid("{976A77CC-5EAE-45D5-B3F1-49FD323B18E6}");
            public static readonly Guid UnauthorisedSigner = new Guid("{7A35CD13-7D31-4007-A846-1C516F7111F7}");
            public static readonly Guid EnvelopeDiscarded = new Guid("{2A24CF5D-ECF5-417D-8627-42C5E15E121C}");
            public static readonly Guid EnvelopeResendLatest = new Guid("{E1664050-4EDA-4942-A100-CE3334CDC8FE}");
            public static readonly Guid EnvelopeSignProcessMsg = new Guid("{A9F3F208-6481-46C4-B6F1-06A26750D9E5}");
            public static readonly Guid FinalContractConversion = new Guid("{B35AC153-4DDE-4E98-91A5-5492E1762AAC}");
            public static readonly Guid AttachSignedPdf = new Guid("{11F8B179-0D81-4E2E-B697-580F7AE3199A}");
            public static readonly Guid lang_Y = new Guid("{D3C56440-6A50-4259-B24E-24BD9A0693BE}");
            public static readonly Guid lang_N = new Guid("{A5F19D7F-FB5C-4451-9DB2-F499FE9AC583}");
            public static readonly Guid EmailCannotResent = new Guid("{A387AEA4-B7FF-43B4-9045-D5CFB2E0F8A6}");
            public static readonly Guid SignerConfirmed = new Guid("{4743E1D7-E9FB-472C-B455-0845E946CDAE}");
            public static readonly Guid ResendConfirmEmail = new Guid("{6E3DF55D-DD10-4794-B5F9-9C45EDFAC7DE}");
            public static readonly Guid ReviewDetail = new Guid("{39421f57-bc74-4f71-85c2-fe54dd2dbc4a}");
            public static readonly Guid title_downloadFinalDocument = new Guid("{E247D100-A697-44E4-AA95-104C988FCD04}");
            public static readonly Guid lang_CancelUpdate = new Guid("{52F4B2C9-0845-4A2F-A0C5-E743638CB873}");
            public static readonly Guid lang_UpdateInProgress = new Guid("{04d946da-58a8-4e69-8b60-57b902280e88}");
            public static readonly Guid selectTimeZone = new Guid("{F1098702-4BC8-40CE-ACCA-103246E668D5}");
            public static readonly Guid lang_Numeric = new Guid("{29F04A74-E857-403F-9994-2C70D42F144C}");
            public static readonly Guid lang_Text = new Guid("{5A870858-6BB7-4EF9-8889-1F51A637D04E}");
            public static readonly Guid IncludeEnvelopeXmlData = new Guid("{EB97FE21-81D1-4CA8-8105-8C62BD7962BB}");
            public static readonly Guid AllowTemplateEditing = new Guid("{df9dd0e3-5bcb-48ff-8686-a5551c9b9f49}");
            public static readonly Guid EnablePostSigningLoginPopup = new Guid("{DAB3A97B-DAE6-4778-8E4E-EE0EF055E7E1}");
            public static readonly Guid SetAsDefault = new Guid("{5F1D3C69-7092-4C44-898A-9E77A413524D}");
            public static readonly Guid DeleteSignature = new Guid("{FEF3FBD0-E03F-4AC1-B400-6517FEEEE23F}");
            public static readonly Guid SaveSignature = new Guid("{AC91FEC8-14E4-418E-904C-886BCB551888}");
            public static readonly Guid EnterSignatureTextDialogLabel = new Guid("{A130F81D-8DBC-465E-9912-71EB4ED4F96E}");
            public static readonly Guid SignatureNameLabel = new Guid("{F850745D-45D9-42C2-B629-F48F29F80396}");
            public static readonly Guid lang_UserSignatureSuccess = new Guid("{B3D145DC-2ADD-4551-9618-74AC63CD59B7}");
            public static readonly Guid lang_SignatureUniqueNameValidation = new Guid("{EF46AFF8-4FFE-4139-960B-B030E70E80C6}");
            public static readonly Guid lang_SignatureNumberofCharactersValidation = new Guid("{41B83DA5-A96B-4968-B490-2D5714392437}");
            public static readonly Guid lang_SignatureNameSpaceValidation = new Guid("{C14245AF-AD8C-4F86-ADE8-5CF48B594E1F}");
            public static readonly Guid lang_UnsavedSignatureChange = new Guid("{4E06D936-6A9F-4168-95B1-8CFDDACE7624}");
            public static readonly Guid NewSignature = new Guid("{50236229-3305-4244-8A62-EE72C6A6F3D1}");
            public static readonly Guid ValidSignatureName = new Guid("{6A980BC9-4F49-4275-8ABC-6C15D13B36B2}");
            public static readonly Guid PleaseSelectSignName = new Guid("{855B2FAC-96CF-4A6A-8552-FF5E1BD83649}");
            public static readonly Guid lang_SignatureTextSetDefault = new Guid("{DC10D115-5C0F-469B-B1F2-139E5373D958}");
            public static readonly Guid lang_SignatureTextDelete = new Guid("{2B05B26E-825B-42C3-B73E-2F0D4851B94D}");
            public static readonly Guid SignatureTextRequired = new Guid("{D5E2FE24-93CB-480E-B1E4-4EEB4AEABCB4}");
            public static readonly Guid CreateAccount = new Guid("{c5586342-7cc3-42cf-96d9-0aa744580566}");
            public static readonly Guid SignupForFree = new Guid("{b8af9d7b-7d57-47ca-b751-63e9c3aa9a44}");
            public static readonly Guid EmailResentTo = new Guid("{de3d178a-e3a3-4607-b3fc-12aa42187b37}");
            public static readonly Guid SendDocuments = new Guid("{665b8d7e-5a99-4165-aacc-e2b566d5c46e}");
            public static readonly Guid ManageDocuments = new Guid("{f9aeaa1d-4d56-435e-b9ee-66aeebb7b7d4}");
            public static readonly Guid CreateTemplates = new Guid("{625d5467-9614-457f-a888-f202c88dd17f}");
            public static readonly Guid CreateRules = new Guid("{cf5dc542-b906-4acf-bbd4-99b550132331}");
            public static readonly Guid AutomateNotifications = new Guid("{19574b35-1829-49a8-afc8-3fcbb0a2c481}");
            public static readonly Guid TermsAndConditions = new Guid("{de3f7d3c-4887-4806-ac77-2c5e0d456aeb}");
            public static readonly Guid PrivacyPolicy = new Guid("{4469bc07-5c24-4796-876a-f720d09ab182}");
            public static readonly Guid LoginInto = new Guid("{D90E1DCF-8E8A-4DAA-84D9-1CBE9C21CE8E}");
            public static readonly Guid lang_ConfirmDeleteSign = new Guid("{E67B2090-2E27-456C-9494-3AF5609BD12C}");
            public static readonly Guid Period = new Guid("{3011b997-417a-4b26-bc4f-0a02cdbd76d2}");
            public static readonly Guid OrderMissing = new Guid("{86727ccf-011a-43be-b411-0e0e7a429c88}");
            public static readonly Guid FileSupported = new Guid("{18bf6a56-6c0f-48fc-b16f-59d02c988a65}");
            public static readonly Guid lang_authSuccess = new Guid("{F33AD0C7-F55E-4E4E-8906-AE9D109723D4}");
            public static readonly Guid EmailandSettings = new Guid("{ca7cb620-5618-4645-9d28-a80d66f28948}");
            public static readonly Guid eSignExpiredFeatureList = new Guid("{25b27318-817c-4aa8-9972-837de7d41e92}");
            public static readonly Guid eSignExpiredServiceFeature = new Guid("{435424f7-2ef0-4a54-a9b4-eb479f1f4a43}");
            public static readonly Guid LearnMoreAboutRsign = new Guid("{40083db3-a4b8-490a-8efc-727bb59dd3e4}");
            public static readonly Guid FreeTrial = new Guid("{d0f7d3f7-42aa-4a47-ba58-fcde4364fb4e}");
            public static readonly Guid RestoreDefaults = new Guid("{5085a8d2-b2a9-4958-bde1-43d2f353c33d}");
            public static readonly Guid title_SaveDocumentinDraft = new Guid("{2197DBD8-2769-4F7B-AC17-7C574E0EB11A}");
            public static readonly Guid title_ClearDocumentofControls = new Guid("{2D498D13-A7A8-4522-977D-9DF9306AFD0C}");
            public static readonly Guid title_DeleteDocument = new Guid("{C55D26C1-BD07-4417-A234-7BD41C50E5B3}");
            public static readonly Guid title_ViewDocumentAsPDF = new Guid("{3A93DF17-52EE-4C47-ACFF-4E2AADB0A5C2}");
            public static readonly Guid lang_addControllingField = new Guid("{22AE37D8-F388-4101-ADB0-7948FE8015E8}");
            public static readonly Guid Back = new Guid("{958EE23F-6E23-4AEB-8B8D-091934AEE2D0}");
            public static readonly Guid lang_BackClickSaveConfirmation = new Guid("{E21BF7F2-0773-4F2F-9E80-C33A4C8E8F99}");
            public static readonly Guid lang_Information = new Guid("{79A6075C-5313-404C-93A5-6099751E823C}");
            public static readonly Guid lang_BackConfirmationP1 = new Guid("{44B55505-EA2C-4D63-BA92-B44D73621AE5}");
            public static readonly Guid lang_BackConfirmationP2 = new Guid("{81870E93-D039-4B10-984F-9CCBD0EFFBC5}");
            public static readonly Guid lang_BackConfirmationP3 = new Guid("{521D25B0-083A-4C3D-BF3B-6A56552FC478}");
            public static readonly Guid lang_BackConfirmationP4 = new Guid("{EC597424-5893-441B-9418-D71CCD256891}");
            public static readonly Guid lang_EnvelopealreadyDrafted = new Guid("{F124C92D-31D0-4D96-9AEE-55CCD72D07AA}");
            public static readonly Guid CreateMessageTemplate = new Guid("{6e70d70f-62eb-4b9e-a79f-ac116854835c}");
            public static readonly Guid CreateMessageTemplateSettings = new Guid("{a3bb1519-a122-41fc-81d4-d980119e9611}");
            public static readonly Guid PersonalMessageTemplates = new Guid("{7dd50a01-1520-424f-a168-1b89c905b18a}");
            public static readonly Guid SharedMessageTemplates = new Guid("{d2e7d631-07bc-40c7-9d43-d1ed7c03d200}");
            public static readonly Guid View = new Guid("{264976A0-132E-4507-AFE2-875D1648E3A0}");
            public static readonly Guid Setting = new Guid("{67361109-6F8D-4D1E-925B-B8C5161131BF}");
            public static readonly Guid SequentialSigning = new Guid("{954CE0D6-ABB9-4DC9-A61F-5F05604045DE}");
            public static readonly Guid PrintSettings = new Guid("{4B81F695-2EC1-4BFF-BBAC-AE5D736C46E2}");
            public static readonly Guid AllTemplates = new Guid("{47abcadc-8541-4662-8067-8b46742eb688}");
            public static readonly Guid IAgreeTo = new Guid("{cb2310fb-87f6-4a5e-abb0-a6425fe30699}");
            public static readonly Guid AutoPopulateSignature = new Guid("{DD570012-AB78-4269-8045-DDE21A7778AE}");
            public static readonly Guid Disclaimer = new Guid("{1F451BC4-8095-4979-AFFF-70CD92C8EB6F}");
            public static readonly Guid SendIndividualNotifications = new Guid("{c222c3d9-6c2c-4b9c-8e9b-4809c103c40d}");
            public static readonly Guid AddDatetoSignedDocumentName = new Guid("{CB015116-7BF7-4C81-9194-05205241FF13}");
            public static readonly Guid UploadDocumentDragDropFiles = new Guid("{2A03EF70-C758-4C3A-A28E-0596A0DF6710}");
            public static readonly Guid DragDropFiles = new Guid("{38FFDB00-9C9C-4A4B-B7A9-70A8747A5A37}");
            public static readonly Guid Display = new Guid("{E75C13B9-C06F-4971-B653-7B317F327674}");
            public static readonly Guid lang_Int = new Guid("{9506A039-DF8D-4650-8552-8B650C9889B1}");
            public static readonly Guid NewInitials = new Guid("{166F5919-7B0E-4A7E-916B-0D0811EB2D63}");
            public static readonly Guid IsDefaultSignatureRequiredForStaticTemplate = new Guid("{6021D655-0062-4198-A24F-DFF809C7DE51}");
            public static readonly Guid lang_PleaseCheckEmailAndSpam = new Guid("{48EE783E-DFDB-440E-991C-1762D5BC44A4}");
            public static readonly Guid lang_PFP_StillCantLoginContactAdmin = new Guid("{5A5AC3CA-77BC-497B-B74E-7FFBA0677567}");
            public static readonly Guid lang_PFP_ReadytoStartUsingRsign = new Guid("{6A943D8E-1FB6-4272-B0C2-AFDB8E222958}");
            public static readonly Guid lang_PFP_ViewGuidVideoTraining = new Guid("{9BEF9E87-7CCD-4A91-825C-8D6C289B9249}");
            public static readonly Guid lang_LogIn = new Guid("{1494F113-AA0A-4697-9510-158079AF6D18}");
            public static readonly Guid lang_NewToRSign = new Guid("{DC5821BA-D39E-4912-B40A-BA63DE32CDD2}");
            public static readonly Guid lang_EnterEmailSearchStr = new Guid("{689A469F-75A1-48E5-9D56-E68C49F34C67}");
            public static readonly Guid lang_SelectSignature = new Guid("{26FBE34A-6A4B-442D-B353-0BBF5234444F}");
            public static readonly Guid lang_DownloadSignatureTooptip = new Guid("{D65763C4-61ED-4257-B11F-A8BBB3F21A9E}");
            public static readonly Guid lang_MessageTemplate = new Guid("{E269F298-3DE5-406F-BC12-CB11D10C8A75}");
            public static readonly Guid lang_UploadImage = new Guid("{24DABD5C-C5F6-4BBD-BBE0-6D72D701CBCD}");
            public static readonly Guid lang_LOGINUpperCase = new Guid("{7F158550-3BCE-4930-A8A9-5B89C3E52818}");
            public static readonly Guid lang_NOTHANKSUpperCase = new Guid("{64C6DFE9-B452-433F-85E6-6F5DFAB9A62C}");
            public static readonly Guid lang_PostSignInfo = new Guid("{386BE4A1-AF8C-4A88-A159-87B55FB72BCB}");
            public static readonly Guid lang_SignatureCertificate = new Guid("{A4898B06-D587-4A12-984F-F1BAD1B39119}");
            public static readonly Guid lang_loaderLodingSettings = new Guid("{982B5394-7AC1-4F01-BD1B-BE87860324A8}");
            public static readonly Guid OUTOFOFFICESETTINGS = new Guid("{4B9F2716-5EDE-4857-8289-3277A5234C1B}");
            public static readonly Guid EnableOutOfOfficeMode = new Guid("{FD68EC1A-5B3B-48A6-82CF-126528D60363}");
            public static readonly Guid FirstDay = new Guid("{1D8864DA-0515-4573-ADCA-982C70683AE7}");
            public static readonly Guid LastDay = new Guid("{1725A930-B618-4A31-8F6A-43610F66B776}");
            public static readonly Guid CopyAllRSignEmailsToThisaddress = new Guid("{2841C7ED-D1BE-4A8E-8671-7131446D837B}");
            public static readonly Guid RerouteAllRSignEmailsToThisaddress = new Guid("{E3EDD14C-1A42-43FA-800C-4BCE1A9B5D26}");
            public static readonly Guid DateRange = new Guid("{A385CEEF-62D6-4C19-818C-957E5B5BF6E4}");
            public static readonly Guid EnableTheDependenciesFeature = new Guid("{9CDDD110-12F4-45A7-9535-627C605B5933}");
            public static readonly Guid ModifySelectedGroupSettingsTitle = new Guid("{4B2FF617-685F-4069-919A-FEC0F243C124}");
            public static readonly Guid Inbox = new Guid("{BE86EA1C-698F-4DBB-91DD-2E0D4423CF4E}");
            public static readonly Guid SelectTemplate = new Guid("{8C003C86-320C-4DF7-8CE8-253CFC397BEF}");
            public static readonly Guid SendFinalReminderBeforeExpiration = new Guid("{BF9B7DCC-950E-4A00-8877-38D06770E2E1}");
            public static readonly Guid SendFirstReminderIn = new Guid("{120F50FA-FDAB-4D8D-99E3-540E462B3BBF}");
            public static readonly Guid ThenSendFollowUpRemindersEvery = new Guid("{363D9101-8682-41FC-8F6B-2DF61BD78984}");
            public static readonly Guid SendInvitationEmailToSigner = new Guid("{D9C0BCDA-B359-4A67-97D7-128BAB99A523}");
            public static readonly Guid DocumentsCombinedInOneEnvelope = new Guid("{F154A9DD-2DE9-4ED4-821F-FFED4EC90D62}");
            public static readonly Guid SentForSignature = new Guid("{EFBE0156-FE26-4883-ACF7-75C7E82D7653}");
            public static readonly Guid TypeInitials = new Guid("{D1BFA64C-A0A4-40B3-895B-BA859C971EB1}");
            public static readonly Guid Hyperlink = new Guid("{93D87429-C34A-4810-B3F1-570F6511BB26}");
            public static readonly Guid ReferenceCode = new Guid("{C9CC6C6E-6F8E-4A88-8549-C95BD6EFCA2F}");
            public static readonly Guid ReferenceEmail = new Guid("{C350C9F2-A49D-4F27-93CE-DD164B55BD04}");
            public static readonly Guid CopyTemplateConfirm = new Guid("{5C04C08B-AD43-4985-9013-D59606290BCE}");
            public static readonly Guid CopyRuleConfirm = new Guid("{CB69C681-64B3-41EC-B49C-6E65A4921B0F}");
            public static readonly Guid Drive = new Guid("{CA3D34FD-F0C3-41DF-971C-13E602786EC6}");
            public static readonly Guid Deleted = new Guid("{9EACA382-0670-4B6E-AD5E-93BF4E0ED929}");
            public static readonly Guid Folders = new Guid("{BA96A64F-B8C7-43BF-9CA9-73BD4F1EC4E6}");
            public static readonly Guid ActionRequired = new Guid("{30B210B7-8C22-4D94-88BF-9266DE8F7347}");
            public static readonly Guid WaitingforOthers = new Guid("{147760AD-7EDC-42A9-BE65-4532A49C7FF8}");
            public static readonly Guid Waitingforyou = new Guid("{C195D0EA-DDF6-4F40-BCFF-A057588EC686}");
            public static readonly Guid SendDocument = new Guid("{E61F4FF1-926A-4D81-99EE-9B69853A864E}");
            public static readonly Guid ActivitySummary = new Guid("{B1BD1A05-C015-43DA-A7DD-3DA343D0C6CD}");
            public static readonly Guid Past30days = new Guid("{AC5D4B86-61F6-457E-A4AF-603C075884DB}");
            public static readonly Guid Today = new Guid("{42C1674D-E06B-4701-AC15-CD5FE2796E9B}");
            public static readonly Guid Past24hours = new Guid("{D9662136-C27F-4521-8B61-D40C68B43BC2}");
            public static readonly Guid Past48hours = new Guid("{BA6234EA-15F9-41B4-91DA-A597A2057294}");
            public static readonly Guid Past7days = new Guid("{EAE861DB-6882-4824-B407-4BFBCA307E38}");
            public static readonly Guid Past14days = new Guid("{36BBD481-EA7B-44E1-B7FF-A1F072CBDB19}");
            public static readonly Guid Past90days = new Guid("{4905825E-5DF3-4EE7-9793-CA3973AA2F00}");
            public static readonly Guid SelectDays = new Guid("{B0EBB263-B423-45FD-BEB0-F6A6A900B6D4}");
            public static readonly Guid PercentTotalSent = new Guid("{1365A2F8-7665-430A-8FF1-47D0383C73BE}");
            public static readonly Guid Times = new Guid("{31FECC0B-FF60-40DB-A724-EF68F9F2282E}");
            public static readonly Guid Best7Minutes = new Guid("{551A0915-3A18-4296-8538-CE53CB77A3DA}");
            public static readonly Guid AverageHours = new Guid("{9F72B75A-1F21-4D96-A94F-88768331ADE8}");
            public static readonly Guid Pending = new Guid("{38B4B6B8-EB89-4D77-9494-73BBEECC2103}");
            public static readonly Guid ManageFolder = new Guid("{EC73441C-EEDE-40E0-AD1D-FC35A0755FF3}");
            public static readonly Guid CreateFolder = new Guid("{8F887B15-F8F6-40C6-BB6F-3303723C322F}");
            public static readonly Guid NewFolder = new Guid("{9D2CD908-A40B-4862-BA7B-D49EA4E94B40}");
            public static readonly Guid MoveAll = new Guid("{345C89C3-1B11-4C56-86E3-71016015E65B}");
            public static readonly Guid AuditTrail = new Guid("{DBC21A45-FE01-41A7-800F-0E7BC7ED1149}");
            public static readonly Guid Action = new Guid("{1B4C429D-F3FD-4F58-95BA-00B18D720583}");
            public static readonly Guid DragDropFileText = new Guid("{67CD4916-3562-4EEC-B698-BC0D542670C0}");
            public static readonly Guid MoveSelectedEnvelope = new Guid("{4A4F3DE3-83AB-4C93-B479-5CE912A511E1}");
            public static readonly Guid ConfirmEnvelopeDelete = new Guid("{CE92BEB6-0D2A-4869-BA84-EF3209A2011B}");
            public static readonly Guid EnvelopeMoveSuccess = new Guid("{002F3E87-22F2-4985-919C-9EAEC3492868}");
            public static readonly Guid ConfirmRestoreEnvelopes = new Guid("{74930BED-819C-4652-B13C-14C8D7B84A0B}");
            public static readonly Guid ConfirmRestoreEnvelopesFromFolder = new Guid("{9A2D4FD9-F877-44DB-A554-79F40EB686F4}");
            public static readonly Guid FolderNameExist = new Guid("{D204A056-8685-4A2B-A4AA-8549216A3116}");
            public static readonly Guid FolderDeleteSuccess = new Guid("{2B305E57-6B4E-48E9-AEEE-9DB348CEEAEA}");
            public static readonly Guid FolderCannotBeDeleted = new Guid("{32FD915D-00B0-407D-A545-9F09FD967548}");
            public static readonly Guid ErrorDeletingFolder = new Guid("{420A66D3-7091-4C2D-80CC-407F25F2D94C}");
            public static readonly Guid CreateFolderValidation = new Guid("{79EC7CEF-036D-45BF-9449-E977D27EF3F5}");
            public static readonly Guid AddLevel = new Guid("{DCFABEF0-DD61-4758-A997-E461B12328FA}");
            public static readonly Guid EnterNameAndDescription = new Guid("{5BB2BA85-0873-4878-BB9A-6767A76A83C1}");
            public static readonly Guid EnvelopeDeclineValidation = new Guid("{DD882899-F945-42AE-BD28-89239BFFC183}");
            public static readonly Guid SelectTempValidation = new Guid("{15195A7C-7BB4-4E13-91BD-234717E5FAF1}");
            public static readonly Guid DelegateEnvelopeError = new Guid("{205F728D-B5F2-4B68-A7A8-B2E4316E3601}");
            public static readonly Guid SelectOptionValidation = new Guid("{7B7767A6-117B-41B7-BDE4-2E6D5C291667}");
            public static readonly Guid CommentsValidation = new Guid("{7DDD3262-D913-46A0-936E-7EC509307E80}");
            public static readonly Guid EnvelopeRestoreSuccess = new Guid("{E1707E32-2185-4A4A-93AF-0AB48A3CDA69}");
            public static readonly Guid lang_RestoreValidation = new Guid("{BB292A01-5EF2-43F2-A081-A5CAD6EB9879}");
            public static readonly Guid SignMultipleTempLabel = new Guid("{E446F305-B910-4163-BA56-50158958E8A7}");
            public static readonly Guid SignMultipleTempLabel2 = new Guid("{0FE5BDE6-F0BC-45E0-B987-C1B72AAF30C4}");
            public static readonly Guid lang_RestoreEnvelopeFromFolder = new Guid("{F2A0BA80-B91A-4A66-BDCC-5880312F9A32}");
            public static readonly Guid lang_selectEnvelopeValid = new Guid("{8DE2C367-E551-4E08-BBEE-77C4CC5A54EB}");
            public static readonly Guid SenttoOthers = new Guid("{16FB1E05-7525-4F90-B44C-A42FFFFBD64B}");
            public static readonly Guid SelectAction = new Guid("{632F68A2-5F8A-4744-A1FB-1DE41B40587C}");
            public static readonly Guid Finish = new Guid("{057DC8AB-1B98-4E88-8FDB-6473CCA54346}");
            public static readonly Guid Prescriber = new Guid("{54CCAAEF-A269-4963-8553-16565458FF1B}");
            public static readonly Guid UpdatedStatusDate = new Guid("{A936EB37-A890-4141-9EEC-D086B6205B3D}");
            public static readonly Guid AdditionalInfo = new Guid("{B8079C9D-D1E9-4506-A12B-48AE6F7093B2}");
            public static readonly Guid AddRow = new Guid("{E113EF52-D985-4B8C-8D91-FA65E7D8F053}");
            public static readonly Guid AttachmentRequest = new Guid("{555D976C-FF5C-417C-9D4C-37855905DF7F}");
            public static readonly Guid Upload = new Guid("{36AD1580-7F22-4BD8-B84B-4951BE47530C}");
            public static readonly Guid AddNew = new Guid("{C5197180-05EB-46BD-A64F-A3A03E575E3F}");
            public static readonly Guid lang_ExportingtoCSV = new Guid("{2D5064D0-9A7A-4486-B61D-A45CF9668565}");
            public static readonly Guid Any = new Guid("{568D8350-89EC-489A-BD8B-91A8A8978C9E}");
            public static readonly Guid Expiringwithin = new Guid("{F53EE39E-5BB2-4C44-8989-3F5B7090546D}");
            public static readonly Guid MoveEnvelopesbyReferencecode = new Guid("{9D006199-CAD9-4D92-9497-546F9A64802D}");
            public static readonly Guid MoveEnvelopesBySender = new Guid("{D2616C14-8D9F-4F87-B5A6-E03F9854BCEE}");
            public static readonly Guid MoveEnvelopesbyReferenceAddress = new Guid("{76EB4582-6A3D-4C64-B2A2-CCEAC0306AF6}");
            public static readonly Guid lang_delegate = new Guid("{F8FA7589-3D2A-425A-9F5A-E5079428FBD5}");
            public static readonly Guid SenttoMe = new Guid("{36D9ACE4-5679-4EC5-B90E-600FF3C56B80}");
            public static readonly Guid SameRecipientsforalltemplates = new Guid("{11A13B84-CD79-4AF8-A2E5-F93A1E84F01D}");
            public static readonly Guid RetailGroupConfirm = new Guid("{179F55FD-AAF0-4961-BEA0-659F02625428}");
            public static readonly Guid WaitingforMe = new Guid("{E59A1A1D-6C8D-45D0-B143-652EC53C280B}");
            public static readonly Guid Apply = new Guid("{007DDDA4-68B0-447D-9EB2-F2E636CAF1CF}");
            public static readonly Guid Received = new Guid("{AF79DAE0-0E1E-4C68-B44A-D2D66FAE6512}");
            public static readonly Guid CreatedOn = new Guid("{8C3371FD-8C8D-418F-ABE0-748911174755}");
            public static readonly Guid SelectFromTo = new Guid("{50612D76-F786-4BBB-B3A2-BD2A0B985317}");
            public static readonly Guid AddMore = new Guid("{FA422594-753A-422E-B2C5-522A64DB65A2}");
            public static readonly Guid AddItemGroup = new Guid("{7A21FF91-7C2A-4DD8-B515-069B645BB1C6}");
            public static readonly Guid CreateDocumentGroup = new Guid("{3353CB17-3C92-4462-B503-B46D9CD55089}");
            public static readonly Guid LoadMore = new Guid("{E70EB727-19C2-40DF-BC78-48B047D0550F}");
            public static readonly Guid DeleteAll = new Guid("{30D1948B-84AF-4076-8351-5C3AF87D0EFF}");
            public static readonly Guid RestoreAll = new Guid("{419E6238-6561-43C9-90BC-A9A445AB9652}");
            public static readonly Guid A4 = new Guid("{B8C5A1AE-53FF-4B9D-A867-3A5EC7FD0D13}");
            public static readonly Guid USLegal = new Guid("{744C3245-3BEC-4FD9-A732-41B7E83C83C0}");
            public static readonly Guid USLetter = new Guid("{5C40037B-EA83-45F4-9B84-9CC7020D4E28}");
            public static readonly Guid AutomaticallyDetect = new Guid("{E9EF5B2B-54AF-4EBB-AC77-73AB382BE16A}");
            public static readonly Guid SelectRule = new Guid("{1A177D36-4D1C-42E6-A908-5717CCDA94D7}");
            public static readonly Guid StampWatermarkSetting = new Guid("{45AD3108-2229-4F74-BA93-077088F0D585}");
            public static readonly Guid DGPostSignPopupDesc = new Guid("{E3950879-FFE2-42AA-9BF2-E0DF2320F549}");
            public static readonly Guid DGPostSignPopupSignAnotherButton = new Guid("{8C8CA552-782C-4951-9236-A5BCD3B11F25}");
            public static readonly Guid DGPostSignPopupBackButton = new Guid("{D219EB75-B9F2-4B0D-B949-D678C2CBBE19}");
            public static readonly Guid DGClickFinishToComplete = new Guid("{E9D13197-DEF1-45F5-910B-64A7E31FA655}");
            public static readonly Guid DGPendingAttachRequests = new Guid("{C8B78D3B-623A-40C0-8976-185CB8039E38}");
            public static readonly Guid DGMyList = new Guid("{60F23E27-8E50-44F6-BBEA-E353313D2A04}");
            public static readonly Guid ViewingPersonalSettings = new Guid("{B91213FD-953D-4957-9903-998AF97878B7}");
            public static readonly Guid YouAreViewing = new Guid("{DFCF3FBB-D227-48EF-BFC9-F978AD029031}");
            public static readonly Guid CompanySettings = new Guid("{DC9184CC-A094-4768-8E25-F33647007490}");
            public static readonly Guid UserSettings = new Guid("{D47A53F1-A7E1-4B94-B057-A3A8774621CE}");
            public static readonly Guid DeclineWarning = new Guid("{8A4589B4-7B1B-44B0-B0AD-0CE30D1EADC4}");
            public static readonly Guid EnvelopefinishedMessage = new Guid("{5D86E565-F701-4A16-9515-EB3DC7738961}");
            public static readonly Guid SignatureRequestReplyAddress = new Guid("{03301BE2-DAE7-49DE-851E-44E441BEAE38}");
            public static readonly Guid SenderEmail = new Guid("{B698C905-7766-4DCD-B2A2-C5C8F333AAA0}");
            public static readonly Guid SignatureRequestReplyToMessage = new Guid("{32407470-2F58-4374-AF6C-99CAE7DED8EE}");
            public static readonly Guid Sending = new Guid("{17E3E69C-6538-446A-8884-EA3C2E937371}");
            public static readonly Guid ShowRSignLogo = new Guid("{99ef7eb6-22db-417a-ad08-362e6a363b9d}");
            public static readonly Guid ShowCompanyLogo = new Guid("{a1544a83-f345-48f5-8ea6-f856b5ddab38}");
            public static readonly Guid CompanyLogoImage = new Guid("{e5bfb06e-c1d4-4ede-8fd6-e5f6c062830f}");
            public static readonly Guid lang_selectCompany = new Guid("{AE12971B-B970-4FA6-8562-67E5690C851E}");
            public static readonly Guid DisplayOnSendTab = new Guid("{5CBAA26F-BC91-402D-8CBC-9C72A4059CFA}");
            public static readonly Guid Profile = new Guid("{FFBE3506-7B5F-4023-BD26-A9FA68B03224}");
            public static readonly Guid Storage = new Guid("{8A6AAC8A-96A6-4079-AED6-6A0AF366C469}");
            public static readonly Guid Advanced = new Guid("{C7C99BD1-D59E-49F5-A74D-C33124A43BF2}");
            public static readonly Guid ServicePlan = new Guid("{FC9F6A02-7979-4660-BB6A-1D291440F3D7}");
            public static readonly Guid EDisclosure = new Guid("{38B19751-59FD-4D13-B53F-469438C36295}");
            public static readonly Guid EnableIntegrationAccess = new Guid("{BF37A7C2-44B5-4071-AB82-3DA7D9AC7551}");
            public static readonly Guid EmailRouting = new Guid("{0AA274DF-D8CF-4EA3-9E54-66D572D3A2FE}");
            public static readonly Guid ElectronicSignatureIndication = new Guid("{BFA01C85-A0F8-4341-AFDC-CCCE660C1D30}");
            public static readonly Guid TextforAllOther = new Guid("{AF749A4A-696A-4B00-B992-776913AB800B}");
            public static readonly Guid ReferenceKey = new Guid("{44075B63-4B42-4A6F-95A9-5A5403702F82}");
            public static readonly Guid SigningCertificatePaperSize = new Guid("{025BBD2F-4048-4E45-ABE7-9E2069C471D1}");
            public static readonly Guid SearchCompanyUser = new Guid("{C14E6DE2-5AC8-4628-B17A-7B9011C4302C}");
            public static readonly Guid SearchUser = new Guid("{1C9A5E6C-F79C-4E47-8AE8-05475A4A4508}");
            public static readonly Guid TextForSender = new Guid("{2740974C-7F98-4993-903F-E5F605508090}");
            public static readonly Guid EnterTextforSender = new Guid("{573D1858-2CD8-44B5-8655-21DE1D5E98B6}");
            public static readonly Guid EnterTextforAllOther = new Guid("{479880D6-D8DE-4124-937C-B338718BF3E8}");
            public static readonly Guid Responses = new Guid("{ED060C78-D8A4-4BD0-986E-C0FBB0F218A4}");
            public static readonly Guid AddOtherWithTextFields = new Guid("{3732EFA8-67A2-4AF3-BD06-53A603252431}");
            public static readonly Guid Controls = new Guid("{8E857F1B-072D-47F1-B79E-399B21E3F1DE}");
            public static readonly Guid DeclineEnvelope = new Guid("{8089D5C0-08CB-4752-B6EC-3333A4F5EA45}");
            public static readonly Guid TitleText = new Guid("{F9CBB171-437F-4CDA-9246-5DC5855EB0F8}");
            public static readonly Guid EmailBanner = new Guid("{9A1545EF-6663-458D-BC14-E06D924B9845}");
            public static readonly Guid EnterLastName = new Guid("{3C8F88EA-839E-483D-9237-18306821978F}");
            public static readonly Guid EnterCompanyName = new Guid("{19E3E879-EF3E-45F0-88A0-025553006FAB}");
            public static readonly Guid EnterJobTitle = new Guid("{39B3A2FA-7C03-42F1-A097-0088A1781673}");
            public static readonly Guid EnterFirstName = new Guid("{C02E81E3-C100-432D-BEB2-C01729E713AE}");
            public static readonly Guid EnterInitials = new Guid("{7676B253-5FE6-4E77-81B1-7CCA9A55AFCD}");
            public static readonly Guid BannerTextColor = new Guid("{c8c6f521-3ab4-4834-b9ca-253e0ef06337}");
            public static readonly Guid BannerColor = new Guid("{a4bfe463-f7dd-484f-8cfe-23d13ac78479}");
            public static readonly Guid ModeViewOnly = new Guid("{5BA869DA-438A-4285-B50D-03FA48B286DA}");
            public static readonly Guid OverrideTooltip = new Guid("{92DB27C8-00FE-43E0-999F-A7F9F895C1F2}");
            public static readonly Guid DeleteData = new Guid("{ECAA2CD8-358C-4637-80EB-5027140B083F}");
            public static readonly Guid DataMasking = new Guid("{D6B591B8-6DE3-4460-B48A-1A7C42FFB764}");
            public static readonly Guid GDPR = new Guid("{3CC5D6BA-A0BA-422C-B0D9-8E22C124662B}");
            public static readonly Guid PrivateMode = new Guid("{1E29E18D-44C0-49F8-A92E-9910E4525927}");
            public static readonly Guid Privacy = new Guid("{F19EBF72-F694-4565-B008-25CFA06657B4}");
            public static readonly Guid RemoveData = new Guid("{1342D97C-743C-4F29-AD78-F12A16743009}");
            public static readonly Guid Envelopes = new Guid("{66822ECC-1DF6-49AB-B1F3-88538CF1440F}");
            public static readonly Guid Account = new Guid("{E8A1DD0B-1E20-440A-BCBA-DEC03D2476E8}");
            public static readonly Guid Or = new Guid("{F77AF692-0768-4B62-A6C8-D354B7BCC624}");
            public static readonly Guid Optional = new Guid("{B776E314-5228-4A64-811F-E3F4C1F07A6C}");
            public static readonly Guid StoreOriginalDocument = new Guid("{24EC9EA9-BCD0-40C6-BF3F-F603541AD285}");
            public static readonly Guid StoreSignatureCertificate = new Guid("{1DA95824-EE7F-4DA5-8E3D-48322B8F2D88}");
            public static readonly Guid DeleteOriginalDocument = new Guid("{05166F76-B258-476C-908A-2CC76CACC89E}");
            public static readonly Guid AllowBulkSending = new Guid("{DCDE76BA-9A2D-49A3-83AB-FF612F79773E}");
            public static readonly Guid lang_DownloadBulkSendingCSV = new Guid("{D5AA0EB1-9D3D-4282-9EEB-BFBA0932D735}");
            public static readonly Guid ExternalDrives = new Guid("{E3AEC0EB-7B9F-4842-A747-5835B38848CE}");
            public static readonly Guid ExternalDrivesSetup = new Guid("{9A66765D-6B28-4700-BCBE-A1640FBEFAD8}");
            public static readonly Guid Browse = new Guid("{FE7DAAE7-6AB7-419B-B59C-99EA8ADADC67}");
            public static readonly Guid TemplateDetails = new Guid("{B56B1324-8E06-4DA6-AC9C-42F68C169FC6}");
            public static readonly Guid DownloadandFillCSV = new Guid("{AD204CD3-22E2-4CD6-B4CC-4C64879D224B}");
            public static readonly Guid RoleType = new Guid("{6FFFBB7E-3C69-4D3E-931A-F10AD559B3E7}");
            public static readonly Guid RecipientName = new Guid("{D041C7AB-5391-4CAB-BFE5-77B535BDCA01}");
            public static readonly Guid MessageTemplateCode = new Guid("{56FDB6A4-BD39-47AB-8935-1DE34CD0ACCF}");
            public static readonly Guid Message = new Guid("{333851F9-10D6-484A-83B6-1AB5A940EA26}");
            public static readonly Guid BulkSend = new Guid("{1396734F-7F33-45E8-B9F2-C625567952BD}");
            public static readonly Guid UploadBulkSendCSV = new Guid("{FD33222E-06E4-47AE-B7DE-4699593E845A}");
            public static readonly Guid RoleName = new Guid("{46397B51-D70D-435F-91C6-3F50D55EB59F}");
            public static readonly Guid RecipientEmail = new Guid("{7CA44903-E9BD-4E2B-A1FF-BA97CB5499BF}");
            public static readonly Guid BulkSendReview = new Guid("{37C496A2-A0FA-4EDA-B0C9-B544B4549F4C}");

            public static readonly Guid RenameFolder = new Guid("{823F8E04-2456-4AEE-8BF4-34C934D37682}");
            public static readonly Guid AlignTop = new Guid("{AA70A8D5-BCA6-4914-8224-04232A689CC9}");
            public static readonly Guid AlignMiddle = new Guid("{82669E74-787A-48C2-806D-AEB4F1C93C85}");
            public static readonly Guid AlignBottom = new Guid("{06C0667C-2026-41AC-9656-E0FA601BCC9A}");
            public static readonly Guid DistributeHorizontally = new Guid("{4ACBB5B0-B418-43DC-A2E8-E13E6421454B}");
            public static readonly Guid DistributeVertically = new Guid("{FCB0EF2D-BE1C-4108-9178-D26B6CABE435}");
            public static readonly Guid TemplateOrRuleSearch = new Guid("{56899DDC-6186-4524-8028-A0E3C0A3E717}");
            public static readonly Guid SendDocumentConfirmation = new Guid("{5A62DC25-A68F-4E92-B7DE-396364E50923}");
            public static readonly Guid BacktoSendDocument = new Guid("{4B78BC92-A10D-4A29-BDA7-F68750EEDFF8}");
            public static readonly Guid ServiceLanguage = new Guid("{68F4A856-8C3F-4702-9970-EA3731A2B664}");
            public static readonly Guid LockSettings = new Guid("{E62BB4D7-9857-43CA-A632-9CDD9A2A1821}");

            public static readonly Guid SignIntoYourAccount = new Guid("{25EE3A5E-5D2D-487D-9DB5-4377530E8798}");
            public static readonly Guid Notyou = new Guid("{9F112364-4747-463D-ADC0-B3AC36FC7C0A}");
            public static readonly Guid Signinginas = new Guid("{0F0E0477-02CE-4597-A20D-A6F6C7A1B8B6}");
            public static readonly Guid PostSigningUrl = new Guid("{EF51757F-F0F7-4F29-A74E-87B02B8E9F8E}");
            public static readonly Guid ColorPickerMsg = new Guid("{4134C9B7-5D87-43DD-9A37-372502A0AF8D}");
            public static readonly Guid EnvelopesPostSigningUrl = new Guid("{0162FD3C-957D-410F-97F4-497103BA2A8B}");
            public static readonly Guid Other = new Guid("{D96A5866-E541-45C3-88CE-473CF512B4BA}");
            public static readonly Guid History = new Guid("{E2FD5A60-D72C-4224-9D26-DBCB8B568614}");
            public static readonly Guid ModifiedDate = new Guid("{D2848BB3-CC74-4301-AF00-6779BDEEAF2D}");
            public static readonly Guid NotSelected = new Guid("{47AE9D46-C5C8-4650-89AD-BC77DE51E19B}");
            public static readonly Guid CompanyHistory = new Guid("{BE98EDE7-E68E-4C97-A3CE-E8083AA67C90}");
            public static readonly Guid UserHistory = new Guid("{A3499E71-E713-4821-9F90-877B09767C60}");
            public static readonly Guid PreviousData = new Guid("{F311A251-E8D3-4C0A-904D-1D467230B578}");
            public static readonly Guid UpdatedData = new Guid("{A51EE5E2-3DFE-45E2-B939-BCC5A5D21D78}");
            public static readonly Guid GoogleDrive = new Guid("{5458DC49-A9F4-40CA-846C-FDEBA283AA5F}");
            public static readonly Guid OneDrive = new Guid("{3DAD5DBE-C374-4434-B4C5-C13C3EE32F5D}");
            public static readonly Guid Dropbox = new Guid("{E593BD02-C8BA-45DE-8546-0441F6F146AA}");
            public static readonly Guid lang_notfoundHistory = new Guid("{A01BB9FC-8C79-4CC7-AE8B-4EE6710FC482}");
            public static readonly Guid lang_NA = new Guid("{D3458BBC-6262-4605-917E-4E97AC651EDB}");
            public static readonly Guid lang_DocumentSentInProcess = new Guid("{7123E18D-1B88-414F-836B-8894D36E9436}");
            public static readonly Guid lang_DocumentFormatChangedWarning = new Guid("{01836DB4-A221-4CA2-B154-FAC798EF9FEF}");
            public static readonly Guid iManage = new Guid("{48C710C8-9F49-410A-A6AD-6132E5C35927}");
            public static readonly Guid netdocuments = new Guid("{2E5DD5D3-672B-493E-8A8A-1C83C1903B9C}");
            public static readonly Guid AppliedEpic = new Guid("{AFE3E372-3B08-42CB-B7B9-6D8AD510CD71}");
            public static readonly Guid Bullhorn = new Guid("{060AC0D1-5713-4DEF-8FFD-D6A910DE6E1B}");
            public static readonly Guid Vincere = new Guid("{E9F13916-6D4D-43C0-BE79-B7E7538A05D9}");
            public static readonly Guid Source = new Guid("{3331AEAD-348E-4B4F-8C8F-DE02526B9AB3}");
            public static readonly Guid lang_none = new Guid("{23EE52A0-41EB-485C-AEC3-B446768C8F8C}");
            public static readonly Guid CurrentSettings = new Guid("{64F9CC80-E2FC-4518-A721-7140976C87A5}");
            public static readonly Guid Size = new Guid("{DAB925F0-3FB9-4F57-8B82-EE9BB2A68910}");
            public static readonly Guid ManageEnvelope = new Guid("{229A92EE-8558-4F28-A225-D1ADF9136697}");
            public static readonly Guid RegenerateFinalDocument = new Guid("{897806E7-7044-4A66-97A9-8A67C816A6A2}");
            public static readonly Guid Contracts = new Guid("{C91BC834-D590-4D35-B05D-8C17DD9B4B7A}");
            public static readonly Guid SignerAttachedFile = new Guid("{663FEADE-1EF3-48CB-A0BC-B4275820E6C3}");
            public static readonly Guid lang_MoreActions = new Guid("{A69529B5-BC00-4C99-8744-48BBC408B686}");
            public static readonly Guid DocumentType = new Guid("{29918858-B944-4B22-B81C-794761145A45}");
            public static readonly Guid OriginalDocument = new Guid("{99D5CE62-CD39-46D1-80CB-EBA4EF59B5C9}");
            public static readonly Guid EnableFileReview = new Guid("{A1921EC5-2708-4163-9B04-518611042F57}");
            public static readonly Guid ShowControlID = new Guid("{8336357A-D384-4C9C-A74F-9D61474429A3}");
            public static readonly Guid TemplateName = new Guid("{E7599CAD-30C1-49BA-811B-014569A75E89}");
            public static readonly Guid RuleName = new Guid("{89726913-C134-4940-B62B-F81F5D388CA0}");
            public static readonly Guid FileReviewStandardTemplate = new Guid("{4FBF0F57-48E8-4DB2-8943-F6E70097B75D}");
            public static readonly Guid DeletedBy = new Guid("{47A8C53C-93A3-4841-856E-F0BCC8AFB7E5}");
            public static readonly Guid DeletedOn = new Guid("{456B7672-3CE5-4503-A139-E66C959F52AD}");
            public static readonly Guid SaveAndContinue = new Guid("{BD2861F5-3133-4701-9A85-09B45DEC2A3F}");
            public static readonly Guid AllowRuleEditing = new Guid("1B26376D-AB62-4354-89CB-68DB71659F09");
            public static readonly Guid AllowRuleUse = new Guid("{049E8599-6C90-4DF4-B82B-2778E989896B}");
            public static readonly Guid SignatureRequest = new Guid("{560129B3-D5C3-45E3-BB0B-4A686AB4C238}");
            public static readonly Guid SendingConfirmation = new Guid("{1BD2C2A3-EB87-4813-B79D-406D9258E6F5}");
            public static readonly Guid PasswordNotification = new Guid("{0A065A6D-B244-4100-AF87-28848DF0BAE3}");
            public static readonly Guid SignatureCarbonCopy = new Guid("{87922E84-6F1C-430A-A23C-2B2E2CC6E087}");
            public static readonly Guid SignatureReminder = new Guid("{4EA9B13E-4747-437F-A448-FE6E9475DF18}");
            public static readonly Guid DocumentSigned = new Guid("{B8F31C47-9A7E-4E0C-B440-50BF03341339}");
            public static readonly Guid FinalDocument = new Guid("{DABA451D-F985-42BF-9C2D-2AF7C62F5996}");

            public static readonly Guid SharedRuleCode = new Guid("{A91B4801-31C7-4280-BAF6-12A086FCF2FD}");
            public static readonly Guid SharedRuleName = new Guid("{EB327189-2D5D-4E58-9377-198365A7F439}");
            public static readonly Guid SharedTemplateName = new Guid("{CF6892FC-E102-457A-B640-1A8B5FAAAAF8}");
            public static readonly Guid RuleVersionHistory = new Guid("{3BAB1783-8249-4FE8-B36E-239803F5EF35}");
            public static readonly Guid SendToRSign = new Guid("{F31DB518-621A-4171-9FD3-381FA9D506EF}");
            public static readonly Guid SharedBy = new Guid("{33EE694B-A073-4305-AD94-42135F7B0274}");
            public static readonly Guid CreatedBy = new Guid("{E219FE4B-E6D4-4992-B786-5C10C1596415}");
            public static readonly Guid TemplateVersionHistory = new Guid("{6AB802A5-2D79-425C-B6B0-7C21605308DB}");
            public static readonly Guid Envelopecreationinprogress = new Guid("{206F81CC-1D4C-4559-85C8-848B2E0A967B}");
            public static readonly Guid VersionNo = new Guid("{3675AD3F-ACBC-45EF-B124-A7A87F9DF22E}");
            public static readonly Guid LastModifiedDate = new Guid("{7A530B73-7F32-436D-93A0-A994C78DD9D5}");
            public static readonly Guid SharedTemplateCode = new Guid("{F3CB62D3-5EEE-4302-B906-B249CABC9EB7}");
            public static readonly Guid ViewHistory = new Guid("{F6D56928-87CA-4D80-9AB2-C1A74A888CFA}");
            public static readonly Guid UpdatedDate = new Guid("{77CDFF27-79BA-41E6-9AAC-F4F6D87212C4}");
            public static readonly Guid AllowCopyTemplate = new Guid("{16DE5081-F3AB-45BF-AC41-DAA8D1681AC5}");
            public static readonly Guid ShareMasterTemplate = new Guid("{286D47DA-5CCB-4C20-8892-A7A75AACC742}");
            public static readonly Guid Both = new Guid("{D85B9268-6131-4C8A-9805-95B748D8AE05}");
            public static readonly Guid ConfirmShareTemplate = new Guid("{1059E8E5-FCF5-4A9A-88BB-F1E311CDCD1A}");
            public static readonly Guid SharedOption = new Guid("{A35C77BA-29CB-47FD-9F6E-BF92B84DDE85}");
            public static readonly Guid lang_sharing = new Guid("{51D69A43-A8BD-4297-8AEF-6902039823B2}");
            public static readonly Guid AllowMessageTemplate = new Guid("{466A8951-281F-4EB3-BBC2-A554877E6209}");
            public static readonly Guid MasterTemplates = new Guid("{1178B156-7F13-46E3-9E73-B81242D602A9}");
            public static readonly Guid lang_templateNotAvailable = new Guid("{5CD65B79-C563-46E9-84C9-B971E34170BB}");
            public static readonly Guid MasterRules = new Guid("{DE84F6CC-1C5A-46B3-8D0A-9C2A787AB4CD}");
            public static readonly Guid PersonalRules = new Guid("{E2CCD302-9357-46DF-8A09-BEFECFC3EB84}");
            public static readonly Guid lang_ruleNotAvailable = new Guid("{47DF468C-ADCF-49A1-B47D-7CCA13D416CC}");
            public static readonly Guid lang_ValidAllowEdit = new Guid("{157139C6-FF15-454A-AAA3-1FE3BF6439FE}");
            public static readonly Guid lang_ValidSharing = new Guid("{2E0D2D91-7BE2-4948-98E0-14E0259907FA}");
            public static readonly Guid Master = new Guid("{01E9D0AA-88CD-4229-A02E-7654260AD6D6}");
            public static readonly Guid SharedAsMaster = new Guid("{D5F6EBFD-87E1-4CE3-92C6-610B7310078E}");
            public static readonly Guid SharedAsCopy = new Guid("{5BEC15DB-3543-410C-AD4C-E3498BEDAFF3}");
            public static readonly Guid SharedAsBoth = new Guid("{D749D834-0D69-4B9B-A2F9-46303EB93A56}");
            public static readonly Guid lang_EmptyMasterTemplate = new Guid("{5CCF1DAB-4BA2-4ABB-BDF7-D01D53112CB3}");
            public static readonly Guid lang_EmptyMasterRule = new Guid("{2D98AB06-B1FA-424A-ADCA-A4F75CE508B7}");
            public static readonly Guid lang_AllRoles = new Guid("{894E96B5-9428-4226-92FA-B768B6E75C4C}");
            public static readonly Guid ClickToShare = new Guid("{5ED5544D-EA51-459D-979D-F2FE2A5694A3}");
            public static readonly Guid ClickToDelete = new Guid("{B2499F5F-6402-4133-9BB3-717C718B3EEC}");
            public static readonly Guid ClickToEdit = new Guid("{5EDCF1A1-8734-474F-8485-830A32FAB993}");
            public static readonly Guid ClickToUnshare = new Guid("{EF407CF3-C515-4EC1-89A0-C52670DDEFB4}");
            public static readonly Guid ClickToCopy = new Guid("{2C729465-D818-4C62-B262-AF4F3E182D9B}");
            public static readonly Guid ClickToAdd = new Guid("{4EBBEF78-A894-42F6-A263-F3F45629715A}");
            public static readonly Guid Preview = new Guid("{258DBE19-9F81-4D8D-913F-0469AE78B324}");
            public static readonly Guid AssignedTo = new Guid("{24B7610C-989E-480E-AC85-238A1E9E51C7}");
            public static readonly Guid AdditionalAttachments = new Guid("{A7FF94BC-0836-4AEA-9DAD-F072F866DD00}");
            public static readonly Guid DeliveryStatus = new Guid("{22FBF1A2-01A9-48F7-8EE6-A172AC67452C}");
            public static readonly Guid AssignedToAll = new Guid("{F90FED0F-08BD-45F3-B18D-6FBCFB249876}");
            public static readonly Guid EnableClickToSign = new Guid("{29ADE289-EC1F-461A-AAFD-0591F8F32FBE}");
            public static readonly Guid PostSendingNavigationPage = new Guid("{989C2B87-C7ED-4F6D-A58F-DBDA175C9837}");
            public static readonly Guid EnableAutoFillTextControls = new Guid("{D300901A-F359-455A-BD7E-F01C575478E1}");

            public static readonly Guid SignerDetails = new Guid("{B5C303E2-85A0-47BA-AE07-3F1C6C5B0251}");
            public static readonly Guid ClicktoSign = new Guid("{908BC191-6129-40DD-9018-E168245475FC}");
            public static readonly Guid ConfirmClicktoSignMsg = new Guid("{AD015C70-E514-471D-862D-65A4E7FD4028}");
            public static readonly Guid SignerTermsCond = new Guid("{4C983255-3604-4C21-84D0-92990960941E}");
            public static readonly Guid AcceptandSign = new Guid("{5FE387E8-1304-49CF-A6B4-07B47A1FC3B3}");

            public static readonly Guid DataControl = new Guid("{B20205F1-85C8-4B77-A7E6-A397598F12D9}");
            public static readonly Guid PersonalControl = new Guid("{65A22287-ED2C-4163-B5D2-24ACCD692863}");
            public static readonly Guid SignatureControl = new Guid("{E02FF794-35AC-4BE8-9B4E-3EA19CD329FC}");
            public static readonly Guid AdvanceControl = new Guid("{D93CF3D0-0A9D-4A25-93DF-95B6AAF44566}");

            public static readonly Guid lang_dataControl = new Guid("{4EE21C4E-C0D7-4267-9B54-2D18DD2897EE}");
            public static readonly Guid lang_PersonalControl = new Guid("{FA893233-9099-4C8A-9A5F-D0E779EEFF8D}");
            public static readonly Guid lang_SignatureControl = new Guid("{896312C6-9B8C-44E3-8A4F-A416DBCDB64E}");
            public static readonly Guid lang_AdvanceControl = new Guid("{1833B5B4-F202-4C19-B906-E9175389DF0A}");


            public static readonly Guid ClickToMoveRightControl = new Guid("{07E2BA10-B31B-4FDA-BF53-D9606E1C9E0C}");
            public static readonly Guid ClickToMoveLeftControl = new Guid("{47621C76-E9B9-4919-B076-A370EEBC7E97}");
            public static readonly Guid BottomControl = new Guid("{83444A8F-D1F5-4C55-BEC0-6CA32B51B386}");
            public static readonly Guid lang_defaultDateSigned = new Guid("{97C864A2-653C-4830-8187-F42B6376B194}");
            public static readonly Guid CustomTooltip = new Guid("{431484AF-6EBB-4414-9210-D2937B4B46C9}");
            public static readonly Guid TextColor = new Guid("{1BBBAB56-D8D5-4F84-9396-F6DD50ABE600}");

            public static readonly Guid URL = new Guid("{627A03AA-9CA4-48FA-BADB-6947E415D53C}");
            public static readonly Guid TexttoDisplay = new Guid("{8C2F49BD-D038-485C-8E9A-6D82AAD2A565}");
            public static readonly Guid lang_savedefault = new Guid("{8DA068CF-9E61-4C8F-9CA8-357A32032822}");
            public static readonly Guid lang_removedefault = new Guid("{5EBE0307-9020-4956-91CC-3A07FAA74865}");
            public static readonly Guid lang_successsavedefault = new Guid("{B509672C-C7D1-4273-96E1-778F0F4515A1}");
            public static readonly Guid lang_successremovedefault = new Guid("{D400D7FF-7D1F-4EE7-8F9C-607602A61D49}");
            public static readonly Guid AlternateSigningOption = new Guid("{AA0C3458-D155-42EC-BD3C-8FCE5C810DDD}");
            public static readonly Guid QuestionsAboutDocumentText = new Guid("{715B8173-7B8F-42AA-9A3B-75498A72DDAA}");
            public static readonly Guid QuestionsAboutDocument = new Guid("{7ABEA5E1-3930-4694-9E89-FB17B75E0437}");
            public static readonly Guid DoNotShareMail = new Guid("{1F473762-D287-401A-8C33-59E7251AAFAF}");
            public static readonly Guid InviteExpiresIn = new Guid("{C1F08281-3B92-4341-BB36-416389523AFC}");
            public static readonly Guid SignFromRSignWebSite = new Guid("{B952A746-8DB2-470C-B53C-010A7E3D6D67}");
            public static readonly Guid LeagalNotices = new Guid("{293D1DA7-910B-4F9E-81B6-4EDCCF430465}");
            public static readonly Guid PatentTrademark = new Guid("{2AEB7505-2BB0-4D98-9340-89E668D0DA8F}");
            public static readonly Guid TermsConditionsPrivacy = new Guid("{BE494177-324D-46E6-8CCE-C74A06DA9F7D}");
            public static readonly Guid GeneralTermsAndConditions = new Guid("{4FEB8761-3F73-46CE-9636-672755254901}");
            public static readonly Guid EnableWebhook = new Guid("{775B86A4-1137-4254-94C5-93430DF5B4C3}");
            public static readonly Guid ValidateSigner = new Guid("{E99F3D36-D42C-4634-ABAA-6EAE51892B95}");
            public static readonly Guid TypographySizeHint = new Guid("{7985D3B9-3596-49BC-8C32-247E0A193BC5}");
            public static readonly Guid TypographySize = new Guid("{BBBAA225-9330-4200-A954-0FABAB6C377F}");
            public static readonly Guid UploadSignature = new Guid("{F09ABA05-C4BB-4383-B382-30AE01F812A4}");
            public static readonly Guid ViewSettings = new Guid("{CC147CC8-ED00-47CE-939C-F120CC6894B8}");
            public static readonly Guid OnlycharactersAndnumbers = new Guid("{CAFD9E78-1CA7-4A80-9AF1-B11C35E77DC6}");
            public static readonly Guid checkboxGroupsOptional = new Guid("{52258AAE-CC2F-4ED1-B8FC-1D8DCDF177F2}");

            //public static readonly Guid ForgotPasswordSubmit = new Guid("{8DE5D787-26F9-4756-B68A-3D030D87DF32}");
            public static readonly Guid BackToLogin = new Guid("{E273049D-E34B-4571-B219-504CCA3D7855}");
            public static readonly Guid ForgotPasswordLabelText = new Guid("{FB9EA4C5-6C0F-4C9F-AC51-9F1FF0FEC1BF}");
            public static readonly Guid EmailIsRequired = new Guid("{78AC958F-4BF8-4EF6-B7BE-39152C04F8CB}");

            public static readonly Guid lang_export = new Guid("{B77BCAFE-DE2A-4395-8E0C-6D853DD418EB}");
            public static readonly Guid exportCurrentView = new Guid("{67837E88-1969-482B-8C6F-AE66B03F8F6D}");
            public static readonly Guid exportAll = new Guid("{E91CB2F4-16CE-4063-8026-DEFE0C22096E}");
            public static readonly Guid LoggedinUser = new Guid("{6092A97B-BCB6-4F09-835D-875BEA1AE10E}");
            public static readonly Guid UploadSignedDocument = new Guid("{B832E543-D93C-4432-BD10-38139130966A}");
            public static readonly Guid Move = new Guid("{ACCE125A-B620-4B8F-96F3-1A3EE7308F68}");
            public static readonly Guid SearchAll = new Guid("{BB9C9736-3AE7-4005-84F7-A97BEBE90FA8}");
            public static readonly Guid SelectFromToFilter = new Guid("{7EEE4F8D-9C3E-4242-9B9F-696349EB879E}");
            public static readonly Guid SelectReferenceCode = new Guid("{4E5BB8C5-4F02-45CE-9F86-510E38E6F13D}");
            public static readonly Guid StartDate = new Guid("{FBB25A2C-DF10-4D31-AFDD-27593FE63830}");
            public static readonly Guid EndDate = new Guid("{ADA247F1-648F-4815-AF82-F1B00F93A8EE}");
            public static readonly Guid orText = new Guid("{F4A168B9-A7BC-4F77-B8EC-F6317EC0FF1F}");
            public static readonly Guid AddName = new Guid("{54D9E3B9-F24D-49A9-B487-58D8EB8C3564}");
            public static readonly Guid AddDescription = new Guid("{9A87A712-8D4B-4EA9-B7CF-CD9D23EE5DA1}");
            public static readonly Guid AddAdditionalInfo = new Guid("{4792A16C-90C6-4D01-94F4-414A78EB78D5}");
            public static readonly Guid CreateEnvelope = new Guid("{6CA51FB2-7E65-4936-B34C-9521C6872022}");
            public static readonly Guid EnvelopeStatus = new Guid("{29F725D7-0C91-433C-9141-02C5095176F6}");
            public static readonly Guid HyperTexttoDisplay = new Guid("{D3CD7352-5A9D-46E2-B17B-B8866D176806}");
            public static readonly Guid Register = new Guid("{1D504526-617E-41AF-A20E-C132A57DAE5C}");
            public static readonly Guid DaysExpire = new Guid("{95F81600-5DE3-404C-AEEB-5A32A1DE7B3F}");
            public static readonly Guid lang_SearchUser = new Guid("{096E1695-0ECD-4690-9C09-C5538C9AD64E}");
            public static readonly Guid SelectContact = new Guid("{040C1768-4AB3-41F5-8620-0D7B82C35E6B}");
            public static readonly Guid ContactSearch = new Guid("{8F3110F5-A42E-4AC8-828E-4A5DD8D977DC}");


            //Start - Contacts Language Keys
            public static readonly Guid NoContactsAreAvaliable = new Guid("{6F5167DF-90BF-4546-84D3-AB8E2DBB86EE}");
            public static readonly Guid ManageContacts = new Guid("{02838C2A-F916-445F-94B1-BB5B0CC5F6D9}");
            public static readonly Guid Contacts = new Guid("{40F9BCCB-C636-4E76-9302-0ABF6D5B552B}");
            public static readonly Guid AddContact = new Guid("{D7FD00E1-62AF-402C-9A66-32F3FD3D17BD}");
            public static readonly Guid EditContact = new Guid("{5CA56DBD-EAFA-4FDA-968D-C86E212AE6D8}");
            public static readonly Guid DownloadOrDragDrop = new Guid("{C9B2DA42-7F93-4732-883B-3C17608C38DB}");
            public static readonly Guid DownloadXLS = new Guid("{8272BE28-6205-484D-9D66-4D82D9B140E6}");
            public static readonly Guid DownloadCSV = new Guid("{02169A30-AC59-482A-8C9F-28C1C0465EB7}");
            public static readonly Guid SupportedFileFormats = new Guid("{8E282745-387E-49ED-84DC-AC79886373CA}");
            public static readonly Guid Warning = new Guid("{C0544551-2B5F-49EC-9DF8-F1099C3E38C2}");
            public static readonly Guid UploadedBy = new Guid("{B56A57E9-7EF8-4D10-A20B-5FB0A08450AE}");
            public static readonly Guid UploadedDate = new Guid("{A98D0140-B3C6-4D84-8F6C-46C0A654CD7A}");
            public static readonly Guid LoadingFiles = new Guid("{F1FA7068-B6AE-4DC4-B637-DEDB1FCF13B3}");
            public static readonly Guid LoadingContacts = new Guid("{0ECD9345-A042-4D42-B0C8-048A6DC4F2AE}");
            public static readonly Guid DeletingContact = new Guid("{3FF0DF03-A755-453F-98B2-945E5494ED7D}");
            public static readonly Guid DeletingContactText = new Guid("{58DAC415-7575-445C-9CD1-EFD5CCFDBD35}");
            public static readonly Guid SupportedFilesFormatValidationText = new Guid("{32B40711-1B71-4CD8-90D4-8BF2EE1FB75F}");
            public static readonly Guid ValidCSVOrExcelFile = new Guid("{5244A0B1-D1B9-4D6C-8FF4-5081D9C049F4}");
            public static readonly Guid InValidEmailAddressText = new Guid("{7C787FCD-241B-4F40-B4BE-EE4D75624A5C}");
            public static readonly Guid DuplicateEmailExistsInFile = new Guid("{BCAB0613-30A5-43A5-B054-4B345338275D}");
            public static readonly Guid OverWriteText = new Guid("{6B56B859-7D8D-4630-A270-D79E4C4CB1A5}");
            public static readonly Guid DeletingContactSuccess = new Guid("{143B8F70-3B64-45DF-8FA3-40CFDFA53E16}");
            public static readonly Guid ContactSavingSuccess = new Guid("{BDDEECEE-25B9-40B2-9672-FE0E93B9ACA8}");
            public static readonly Guid ContactNotExists = new Guid("{92318E8D-6398-459C-9893-BCDB8AEEEFEC}");
            public static readonly Guid NameEmailRequired = new Guid("{E2DC7642-7991-4795-AB8B-2521A7A200BD}");
            public static readonly Guid SavingContacts = new Guid("{9903D0AD-7B1D-4F07-AE04-5B8629C6CCC0}");
            public static readonly Guid OverWriteContact = new Guid("{B7A6DE04-1834-4B1E-91F8-4E55AD93DCC5}");
            public static readonly Guid ContactAlreadyExists = new Guid("{41FDE2FE-700D-4A67-960D-B2C92F4C17F8}");
            public static readonly Guid ClickHereToSort = new Guid("{6B8AD2AB-000F-4E48-86F0-BF59EB353637}");
            public static readonly Guid ContactPlaceHolderSearchtext = new Guid("{8F3110F5-A42E-4AC8-828E-4A5DD8D977DC}");
            public static readonly Guid SerialNumberText = new Guid("{0F5C6A8E-882E-4008-B078-699E41583361}");
            public static readonly Guid EnterName = new Guid("{C36E85AB-74B5-420B-9838-BAEE73DA499A}");
            public static readonly Guid EnterEmailonly = new Guid("{3D35EE0E-891B-4420-B14D-B55CB24C9A96}");
            public static readonly Guid EnterCompanyonly = new Guid("{A088AF8B-F6F1-453A-891D-0E9B0BDA9E36}");

            //End - Contacts Language Keys
            public static readonly Guid EnterPassword = new Guid("{A965B8FB-52A4-4065-9B30-4035E9B71F2F}");
            public static readonly Guid Success = new Guid("{CC207874-D065-46AD-8C51-07B00BAFA1EB}");
            public static readonly Guid Redirecting = new Guid("{6A80203D-D572-49B9-A967-4E0E666BB328}");
            public static readonly Guid DocumentPassword = new Guid("{65E0A063-BDA0-4BC6-97EF-0173B00B28B6}");
            public static readonly Guid InvalidPassword = new Guid("{5FC1AF01-90EE-4637-AB16-4CDF8D9A83A9}");
            public static readonly Guid EnvelopeExpiredGDPRSettings = new Guid("{550185F6-9745-4628-AC65-F1C51F937098}");
            public static readonly Guid Remarks = new Guid("{CE3B29EE-2920-42C1-AD33-D1AB2ABA811B}");

            public static readonly Guid Lang_LogonCamelCase = new Guid("{5E917F61-9697-4918-AB67-57506FC6E65C}");
            public static readonly Guid Lang_NoThanksCamelCase = new Guid("{9D9DBEC3-BF4F-489F-9A62-36CB488DF029}");

            //Start-Custom Columns
            public static readonly Guid SelectColumnsLabelText = new Guid("{7CEB184D-4EB5-4267-BBB3-9E7E8E2AE28A}");
            public static readonly Guid CustomColumnLayoutLabelText = new Guid("{91F4B022-3ED5-4E95-8D75-F62650124915}");
            public static readonly Guid AvailableColumnsLabelText = new Guid("{597E6E7E-888E-4F4E-914A-25AC8AB7A9A1}");
            public static readonly Guid SelectedColumnsLabelText = new Guid("{E3EC426D-FFF7-49E7-BF0C-7B208CA4C3BD}");
            public static readonly Guid title_sortSender = new Guid("{1AA8A98B-8369-4023-ABBD-C75283E3C8C0}");
            public static readonly Guid MandatoryColumnsValidationText = new Guid("{4685685D-E170-405B-9E74-085BAB9454F8}");
            //End-Custom Columns

            public static readonly Guid Welcomeback = new Guid("{0C7E0314-8A0C-4C83-9DA7-32F43723FF87}");
            public static readonly Guid SendmetheLink = new Guid("{EBA32940-11A8-4815-92C5-734F90E68307}");
            public static readonly Guid ForgotyourPassword = new Guid("{FC3A03D3-D98B-4258-A617-14804A25B595}");
            public static readonly Guid SignmeIn = new Guid("{364E245F-FA63-4142-90FD-1B4D43CE3706}");
            public static readonly Guid Letsgetstarted = new Guid("{A4ADD2AA-6370-4A1E-A1EA-15C0DDC220FF}");
            public static readonly Guid CreateNewPassword = new Guid("{BAA601A7-5C72-4272-B351-EB00650C6377}");
            public static readonly Guid SendReminderTillExpiration = new Guid("{B31655E5-E726-4EB8-81D8-ABBD612110FA}");
            public static readonly Guid EnvelopeExpirationRemindertoSender = new Guid("{7E70886B-54D8-4BC4-B059-A1FA0593F28A}");

            //Prefill User Confirmation Text
            public static readonly Guid PrefillConfirmationFinishLater = new Guid("{605A516E-8873-4622-AD32-804AD827E5D4}");
            public static readonly Guid OKText = new Guid("{9D30B519-47FF-463A-82A5-6FBB42A57E5A}");
            public static readonly Guid lang_BackConfirmationS1 = new Guid("{FCB3DFF0-6F92-4437-A452-FBD4A9BE62B2}");
            public static readonly Guid lang_BackConfirmationS2 = new Guid("{4867AE81-EC7B-49C9-8FED-06A2F8ADB66C}");

            public static readonly Guid From = new Guid("{D7842720-BEB0-416E-95BC-689CBA3B03B6}");
            public static readonly Guid Final = new Guid("{DAE0F7E6-EB61-4836-8543-6FE69E286AA0}");
            public static readonly Guid FinalStepDescription1 = new Guid("{95253969-A90D-4E39-9511-4CDB8947EE6A}");
            public static readonly Guid FinalStepDescription2 = new Guid("{9042C47F-C8A9-4038-9DD6-20B4F06131E1}");
            public static readonly Guid FinalStepDescription3 = new Guid("{50D28A33-A64D-437A-863B-C5CC65BC6CCB}");
            public static readonly Guid RPostActivationEmail = new Guid("{5EE4D0F5-CDEE-463C-B2FA-41DB22854615}");
            public static readonly Guid FinalStepDescription4 = new Guid("{820F4C15-84C6-4281-B788-416392325641}");
            //Static Template
            public static readonly Guid AllowSingleSigner = new Guid("{F9C6FE71-B8EF-4683-811C-42CA3D6BD249}");
            public static readonly Guid AllowMultipleSigners = new Guid("{A22131B1-BE90-4A2C-8CB1-4DFC4971B53F}");

            //Start -- Allow Multi Signer
            public static readonly Guid AddSignControlStaticTemplate = new Guid("{F97A6596-577E-4FAD-9ECA-ABE680D1E4E0}");
            public static readonly Guid TermsandConditions = new Guid("{2DEB715A-6B4C-4A7B-91AC-D456E709E62E}");
            public static readonly Guid ConfirmYourEmailAccept = new Guid("{CA0B41AB-BE2A-4284-ACBF-993B9CA0EDA9}");
            public static readonly Guid AcceptandContinue = new Guid("{C86CA854-92A6-45DA-ADBB-004D464065F6}");
            public static readonly Guid SigningAs = new Guid("{743E2355-CB89-4296-87B7-7A554FCBD5E2}");
            public static readonly Guid InviteSigners = new Guid("{E3F93499-AD60-4DF5-8C0E-FC7EE0FC400C}");
            public static readonly Guid SignAs = new Guid("{6B112C1A-5AB3-4274-B0AD-86697D5B2EF4}");
            public static readonly Guid SignNow = new Guid("{D362F62F-CA4E-4880-8F1E-6903CDBA5892}");
            public static readonly Guid InviteByEmail = new Guid("{29FAA277-105E-479C-A654-80B67D62AF91}");
            public static readonly Guid CanEdit = new Guid("{BDEA933F-9664-43A4-AB54-92A0468997CC}");
            public static readonly Guid Confirm = new Guid("{3B1F4201-52DD-4501-BFAD-8DDACD240F68}");
            public static readonly Guid SelectInviteSignerAndSignNow = new Guid("{0ABC205D-D1EE-46B7-938C-F6F199D3914E}");
            public static readonly Guid CannotInsertDuplicateEmails = new Guid("{E9289ECA-FC95-4D65-9AF4-76CFFC94E05C}");
            public static readonly Guid SelectSignAs = new Guid("{F2FB594B-BF9A-4A70-955A-C30F0412EDFA}");
            public static readonly Guid MultipleSignersRequired = new Guid("{A8398865-DCDF-4F69-BAF8-48BA11BB9A6C}");
            //--End

            public static readonly Guid FinalStepDescription5 = new Guid("{A7344641-9CE0-4DB9-87DC-45419036FFD9}");
            public static readonly Guid ChooseInitials = new Guid("{F1601DF5-5C75-40B1-A648-1D76EE7CF592}");
            public static readonly Guid Acceptingthetermsandconditions = new Guid("{6880F76B-6136-423E-8DA4-D2B6E9D26BA1}");
            public static readonly Guid Lang_Information = new Guid("{95C106C9-F93D-4BE1-8EF6-DECE953C7E3B}");
            public static readonly Guid Lang_InformationWarning = new Guid("{6EBE22F5-5F9A-476E-89A5-4EC946EDD096}");
            public static readonly Guid Lang_SigningWarning = new Guid("{41F3BFD7-0241-43FE-AD56-66671AC2071B}");
            public static readonly Guid Lang_Signing = new Guid("{FFD945D7-261A-4492-8954-5A96B8F51760}");
            public static readonly Guid SendConfirmationEmail = new Guid("{06E9C7F1-C1EB-438A-95CE-2ECA5E4D1C11}");

            public static readonly Guid EmailVerification = new Guid("{1593CED0-6F85-4CA3-87CC-8D52050C3772}");
            public static readonly Guid MyDetails = new Guid("{66674093-9F44-47D4-BF24-3835085E4964}");
            public static readonly Guid ShowTags = new Guid("{18CE2312-4D51-4518-8A8F-B643A21B0D80}");
            public static readonly Guid HideTags = new Guid("{EB421BC0-73D6-4ACD-A868-2051593B657C}");

            //S3-2280 Context Help Language Keys Transalations
            public static readonly Guid AccessAuthenticationTip = new Guid("{9DD29819-46E4-4695-9037-FA1DB2C82B7F}");
            public static readonly Guid AddDatetoSignedDocumentNameTip = new Guid("{621000AF-E774-4EF4-841F-BC789009E583}");
            public static readonly Guid AddresstoCopyAllRSignEmailsTip = new Guid("{B261DE90-0B4D-4B9D-97A6-4BC018F06A2B}");
            public static readonly Guid AddresstoReRouteAllRSignEmailsTip = new Guid("{FB88E754-3E06-4500-9C9E-F0D7256FBC15}");
            public static readonly Guid AllowBulkSendingTip = new Guid("{A1603172-CCD8-4844-9A29-02165ECB918F}");
            public static readonly Guid AllowedUnitsTip = new Guid("{494A747A-2A41-4F23-AEA3-06D12699D807}");
            public static readonly Guid AllowPostSigningLandingPageTip = new Guid("{0BDDBE34-8A67-48E9-A90B-6A95B63E2781}");
            public static readonly Guid AllowRulesEditingTip = new Guid("{F4632619-F687-4CF9-B3FD-1830D8F4FDAA}");
            public static readonly Guid AllowSharingOfMessageTemplateTip = new Guid("{A689E316-BE9D-4957-B6A1-7AD8EF3F9E21}");
            public static readonly Guid AllowSharingOfTemplatesRulesTip = new Guid("{1F1ED7E5-112C-40FA-84AB-7993F55819AB}");
            public static readonly Guid AllowTemplateEditingTip = new Guid("{A5E6B5BA-36FD-44A2-8EB9-6CAD7F494E58}");
            public static readonly Guid AllowTemplatePostSigningLandingPageTip = new Guid("{BED40AD3-DD9B-4264-9A66-C5A01726D55E}");
            public static readonly Guid AllowUsersToAlignFormFieldTip = new Guid("{85EA0989-BC6D-45C3-883B-DDCCD63D57C5}");
            public static readonly Guid AllowUsersToCreateAndUseRulesTip = new Guid("{F67664F7-38A1-487C-9C06-F8ACFB2DF977}");
            public static readonly Guid AllowUsersToCreateAndUseTemplateGroupsTip = new Guid("{A022CFF9-1EFE-489B-914B-792D0B6096D2}");
            public static readonly Guid AllowUsersToCreateMessageTemplatesTip = new Guid("{F108BE6E-23B5-4D45-8906-BD5C3C2DCDDD}");
            public static readonly Guid AllowUsersToDeleteEmailBodyTip = new Guid("{1054FA5E-6EAD-47E4-BB2B-5B8E7F914C81}");
            public static readonly Guid AllowUsersToDeleteOriginalDocumentsTip = new Guid("{7542E821-5B80-4955-9AD8-A119D3DB6016}");
            public static readonly Guid AllowUsersToDeleteSignedContractsTip = new Guid("{C230EA3F-8AE1-4E43-9E8C-A96A1DD5046B}");
            public static readonly Guid AllowUsersToUseRulesTip = new Guid("{04204601-DD44-4562-AB7F-CEAF6F751903}");
            public static readonly Guid AttachSignedPDFForTheseRecipientsTip = new Guid("{BC74BE20-D79E-49E9-AF09-1D27EBCA78F0}");
            public static readonly Guid AttachXMLToSenderOnlyTip = new Guid("{E639E119-FE6D-4462-9E12-D49148B7D86C}");
            public static readonly Guid AutoPopulateSignatureWhileSigningTip = new Guid("{AC499F3F-28EA-4E7F-B49D-613207B0CD1B}");
            public static readonly Guid AvailableOptionsForSignatureCaptureTip = new Guid("{9D822970-B5AE-4CC8-9977-806BF8EEBF19}");
            public static readonly Guid BannerColorTip = new Guid("{24CB2438-319C-476C-B36A-36D37FBE8836}");
            public static readonly Guid BannerTextColorTip = new Guid("{DE10A163-F49B-4D5C-A983-5C002303EF27}");
            public static readonly Guid CreateStaticLinkforTemplateTip = new Guid("{94DE36B8-6B82-40EE-B3E1-384A00AA357B}");
            public static readonly Guid DataMaskingTip = new Guid("{F2635463-16DE-4C38-BD24-2A0ACC2360AA}");
            public static readonly Guid DateFormatTip = new Guid("{69875C08-E4B4-4A9B-B485-A80ECAACE18B}");
            public static readonly Guid DaysBeforeExpirationReminderTip = new Guid("{27FBE8C2-064B-4B06-AC29-1DBFED70FDE3}");
            public static readonly Guid DaysBeforeFirstReminderTip = new Guid("{B396B48B-A0BB-4442-AA33-E76752CA1323}");
            public static readonly Guid DaysBetweenRemindersTip = new Guid("{086AB8E7-14FD-488F-AB63-763D92184F7A}");
            public static readonly Guid DeclineEnvelopeTip = new Guid("{73D05C4F-1EC9-4280-A646-DA8C66757EA7}");
            public static readonly Guid DefaultFontSizeMeasurementTip = new Guid("{2DE80C4E-760B-41DF-9D79-9AAA3D8432CB}");
            public static readonly Guid DeleteDataTip = new Guid("{E76175E5-93E8-44BF-8E5A-C7CC2F2EAA59}");
            public static readonly Guid DisclaimerEmailFooterTip = new Guid("{AB518504-515A-442F-91A5-C43719E84215}");
            public static readonly Guid DocumentAvailabilityTip = new Guid("{07242036-E178-498C-BC8A-A39D3C85C30A}");
            public static readonly Guid ElectronicSignatureIndicationTip = new Guid("{0AB920A5-1B21-41B1-AB11-01895EF311F1}");
            public static readonly Guid EmailAccessCodeTip = new Guid("{1CA3E6C7-136E-45CD-AF07-79EC3D3B3E62}");
            public static readonly Guid EnableAutoFillTextControlsOfSameLabelTip = new Guid("{A305E065-F8C5-4EBF-A7FF-15EA43845A56}");
            public static readonly Guid EnableClickToSignTip = new Guid("{F2E70AFD-B4DC-4AE7-B8F9-6ED98C291FA3}");
            public static readonly Guid EnableIntegrationAccessReferenceKeyTip = new Guid("{15C94239-0A42-404E-B4DF-7A3D0CC8DE23}");
            public static readonly Guid EnableOutofOfficeModeTip = new Guid("{C1823B8F-3D5D-41B8-87C2-588530368EF0}");
            public static readonly Guid EnablePostSigningLoginPopupTip = new Guid("{40F9BD65-7670-420E-A464-08FFCA6E7210}");
            public static readonly Guid EnableTheDependenciesFeatureTip = new Guid("{039B28E3-D2F5-403D-AB46-685E65463410}");
            public static readonly Guid EnableTheFileReviewTip = new Guid("{FC4BF904-DA31-4EEA-B263-3AEFC10472D1}");
            public static readonly Guid EnableWebhookTip = new Guid("{6C9696AC-5BB5-41C0-BDC0-0DD3B06879AA}");
            public static readonly Guid EnvelopeExpirationRemindertoSenderTip = new Guid("{AED1BC9D-DD4F-4711-90B3-DF4DFAAE3192}");
            public static readonly Guid EnvelopeTimeDateStampLocationTip = new Guid("{3C931FD8-B605-46ED-A5BE-EFA17F740A56}");
            public static readonly Guid FinalContractConversionOptionTip = new Guid("{3D73DA10-38A7-4F14-AB57-23D6915FA1D8}");
            public static readonly Guid FirstDayTip = new Guid("{AC25246A-9213-4E7D-B2E5-ECB7CA2C391E}");
            public static readonly Guid IncludeEnvelopeDataXMLInTheEnvelopeTabTip = new Guid("{D399C5D8-ED8C-438D-BE2F-87D360DC23CF}");
            public static readonly Guid IncludeSignatureCertificateonPDFTip = new Guid("{1A3DD958-2DAE-45C9-A325-9FED99CDC964}");
            public static readonly Guid IncludeTransparencyDocumentTip = new Guid("{67BCCEB1-7BC0-4542-8D47-DF0B5D8554FE}");
            public static readonly Guid LastDayTip = new Guid("{4265731C-5C72-4B6F-B323-AE1F0468638D}");
            public static readonly Guid LoggedinUserTip = new Guid("{AEE49CED-B361-450B-94B2-1D896FE9D18A}");
            public static readonly Guid NetDocsLoggedinUserTip = new Guid("{985E933C-1B04-455B-A0AF-18D8CA9150AB}");
            public static readonly Guid MessageSignatureTextTip = new Guid("{B7200D82-1D62-4B3E-B4A5-23BC2550A92A}");
            public static readonly Guid PlanNameTip = new Guid("{0BD626D0-AAED-4C5E-816D-D0320E25E84D}");
            public static readonly Guid PostSendingNavigationPageTip = new Guid("{19EA02E8-9ED6-4DC9-8DE0-3D15CB06DD02}");
            public static readonly Guid PrivateModeTip = new Guid("{1046F972-1A33-415B-8746-2EF40D5F217B}");
            public static readonly Guid ReceiveSendingConfirmationEmailTip = new Guid("{84BBEEBE-1CE2-45D2-925F-20950274B4CB}");
            public static readonly Guid ReferenceCodeTip = new Guid("{52359B52-984E-42AB-88BA-98CD47229BB5}");
            public static readonly Guid SendAConfirmationEmailToValidateFirstSignerTip = new Guid("{F6EBD287-254B-42AB-BAAD-F1B5F5A2DB0C}");
            public static readonly Guid SendIndividualSignatureNotificationsTip = new Guid("{6FFB6D88-9782-4158-A945-FA3DF164BA47}");
            public static readonly Guid SendReminderUntilExpirationtoSenderTip = new Guid("{986CC792-4868-4A68-928D-D553DA4DF61A}");
            public static readonly Guid SeparateMultipleDocumentsAfterSigningTip = new Guid("{9E055C4F-033E-4AC1-B71B-50A6E15E5E43}");
            public static readonly Guid ServiceLanguageTip = new Guid("{C12D2E29-56B4-4BE5-93C2-88DE3B9BFBF5}");
            public static readonly Guid SetDefaultSignatureControlValuetoRequiredTip = new Guid("{BEE7C9E8-55FA-4DDD-9FDC-0F6E714A1C9E}");
            public static readonly Guid ShowCompanyLogoTip = new Guid("{74E81348-3F0D-409A-BFF9-2640BCB99492}");
            public static readonly Guid ShowControlIDTip = new Guid("{D6254986-674B-467B-BD7A-0812046D99AD}");
            public static readonly Guid ShowRSignLogoTip = new Guid("{7BE057A2-1AA6-4B37-9BCD-DC0022513A67}");
            public static readonly Guid ShowSettingsTabtoEndUsersTip = new Guid("{11DCF693-84DB-46D8-B26C-A8703D56D191}");
            public static readonly Guid SignatureCertificatePaperSizeTip = new Guid("{67FECCB8-4E22-4D8A-A248-B6B3FDD7F326}");
            public static readonly Guid SignatureControlRequiredForStaticTemplateTip = new Guid("{52268279-60C8-4366-886C-899AB9B890F7}");
            public static readonly Guid SignatureRequestReplyAddressTip = new Guid("{7E3F5140-08E5-4096-AE2A-ECCCCABD6943}");
            public static readonly Guid SignedDocumentCopyAddressTip = new Guid("{72AACB0A-F652-4F39-B4A2-106B21FC4A52}");
            public static readonly Guid SignedDocumentReRouteAddressTip = new Guid("{40ABCD00-81B1-45F4-87AD-77F979253312}");
            public static readonly Guid SignerAttachmentOptionsTip = new Guid("{202C3883-4018-4EB8-9461-81EDBAF4D8F8}");
            public static readonly Guid SignInSequenceTip = new Guid("{13A632C3-4010-422C-A490-AACE57ABE492}");
            public static readonly Guid StampWatermarkOntoSignersCompletedDocument = new Guid("{5F737299-6CD9-42D9-B7F9-F025D500EF84}");
            public static readonly Guid StorageDrivesAvailableTip = new Guid("{D8D732C6-129D-4984-976B-5975D32EA4D2}");
            public static readonly Guid StoreEmailBodyTip = new Guid("{D69D05FB-61D2-4BAC-AC8C-43C9FC8E238A}");
            public static readonly Guid StoreOriginalDocumentTip = new Guid("{C9033AAE-3AC6-48E5-AA24-E711399EAF2B}");
            public static readonly Guid StoreSignatureCertificateTip = new Guid("{A27880C0-E9DF-4ADE-8C9C-60E177F38AAD}");
            public static readonly Guid StoreSignedPDFTip = new Guid("{923D267A-17DF-4B08-B254-CE6B4CC736B8}");
            public static readonly Guid TermsOfServiceInSignatureCertificateTip = new Guid("{7FF8FB13-80A7-426A-BB56-B2C84F661F1A}");
            public static readonly Guid TermsOfServiceTip = new Guid("{1AD1B3AB-354F-4AAE-8A26-4CBE7D070D21}");
            public static readonly Guid TimeZoneTip = new Guid("{BDCE2B56-4CE3-4607-94AB-7C891C039D9C}");
            public static readonly Guid UnitSentTip = new Guid("{31ADE655-7050-41E6-8FDC-12875BCB19DB}");
            public static readonly Guid UploadSignedDocumentTip = new Guid("{B7B69363-7E52-4C81-AC38-EF257C78EEAC}");
            public static readonly Guid SelectDigitalCertificate = new Guid("{E566D1E3-D9B3-4529-A739-41BB1C3346A7}");
            public static readonly Guid EnvelopesTip = new Guid("{91ad82e0-619d-4ec0-a6ba-b16f323e08b7}");
            public static readonly Guid ActivitySummaryTip = new Guid("{5e40128d-bfe2-4f8d-b7da-d187079f52c8}");

            public static readonly Guid SendInvitationEmailToSignerTip = new Guid("3BBE8BE3-4B62-42E7-8299-0FDCA79B17F1");
            //S3-2280 Context Help Language Keys Transalations

            public static readonly Guid ChooseImage = new Guid("{7B5DD33D-6547-447D-8C9B-05A38C50E0CC}");
            public static readonly Guid UploadPictureSignature = new Guid("{773A5C51-C1D4-45F4-9F77-E8FB6F24162D}");
            public static readonly Guid DragandDropSignature = new Guid("{39C12BB5-2480-4D5D-9B03-A4B40E7F04CA}");
            public static readonly Guid UploadDocumentValidation = new Guid("{E8B6D88B-F56F-449E-9BF3-F889ECDB6835}");
            public static readonly Guid RedirectPostSigningLandingPage = new Guid("{A43B475A-1180-4117-965C-53E65B4C1B82}");
            public static readonly Guid ThankYouSessionTextOnConfirm = new Guid("{04CF54CD-15C0-4D6C-96B8-6392EABD8350}");
            public static readonly Guid MaxFileSize = new Guid("{77D40282-883B-476D-B7FA-82244FA391EB}");
            public static readonly Guid ConfirmEmailAddress = new Guid("{A22CA1F9-F7ED-4412-9F55-1019105CD264}");
            public static readonly Guid RSignConfirmationMail = new Guid("{5F3665F1-3E35-4D4E-9F4E-99023CBEBF34}");
            public static readonly Guid AdditionalSignerRequiredDocument = new Guid("{B9CEC1C9-9CC5-4CD4-A67C-23878A55FA37}");
            public static readonly Guid DuplicateEmailsNotAllowed = new Guid("{DB3F20AB-7654-4354-9D5A-2EC35449DDC1}");
            public static readonly Guid Translations = new Guid("{98D8401A-98D6-4231-8CF9-843870FEDFAE}");
            public static readonly Guid AppKeyTip = new Guid("{206A8DD0-A138-4453-B142-631D33F724B9}");
            public static readonly Guid AppKey = new Guid("{EF286951-843B-4749-9AC4-9768F74F6EF9}");
            public static readonly Guid AttachPDFForCCRecipientsTip = new Guid("{145CC8E3-4032-447B-A1B1-81D6E4D304F9}");
            public static readonly Guid EnableCcOptions = new Guid("{00D85A80-EA48-4A0E-BD36-5F3C699FBB8C}");
            public static readonly Guid Copiedfrom = new Guid("{1FC446B7-0C38-4A82-BC99-84E5D3117094}");
            public static readonly Guid RuleDetails = new Guid("{C03BBADB-16D0-4F6D-825A-5DD01358AD3A}");
            public static readonly Guid Lang_TemplateDetails = new Guid("{DECF0237-E4F7-4E5A-B8EA-95D7B6C3CC67}");

            public static readonly Guid RSignMakesESignatureText = new Guid("{EF529659-7036-4772-982C-0D7EF54A51F2}");

            public static readonly Guid DisclaimerLocation = new Guid("{9C56FD65-81F8-492C-B7FA-F15D6F788981}");
            public static readonly Guid DisclaimerLocationTip = new Guid("{9892C1BF-8233-443F-8505-D53F2529CEED}");
            public static readonly Guid LearnMore = new Guid("{b7c92495-5f50-4d67-ad3d-f3e83a68f71f}");

            public static readonly Guid DefaultLandingPage = new Guid("{ED788190-A3CF-4385-BAE4-038C91FD0F33}");
            public static readonly Guid DefaultLandingPageTip = new Guid("{0A038A9D-2607-41AD-B270-B5524DE0BCC0}");

            public static readonly Guid SigningProcessCompleteComment = new Guid("{14B0012C-6E59-47A1-A3A7-C948328C632B}");
            public static readonly Guid FilingFinalSignedDocuments = new Guid("{c2800372-86e8-43af-85dd-59a7b1126b7b}");
            public static readonly Guid FinalSignedDocumentNameFormat = new Guid("{952e067e-c763-4bff-a749-a0d9779c6e7d}");
            public static readonly Guid SignatureCertificateDocumentFormat = new Guid("{7490AD04-03D8-46A8-BEF7-93CB0402D909}");
            public static readonly Guid SignedDocumentLocation = new Guid("{8F5F9BDF-04D7-4152-860A-F6D65C6B6DA2}");
            public static readonly Guid CheckOutComment = new Guid("{C1BF45D6-4DA0-4F90-AA63-317B8BA15BAC}");
            public static readonly Guid CheckInComment = new Guid("{F52023FC-C3D5-48F8-B20D-EF681F921EA0}");
            public static readonly Guid FilingFinalSignedDocumentOptionsTip = new Guid("{93DF9DEE-3736-46AA-B497-AA3E4FF6A7CB}");
            public static readonly Guid FilingFinalSignedDocumentOptions = new Guid("{51FF8695-5716-4992-8322-BC7EAAB83D4D}");
            public static readonly Guid CheckInCommentTip = new Guid("{2A44E9C8-0B06-4055-B773-62BF0879AEBE}");
            public static readonly Guid CheckOutCommentTip = new Guid("{3A7564E2-9B90-4DD0-B967-370B9BFAECA0}");
            public static readonly Guid SignedDocumentLocationTip = new Guid("{AD8BDB06-7D28-4250-9AF0-0A6B8549506E}");
            public static readonly Guid SignatureCertificateDocumentFormatTip = new Guid("{9F47A993-D028-4BA8-99A6-DC7A12AC3178}");
            public static readonly Guid FinalSignedDocumentNameFormatTip = new Guid("{0223E425-0FB8-430E-8D11-605CCFB411C8}");
            public static readonly Guid FilingFinalSignedDocumentsTip = new Guid("{4B7E5148-828F-4D61-B33F-D33B2020A618}");
            public static readonly Guid SigningProcessCompleteCommentTip = new Guid("{AB176DE6-7501-4F4F-8DD5-5742BD68DA04}");
            public static readonly Guid DefaultSignedDocumentLocation = new Guid("{0F8AE1A8-B2F5-46D7-871E-0F76E8D63E5F}");
            public static readonly Guid DefaultSignedDocumentLocationTip = new Guid("{2FBEBCCF-BF8E-413B-BB30-32F55C2A5350}");


            public static readonly Guid EnableRecipientLanguage = new Guid("{68fd5876-629c-49ed-ac72-0468dfeb0dbf}");
            public static readonly Guid EnableRecipientLanguageTip = new Guid("{64ae8d33-c9a0-44e8-94dc-47046e236698}");

            //SSO
            public static readonly Guid SignInWithLabel = new Guid("{D2CC05B0-B4E5-4F57-A17A-4B6853B8FD1C}");
            public static readonly Guid SingleSignOnLabel = new Guid("{F793CB37-2985-484F-ADDF-D5AFE40F602E}");

            public static readonly Guid Language = new Guid("{F00A27CA-2E8A-4CAB-AA92-9CA579847348}");
            public static readonly Guid RSignSingleSignerConfirmationMail = new Guid("{CE42E1E3-745B-4765-82C9-54E4DB94350A}");

            public static readonly Guid ResellerName = new Guid("{470FE3BF-5361-4767-A328-FB9F30E05C51}");
            public static readonly Guid ApiID = new Guid("{30943731-7825-4D7C-B9FD-0AC04C7964CC}");
            public static readonly Guid ApiIDTip = new Guid("{8CCDCABC-0C62-4F29-B421-C46222C6E871}");
            public static readonly Guid ApiKey = new Guid("{FDA13640-4F8C-47FB-A201-9BC6325DF25B}");
            public static readonly Guid IntegrationEmail = new Guid("{C5A722DE-AF63-4425-867E-7679D37F419B}");
            public static readonly Guid IntegrationEmailTip = new Guid("{8DD14289-8267-4133-94EC-66D0EA22A203}");
            public static readonly Guid ResellerNameTip = new Guid("{338AB3A4-FCDB-4D15-A3AB-A350AB150494}");
            public static readonly Guid CreateAutoReminders = new Guid("{40CA02CE-FCE2-4E94-AA5A-79A2C71D8D98}");
            public static readonly Guid GetStartedHere = new Guid("{699d9c5c-715b-4f36-a582-612dcb65a8bd}");
            public static readonly Guid DisplayAppliedEpicContacts = new Guid("{502D3E84-7273-4E8B-869C-8192E6B3D88D}");
            public static readonly Guid DisplayBullhornContacts = new Guid("{72A2AD10-F2D9-402D-B76D-78A5278E1979}");
            public static readonly Guid LearnMoreTextOnly = new Guid("{30DAEF26-A701-4CB4-8C9D-9A63E5D760DB}");

            public static readonly Guid CheckYourEmail = new Guid("{A983F157-5788-4F47-9723-96DB485186FF}");
            public static readonly Guid RememberToLookInYourText = new Guid("{98189B0E-13C4-43AD-A7A8-F87D5B86C641}");
            public static readonly Guid Spam = new Guid("{15D6EF00-2823-4F56-997D-864CB2ECEA2C}");
            public static readonly Guid Folder = new Guid("{825AC9B5-D5FB-417F-86D2-B88D73907063}");
            public static readonly Guid ExportAsCSV = new Guid("{4A592E63-18C3-4BB6-AAFD-3C11F78C79B7}");
            public static readonly Guid ApplicationURL = new Guid("{4B934FBE-5AB1-467C-A0F9-58F1C453F4F3}");
            public static readonly Guid APIServerURL = new Guid("{6E4105BB-7646-49A2-9AAD-E289B69CBC13}");
            public static readonly Guid ClientId = new Guid("{4C62DF09-5EEB-4752-A6AB-6FEE985A1065}");
            public static readonly Guid UnsignedTooltip = new Guid("{62C261F7-DDDF-476C-A094-F68A162BE74C}");
            public static readonly Guid SignedTooltip = new Guid("{1BC914FC-4536-4D31-A292-3B2EAE835CC2}");
            public static readonly Guid UnsignedAndSignedTooltip = new Guid("{288B4A95-10CA-463C-A8D3-0A02E5E2E7D4}");
            public static readonly Guid NotificationTooltip = new Guid("{FF838BD8-D0C6-4347-9050-0CE7E4154B2D}");
            public static readonly Guid NotificationAndSignedTooltip = new Guid("{112BB3AC-DD18-49A7-A772-E7F9589E9E14}");
            public static readonly Guid Region = new Guid("{E4A389C4-3CD0-44BF-856A-B02672941BCF}");
            public static readonly Guid RegionTooltip = new Guid("{783832BD-C4FF-42A7-B414-528A29E8B2DF}");
            public static readonly Guid CustomEntity = new Guid("{D7A727F6-25AE-4FF7-880C-9D27BC0E2BD8}");
            public static readonly Guid EntityFields = new Guid("{954D609B-E686-4168-AD10-7AE8DA06C110}");
            public static readonly Guid Awaiting = new Guid("{73132718-1076-41D2-9202-FEE24C1B24E8}");
            public static readonly Guid Details = new Guid("{1B1F8664-9269-46F2-87E9-11E52CFB8CC0}");
            public static readonly Guid AwaitingRecipients = new Guid("{D9A0CCA5-038C-4A97-B23A-5805B93C2BC6}");
            public static readonly Guid Auto = new Guid("{A79AAC55-CD5C-4B18-8CDD-5A86F5AA0409}");
            public static readonly Guid Manual = new Guid("{83A42615-9388-4849-A5C0-06474E5E5AC8}");
            public static readonly Guid RPortalStatus = new Guid("{CB04EB61-B1FA-48CB-A9AF-F05E5AD3EB77}");

            public static readonly Guid lang_SSORegMessage = new Guid("{E47F90AD-032E-47D6-BAC0-6EDBB2BDB8A8}");
            public static readonly Guid lang_SSOLoginMessage = new Guid("{6AD865EB-DB35-469D-AD2E-C731E67F17FB}");
            public static readonly Guid lang_SSOForgotPwdMessage = new Guid("{47405858-D240-4264-B4A0-757C5EB376C6}");
            public static readonly Guid lang_applicationError = new Guid("{1694D5E7-6AE6-4758-B9D8-9934D0692361}");

            public static readonly Guid ClickOnCreateEnvelope = new Guid("{7716D373-5C3C-4722-B14A-FA9F4C824192}");
            public static readonly Guid ClickOnCreateTemplate = new Guid("{4896A9E7-244D-44B8-B816-38C35A68DD3B}");
            public static readonly Guid BullhornNOTE = new Guid("{18A22A55-42CD-4A7F-93CF-E39C0070803D}");
            public static readonly Guid VincereNOTE = new Guid("{97BCDE00-F341-4E4C-B3CF-1B91EA093F4B}");
            public static readonly Guid SelectTemplateToEdit = new Guid("{5C5DED51-4697-4958-8C45-3CEE2439D225}");
            public static readonly Guid SelectRuleToEdit = new Guid("{A039742D-D825-412D-A081-77C94F8C5404}");
            public static readonly Guid WelcometoRSign = new Guid("{7FA3E417-0D94-41BD-A6C0-48FB891F65AE}");

            public static readonly Guid RemindMeLater = new Guid("{F926897E-A8CD-460C-A1F0-617FF37239A1}");
            public static readonly Guid ArchivingSoon = new Guid("{7A4E788A-D0F3-475F-B8CE-1625F2185419}");

            public static readonly Guid PasswordRequirements = new Guid("{44CBC6F2-C35C-4A26-A5AE-D7644288558F}");
            public static readonly Guid UpperCaseLetter = new Guid("{EECD1225-ED24-45E8-993D-9C687CB8DC21}");
            public static readonly Guid LowerCaseLetter = new Guid("{551B6FAE-22D4-4CBC-BF4F-8A4A03BCCB83}");
            public static readonly Guid OneNumber = new Guid("{CAFDB262-E11B-4CA7-A718-53DB7009E23E}");
            public static readonly Guid SpecialCharacter = new Guid("{CBDF0959-1F3A-477D-865D-C567BA19BD40}");
            public static readonly Guid CharactersInLength = new Guid("{07560BB4-8EA7-4679-85BD-3CCFD0F6837D}");

            public static readonly Guid FilesforReview = new Guid("{5965EE72-4A75-4668-8F89-F9BA93F9A458}");
            public static readonly Guid DownloadAndReviewFiles = new Guid("{08F02C00-CC7F-451A-920C-157EB7A36158}");
            public static readonly Guid ConfirmFilesReviewed = new Guid("{CF1B673C-20A4-442A-91F8-0050C598348F}");
            public static readonly Guid DownloadFiles = new Guid("{2E2D10EB-9E9A-4E9A-9466-9BDB682D6EF6}");
            public static readonly Guid Files = new Guid("{56e27613-ff86-4267-9803-ad0ca55691d8}");
            public static readonly Guid FilesReview = new Guid("{ae2454eb-95a0-4aa1-b6c9-644e4402b973}");
            public static readonly Guid FilesReviewed = new Guid("{D74E8CD2-B5B3-4CF0-8EC4-19AD5FFB144F}");

            public static readonly Guid DontShowMeAgain = new Guid("{4793D115-1A63-4484-AAD0-95B6B2A9EC1D}");
            public static readonly Guid Exit = new Guid("{04c7fefd-eadd-4e1f-b9b5-d6ab57bf8827}");

            public static readonly Guid PersonalEnableiManageAccessTip = new Guid("{34495A43-CD31-456E-BD13-AAA170DB05F3}");
            public static readonly Guid EnableSSOforiManageTip = new Guid("{73E6B25B-41FD-4896-AC1B-FC17A0FF5826}");
            public static readonly Guid EnableSSO = new Guid("{C00DF3A2-B131-4CF4-A413-4CF3A96E767D}");
            public static readonly Guid SettingApplicationURLTip = new Guid("{BF73691A-C8E4-45F8-A459-74D59CCD7516}");
            public static readonly Guid OtherOptions = new Guid("{A0141799-69F7-4FFD-A37C-70831640649B}");

            public static readonly Guid EmailListReasonsforDeclining = new Guid("{20DAED3D-39D0-48DF-8050-E126BA0B17D3}");
            public static readonly Guid sendTo = new Guid("{03EBC3D3-CF56-4FEB-8973-CAE9BE4D3FFD}");
            public static readonly Guid SendingFrequency = new Guid("{55F90A82-195D-4CC0-AE59-BCD9623C0A22}");
            public static readonly Guid EmailListReasonsforDecliningToolTip = new Guid("{F1766CB3-525B-4E86-AB15-54CBB3800C5D}");

            public static readonly Guid IsActivityEnabled = new Guid("{335206BA-AE45-4265-9F71-FDF2596364EF}");
            public static readonly Guid ActivityCode = new Guid("{17327E90-A7DC-4537-B32B-C6AD99192F19}");
            public static readonly Guid ActivityCodeToolTip = new Guid("{98B58B6B-7B15-4418-951E-63061903D032}");
            public static readonly Guid EnableActivityToolTip = new Guid("{B8707434-57AE-4A31-B874-C92792598874}");

            public static readonly Guid WhatsnewandcoolinRSign = new Guid("{6e16f07c-1761-4269-ac84-225527627cd2}");
            public static readonly Guid DisableDownloadOptionOnSignersPage = new Guid("{2F3F7AD1-C571-4F0D-8887-0F2421D397C8}");
            public static readonly Guid DisableDownloadOptionOnSignersPageTip = new Guid("{A45646EE-20BD-4FF7-A14E-D4F06BBEE196}");

            public static readonly Guid ConfirmWaitingFOrSignatureEnvelopeDelete = new Guid("{12b18d24-4726-40fb-aa06-2b7f2d52b637}");
            public static readonly Guid BulkSendTemplate = new Guid("{28DC8B18-B94E-436C-A1EF-C50B2A941A3C}");

            public static readonly Guid EnableSendingMessagesToMobile = new Guid("{C9E845A4-ABF5-4D37-B82B-68C87015308D}");
            public static readonly Guid EnableSendingMessagesToMobileToolTip = new Guid("{270AF09C-1215-47C5-A749-B27D681D8639}");
            public static readonly Guid DefaultCountryCode = new Guid("{EB3E4BB9-EE79-4348-B11B-1E31CDCF5165}");
            public static readonly Guid DefaultCountryCodeToolTip = new Guid("{618BCCF7-AD3E-41C0-B688-ED721B22A12B}");
            public static readonly Guid DefaultDeliveryMode = new Guid("{00824A11-B93B-4F1C-B0E5-3C9FBCD0FB09}");
            public static readonly Guid DefaultDeliveryModeToolTip = new Guid("{58914DD3-CCF1-4AF2-BC33-169C03206A79}");
            public static readonly Guid CountryCode = new Guid("{FA034064-961C-406B-9ACE-07974408484A}");
            public static readonly Guid Mobile = new Guid("{C340A9BC-622A-4C39-85D4-BA311056D212}");
            public static readonly Guid ReadOnly = new Guid("{BCB4603C-EA9A-4DE8-99D9-EAF26EE03BFC}");
            public static readonly Guid EnterPhoneNumber = new Guid("{7901F979-E191-49D8-B525-F62DF1FC5D8C}");
            public static readonly Guid MobileNumberValidation = new Guid("{08156D3F-8197-4DB3-B82E-11008376009D}");

            public static readonly Guid SendFinalSignDocumentChangeSigner = new Guid("{69A5E504-6AB5-4871-887B-E1A5F3530AD5}");

            public static readonly Guid EmailMobileRequiredConfirmation = new Guid("{7E851868-367A-40DC-8EBA-5CF158BBFD48}");
            public static readonly Guid MobileRequiredConfirmation = new Guid("{F292001B-AA91-49E1-82DC-2BFCD0B9280D}");
            public static readonly Guid lang_VerifyYourMobileFirstSigner = new Guid("{C1F054B7-7FE1-48B0-946D-FA2F84DBE1CB}");
            public static readonly Guid lang_VerifyYourEmailMobileFirstSigner = new Guid("{A62137E3-73C2-4C64-A0AA-D339B2C3E892}");
            public static readonly Guid ThankYouSessionTextOnMobileConfirm = new Guid("{C7DFF58E-530E-4FA5-AB39-DE2C6D0F04EA}");
            public static readonly Guid ThankYouSessionTextOnEmailMobileConfirm = new Guid("{03BB8C92-EEC5-4884-917E-5545E0D8FBD1}");
            public static readonly Guid MobilePaswordText = new Guid("{82EF8EDF-64D9-4203-8130-0C5DBDD673BF}");
            public static readonly Guid MobileEmailPasswordText = new Guid("{2D569ED9-38E0-4E7F-85A2-9CCA8C84B8ED}");
            public static readonly Guid InviteSignerHeaderText = new Guid("{EA216AB3-4549-4C24-8C01-7885B0E8E026}");
            public static readonly Guid SendConfirmationMobileText = new Guid("{C13FB96E-3E4A-4006-A857-A3FEC0E5DA95}");
            public static readonly Guid SendConfirmationEmailMobileText = new Guid("{527EDA87-5534-4F68-8C0E-29459509F57A}");
            public static readonly Guid ConfirmEmailOnly = new Guid("{07A75514-EC57-42B0-B38C-38AEE36D2C82}");
            public static readonly Guid ConfirmMobileText = new Guid("{3E116D0C-DEDC-4F2D-A881-6E7BFF4B1F49}");
            public static readonly Guid MultiSignerConfirmEmailMobileText = new Guid("{CA3C3B74-5E6D-428A-AB31-9EFD46D83D2A}");
            public static readonly Guid ConfirmEmailMobileText = new Guid("{A8BB729A-4A47-48BB-BDF1-9D3601C9A459}");
            public static readonly Guid ConfirmSingleSignerEmailOnly = new Guid("{A42BD520-A8B4-4020-A0FB-35464AD4826A}");

            public static readonly Guid SignerTermsCondEmailOrMobile = new Guid("{49BB6191-3F3E-46E0-8437-C33772C46934}");
            public static readonly Guid SignerTermsCondMobile = new Guid("{A8822828-7E19-4297-A959-8D49CD584760}");
            public static readonly Guid EmailPaswordText = new Guid("{67FE0B83-C563-4240-9E8E-6D3A39385CEF}");
            public static readonly Guid SendSingleSignerConfirmationEmailMobileText = new Guid("{D65EB200-D15B-443A-A0E1-FDE471D31907}");
            public static readonly Guid SendSingleSingnerConfirmationMobileText = new Guid("{5315364D-5D9E-45D2-8488-A8C66B79E39A}");
            public static readonly Guid SendConfirmMobileHeaderText = new Guid("{53E588CD-6293-4825-8B41-B5DDDA4822B4}");
            public static readonly Guid SendConfirmEmailMobileHeaderText = new Guid("{2ECBE31C-72CE-4DAC-97FD-5B8A5AB420AC}");


            public static readonly Guid Transaction = new Guid("{B704594E-7AF5-4BA6-BA18-F92E73675524}");
            public static readonly Guid Descriptions = new Guid("{8D828BAD-4EE3-4059-ABBE-6CC680A67D70}");

            public static readonly Guid EmailNotFoundinEnvelopesWithin60Days = new Guid("{B0A506AF-F98C-49D6-A8FC-08BFA594B945}");
            public static readonly Guid EmailNotFoundinEnvelopes = new Guid("{18312638-4F1A-48D9-9B32-FC51C9829C7D}");
            public static readonly Guid SearchUserTip = new Guid("{A4F06696-9150-4F93-BF4F-464FD08DAB68}");
            public static readonly Guid Company_User_Search = new Guid("{0CD94071-976A-4DA0-95BA-49B7BE46E36F}");
            public static readonly Guid EmailAddressNotExist = new Guid("{0CD94071-976A-4DA0-95BA-49B7BE46E36F}");
            public static readonly Guid RestrictRecipientsToContactListonly = new Guid("{2217EB22-37A9-46C4-AD76-7D75493966D1}");
            public static readonly Guid RestrictRecipientsToContactListonlyToolTip = new Guid("{0FF8AD2A-C414-4A9D-BDC6-13D42A0BA723}");

            public static readonly Guid UserName = new Guid("{B61A1DD0-96F2-4301-A81D-708B187DE010}");
            public static readonly Guid UserNameToolTip = new Guid("{E9271F1C-CC93-4278-B3B6-DEBAC430707F}");
            public static readonly Guid ExternalSettingsPassword = new Guid("{3223A2B8-B98F-4F6F-A0EF-234AEAE6D4A7}");
            public static readonly Guid PasswordToolTip = new Guid("{DA1C0070-C10A-41F7-8FEA-CEF52453B5A8}");
            public static readonly Guid VincereBaseURL = new Guid("{B7A1194D-6D07-4DC2-893D-D41A9E981751}");
            public static readonly Guid VincereBaseURLToolTip = new Guid("{275FA3A1-55AB-4B74-8E95-13CE30B08C24}");
            public static readonly Guid ApplicationClientID = new Guid("{5754294F-2851-4A3B-A90F-35B8CCFB54AB}");
            public static readonly Guid ClientIDToolTip = new Guid("{C55FC3E2-E284-4146-BD2E-B874C01DBC22}");
            public static readonly Guid ExternalSettingsApiKey = new Guid("{A48C87CB-4806-49C7-AE34-6AE2F598A7D1}");
            public static readonly Guid ApiKeyToolTip = new Guid("{2593BE31-DB08-44C6-A7F2-CC927BF4047B}");
            public static readonly Guid lang_MobileNumber = new Guid("{4670FE7E-0DF6-4234-964F-2CD6B7985373}");
            public static readonly Guid VincereFinalSignedDocumentTooltip = new Guid("{1FA6128B-AA5D-4DED-ACD4-13178C8E5412}");
            public static readonly Guid VincereEnableActivityTooltip = new Guid("{41DD01F5-A8C1-47C2-B01F-4339DDECD1B8}");

            public static readonly Guid ConfirmToAccess = new Guid("{91879639-3784-4CC1-AC67-FFF435B0823D}");

            public static readonly Guid RequiresSignersConfirmationonFinalSubmit = new Guid("{34775C2F-431A-4655-A7B4-BA217FDD8A72}");
            public static readonly Guid RequiresSignersConfirmationonFinalSubmitToolTip = new Guid("{635E2E77-C49A-4F7F-9E69-05AB0FDB2D8F}");
            public static readonly Guid IncludeStaticTemplates = new Guid("{77299E86-3ACE-4386-94E5-866986307E40}");
            public static readonly Guid IncludeStaticTemplatesToolTip = new Guid("{E2E4EA3C-AB63-4AC5-A4E1-A02E419EBE0D}");
            public static readonly Guid iManageSameFolderLocation = new Guid("{E622F6E9-A7AB-4741-81E6-99644CD68CD3}");
            public static readonly Guid iManageDifferentFolderLocation = new Guid("{B6ECFFF9-1214-45D1-AC99-6B4E240D6821}");

            public static readonly Guid NetdocumentsSameFolderLocation = new Guid("{31B51EC3-6028-4DA2-B3B7-84EF3CB18179}");
            public static readonly Guid NetdocumentsDifferentFolderLocation = new Guid("{01F49749-708A-40A5-AD31-7009EA8F515B}");

            /*---NetDocuments Remaining Keys---*/
            public static readonly Guid ApplicationURLToolTip = new Guid("{66700478-3851-42E3-98BB-811C271FFD72}");
            public static readonly Guid APIServerURLToolTip = new Guid("{B7419C17-59B4-40CE-B8A4-074D30597012}");
            public static readonly Guid ClientIdToolTipNetDocuments = new Guid("{EF96DB0F-E5D9-43DB-AE84-FFA75E1CABA6}");
            public static readonly Guid ClientSecretNetDocuments = new Guid("{3331D5DD-E50E-454E-BE8E-0A8A877930E4}");
            public static readonly Guid ClientSecretToolTipNetDocuments = new Guid("{5BD2DA46-3B6F-47E8-A477-C6DF972B3C6C}");
            public static readonly Guid RedirectURI = new Guid("{399E5B37-4B3D-4EA3-9174-35E8DE01F95E}");
            public static readonly Guid RedirectURIToolTip = new Guid("{79F874CC-C8A4-45CD-8627-A4CE12BF33AF}");
            public static readonly Guid DefaultRepository = new Guid("{961C9750-4D31-422C-99C8-59C874FB951E}");
            public static readonly Guid DefaultRepositoryToolTip = new Guid("{CCDF0D57-D38F-4B2A-85B2-8006876BFECD}");

            /*---Bullhorn Remaining Keys---*/
            public static readonly Guid ClientIdToolTipBullhorn = new Guid("{6A568247-9816-4D32-9D48-C903365233C3}");
            public static readonly Guid ClientSecretBullhorn = new Guid("{27FB7CED-B229-4F49-86C2-39F50CBE1016}");
            public static readonly Guid ClientSecretToolTipBullhorn = new Guid("{1A1F136D-2557-4B42-A596-A2A8BA81E02D}");
            public static readonly Guid BullhornUserName = new Guid("{0B495BAC-F3DB-4C98-9107-E04C9403A0EF}");
            public static readonly Guid BullhornUserNameToolTip = new Guid("{0CB83FE0-13D3-4838-95A2-6604BA81E385}");
            public static readonly Guid BullhornPassword = new Guid("{F32F80D9-C8FE-4907-8FC6-856B41059174}");
            public static readonly Guid BullhornPasswordToolTip = new Guid("{328B1961-ECBB-4AC9-849B-44BCD2C0AA2E}");
            public static readonly Guid UploadSignedDocumentToolTipBullhorn = new Guid("{D1202E65-2741-4571-AA76-55F26E6C3BE7}");
            public static readonly Guid SignatureCertificateDocumentFormatToolTipBullhorn = new Guid("{90DFFC51-53F3-4135-A426-641D10F7BCF0}");

            /*---Applied Epic Remaining Keys---*/
            public static readonly Guid AppliedEpicAPIServerURLToolTip = new Guid("{C04FD53A-3F61-4B13-ADB6-D838BAA4383B}");
            public static readonly Guid DatabaseName = new Guid("{97BF4F13-B20C-4084-B3F3-3F45F5D1E5C9}");
            public static readonly Guid DatabaseNameToolTip = new Guid("{6620F440-1C15-4595-A1A0-5426AA7D268B}");
            public static readonly Guid AuthenticationKey = new Guid("{85CD8D03-F493-4D1C-BE70-2066E6AD5643}");
            public static readonly Guid AuthenticationKeyToolTip = new Guid("{18455F21-D554-4101-BE4D-33408C6799A7}");
            public static readonly Guid IntegrationKey = new Guid("{936ABF1E-CABC-473C-B518-CEE9C84680D4}");
            public static readonly Guid IntegrationKeyToolTip = new Guid("{5ABD5325-304D-490F-A877-209A477BE3ED}");
            public static readonly Guid UploadSignedDocumentToolTipAppliedEpic = new Guid("{10203B8E-A992-404B-A801-8FB0FF013F7C}");
            public static readonly Guid SignedDocumentLocationToolTip = new Guid("{9D26A374-659D-4BCA-AE3E-CBE522D3D893}");
            public static readonly Guid AppliedEpicSameLocation = new Guid("{6549183C-75F4-4D9D-9996-0B801E9F6AE4}");
            public static readonly Guid AppliedEpicDifferentLocation = new Guid("{628B51B0-035B-42BE-A6AA-81C538BECF99}");
            public static readonly Guid SignatureCertificateDocumentFormatToolTipAppliedEpic = new Guid("{FAB6FEBE-D631-461D-8236-DCD3D2B19C47}");

            /*---Vincere Remaining Keys---*/
            public static readonly Guid SignatureCertificateDocumentFormatToolTipVincere = new Guid("{77C49FB6-CBE8-4B7C-9443-8AECEA2B9354}");
            public static readonly Guid Users = new Guid("{91A62DF0-EC1B-4446-91CE-4EEC33366AF4}");
            public static readonly Guid Security = new Guid("{9E0F0314-54A8-43FF-9BC4-54196064A65D}");
            public static readonly Guid Requests = new Guid("{1C5FCA60-7099-4EBE-8F61-FB8262EF1EFD}");
            public static readonly Guid SMSProvider = new Guid("{63e3b49f-3111-4f58-a07c-7bec4f441809}");
            public static readonly Guid SMSProviderTootip = new Guid("{1fc42b6e-4539-4a9c-9131-91548b2f797e}");
            public static readonly Guid SignerSessionAboutExpire = new Guid("{645A3F42-1078-4AA8-9C01-0D8188B27FBB}");

            public static readonly Guid EmailOrMobile = new Guid("{F9A49289-1EF8-438E-9AF2-BC61B08C2F81}");
            public static readonly Guid EmailOrMobileTooltip = new Guid("{9E150D17-7290-4DDE-8D73-5EEF08379CF0}");
            public static readonly Guid ReminderTypeTooltip = new Guid("{04FAE8AB-9F2D-493F-AB95-E285AD628026}");
            public static readonly Guid DeliveryMode = new Guid("{A4270A37-EE64-477E-AB2A-E4AAE53D0C29}");
            public static readonly Guid SMSAccessCode = new Guid("{525EA046-801D-4195-93B8-8A48C299FED4}");

            public static readonly Guid DownloadFinalSignedDocText = new Guid("{FB1A24BD-3B96-4EB7-946C-39BB7EC789FE}");
            public static readonly Guid DocumentAlreadySignedText = new Guid("{2391C14E-A914-4761-BA1E-16F27602B156}");
            public static readonly Guid DownloadDocAfterSignedText = new Guid("{BB0C81DD-8766-4AEB-92C6-93BC7F6131DB}");
            public static readonly Guid Lang_DownloadFinalSignedDoc = new Guid("{C55681A4-0E36-4F64-94E7-997385A4A68A}");
            public static readonly Guid AllowSignerstoDownloadFinalContract = new Guid("{4139ECF4-DAB0-46A5-AE86-C6CB8EE3556B}");
            public static readonly Guid AllowSignerstoDownloadFinalContractToolTip = new Guid("{AA6DDFC1-7BFC-453C-B499-D55D0DBAF0F6}");
            public static readonly Guid DocumentGenerationMessage = new Guid("{DA9B9765-5116-4291-AAF7-B8AD40B0FE97}");
            public static readonly Guid SignupDownloadFinalSignedDocText = new Guid("{6B4597D8-EAD8-4725-A689-0FED9D40E74D}");

            public static readonly Guid ActivityAssignation = new Guid("{05C07E4C-7265-4E95-AB07-03F48DC6BBB4}");
            public static readonly Guid SameUserAsTheOriginalCreatorOfTheEnvelope = new Guid("{DAB12BFB-6D5A-48D0-AED6-1521E2ED8802}");
            public static readonly Guid DifferentUser = new Guid("{59812941-81C0-488A-861C-1EAB6A5BB11C}");
            public static readonly Guid CloseActivity = new Guid("{1BD2BC63-A357-47CF-AD81-D82ED4177FA2}");
            public static readonly Guid EnterTheExactUserCode = new Guid("{28C8783A-09C4-4DC1-A0B6-BEE67D32D072}");
            public static readonly Guid EnterTheExactFolderName = new Guid("{90161E80-8BA1-42AA-9C13-DD31B59354E6}");
            public static readonly Guid ActivityAssignationToolTip = new Guid("{1191CA87-3BE6-439A-AC86-2CA6F07CD5F8}");
            public static readonly Guid CloseActivityToolTip = new Guid("{ABF046E2-06CF-4E42-BF65-24A488446413}");

            public static readonly Guid OfficialVersion = new Guid("{360D9224-A9F0-46F2-91DD-E93BC0F7C324}");
            public static readonly Guid OfficialVersionToolTip = new Guid("{D2D38BD1-E1C3-48B8-8ECE-0C917B011CB1}");

            public static readonly Guid ManageFinalDocument = new Guid("{DDB75538-A9B4-40F9-8187-C0D96438435E}");
            public static readonly Guid RegenerateandSendFinalDocument = new Guid("{1E280F12-87BE-4691-B47F-7BA5BE461DD6}");
            public static readonly Guid SelectOptionandClickSubmit = new Guid("{AE074E48-B69B-4E92-A6BE-0864E66BC3C2}");
            public static readonly Guid RegenerateFinalSignedDocument = new Guid("{39EEF668-309E-42F0-94C8-6772ECF64599}");
            public static readonly Guid SendtheFinalSignedDocument = new Guid("{78956660-33BE-4CB4-AC63-798E0B015F19}");
            public static readonly Guid RegenerateandSendtheFinalSignedDocument = new Guid("{EF3EE8C7-AFF8-40D9-B1D3-236FF519FE4A}");
            public static readonly Guid FixedWidth = new Guid("{0F2F1B37-CB4B-4D46-9448-0965ADE8A944}");

            public static readonly Guid DocumentExpiryDate = new Guid("{E8820D1C-758C-4F63-B0B5-AEF806843DBD}");
            public static readonly Guid DocumentExpiryDateToolTip = new Guid("{E8454089-D286-422B-9066-95E75B10497E}");

            public static readonly Guid LastUpdated = new Guid("{AC67C213-D705-475B-93B7-96960D1CF900}");
            //public static readonly Guid title_sortEnvelopeLastUpdated = new Guid("{8F738B6A-D04A-449F-A9CE-3075ECB81FCA}");

            public static readonly Guid FinalSignedDocumentNamingForMultipleDocuments = new Guid("{3C81BA1E-1BF3-4D5D-9AB3-7D70926F9FAC}");
            public static readonly Guid RenameFileTo = new Guid("{ED0BDFE8-D1BD-468F-A346-A090C72095CB}");
            public static readonly Guid AddPrefixToFileName = new Guid("{AF677C19-831F-416B-A632-830A14340FD1}");
            public static readonly Guid AddSuffixToFileName = new Guid("{179D3E7B-BCD7-452B-A6D9-7CF07CD49470}");
            public static readonly Guid DateTimeStampForMultipleDocSettingsForPrefix = new Guid("{1f4233db-a409-4059-9aae-ff4c7ecdd853}");
            public static readonly Guid DateTimeStampForMultipleDocSettingsForSufix = new Guid("{81ed36c3-647c-41ce-98c9-bcf771279945}");
            public static readonly Guid FinalSignedDocumentNamingForMultipleDocumentToolTip = new Guid("{81B44929-B7D5-46D5-89D9-262A30CEC5DD}");
            public static readonly Guid AddPrefixtoFileNameTooltip = new Guid("{B05D9BE9-F3B0-48E2-B67C-CE0C2EB5777E}");
            public static readonly Guid AddSuffixtoFileNameTooltip = new Guid("{DFA3C5FF-27F5-41F2-A118-4BCC746B5F81}");

            public static readonly Guid Lang_DeclineAndDownloadFinalSignedDoc = new Guid("{807A5EDD-DC74-4730-B983-06E549CC068D}");
            public static readonly Guid SearchFolder = new Guid("{0879562F-C034-489E-9AED-BEE8B96C0E7E}");
            public static readonly Guid FolderSelectionHeaderTitle = new Guid("{C23DE2AC-CFCF-4F8C-BB27-DB30288BCBD2}");
            public static readonly Guid SearchCabinets = new Guid("{E7EA90D9-36F1-438F-83CA-B490CF733BF7}");
            public static readonly Guid SearchClients = new Guid("{487216B1-420E-47DC-BCB2-58E122385ABF}");
            public static readonly Guid SearchMatters = new Guid("{4B7072B6-5E67-4719-B649-859B8716D15B}");
            public static readonly Guid SearchFolders = new Guid("{2E9825FF-EF67-4434-8E48-6E9FE3B1C051}");
            public static readonly Guid SearchCabinetsToolTip = new Guid("{D1FE87BD-D7E2-4797-A9CA-10C93BDF57BC}");
            public static readonly Guid SearchClientsToolTip = new Guid("{ED16D09A-D1F3-498B-91D2-453C28DE1C26}");
            public static readonly Guid SearchMattersToolTip = new Guid("{B911B922-CF49-4192-86D9-C86CAFD2218A}");
            public static readonly Guid SearchFoldersToolTip = new Guid("{C98253B0-1622-4A58-AC36-E51DE88F1E1C}");
            public static readonly Guid NoCabinetFound = new Guid("{C9840872-0CD7-4F18-95E9-7AC8C57A56C1}");
            public static readonly Guid NoClientFound = new Guid("{2EFF7E2B-7DA7-44F5-B403-7C38F0C54F39}");
            public static readonly Guid NoMatterFound = new Guid("{72BF5E2C-C7EA-4F94-9D79-7B67D0FB7565}");
            public static readonly Guid NoFolderFound = new Guid("{0AB02912-CA20-4080-A89C-87CE3213C6FA}");

            public static readonly Guid DisableFinishLaterOption = new Guid("{5a4ad709-4b08-46aa-9856-91c9317ef59b}");
            public static readonly Guid DisableFinishLaterOptionToolTip = new Guid("{3CE5E19D-E6CB-4596-AFCD-B7C8A3BD95B9}");
            public static readonly Guid DisableDeclineOption = new Guid("{CAD2CC7F-F62C-443E-A402-F064E2453D47}");
            public static readonly Guid DisableDeclineOptionToolTip = new Guid("{F700A67E-D619-435A-B307-8C09D90F96F3}");
            public static readonly Guid DisableChangeSigner = new Guid("{E32C44B7-19E8-4B87-BA9D-B857CE2343AA}");
            public static readonly Guid DisableChangeSignerToolTip = new Guid("{9F009B86-9FAC-45D3-9329-AD795181B873}");
            public static readonly Guid AddAppName = new Guid("{5E43D64D-DE1D-4C76-902B-7667B6E597E9}");
            public static readonly Guid AppName = new Guid("{10B664DA-28FC-40EE-BA3A-BF062946B182}");

            public static readonly Guid EmailVerificationText = new Guid("{f8b977d4-467a-4223-b55a-12dd42a44072}");
            public static readonly Guid MessageToCodeTo = new Guid("{E69BFEA4-A78F-4785-A4C2-B8D6A0D6AB6B}");
        }

        public static class Languages
        {
            public static readonly Guid English = new Guid("{A35674A3-E201-4AAD-8407-2D3B51E1A891}");
            public static readonly Guid Spanish = new Guid("{7C334AD3-AD33-4E05-8E1E-B094C4BDE847}");
            public static readonly Guid portiguees = new Guid("{9FE18234-5156-498E-B2A9-C38BD3531B1F}");
            public static readonly Guid German = new Guid("{098E767C-917D-45CA-9FB2-D7EE84C9893B}");
            public static readonly Guid French = new Guid("{071D4E14-7190-47F1-8C41-FBA1166FABE1}");
            public static readonly Guid Danish = new Guid("{C28176A6-4A18-4FAA-A2CD-E069B22237AE}");
        }
        public static class PageName
        {
            public static readonly string Layout = "Layout";
            public static readonly string Home = "Home";
            public static readonly string Send = "Send";
            public static readonly string Template = "Template";
            public static readonly string Draft = "Draft";
            public static readonly string SignDocument = "SignDocument";
            public static readonly string LogOn = "LogOn";
            public static readonly string Rules = "Rule";
            public static readonly string MessageTemplate = "MessageTemplate";
            public static readonly string DocumentGroup = "DocumentGroup";
        }
        public static class FinalContractOptions
        {
            public static readonly int Aspose = 1;
            public static readonly int FromImages = 2;
            public static readonly int iText = 3;
        }
        public static class ImageOptions
        {
            public static readonly int Aspose = 1;
            public static readonly int GhostScript = 2;
        }
        public static class AttachSignedPDF
        {
            public static readonly Guid SenderOnly = new Guid("{CDFC7178-03C4-4864-B3FB-097AA2335557}");
            public static readonly Guid SenderSigner = new Guid("{AAFB0BBB-21DC-4AAD-816F-6814B191CEDC}");
            public static readonly Guid RetrievalLink = new Guid("{037CC450-5065-4DC1-BF01-DA12637C4FB8}");
            public static readonly Guid SenderOnlyandNoemailtoRecipient = new Guid("{6F0CC529-F418-4A33-852C-3E470A150DD9}");
            public static readonly Guid SenderOnlyandRecipientSignIn = new Guid("{796E44F3-1D11-4A85-B460-3889061DC2C5}");
            public static readonly Guid SenderRetrievallinkandNoemailtoRecipient = new Guid("{EA92FA10-71FC-445E-B5D9-50E317A9F7A1}");
            public static readonly Guid SenderRetrievallinkandRecipientSignIn = new Guid("{461EEB77-4DFF-44B9-B2A0-00A5DD015B5D}");
        }

        public static class DocumentPaperSize
        {
            public static readonly int AutomaticallyDetect = 1;
            public static readonly int USLetter = 2;
            public static readonly int USLegal = 3;
            public static readonly int A4 = 4;
        }
        public static class HeaderFooterSettings
        {
            public static readonly int None = 1;
            public static readonly int LeftFooter = 2;
            public static readonly int RightFooter = 3;
            public static readonly int LeftHeader = 4;
            public static readonly int RightHeader = 5;

        }
        public static class ShowEnvelopeData
        {
            public static readonly int All = 1;
            public static readonly int NinetyDay = 2;
            public static readonly int AfterNinetyDay = 3;
        }
        public static class ShowEnvelopeDataThirtyDays
        {
            public static readonly int All = 1;
            public static readonly int ThirtyDay = 2;
            public static readonly int AfterThirtyDay = 3;
        }
        public static class Language
        {
            public static readonly string English = "en-us";
            public static readonly string Spanish = "es-es";
            public static readonly string Portuguese = "pt-pt";
            public static readonly string German = "de-de";
            public static readonly string French = "fr-fr";
            public static readonly string Dutch = "nl-nl";
            public static readonly string Danish = "da-dk";
            public static readonly string Italian = "it-it";
            public static readonly string Latvian = "lv-lv";
            public static readonly string Lithuanian = "lt-lt";
            public static readonly string Polish = "pl-pl";
            public static readonly string Hungarian = "hu-hu";
            //public static readonly string Colombia = "co-es";
            public static readonly string Norwegian = "no-na";
            public const string NorwegianConst = "no-na";
            //public const string SpanishColombiaConst = "co-es";
            public const string SpanishConst = "es-es";
            public static readonly string Romanian = "ro-ro";
        }
        public static class LanguageName
        {
            public static readonly string English = "English";
            public static readonly string Spanish = "Español";
            public static readonly string Portuguese = "Português";
            public static readonly string German = "Deutsch";
            public static readonly string French = "Français";
            public static readonly string Dutch = "Nederlands";
            public static readonly string Danish = "Dansk";
            public static readonly string Italian = "Italian";
            public static readonly string Latvian = "Latvian";
            public static readonly string Lithuanian = "Lithuanian";
            public static readonly string Polish = "Polish";
            public static readonly string Hungarian = "Hungarian";
            //public static readonly string Colombia = "Spanish (Colombia)";
            public static readonly string Norwegian = "Norwegian";
            public static readonly string Romanian = "Romanian";
        }

        public static class SettingsDisplayOptions
        {
            public static readonly int Active = 1;
            public static readonly int Disable = 2;
            public static readonly int ViewOnly = 3;
        }

        public static class EnvelopeDirectoryInfo
        {
            public static readonly string Converted = "Converted";
            public static readonly string DraftedDocuments = "DraftedDocuments";
            public static readonly string Final = "Final";
            public static readonly string Images = "Images";
            public static readonly string OriginalConverted = "OriginalConverted";
            public static readonly string Thumbs = "Thumbs";
            public static readonly string UploadedDocuments = "UploadedDocuments";
        }
        public static class Platform
        {
            public static readonly string Cloud = "Cloud";
        }
        public static class UploadDriveType
        {
            public static readonly string Google = "Google";
            public static readonly string Dropbox = "Dropbox";
            public static readonly string Skydrive = "Skydrive";
            public static readonly string iManage = "iManage";
            public static readonly string Appliedepic = "Applied Epic";
            public static readonly string Bullhorn = "Bullhorn";
            public static readonly string netDocuments = "netdocuments";
            public static readonly string Vincere = "Vincere";
        }
        public static class DatetoSignedDocNameSettingsOptions
        {
            public static readonly int None = 1;
            public static readonly int FileNameYYYY_MM_ddHH_MM_SS_2 = 2;
            public static readonly int FileNameMMDDYYYYHHMMSS_3 = 3;
            public static readonly int FileNameDDMMYYYYHHMMSS_4 = 4;
            public static readonly int YYYYMMDDHHMMSSFileName_5 = 5;
            public static readonly int MMDDYYYYHHMMSSFileName_6 = 6;
            public static readonly int DDMMYYYYHHMMSSFileName_7 = 7;
            public static readonly int FileName_YYYY_MM_DD_HH_MM_SS_8 = 8;
            public static readonly int FileName_MM_DD_YYYY_HH_MM_SS_9 = 9;
            public static readonly int FileName_DD_MM_YYYY_HH_MM_SS_10 = 10;
            public static readonly int YYYY_MM_DD_HH_MM_SS_FileName_11 = 11;
            public static readonly int MM_DD_YYYY_HH_MM_SS_FileName_12 = 12;
            public static readonly int DD_MM_YYYY_HH_MM_SS_FileName_13 = 13;
            public static readonly int FileName_DD_MMM_YYYY_HH_MM_SS_14 = 14;
            public static readonly int DD_MMM_YYYY_HH_MM_SS_FileName_15 = 15;

        }
        public static class ReferenceCodeOptions
        {
            public static readonly int None = 1;
            public static readonly int ReferenceCodeOnly = 2;
            public static readonly int ReferenceEmailOnly = 3;
            public static readonly int ReferenceCodeandEmail = 4;
        }
        public static class ElectronicSignIndicationOption
        {
            public static readonly int Disable = 1;
            public static readonly int EnablewithEnvelopeID = 2;
            public static readonly int EnablewithDateTimeStamp = 3;
        }

        public static class InvitationEmailToSignerOptions
        {
            public static readonly int InboxOnly = 1;
            public static readonly int InvitationEmailOnly = 2;
            //public static readonly int Both = 3;
        }

        public static class DeclineTemplateControlsSetting
        {
            public static readonly int RadioButtons = 1;
            public static readonly int CheckBoxes = 2;
            public static readonly int TextFields = 3;
        }

        public static class SignatureRequestReplyToAddressOptions
        {
            public static readonly int rpostEmail = 1;
            public static readonly int senderEmail = 2;
        }

        public static class SettingsTabInfo
        {
            public const int Admin = 1;
            public const int Profile = 2;
            public const int Sending = 3;
            public const int Template = 4;
            public const int Storage = 5;
            public const int System = 6;
            public const int Advanced = 7;
            public const int Disclosure = 8;
            public const int OutOfOffice = 9;
            public const int Contacts = 10;
            public const int ServicePlan = 11;
            public const int Privacy = 12;
            public const int ExternalIntegration = 13;
            public const int Webhook = 14;
            public const int ExternalIntegrationDrives = 15;
            public const int Contact = 16;
            public const int Users = 17;
        }

        public static class EmailBannerColors
        {
            public static readonly string BackgroundColor = "#AC2329";
            public static readonly string FontColor = "#FFFFFF";
        }
        public static class DropdownFieldKeyType
        {
            public static readonly string ElectronicSignIndication = "ElectronicSignIndication";
            public static readonly string ReferenceCode = "ReferenceCode";
            public static readonly string PostSendingNavigationPage = "PostSendingNavigationPage";
            public static readonly string DocumentPaperSize = "DocumentPaperSize";
            public static readonly string InvitationEmailToSigner = "InvitationEmailToSigner";
            public static readonly string SignatureReqReplyToAddress = "SignatureReqReplyToAddress";
            public static readonly string ServiceLanguage = "ServiceLanguage";
            public static readonly string DropdownSortItems = "DropdownSortItems";
            public static readonly string SignerAttachmentOptions = "SignerAttachmentOptions";
            public static readonly string TypographySize = "TypographySize";
            public static readonly string SendReminderTillExpiration = "SendReminderTillExpiration";
            public const string AToZ = "A–Z";
            public const string ZToA = "Z–A";
            public const string NewestToOldest = "Newest-Oldest";
            public const string OldestToNewest = "Oldest-Newest";
            public const string MostFrequent = "MostFrequent";
            public const string LeastFrequent = "LeastFrequent";

            public static readonly string HeaderFooter = "HeaderFooter";
            public static readonly string DatetoSignedDocNameSettings = "DatetoSignedDocNameSettings";
            public static readonly string UserStatus = "UserStatus";
            public static readonly string Controls = "Controls";
            public static readonly string BannerBackgroundColor = "BannerBackgroundColor";
            public static readonly string BannerTextColor = "BannerTextColor";

            public static readonly string OneEmailperEnvelope = "Individual";
            public static readonly string OneEmailforallExpiringEnvelopes = "Combined";
            public static readonly string DigitalCertificate = "DigitalCertificate";
            public static readonly string AppKey = "AppKey";

            public static readonly string RAppNotificationExpiryDays = "RAppNotificationExpiryDays";
            public static readonly string RAppNotificationTypes = "RAppNotificationTypes";

            public static readonly string DisclaimerLocation = "DisclaimerLocation";
            public static readonly string DefaultLandingPage = "DefaultLandingPage";

            public static readonly string FilingFinalSignedDocuments = "FilingFinalSignedDocuments";
            public static readonly string FilingFinalSignedDocumentOptions = "FilingFinalSignedDocumentOptions";
            public static readonly string FinalSignedDocumentNameFormat = "FinalSignedDocumentNameFormat";
            public static readonly string SignatureCertificateDocumentFormat = "SignatureCertificateDocumentFormat";
            public static readonly string RegionNetDocuments = "RegionNetDocuments";
            public static readonly string DeclineEmailSendingFrequency = "DeclineEmailSendingFrequency";
            public static readonly string DialingCountryCodes = "DialingCountryCodes";
            public static readonly string DeliveryModeOptions = "DeliveryModeOptions";
            public static readonly string SMSProvider = "SMSProvider";
            public static readonly string RenameFileTo = "RenameFileToModeOptions";
            public static readonly string AddPrefixToFileName = "AddPrefixToFileNameModeOptions";
            public static readonly string AddSuffixToFileName = "AddSuffixToFileNameModeOptions";
            public static readonly string DateTimeStampForMultipleDocsForPrefix = "DateTimeStampForMultipleDocSettingsForPrefix";
        }
        public static class BooleanValues
        {
            public const bool TrueValue = true;
            public const bool FalseValue = false;
        }
        public enum DeliveryStatus
        {
            /// <summary>
            /// Destination address unreachable
            /// </summary>
            Unreachable = -2,
            /// <summary>
            /// Mailbox full
            /// </summary>
            MailboxFull = -1,
            /// <summary>
            /// Untried
            /// </summary>
            Untried = 0,
            /// <summary>
            /// Delivery failed
            /// </summary>
            Failed = 1,
            /// <summary>
            /// Delivery relayed
            /// </summary>
            Relayed = 2,
            /// <summary>
            /// Delivered to mail server - minimum for success
            /// </summary>
            DeliveredToMailServer = 3,
            /// <summary>
            /// Sent to MTA - delivered and may still get a DSN
            /// </summary>
            SentToMta = 4,
            /// <summary>
            /// Delivered to mailbox - got a DSN
            /// </summary>
            DeliveredToMailbox = 5,
            /// <summary>
            /// Message opened - web bug callback
            /// </summary>
            Opened = 6,
            /// <summary>
            /// Has been notified
            /// </summary>
            Notified = 7,
            /// <summary>
            /// Has been delivered but notification of delivery status still pending
            /// </summary>
            DeliveredNotificationPending = 8
        }

        /// <summary/>
        public enum DeliverySubStatus
        {
            /// <summary>default substatus: 0</summary>
            Unknown = 0,
            /// <summary>bad address substatus: 1</summary>
            BadAddress = 1,
            /// <summary>bad domain substatus: 2</summary>
            BadDomain = 2,
            /// <summary>security failure substatus: 3</summary>
            SecurityFailure = 3,
            /// <summary>region failure substatus: 4</summary>
            RegionFailure = 4
        }
        public static class EmailDeliveryStatusDecription
        {
            public static readonly string DeliveryFailed = "Delivery Failure";
            public static readonly string OutOfOffice = "Out-Of-Office";
            public static readonly string DeliveredtoMailServer = "DeliveredtoMailServer";
        }

        public static class SettingsFieldName
        {
            public const string PrivateMode = "PrivateMode";
            public const string DataMasking = "DataMasking";
            public const string DeleteData = "DeleteData";
            public const string TemplateTitle = "TemplateTitle";
            public const string TemplateControls = "TemplateControls";
            public const string TemplateResponse = "TemplateResponse";

        }
        public static class StoreageDriveNames
        {
            public static readonly string LocalMachine = "LocalMachine";
            public static readonly string Template = "Template";
            public static readonly string GoogleDrive = "GoogleDrive";
            public static readonly string Dropbox = "Dropbox";
            public static readonly string OneDrive = "OneDrive";
            public static readonly string iManage = "iManage";
            public static readonly string netDocuments = "netdocuments";
            public static readonly string System = "System";
            public static readonly string Appliedepic = "Applied Epic";
            public static readonly string Bullhorn = "Bullhorn";
            public static readonly string Vincere = "Vincere";

        }
        public static class DialogConstants
        {
            public static string DialogTokenEndpoint = "{{serverUrl}}/api/v2/customers/{{customerId}}/dialog-tokens";
            public static string DialogFilePicker = "{0}/work/partner-apps/{1}/dialogs/file-picker/?protocol=postmessage&mode=open&dialogToken={2}&types=document&customerId={3}&userId={4}";
            public static string DialogFolderPicker = "{0}/work/partner-apps/{1}/dialogs/file-picker/?protocol=postmessage&mode=browse&dialogToken={2}&types=folder&customerId={3}&userId={4}";
            public static string DialogFileSave = "{0}/work/partner-apps/{1}/dialogs/file-picker/?protocol=postmessage&mode=save&dialogToken={2}&types=folder&customerId={3}&userId={4}";
            public static string DialogWebUi = "{0}/work/partner-apps/{1}/dialogs/file-picker/?protocol=postmessage&mode=emm&dialogToken={2}&customerId={3}&userId={4}";
            public static string DiscoveryEndpoint = "{{serverUrl}}/api";
            public static string LogoutEndpoint = "{{serverUrl}}/auth/oauth2/revoke-token";
            public static string DownloadiManageDocument = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}/download";
            public static string DocumentFullPath = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}/path";
            public static string netDocsDownloadUrl = "{0}/v1/Document/{1}/{2}?$select=123";
            public static string iManageCheckOutDocument = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}/checkout";
            public static string iManageUpdateDocProfile = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}";
            public static string iManageRemoveLock = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}/lock";
            public static string netDocsDocumentURL = "{0}/v1/Document";
            public static string iManageUploadDocumentVersion = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}/versions";
            public static string iManageUpdateDocumentVersion = "{0}/work/api/v2/customers/{1}/libraries/{3}/documents/{2}/file";
            public static string VincereGetCandidateDetails = "{0}/api/v2/{1}/{2}";
            public static string VincereGetCandidateFilesDetails = "{0}/api/v2/{1}/{2}/files";
            public static string VincereRefreshToken = "{0}/oauth2/token?";
            public static string VincereCreateTask = "{0}/api/v2/activity/task";
            public static string VincereUpdateTask = "{0}/api/v2/activity/task/{1}";
            public static string VincereCompleteTask = "{0}/api/v2/activity/task/{1}/status";  //https://api.vincere.io
            public static string VincerePostDocument = "{0}/api/v2/{1}/{2}/file";
            public static string VincereGetTask = "{0}/api/v2/activity/task/{1}";
        }
        public static class GetDocTypeAndClass
        {
            public static string Pdf = "application/pdf";
            public static string XPdf = "application/x-pdf";
            public static string WordProcessing = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            public static string MsWord = "application/msword";
            public static string Presentation = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            public static string PlainText = "text/plain";
            public static string Msg = ".MSG";
            public static string Doc = "DOC";
            public static string Acrobat = "ACROBAT";
            public static string Wordx = "WORDX";
            public static string Word = "WORD";
            public static string Pptx = "PPTX";
            public static string Ansi = "ANSI";
            public static string Email = "E-MAIL";
            public static string Mime = "MIME";
        }
        public static class ActionTypes
        {
            public static readonly string Sign = "Sign";
            public static readonly string Review = "Review";
        }

        public static class DocumentTypes
        {
            public static readonly string Contracts = "Contracts";
            public static readonly string Original = "Original";
            public static readonly string Review = "Review";
            public static readonly string EmailBody = "EmailBody";
            public static readonly string Certificate = "Certificate";
        }
        public static class EmailSubject
        {
            public static string PasswordToSign = "ElectronicPasswordNotification";
            public static string PasswordToOpen = "ElectronicPasswordNotification";
            public static string SignatureRequest = "ElectronicSignatureRequest";
            public static string SendingConfirmation = "SendingConfirmation";
            public static string PasswordNotification = "ElectronicPasswordNotification";
            public static string SignatureCarbonCopy = "ElectronicSignatureCarbonCopy";
            public static string SignatureReminder = "ElectronicSignatureReminder";
            public static string DocumentSigned = "DocumentSigned";
            public static string FinalDocument = "FinalDocument";
            public static string SendingPasswordConfirmation = "SendingPasswordConfirmation";
            public static string ElectronicSignatureVerificationCode = "ElectronicSignatureVerificationCode";
            public static string FinalDocumentCC = "FinalDocumentCC";
            public static string ExpirationReminder = "ExpirationReminder";
            public static string EmailVerification = "EmailVerification";
        }

        public static class TemplateShareType
        {
            public static readonly int None = 0;
            public static readonly int Copy = 1;
            public static readonly int Share = 2;
            public static readonly int Both = 3;
        }

        public static class TemplateSelectedType
        {
            public static readonly string Personal = "Personal";
            public static readonly string Master = "Master";
        }

        public static class SignerAttachmentOptions
        {
            public static readonly int None = 0;
            public static readonly int AllowSignersToUploadAttachments = 1;
            public static readonly int EnableAttachmentRequest = 2;
        }
        public static class BulkUploadTags
        {
            public static readonly string FirstName = "FirstName";
            public static readonly string LastName = "LastName";
        }
        public static class TimeZone
        {
            public static readonly string WEST = "WEST";
            public static readonly string WEDT = "WEDT";
            public static readonly string WET = "WET";
        }

        public static class EmailQueueStatus
        {
            public static int NewRecord = 0;
            public static int PickedByService = 1;
            public static int SuccessFullyProcessed = 2;
            public static int PartiallyProcessed = 3;
            public static int ErrorWhileProcessing = 4;
        }
        public static class EmailQueueRecipientStatus
        {
            public static int NewRecord = 0;
            public static int SuccessFullyProcessed = 1;
            public static int ErrorWhileProcessing = 2;
        }

        public static class SignatureRequestReplyAddress
        {
            public static int noreply = 1;
            public static int SendersEmail = 2;
            public static int SendersEmailRsignRpostNet = 3;
        }

        public static class EmailTypes
        {
            public const string ESR = "ESR";
            public const string SC = "SC";
            public const string ESPN = "ESPN";
            public const string SCPN = "SCPN";
            public const string ESCC = "ESCC";
            public const string DS = "DS";
            public const string FD = "FD";
            public const string ESN = "ESN";
            public const string DESR = "DESR";
            public const string RESR = "RESR";
            public const string AESR = "AESR";
            public const string SR = "SR";
            public const string ESPO = "ESPO";
            public const string DGESR = "DGESR";
            public const string DSR = "DSR";
            public const string ESNCC = "ESNCC";
        }

        public static class WebhookEventNames
        {
            public const string EnvelopeStatus = "EnvelopeStatus";
            public const string EnvelopeCompleted = "EnvelopeCompleted";
            public const string DocumentSigned = "DocumentSigned";
            public const string SignerStatus = "SignerStatus";
            public const string NotificationType = "EnvelopeDelegated";
        }
        public static class PrivacyDefaultValues
        {
            public const int PrivateModeOptionFlag = 2;
            public const bool PrivateModeOptionValue = false;
            public const bool PrivateModeIsOverride = false;
            public const bool PrivateModeIsLock = false;
            public const bool DataMaskingOptionValue = false;
            public const bool DataMaskingIsOverride = false;
            public const string DeleteDataRetentionDays = "30";
            public const bool DeleteDataOptionValue = false;
            public const bool DeleteDataIsOverride = false;

        }

        public static class CreatedSource
        {
            public const string WebApp = "WebApp";
            public const string API = "API";
            public const string RApp = "RApp";
            public const string NoSource = "API";
        }

        public static class SettingsSource
        {
            public const string Service = "Service";
            public const string Settings = "Settings";
            public const string LogIn = "LogIn";
        }

        public static class DraftTypes
        {
            public static readonly string Sign = "Sign";
            public static readonly string Prefill = "Prefill";
        }

        public static class DigitalCertificate
        {
            public static readonly int Default = 0;
            public static readonly int RpostLabsInternal = 1;
        }

        public static class AppKey
        {
            public static readonly string None = "None";
            public static readonly string SSM = "SSM";
        }
        public static class UsersRoles
        {
            public static readonly string LanguageTranslator = "LanguageTranslator";
            public static readonly string LanguageAdmin = "LanguageAdmin";
        }
        public static class DisclaimerLocation
        {
            public static readonly int TopOfTheEmailBody = 1;
            public static readonly int BelowEmailBody = 2;
            public static readonly int BottomOfEmail = 3;
        }

        public static class CCRecipientType
        {
            public static readonly int Empty = 0;
            public static readonly int DefaultCcType = 1;
            public static readonly int UnsignedandSigned = 1;
            public static readonly int Unsigned = 2;
            public static readonly int Signed = 3;
            public static readonly int NotificationsOnly = 4;
            public static readonly int NotificationsAndSigned = 5;
        }

        public static class DefaultLandingPage
        {
            public static readonly int Home = 0;
            public static readonly int Send = 1;
            public static readonly int Envelopes = 2;
            public static readonly int Templates = 3;
        }

        public static class TagErrorMessage
        {
            public static string Errormessage = "One of the text control size out of bounds. Please adjust the tag position.";
        }

        public static class SignInSequenceDesc
        {
            public static string Auto = "Auto";
            public static string Manual = "Manual";
        }
        public static class IntegrationActivity
        {
            public static string SuccessMessage = "Envelope processed succesfully.";
            public static string UnSuccessMessage = "Envelope not processed succesfully.";
            public static string EmployeeName = "ADM";
            public static string AccountTypeCode = "CUST";
            public static string Account = "Account";
            public static string ESignatureDescription = "RSign eSignature document";
            public static string PublicAccessLevel = "Public";
            public static string DefaultActivityCode = "SRVC";
            public static string ClosedStatus = "Successful";
            public static string Closed = "Closed";
            public static string TaskCompleted = "Completed";
            public static string Candidate = "CANDIDATE";
        }
        public static class EntityType
        {
            public static string Client = "Client";
            public static string ClientCorporation = "ClientCorporation";
            public static string JobOrder = "Job Order";
            public static string JobPosting = "Job Posting";
            public static string Jobs = "JobOrder";
            public static string ClientContact = "Client Contact";
            public static string Contact = "ClientContact";
            public static string Candidate = "Candidate";
            public static string Placement = "Placement";
            public static string VincereCandidate = "candidate";
            public static string VincereContact = "contact";
            public static string VincereJob = "job";
            public static string VincereCompany = "company";
        }
        public static class EntityFields
        {
            public static string FirstName = "First Name";
            public static string LastName = "Last Name";
            public static string MiddleName = "Middle Name";
            public static string Name = "Name";
            public static string NamePrefix = "Name Prefix";
            public static string NameSuffix = "Name Suffix";
            public static string Address1 = "Address1";
            public static string Address2 = "Address2";
            public static string City = "City";
            public static string State = "State";
            public static string DateAvailable = "Date Available";
            public static string DateAvailableEnd = "Date Available End";
            public static string DateI9Expiration = "Date I9 Expiration";
            public static string DateOfBirth = "Date Of Birth";
            public static string DayRate = "Day Rate";
            public static string DayRateLow = "Day Rate Low";
            public static string Email = "Email";
            public static string Mobile = "Mobile";
            public static string ReferredBy = "Referred By";
            public static string SSN = "SSN";
            public static string DateBegin = "Date Begin";
            public static string DateEnd = "Date End";
            public static string DaysGuaranteed = "Days Guaranteed";
            public static string DurationWeeks = "Duration Weeks";
            public static string EmployeeType = "Employee Type";
            public static string EmploymentStartDate = "Employment Start Date";
            public static string EmploymentType = "Employment Type";
            public static string HoursOfOperation = "Hours Of Operation";
            public static string HoursPerDay = "Hours Per Day";
            public static string PayRate = "Pay Rate";
            public static string ReportedMargin = "Reported Margin";
            public static string ReportTo = "Report To";
            public static string Salary = "Salary";
            public static string SalaryUnit = "Salary Unit";
            public static string Shift = "Shift";
            public static string NickName = "Nick Name";
            public static string Division = "Division";
            public static string Occupation = "Occupation";
            public static string Office = "Office";
            public static string Phone = "Phone";
            public static string StartDate = "Start Date";
            public static string Title = "Title";
            public static string TravelRequirements = "Travel Requirements";
            public static string Type = "Type";
            public static string YearsRequired = "Years Required";
            public static string NoOfEmployees = "No Of Employees";
            public static string NoOfOffices = "No Of Offices";

            public static string JobHoursOfOperation = "Job Hours Of Operation";
            public static string JobDurationWeeks = "Job Duration Weeks";
            public static string JobStartDate = "Job Start Date";
            public static string JobTitle = "Job Title";
            public static string JobTravelRequirements = "Job Travel Requirements";
            public static string JobType = "Job Type";
            public static string JobReportTo = "Job Report To";
            public static string JobSalary = "Job Salary";
            public static string JobSalaryUnit = "Job Salary Unit";
            public static string JobEmploymentType = "Job Employment Type";
            public static string JobAddress1 = "Job Address1";
            public static string JobAddress2 = "Job Address2";
            public static string JobCity = "Job City";
            public static string JobState = "Job State";
            public static string JobYearsRequired = "Job Years Required";

            public static string CandidateFirstName = "Candidate First Name";
            public static string CandidateLastName = "Candidate Last Name";
            public static string CandidateMiddleName = "Candidate Middle Name";
            public static string CandidateName = "Candidate Name";
            public static string CandidateNamePrefix = "Candidate Name Prefix";
            public static string CandidateNameSuffix = "Candidate Name Suffix";
            public static string CandidateAddress1 = "Candidate Address1";
            public static string CandidateAddress2 = "Candidate Address2";
            public static string CandidateCity = "Candidate City";
            public static string CandidateState = "Candidate State";
            public static string CandidateDateAvailable = "Candidate Date Available";
            public static string CandidateDateAvailableEnd = "Candidate Date Available End";
            public static string CandidateDateI9Expiration = "Candidate Date I9 Expiration";
            public static string CandidateDateOfBirth = "Candidate Date Of Birth";
            public static string CandidateDayRate = "Candidate Day Rate";
            public static string CandidateDayRateLow = "Candidate Day Rate Low";
            public static string CandidateEmail = "Candidate Email";
            public static string CandidateMobile = "Candidate Mobile";
            public static string CandidateReferredBy = "Candidate Referred By";
            public static string CandidateSSN = "Candidate SSN";

            public static string TownCity = "Town City";
            public static string Country = "Country";
            public static string ZipPostalCode = "Zip Postal Code";
            public static string LocationName = "LocationName";
            public static string EmployementType = "Employement Type";
            public static string Skills = "Skills";
            public static string RegistrationDate = "Registration Date";
            public static string PlaceOfBirth = "Place Of Birth";
            public static string Nationality = "Nationality";
            public static string AvailabilityStart = "Availability Start";
            public static string WorkEmail = "Work Email";
            public static string HomePhone = "Home Phone";
            public static string WorkPhone = "Work Phone";
            public static string CurrencyType = "Currency Type";
            public static string SalarymonthsPerYear = "Salary months Per Year";
            public static string NoticeDays = "Notice Days";
            public static string PreferredLanguage = "Preferred Language";
            public static string Gender = "Gender";
            public static string MaritalStatus = "Marital Status";
            public static string CurrentSalary = "Current Salary";
            public static string SalaryType = "Salary Type";
            public static string PresentSalaryRate = "Present Salary Rate";
            public static string CurrentBonus = "Current Bonus";
            public static string ContractRate = "Contract Rate";
            public static string ContractInterval = "Contract Interval";
            public static string PassportNo = "Passport No";
            public static string Driving_LicenseType = "Driving_License Type";
            public static string DrivingLicenseNumber = "Driving_License Number";
            public static string PaymentType = "Payment Type ";
            public static string CurrentLocationName = "Current Location Name";
            public static string CurrentAddress = "Current Address";
            public static string CurrentAddressLine1 = "Current Address Line1";
            public static string CurrentAddressLine2 = "Current Address Line2";
            public static string CurrentCountry = "Current Country";
            public static string CurrentState = "Current State";
            public static string CurrentTownCity = "Current Town City";
            public static string CurrentDistrictSuburb = "Current District Suburb";
            public static string CurrentZipCode = "Current Zip Code ";
        }

        public static class ExternalSettings
        {
            public static string SignatureCertificateDocumentFormat = "SignatureCertificateDocumentFormat";
            public static string FilingFinalSignedDocumentOptions = "FilingFinalSignedDocumentOptions";
            public static string FinalSignedDocumentNameFormat = "FinalSignedDocumentNameFormat";
            public static string FilingFinalSignedDocuments = "FilingFinalSignedDocuments";
            public static string SigningProcessCompleteComment = "SigningProcessCompleteComment";
            public static string CheckInComment = "CheckInComment";
            public static string CheckOutComment = "CheckOutComment";
            public static string UploadSignedDocument = "UploadSignedDocument";
            public static string EnableSSO = "EnableSSO";
            public static string OfficialVersion = "OfficialVersion";
        }

        public static class StorageLevels
        {
            public static string Level1 = "L1";
        }
        public static class RCAPStatusCodes
        {
            public static string RCAP_1016 = "rcap-1016";
            public static string RCAP_1018 = "rcap-1018";
            public static string RCAP_1019 = "rcap-1019";
            public static string RCAP_1020 = "rcap-1020";
            public static string RCAP_1021 = "rcap-1021";
        }
        public static class MaximumFileSize
        {
            public static readonly double MaxFileSize = 20971520;
            public static readonly int MaxFileSizeInMB = 20;
            public static readonly int FilesLength = 10;
        }

        public static class DeclineEmailSendingFrequency
        {
            public static readonly int Monthly = 1;
            public static readonly int Weekly = 2;
        }

        public static class EnvelopeContractStatus
        {
            public static int NewRecord = 1;
            public static int InProgress = 2;
            public static int SuccessFullyGenerated = 3;
        }

        public static class DeliveryModes
        {
            public static int EmailSlashEmail = 1;
            public static int EmailSlashMobile = 2;
            public static int EmailSlashEmailAndMobile = 3;
            public static int MobileSlashMobile = 4;
            public static int MobileSlashEmail = 5;
            public static int MobileSlashNone = 6;
            public static int MobileSlashEmailAndMobile = 7;
            public static int EmailAndMobileSlashMobile = 8;
            public static int EmailAndMobileSlashEmail = 9;
            public static int EmailAndMobileSlashEmailAndMobile = 10;
            public static int EmailSlashNone = 11;
            public static int EmailAndMobileSlashNone = 12;
        }
        public static class SMSProviders
        {
            public static int PrimaryProvider = 1;//Twilio
            public static int SecondaryProvider = 2;//Vonage
        }
        public static class DefaultDialingCode
        {
            public static int Select = 0;
            public static int US = 1;
            public static int India = 23;
        }
        public static class MessageDeliveryModes
        {
            //public static string Email = "Email";
            //public static string EmailMobile = "Email and Mobile";
            //public static string Mobile = "Mobile";
            public static string PrefillNA = "NA";

            public static string EmailSlashEmail = "Email / Email";
            public static string EmailSlashMobile = "Email / Mobile";
            public static string EmailSlashEmailAndMobile = "Email / Email & Mobile";
            public static string MobileSlashMobile = "Mobile / Mobile";
            public static string MobileSlashEmail = "Mobile / Email";
            public static string MobileSlashNone = "Mobile / None";
            public static string MobileSlashEmailAndMobile = "Mobile / Email & Mobile";
            public static string EmailAndMobileSlashMobile = "Email & Mobile / Mobile";
            public static string EmailAndMobileSlashEmail = "Email & Mobile / Email";
            public static string EmailAndMobileSlashEmailAndMobile = "Email & Mobile / Email & Mobile";
            public static string EmailSlashNone = "Email / None";
            public static string EmailAndMobileSlashNone = "Email & Mobile / None";
        }

        public static class UserRoles
        {
            public static readonly string GlobalAdmin = "GlobalAdmin";
            public static readonly string ProfessionalServicesRep = "ProfessionalServicesRep";
            public static readonly string IntegrationsManager = "IntegrationsManager";
            public static readonly string GlobalAdminDesc = "Global Admin";
            public static readonly string ProfessionalServicesRepDesc = "Professional Services Rep";
            public static readonly string IntegrationsManagerDesc = "Integrations Manager";
        }
        public static class DynamicPrefixNameToContractsFile
        {
            public static readonly int None = 1;
            public static readonly int EnvelopeID = 2;
            public static readonly int CustomName = 3;
            public static readonly int Contracts = 4;
            public static readonly int DateandTimeStamp = 5;
        }

        public static class RenameContractsFileTo
        {
            public static readonly int FirstUploadedDocumentName = 1;
            public static readonly int LastUploadedDocumentName = 2;
        }

    }
}
