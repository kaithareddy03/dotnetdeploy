using RSign.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IConditionalControlRepository
    {
        ConditionalControlsDetailsNew GetAllConditionalControl(string envelopeStage, Guid iD1, Guid iD2, EnvelopeDetails envelopeDetails, List<ConditionalControlMapping> conditionalControlMappings = null, bool checkConditionalControl = true);
        DependentFieldsPOCO GetControllingFieldOfControl(Guid controlID);
        bool SaveConditionalControlForSigner(ConditionalControlsDetailsNew conditionalControls);
        bool SaveConditionalControl(ConditionalControlsDetailsNew conditionalControls);
    }
}
