using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Interfaces
{
    public interface IModelHelper
    {
       // string GetEnvelopeDirectory(Guid iD);
        string GetEnvelopeDirectoryNew(Guid envelopeId, string UNCPath);
        string GetIdEnvelopeDirectory(string UNCPath);
        string GetTemplateDirectory(Guid templateId, string UNCPath);
        string GetIdTemplateDirectory(string UNCPath);
        string GetEnvelopeDirectoryByName(string UNCPathValue);
        string GetTemplateDirectoryName(string UNCPathValue);
        string CalculateMD5HashForShortURL(string input);
    }
}
