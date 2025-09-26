using System.ComponentModel.DataAnnotations;
namespace ImageLinks.Domain.Users
{
    public class User
    {
        [Key]
        public int Rec_ID { get; set; }
        public required string User_Name { get; set; }
        public required string Password { get; set; }
    }
}
