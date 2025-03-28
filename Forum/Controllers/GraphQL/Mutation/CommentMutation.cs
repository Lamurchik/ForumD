using Forum.Model.Services;

namespace Forum.Controllers.GraphQL.Mutation
{
    [ExtendObjectType(typeof(Mutation))]
    public class CommentMutation
    {
        public async Task<string> AddComment([Service] ICommentManager commentManager ,InputComment ic, IFile? file)
        {
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
