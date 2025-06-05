using Forum.Controllers.GraphQL.Subscription;
using Forum.Model.DB;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Forum.Model.Services
{
    public class SubscriptionInput
    {
        public string Message { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public int? CommentId { get; set; } = null;

        public NotificationType NotificationType { get; set; }

        public static DateTime Date { get => DateTime.UtcNow.Date; }     
        public static TimeOnly Time { get => TimeOnly.FromDateTime(DateTime.Now); }




        public SubscriptionInput(string message, int userId, int postId, NotificationType notificationType, int? commentId = null)
        {
            Message = message;
            UserId = userId;
            PostId = postId;
            NotificationType = notificationType;
            CommentId = commentId;
        }
    }
    public class SubscriptionService
    {

        private ForumDBContext _context;
        private ITopicEventSender _sender;
        public SubscriptionService(ForumDBContext dbContext, ITopicEventSender sender)
        {
            _context = dbContext;
            _sender = sender;
        }
        public string CreateTitle(NotificationType notificationType)
        {            
            switch(notificationType)
            {
                case NotificationType.AnswerComment: return "Ответ на коментарий";

                case NotificationType.PostComment: return "Ваш пост прокомментировали";

                case NotificationType.GradeScore: return "Оценка";

                case NotificationType.Subscribe: return "Новый пост";

                default: return "Уведомление";
            }
        }

        private SocialNotification CreateNotification(SubscriptionInput subscriptionInput)
        {
            var result = new SocialNotification() 
            {
                Title = CreateTitle(subscriptionInput.NotificationType),
                Message = subscriptionInput.Message,
                PostId = subscriptionInput.PostId,
                UserId = subscriptionInput.UserId,
                IsRead  = false,
                Date = SubscriptionInput.Date,
                Time = SubscriptionInput.Time,
                CommentId = subscriptionInput.CommentId                
            };
            return result;
        }

        //ответ на комментарий 
        public async Task ReplyComment(InputComment ic)
        {
            var userName =  _context.Users.FirstOrDefaultAsync(u => u.Id == ic.UserId).Result?.NickName;
            var comm = (await _context.Comments.FirstOrDefaultAsync(u => u.Id == ic.ParentCommentId));
            if (comm == null) 
                throw new ArgumentException();

            var userId = comm.UserId;

            if (userId == ic.UserId) // уведомления не приходят если вы отвечайте сами себе 
                return;


            string msg = $"пользователь {userName} оставил ответ на ваш комментарий :{ic.ParentCommentId}.";

            SubscriptionInput subscriptionInput = new SubscriptionInput(msg, userId, ic.PostId, NotificationType.AnswerComment, ic.ParentCommentId);

            await _sender.SendAsync($"{nameof(Subscription.ReplyComment)}_{userId}", CreateNotification(subscriptionInput));

        }

        //ответ на пост
        public async Task PostComment(InputComment ic)
        {
            var userName = _context.Users.FirstOrDefaultAsync(u => u.Id == ic.UserId).Result?.NickName;
            var post = (await _context.Posts.FirstOrDefaultAsync(u => u.Id == ic.PostId));
            if (post == null)
                throw new ArgumentException();

            var userId = post.UserAuthorId;

            if (userId == ic.UserId) // уведомления не приходят если вы отвечайте сами себе 
                return;


            string msg = $"пользователь {userName} оставил коментарий под вашим постом :{ic.PostId}.";

            SubscriptionInput subscriptionInput = new SubscriptionInput(msg, userId, ic.PostId, NotificationType.PostComment);

            await _sender.SendAsync($"{nameof(Subscription.PostComment)}_{userId}", CreateNotification(subscriptionInput));
        }

        //а нужно ли уведомление о лайках?
        public async Task GradeScore(InputGrade inputGrade)
        {
            if (!inputGrade.IsLike) //уведомляем только о лайках
                return;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == inputGrade.UserId); //чья оценка
            if (user == null)
                throw new ArgumentException();
            User? uc; //для кого оценка
            string obj;
            int postId;

            if (inputGrade.PostId != null)
            {
                var post = await _context.Posts.Include(p=>p.User).FirstOrDefaultAsync(p => p.Id == inputGrade.PostId);
                if(post==null) throw new ArgumentException();
                uc = post.User;
                postId = post.Id;
                obj = "пост";
            }
            else if(inputGrade.CommentId!=null)
            {
                var comment = await _context.Comments.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == inputGrade.CommentId);
                if (comment == null) throw new ArgumentException();
                uc = comment.User;
                postId = comment.PostId;
                obj = "комментарий";
            }
            else throw new ArgumentException();


            if(uc == null) throw new ArgumentException();


            if (user.Id == uc.Id) return;


            string msg = $"пользователь {user.NickName} оценил ваш {obj}.";

            SubscriptionInput subscriptionInput = new SubscriptionInput(msg, user.Id, postId, NotificationType.GradeScore);

            await _sender.SendAsync($"{nameof(Subscription.GradeScore)}_{user.Id}", CreateNotification(subscriptionInput));
        }
        // не нужный метод?
        public async Task AdjustmentId(SocialNotification socialNotification,int userId)
        {
            try
            {
                socialNotification.UserId = userId;
                _context.Update(socialNotification);
                await _context.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении уведомления: {ex.Message}");
            }
        }
    }
}
