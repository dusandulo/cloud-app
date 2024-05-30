using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace RedditService_Data
{
    public class RedditDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _userTable;
        private CloudTable _topicTable;
        private CloudTable _commentTable;

        public RedditDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);

            _userTable = tableClient.GetTableReference("UserTable");
            _userTable.CreateIfNotExists();

            _topicTable = tableClient.GetTableReference("TopicTable");
            _topicTable.CreateIfNotExists();

            _commentTable = tableClient.GetTableReference("CommentTable");
            _commentTable.CreateIfNotExists();
        }

        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _userTable.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }

        public void AddUser(User newUser)
        {
            TableOperation insertOperation = TableOperation.Insert(newUser);
            _userTable.Execute(insertOperation);
        }

        public List<Topic> RetrieveAllTopics()
        {
            var query = new TableQuery<Topic>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Topic"));
            return _topicTable.ExecuteQuery(query).ToList();
        }

        public void AddTopic(Topic newTopic)
        {
            TableOperation insertOperation = TableOperation.Insert(newTopic);
            _topicTable.Execute(insertOperation);
        }

        public IQueryable<Comment> RetrieveAllComments()
        {
            var results = from g in _commentTable.CreateQuery<Comment>()
                          where g.PartitionKey == "Comment"
                          select g;
            return results;
        }

        public void AddComment(Comment newComment)
        {
            TableOperation insertOperation = TableOperation.Insert(newComment);
            _commentTable.Execute(insertOperation);
        }

        public bool Exists(string indexNo)
        {
            return RetrieveAllTopics().Where(t => t.RowKey == indexNo).FirstOrDefault() != null;
        }

        //delete

        public void DeleteTopic(string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<Topic>("Topic", rowKey);
            var retrievedResult = _topicTable.Execute(retrieveOperation);
            var deleteEntity = (Topic)retrievedResult.Result;

            if (deleteEntity != null)
            {
                // Delete the topic
                var deleteOperation = TableOperation.Delete(deleteEntity);
                _topicTable.Execute(deleteOperation);
            }
        }

        //upvote downvote

        public void UpvoteTopic(string topicId)
        {
            var retrieveOperation = TableOperation.Retrieve<Topic>("Topic", topicId);
            var retrievedResult = _topicTable.Execute(retrieveOperation);
            var topic = (Topic)retrievedResult.Result;

            if (topic != null)
            {
                topic.Upvote();
                var updateOperation = TableOperation.Replace(topic);
                _topicTable.Execute(updateOperation);
            }
        }

        public void DownvoteTopic(string topicId)
        {
            var retrieveOperation = TableOperation.Retrieve<Topic>("Topic", topicId);
            var retrievedResult = _topicTable.Execute(retrieveOperation);
            var topic = (Topic)retrievedResult.Result;

            if (topic != null)
            {
                topic.Downvote();
                var updateOperation = TableOperation.Replace(topic);
                _topicTable.Execute(updateOperation);
            }
        }
    }
}
