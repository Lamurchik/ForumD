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

        public string  TestPy([Service] SearchService searchService, string input)
        {
            searchService.Tets(input);
            return searchService.Tets(input); 
        }

    }
}
