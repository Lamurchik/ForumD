using Forum.Model.DB;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.Services
{
    public interface IGrateService
    {

    }

    public class InputGrade
    {
       
        public int? PostId { get; set; }
        [ForeignKey("Comment")]
        public int? CommentId { get; set; }
        [ForeignKey("User")]
        public required int UserId { get; set; }
        public required bool IsLike { get; set; }

        public static DateTime GradeDate { get => DateTime.UtcNow.Date; } // Дата комментария     
        public static TimeOnly GradeTime { get => TimeOnly.FromDateTime(DateTime.Now); } // Время комментария
    }



    public class GrateService
    {
        ForumDBContext _dbContext;
        

        public GrateService (ForumDBContext forumDBContext)
        {
            _dbContext = forumDBContext;
        }

        public async Task<string> AddGrate(InputGrade grade)
        {
            if ((grade.PostId == null && grade.CommentId == null) || (grade.PostId != null && grade.CommentId != null))
                throw new ArgumentException(" One object can have only one grade");

            Grade g = new Grade()
            {
                PostId = grade.PostId ,
                CommentId = grade.CommentId,
                UserId = grade.UserId,
                IsLike = grade.IsLike
            };
            return $"Grade added whit id{g.Id}";
        }

    }
}
