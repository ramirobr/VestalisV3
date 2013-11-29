using System;
using Cotecna.Vestalis.Core.MonitoringServiceReference;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class contains the methods for send reported errors to monitoring service
    /// </summary>
    public static class ExceptionBusiness
    {
        /// <summary>
        /// Make a call to Monitoring Service with the information of exception
        /// </summary>
        /// <param name="exception">Catched exception</param>
        /// <returns>ticketNumber</returns>
        public static void CatchExceptionInMonitoringService(Exception exception)
        {
            //instance the client
            MonitoringServiceClient monitoringClient = new MonitoringServiceClient();
            try
            {
                //Set the parameters for monitoring service
                ParameterMonitoring parameters = new ParameterMonitoring
                                                     {
                                                         Application = "VestalisV3",
                                                         Category = EnumCategory.Server,
                                                         MessageException =
                                                             exception.InnerException == null
                                                                 ? exception.Message
                                                                 : String.Format(
                                                                     "Exception message :{0} " + Environment.NewLine +
                                                                     " InnerException message:{1}", exception.Message,
                                                                     exception.InnerException.Message),
                                                         StackTrace =
                                                             exception.InnerException == null
                                                                 ? exception.StackTrace
                                                                 : String.Format(
                                                                     "StackTrace exception: {0} " + Environment.NewLine +
                                                                     " StackTrace inner exception: {1}",
                                                                     exception.StackTrace,
                                                                     exception.InnerException.StackTrace),
                                                         Type = EnumTicketType.Bug,
                                                         UserModule = "VestalisV3",
                                                         UserName = "VestalisV3"
                                                     };


                //save the exception information and get the ticket number
                monitoringClient.SaveSupportTicket(parameters);
            }
            finally
            {
                if (monitoringClient != null)
                {

                    ((IDisposable)monitoringClient).Dispose();

                }

            }
        }
    }
}
