using RedditService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index()
        {
            var topics = await _repository.RetrieveAllTopicsAsync();
            return View(topics);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.CreatedAt = DateTime.Now;
                topic.RowKey = Guid.NewGuid().ToString();
                await _repository.AddTopicAsync(topic);
                return RedirectToAction(nameof(Index));
            }
            return View(topic);
        }

        public async Task<IActionResult> Details(string id)
        {
            var topic = (await _repository.RetrieveAllTopicsAsync()).FirstOrDefault(t => t.RowKey == id);
            if (topic == null)
            {
                return NotFound();
            }
            var comments = (await _repository.RetrieveAllCommentsAsync()).Where(c => c.TopicId == id).ToList();
            ViewBag.Comments = comments;
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string topicId, string content)
        {
            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.Now,
                RowKey = Guid.NewGuid().ToString(),
                TopicId = topicId,
                UserId = User.Identity.Name // Assuming you have user identity setup
            };

            await _repository.AddCommentAsync(comment);
            // Send notification logic here (if needed)
            return RedirectToAction(nameof(Details), new { id = topicId });
        }
    }
}