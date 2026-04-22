using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Models.Custom
{
   public partial class MessageTemplate
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Subject { get; set; }
        public string? EmailBody { get; set; }
        public bool IsShared { get; set; }
        public Guid SharedId { get; set; }
    }
}
