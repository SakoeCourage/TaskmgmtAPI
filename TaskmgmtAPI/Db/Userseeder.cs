using Microsoft.EntityFrameworkCore;
using BCrypt;
namespace TaskmgmtAPI.Db
{
    public class Userseeder
    {
        public static void Initialize(Context context)
        {
            if (!context.User.Any())
            {
                context.User.AddRange(
                 new Models.User
                 {
                     name = "Sakoe courage",
                     email = "akorlicourage@gmail.com",
                     password = BCrypt.Net.BCrypt.HashPassword("itsapassword"),
                     createdAt = DateTime.Parse("2023-10-25 13:47:00"),
                     updatedAt = DateTime.Parse("2023-10-25 13:47:00"),
                 },
                 new Models.User
                 {
                     name = "akorlicourage",
                     email = "ceejay995@live.com",
                     password = BCrypt.Net.BCrypt.HashPassword("itsapassword123"),
                     createdAt = DateTime.Parse("2023-10-25 13:47:00"),
                     updatedAt = DateTime.Parse("2023-10-25 13:47:00"),
                 }
                 );
                context.SaveChanges();
            }
        }
    }
}
