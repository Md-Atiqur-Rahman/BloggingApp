using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using coreIdentity.Models;

namespace coreIdentity.Models
{
    public class dbIdentity:IdentityDbContext<User,Role,Guid>
    {
        public dbIdentity(DbContextOptions<dbIdentity> options):base(options)
        {

        }
  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<coreIdentity.Models.Post> Posts { get; set; }

        public DbSet<coreIdentity.Models.Comment> Comments { get; set; }
        public DbSet<coreIdentity.Models.Status> Statuss { get; set; }
    }
    public class User : IdentityUser<Guid>
    { }

    public class Role : IdentityRole<Guid>

    { 
    
    
    }
    public class SeedData
    {
        private dbIdentity _context;
        private readonly UserManager<User> _usermanager;
        private readonly RoleManager<Role> _rolemanager;
        public SeedData(dbIdentity context, UserManager<User> m, RoleManager<Role> r)
        {
            _context = context;
            this._usermanager = m;
            this._rolemanager = r;
        }

        public async void SeedUserAndRole()
        {


            var user = new User
            {
                UserName = "admin",

                Email = "admin@a.com",

                //EmailConfirmed = true,
                //LockoutEnabled = false,
                // SecurityStamp = Guid.NewGuid().ToString()
            };

            // var roleStore = new RoleStore<Role>(_context);
            if(!_context.Users.Any())
            {
               
           
            try
            {

                if (!_context.Roles.Any(r => r.Name == "Admin"))
                {
                    await _rolemanager.CreateAsync(new Role { Name = "Admin", NormalizedName = "Admin" });
                }
                if (!_context.Roles.Any(r => r.Name == "User"))
                {
                    await _rolemanager.CreateAsync(new Role { Name = "User", NormalizedName = "User" });
                }
                if (!_context.Users.Any(u => u.UserName == user.UserName))
                {
                    await _usermanager.CreateAsync(user, "@Test123");

                    await _usermanager.AddToRoleAsync(user, "Admin");
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            }
        }
    }
        public class Post
    {
        public int Id { get; set; }
        public string PostMessage { get; set; }
        public string PostBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PostedDate { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
    public class Comment
    {
        public int Id { get; set; }
        public string CommentMessage { get; set; }
        public string CommentBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PostedDate { get; set; }
        [ForeignKey("Post")]
        public int PostID { get; set; }
        public virtual Post Post { get; set; }
        

    }

    public class Status
    {
        public int Id { get; set; }
        public bool Reacts { get; set; }
        public char ReactStatus { get; set; }
        public string StatusBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StatusDate { get; set; }
        [ForeignKey("Comment")]
        public int CommentID { get; set; }
        public virtual Comment Comment { get; set; }


    }
}
