using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Model.DB
{
    public class Subscriptions
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public int TargetUserId { get; set; }  //  SubscriptionID

        [ForeignKey("SubscriberId")]
        public required User Subscriber { get; set; }

        [ForeignKey("TargetUserId")]
        public required User TargetUser { get; set; }
    }
}
