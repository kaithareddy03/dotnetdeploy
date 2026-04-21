using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common.Helpers
{
    public class FinalContractSettings
    {
        /// <summary>
        /// set/get WatermarkAuthText
        /// </summary>
        public string WatermarkAuthText { get; set; }
        /// <summary>
        /// set/get WatermarkBackgroundText
        /// </summary>
        public string WatermarkBackgroundText { get; set; }
        /// <summary>
        /// set/get FinalContractOptions
        /// </summary>
        public int FinalContractOptions { get; set; }
        /// <summary>
        /// set/get UserTimeZone
        /// </summary>
        public string UserTimeZone { get; set; }
        /// <summary>
        /// set/get IsControlDisplayInTag
        /// </summary>
        public bool IsControlDisplayInTag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsBackgroundStamp { get; set; }
        /// <summary>
        /// set/get WatermarkText
        /// </summary>
        public string WatermarkText { get; set; }

    }
}
