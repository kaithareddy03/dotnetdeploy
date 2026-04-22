using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Models
{
  public class DocumentControls
  {
    public string DocumentName { get; set; }
    public int PageNo { get; set; }
    public int DocumentPageNo { get; set; }
    public int TemplatePageNo { get; set; }
    public int TemplateDocumentPageNo { get; set; }
    public string Label { get; set; }
    public string ElementId { get; set; }
    public string ControlValue { get; set; }
    public double XLeftposition { get; set; }
    public double YTopPosition { get; set; }
    public double ZBottompostion { get; set; }
    public string ControlHhtmlId { get; set; }
    public bool Required { get; set; }
    public string ControlName { get; set; }
    public string ControlType { get; set; }
    public byte[] SingnatureBytes { get; set; }
    public byte[] ImageBytes { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public bool? IsTypeSignature { get; set; }
    public UserControlStyle UserControlStyle { get; set; }
    public IList<UserSelectControl> UserSelectControl { get; set; }
    public bool? IsUploadSignature { get; set; }
    public bool? IsHandSignature { get; set; }
    public bool? IsFixedWidth { get; set; }
  }
}
