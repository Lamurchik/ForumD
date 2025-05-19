using Forum.Model.DB;
using HotChocolate.Data.Projections;

namespace Forum.Controllers.GraphQL.Subscription
{
    

    public class Subscription
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">сообщение </param>
        /// <param name="userId"> кому это сообщение </param>
        /// <returns></returns>
        /// 
        [Subscribe]
        [Topic($"{nameof(ReplyComment)}_{{{nameof(userId)}}}")]
        public async Task<SocialNotification> ReplyComment ([EventMessage] SocialNotification socialNotification, int userId)
        {            
                return socialNotification;         
        }


        [Subscribe]
        [Topic($"{nameof(PostComment)}_{{{nameof(userId)}}}")]
        public async Task<SocialNotification> PostComment([EventMessage] SocialNotification socialNotification, int userId)
        {
            return socialNotification;
        }

        [Subscribe]
        [Topic($"{nameof(GradeScore)}_{{{nameof(userId)}}}")]
        public async Task<SocialNotification> GradeScore([EventMessage] SocialNotification socialNotification, int userId)
        {
            return socialNotification;
        }


    }
}
