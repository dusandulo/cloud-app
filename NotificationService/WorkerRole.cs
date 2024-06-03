using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using RedditService_Data;
using Microsoft.Azure;
using HealthMonitoringService;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private static HealthMonitoringServer healthMonitoringServer;

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            healthMonitoringServer = new HealthMonitoringServer();
            healthMonitoringServer.Open();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // Cloud Storage account connection
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient(); 
            CloudQueue queue = queueClient.GetQueueReference("notificationsqueue"); 

            try
            {
                queue.CreateIfNotExists();
            }
            catch (StorageException ex)
            {
                Trace.TraceError("Error creating queue: " + ex.Message);
                throw;
            }

            var repository = new RedditDataRepository();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Checking queue for new messages.");

                CloudQueueMessage message = await queue.GetMessageAsync();
                if (message != null)
                {
                    string commentId = message.AsString;
                    ProcessMessage(repository, commentId);

                    await queue.DeleteMessageAsync(message);
                }

                await Task.Delay(30000); // Check every 30 seconds
            }
        }

        private void ProcessMessage(RedditDataRepository repository, string commentId)
        {
            var comment = repository.GetCommentById(commentId);

            if (comment != null)
            {
                var topicId = comment.TopicId;
                var subscribers = repository.GetSubscribersByTopicId(topicId);

                foreach (var subscriber in subscribers)
                {
                    SendEmail(subscriber.Email, comment);
                }
            }
        }

        private void SendEmail(string email, Comment comment)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 465,
                Credentials = new NetworkCredential("ducarhd3@gmail.com", "cwnkvgzxtmibntrd"),
                EnableSsl = true,
            };

            string body = $"New comment on topic: {comment.TopicId}\n\n" +
                          $"Comment: {comment.Content}\n" +
                          $"Posted by: {comment.UserId}\n" +
                          $"At: {comment.CreatedAt}";

            smtpClient.Send("ducarhd3@gmail.com", email, "New Comment Notification", body);
            Trace.TraceInformation($"{email} got message: {body}");
        }
    }
}
