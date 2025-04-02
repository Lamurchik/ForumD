using HotChocolate.Subscriptions;
using Forum.Controllers.GraphQL.Subscription;
using Forum.Model.DB;
namespace Forum.Controllers.GraphQL.Mutation
{
    public partial class Mutation
    {
        public async Task<string> SubTest([Service] ITopicEventSender sender, [Service] ForumDBContext context, string str)
        {
            
            await sender.SendAsync(nameof(Subscription.Subscription.UserDidSomeThink), str);
            return str;
        }

    }
}
