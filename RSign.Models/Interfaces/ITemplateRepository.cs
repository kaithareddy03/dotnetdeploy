using RSign.Models.APIModels;
using RSign.Models.APIModels.Envelope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface ITemplateRepository
    {
   
        Template GetCreatedDateTime(Guid templateID);       
        Task<ErrorResponseModel> ReSendPasswordEmail(UserVerificationModel userVerificationModel);
        EnvelopeInfo FillEnvelopeInfoFromInitalizeSignDocumentAPI(ResponseMessageForInitalizeSignDocument responseEnvelope);
        TemplateRoles GetRoleEntity(Guid roleId);
    }
}
