using HotChocolate.Subscriptions;
using Forum.Controllers.GraphQL.Subscription;
using Forum.Model.DB;
using Forum.Model.Services;
namespace Forum.Controllers.GraphQL.Mutation
{
    public partial class Mutation
    {
        public async Task<string> SubTest([Service] ITopicEventSender sender, [Service] ForumDBContext context, string str)
        {
            
            await sender.SendAsync(nameof(Subscription.Subscription.UserDidSomeThink), str);
            return str;
        }

        public async Task <List<string>> TestPy([Service] SearchService searchService, string input)
        {           
            return await searchService.ProcessTextAsync(input);
        }

    }
}
