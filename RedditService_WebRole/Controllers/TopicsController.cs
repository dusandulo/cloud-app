using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using RedditService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace RedditService.Controllers
{
    public class TopicsController : Controller
    {
        private readonly RedditDataRepository _repository;

        public TopicsController()
        {
            _repository = new RedditDataRepository();
        }

        public ActionResult Index() // reseno
        {
            var topics = _repository.RetrieveAllTopics();
            return View(topics);
        }

        public ActionResult Create() // reseno
        {
            return View("AddTopic");
        }

        [HttpPost]
        public ActionResult AddTopic(String RowKey, String Title, String Content, HttpPostedFileBase file)
        {
            try
            {
                if (_repository.Exists(RowKey))
                {
                    return View("Error");
                }

                string uniqueBlobName = string.Format("image_{0}", RowKey);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("roland");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = file.ContentType;


                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(file.InputStream);
                // upis studenta u table storage koristeci StudentDataRepository klasu
                Topic entry = new Topic(RowKey) { Title = Title, Content = Content, ImageUrl = blob.Uri.ToString(), CreatedAt = DateTime.Now, Downvotes = 0, Upvotes = 0}; // + UserId
                _repository.AddTopic(entry);

                CloudQueue queue = QueueHelper.GetQueueReference("roland");
                queue.AddMessage(new CloudQueueMessage(RowKey), null, TimeSpan.FromMilliseconds(30));

                return RedirectToAction("Index");
            }
            catch { return RedirectToAction("AddTopic"); }
        }
    }
}