using System.Web.Mvc;

namespace Cotecna.Vestalis.Web.Common
{
    /// <summary>
    /// Class extending FileStreamResult and returning Microsoft Excel Open XML (.xlsx) files.
    /// </summary>
    public class MicrosoftExcelStreamResult : FileStreamResult
    {
        /// <summary>
        /// Create an spreedsheet stream result
        /// </summary>
        /// <param name="stream">The stream to send to the response</param>
        /// <param name="downloadName">The name of the file when downloaded</param>
        public MicrosoftExcelStreamResult(System.IO.Stream stream, string downloadName) :
            base(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            base.FileDownloadName = downloadName;
        }
    }
}