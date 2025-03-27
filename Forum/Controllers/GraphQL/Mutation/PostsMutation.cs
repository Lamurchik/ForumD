using Forum.Model.DB;
using Forum.Model.Services;

namespace Forum.Controllers.GraphQL.Mutation
{
    [ExtendObjectType(typeof(Mutation))]
    public class PostsMutation
    {
        //вынести в отдельные классы?
        public async Task<string> PostPartialsCreate([Service] PostsMangerService postsManger, string? clientKey, 
            int order, string? body = null, IFile? file = null)
        {
            var result = await postsManger.PostPartialsCreateAsync(clientKey,  order, body, file);

            return result;
        }

        public async Task<string> PostCreate([Service] PostsMangerService postsManger, string title, int userId, string keyPostPartials)
        {
            var result = await postsManger.PostCreateAsync(title, userId, keyPostPartials);

            return result;
        }
        public async Task<string> AddPostPartial([Service] PostsMangerService postsManger, int postId, int afterOrder, string? body, IFile? file)
        {
            var result = await postsManger.AddPartialAsync(postId, afterOrder, body, file);

            return result;
        }

        public async Task<string> PostTitleChange([Service] PostsMangerService postsManger, int postId, string newTitle)
        {
            var result = await postsManger.PostTitleChangeAsync(postId, newTitle);
            return result;
        }

        public async Task<string> PostPartialChange([Service] PostsMangerService postsManger, int postPartialID, string? newBody, IFile? newFile)
        {
            var result = await postsManger.PostPartialChangeAsync(postPartialID, newBody, newFile);

            return result;
        }

        public async Task<string> DeletePost([Service] PostsMangerService postsManger, int postId)
        => await postsManger.DeletePostAsync(postId);

        public async Task<string> DeletePostPartial([Service] PostsMangerService postsManger, int partialId)
        => await postsManger.DeletePostPartialAsync(partialId);

        public async Task<string> AddTag([Service] PostsMangerService postsManger, int postId, params Tags[] tags)
        {
            return await postsManger.AddTagsAsync(postId, tags);
        }




    }
}
