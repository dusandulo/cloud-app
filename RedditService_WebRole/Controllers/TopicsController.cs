using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using RedditService_Data;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditService.Controllers
{
    public class TopicsController : Controller
    {
        private readonly RedditDataRepository _repository;

        public TopicsController()
        {
            _repository = new RedditDataRepository();
        }

        public ActionResult Index()
        {
            try
            {
                var topics = _repository.RetrieveAllTopics().ToList();

                // Log the topics
                System.Diagnostics.Debug.WriteLine("Retrieved topics count: " + topics.Count);
                foreach (var topic in topics)
                {
                    System.Diagnostics.Debug.WriteLine($"Topic - RowKey: {topic.RowKey}, Title: {topic.Title}, Content: {topic.Content}, ImageUrl: {topic.ImageUrl}");
                }

                return View(topics);
            }
            catch (StorageException ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine("StorageException: " + ex.Message);
                // Handle the exception as needed, possibly return an error view
                return View("Error", new HandleErrorInfo(ex, "Topics", "Index"));
            }
        }

        public ActionResult Create()
        {
            return View("AddTopic");
        }

        [HttpPost]
        public ActionResult AddTopic(string RowKey, string Title, string Content, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string uniqueBlobName = $"image_{RowKey}";
                    var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("roland");
                    container.CreateIfNotExists();
                    var blob = container.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = file.ContentType;

                    blob.UploadFromStream(file.InputStream);

                    var entry = new Topic(RowKey)
                    {
                        Title = Title,
                        Content = Content,
                        ImageUrl = blob.Uri.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        Downvotes = 0,
                        Upvotes = 0
                    };

                    _repository.AddTopic(entry);

                    var queue = storageAccount.CreateCloudQueueClient().GetQueueReference("roland");
                    queue.CreateIfNotExists();
                    queue.AddMessage(new CloudQueueMessage(RowKey));

                    return RedirectToAction("Index");
                }
                catch (StorageException ex)
                {
                    System.Diagnostics.Debug.WriteLine("StorageException: " + ex.Message);
                    ModelState.AddModelError("", "A storage error occurred while creating the topic: " + ex.Message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    ModelState.AddModelError("", "An error occurred while creating the topic: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please upload a valid image file.");
            }

            return View("AddTopic");
        }
    }
}