using Forum.Model.Services;

namespace Forum.Controllers.GraphQL.Mutation
{

    [ExtendObjectType(typeof(Mutation))]
    public class GradeMutation
    {
        public async Task<string> ChangeGrate([Service] IGrateService grateService ,int Id, bool isLike)
        {
            var result = await grateService.ChangeGrateAsync(Id, isLike);
            return result;
        }
        public async Task<string> DeleteGrate([Service] IGrateService grateService, int Id)
        {
            var result = await grateService.DeleteGrateAsync(Id);
            return result;
        }
        public async Task<string> AddGrate([Service] IGrateService grateService, InputGrade grade)
        {
            var result = await grateService.AddGrateAsync(grade);
            return result;
        }

    }
}
