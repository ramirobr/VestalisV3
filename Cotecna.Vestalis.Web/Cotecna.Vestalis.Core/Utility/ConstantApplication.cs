using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotecna.Vestalis.Core
{
    public static class ConstantApplication
    {
        #region Status

        /// <summary>
        /// Status indicating that a service order is not published
        /// </summary>
        public static string ServicePendingPublish = "NP";

        /// <summary>
        /// Status indicating that a service order is published
        /// </summary>
        public static string ServicePublishCompleted = "CP";

        /// <summary>
        /// Status indicating that an inspection report is not published
        /// </summary>
        public static string InspectionReportPendingPublish = "NP";

        /// <summary>
        /// Status indicating that an inspection report is published
        /// </summary>
        public static string InspectionReportPublishCompleted = "CP";
        #endregion
    }
}
