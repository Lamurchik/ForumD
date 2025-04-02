using Forum.Model.DB;
using Forum.Model.Services;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers.GraphQL.Mutation
{
    [ExtendObjectType(typeof(Mutation))]
    public class CommentMutation
    {
        public async Task<string> AddComment([Service] ForumDBContext context,[Service] SubscriptionService subscriptionService, [Service] ICommentManager commentManager ,
            InputComment ic, IFile? file)
        {
            ////вынести эту логику в сервис 
            //if(ic.ParentCommentId!=null)
            //{
            //    var userName =  context.Users.FirstOrDefaultAsync(u => u.Id == ic.UserId).Result?.NickName;                
            //    string msg = $"user {userName} replied to your comment:{ic.ParentCommentId}.";
            //    await sender.SendAsync(nameof(Subscription.Subscription.ReplyСomment), msg); 
            //}

            if (ic.ParentCommentId != null)
            {
                await subscriptionService.ReplyComment(ic);
            }
            var result = await commentManager.AddCommentAsync(ic, file);
            return result;
        }


        public async Task<string> ChangeComment([Service] ICommentManager commentManager, int commId, string? body, IFile? file)
        {
            var result = await commentManager.ChangeCommentAsync(commId, body, file);
            return result;
        }


        public async Task<string> DeleteComment([Service] ICommentManager commentManager, int commId)
        {
            var result = await commentManager.DeleteCommentAsync(commId);
            return result;
        }


       

    }
}
