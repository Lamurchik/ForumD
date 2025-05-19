using HotChocolate.Subscriptions;
using Forum.Controllers.GraphQL.Subscription;
using Forum.Model.DB;
using Forum.Model.Services;
namespace Forum.Controllers.GraphQL.Mutation
{
    public partial class Mutation
    {
       

        public async Task <List<string>> TestPy([Service] SearchService searchService, string input)
        {           
            return await searchService.ProcessTextAsync(input);
        }

    }
}
