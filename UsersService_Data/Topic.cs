using Microsoft.WindowsAzure.Storage.Table;

namespace RedditService_Data
{
    public class Topic : TableEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }

        public Topic(string topicId)
        {
            PartitionKey = "Topic";
            RowKey = topicId;
            Title = string.Empty;
            Content = string.Empty;
            ImageUrl = string.Empty;
            Upvotes = 0; 
            Downvotes = 0;
            CreatedAt = DateTime.MinValue;
            UserId = string.Empty;
        }

        public Topic() 
        {
            Title = string.Empty;
            Content = string.Empty;
            ImageUrl = string.Empty;
            Upvotes = 0;
            Downvotes = 0;
            CreatedAt = DateTime.MinValue;
            UserId = string.Empty;
        }
    }
}
