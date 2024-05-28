using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _userTable.CreateIfNotExistsAsync();

            _topicTable = tableClient.GetTableReference("TopicTable");
            _topicTable.CreateIfNotExistsAsync();

            _commentTable = tableClient.GetTableReference("CommentTable");
            _commentTable.CreateIfNotExistsAsync();
        }

        public async Task<List<User>> RetrieveAllUsersAsync()
        {
            var query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "User"));
            var users = new List<User>();
            TableContinuationToken token = null;

            do
            {
                var segment = await _userTable.ExecuteQuerySegmentedAsync(query, token);
                users.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            return users;
        }

        public async Task AddUserAsync(User newUser)
        {
            TableOperation insertOperation = TableOperation.Insert(newUser);
            await _userTable.ExecuteAsync(insertOperation);
        }

        public async Task<List<Topic>> RetrieveAllTopicsAsync()
        {
            var query = new TableQuery<Topic>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Topic"));
            var topics = new List<Topic>();
            TableContinuationToken token = null;

            do
            {
                var segment = await _topicTable.ExecuteQuerySegmentedAsync(query, token);
                topics.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            return topics;
        }

        public async Task AddTopicAsync(Topic newTopic)
        {
            TableOperation insertOperation = TableOperation.Insert(newTopic);
            await _topicTable.ExecuteAsync(insertOperation);
        }

        public async Task<List<Comment>> RetrieveAllCommentsAsync()
        {
            var query = new TableQuery<Comment>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Comment"));
            var comments = new List<Comment>();
            TableContinuationToken token = null;

            do
            {
                var segment = await _commentTable.ExecuteQuerySegmentedAsync(query, token);
                comments.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            return comments;
        }

        public async Task AddCommentAsync(Comment newComment)
        {
            TableOperation insertOperation = TableOperation.Insert(newComment);
            await _commentTable.ExecuteAsync(insertOperation);
        }
    }
}
