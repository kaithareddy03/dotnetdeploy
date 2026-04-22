using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IDocumentContentsRepository
    {
        DelegatedControls GetDelegatedControls(Guid recipientId);
        bool Save(DocumentContents docContents);
        ControlStyle GetControlStyle(Guid DocumentContentId);
        bool Save(ControlStyle controlStyleDetail);
        IQueryable<SelectControlOptions> GetSelectControlOption(Guid DocumentContentId);
        bool Save(SelectControlOptions selectControlDetail);
        Control GetControlData(Guid ControlID);
        bool SaveDocumentContent(DocumentContents content, Guid documentID);
        bool SaveControlOptions(SelectControlOptions selectControlDetail, Guid DocumentContentID);
        DocumentContents GetEntity(Guid documentId);
        bool SaveControlStyle(ControlStyle controlStyleDetail, Guid DocumentContentID);
        string CheckControlSize(string Size);
        int GetControlSize(string Size);
    }
}
