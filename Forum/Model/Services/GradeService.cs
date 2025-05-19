using Forum.Model.DB;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.Services
{
    public interface IGrateService
    {

        public  Task<string> ChangeGrateAsync(int Id, bool isLike);
        public Task<string> DeleteGrateAsync(int Id);
        public  Task<string> AddGrateAsync(InputGrade grade);

        public Task<OutputGrade> CounterPost(int postId);
        public Task<OutputGrade> CounterComment(int commentId);
    }

    public class InputGrade
    {
       
        public int? PostId { get; set; }
        [ForeignKey("Comment")]
        public int? CommentId { get; set; }
        [ForeignKey("User")]
        public required int UserId { get; set; }
        public required bool IsLike { get; set; }

        public static DateTime GradeDate { get => DateTime.UtcNow.Date; }  
        public static TimeOnly GradeTime { get => TimeOnly.FromDateTime(DateTime.Now); } 
    }

    public class OutputGrade
    {
        public int Like {  get; set; }      
        public int DisLike { get; set; }
    }
    public class GradeService : IGrateService
    {
        ForumDBContext _dbContext;
        

        public GradeService (ForumDBContext forumDBContext)
        {
            _dbContext = forumDBContext;
        }

        public async Task<string> AddGrateAsync(InputGrade grade)
        {
            if ((grade.PostId == null && grade.CommentId == null) || (grade.PostId != null && grade.CommentId != null))
                throw new ArgumentException(" One object can have only one grade");

            Grade g = new Grade()
            {
                PostId = grade.PostId ,
                CommentId = grade.CommentId,
                UserId = grade.UserId,
                IsLike = grade.IsLike,
                GradeDate = InputGrade.GradeDate,
                GradeTime = InputGrade.GradeTime
            };

            _dbContext.Grades.Add(g);

            await _dbContext.SaveChangesAsync();

            return $"Grade added whit id{g.Id}";
        }

        public async Task<OutputGrade> CounterPost(int postId)
        {
            var counts = await _dbContext.Posts
                 .Where(p => p.Id == postId)
                .Select(p => new
                 {
                     Like = p.Grades.Count(g => g.IsLike),
                     DisLike = p.Grades.Count(g => !g.IsLike),
                     Exists = true
                 })
                .FirstOrDefaultAsync();

            if (counts == null)
                throw new ArgumentException("post not found");

            return new OutputGrade
            {
                Like = counts.Like,
                DisLike = counts.DisLike
            };
        }

        public async Task<OutputGrade> CounterComment(int commentId)
        {
            var counts = await _dbContext.Comments
                 .Where(p => p.Id == commentId)
                .Select(p => new
                {
                    Like = p.Grades.Count(g => g.IsLike),
                    DisLike = p.Grades.Count(g => !g.IsLike),
                    Exists = true
                })
                .FirstOrDefaultAsync();

            if (counts == null)
                throw new ArgumentException("post not found");

            return new OutputGrade
            {
                Like = counts.Like,
                DisLike = counts.DisLike
            };
        }


        public async Task<string> ChangeGrateAsync(int Id, bool isLike )
        {
            var grate = await _dbContext.Grades.FirstOrDefaultAsync(g => g.Id == Id);
            if (grate == null) throw new ArgumentException("Grate not found");

            grate.IsLike = isLike;

            await _dbContext.SaveChangesAsync();

            return "Grate Changed";
        }

        public async Task<string> DeleteGrateAsync(int Id)
        {
            var grate = await _dbContext.Grades.FirstOrDefaultAsync(g => g.Id == Id);
            if (grate == null) throw new ArgumentException("Grate not found");

             _dbContext.Grades.Remove(grate);
            await _dbContext.SaveChangesAsync();

            return "grate deleted";
        }
    }
}
