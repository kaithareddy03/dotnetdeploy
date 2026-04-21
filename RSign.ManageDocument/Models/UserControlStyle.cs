using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.ManageDocument.Models
{
    public class UserControlStyle
    {
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }
        public string FontColor { get; set; }
        public int FontSize { get; set; }
        public string FontName { get; set; }
        public string FontFileName { get; set; }
    }
}
