using System;
using System.Web.Mvc;
using System.Web.Security;
using RedditService_Data;

namespace RedditService.Controllers
{
    public class CommentController : Controller
    {
        private readonly RedditDataRepository _repository;

        public CommentController()
        {
            _repository = new RedditDataRepository();
        }

        private string GetUserNameFromCookie()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                return authTicket?.Name; // This will be the email in this case
            }
            return null;
        }

        [HttpGet]
        public ActionResult Add(string id)
        {
            string userName = GetUserNameFromCookie();
            // Provera da li je korisnik ulogovan
            if (!String.IsNullOrEmpty(userName))
            {
                ViewBag.TopicId = id;
                return View("AddComment");
            }
            else
            {
                // Ako korisnik nije ulogovan, preusmerimo ga na stranicu za prijavljivanje
                return RedirectToAction("ShowLogin", "Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string topicId, string content)
        {
            string userName = GetUserNameFromCookie();
            // Provera da li je korisnik ulogovan
            if (!String.IsNullOrEmpty(userName))
            {
                // Dodavanje komentara
                var comment = new Comment(Guid.NewGuid().ToString())
                {
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userName,
                    TopicId = topicId
                };

                _repository.AddComment(comment);

                // Preusmeravanje nazad na stranicu sa topic-om nakon dodavanja komentara
                return RedirectToAction("Index", "Topics");
            }
            else
            {
                // Ako korisnik nije ulogovan, preusmerimo ga na stranicu za prijavljivanje
                return RedirectToAction("ShowLogin", "Login");
            }
        }

        
    }
}