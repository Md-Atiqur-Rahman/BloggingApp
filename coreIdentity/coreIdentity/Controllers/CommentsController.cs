using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coreIdentity.Models;
using coreIdentity.ViewModel;

namespace coreIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly dbIdentity _context;
        private readonly List<CommentPostVM> _CommentPostList;



        public CommentsController(dbIdentity context)
        {
            _context = context;

            List<CommentPostVM> CommentPostList = new List<CommentPostVM>
            {
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=001},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=002},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=003},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=004},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=005},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=006},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='L',StatusId=007},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='D',StatusId=008},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='D',StatusId=009},
              new CommentPostVM {Id=1,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now,ReactStatus='D',StatusId=010},
              
                
                
                
                
              new CommentPostVM {Id=2,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 2",CommentBy="User2",CommentedDate=DateTime.Now },
              new CommentPostVM {Id=3,PostID=1,PostMessage ="Post 1",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-1),CommentMessage="Comment 3",CommentBy="User3",CommentedDate=DateTime.Now },

               new CommentPostVM {Id=4,PostID=2,PostMessage ="Post 2",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-2),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now },
              new CommentPostVM {Id=5,PostID=2,PostMessage ="Post 2",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-2),CommentMessage="Comment 2",CommentBy="User2",CommentedDate=DateTime.Now },
              new CommentPostVM {Id=6,PostID=2,PostMessage ="Post 2",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-2),CommentMessage="Comment 3",CommentBy="User3",CommentedDate=DateTime.Now },
              new CommentPostVM {Id=7,PostID=2,PostMessage ="Post 2",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-2),CommentMessage="Comment 3",CommentBy="User3",CommentedDate=DateTime.Now },

               new CommentPostVM {Id=8,PostID=3,PostMessage ="Post 3",PostBy="Admin",PostedDate=DateTime.Now.AddDays(-3),CommentMessage="Comment 1",CommentBy="User1",CommentedDate=DateTime.Now },

            };
            _CommentPostList = CommentPostList;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentPostVM>>> Get()
        {
            List<CommentPostVM> list = new List<CommentPostVM>();
            var comments = _context.Comments.ToList();
            foreach (var item in comments)
            {
                var post = _context.Posts.Where(x => x.Id == item.PostID).FirstOrDefault();
                if (post != null)
                {
                    CommentPostVM entity = new CommentPostVM();
                    entity.PostID = post.Id;
                    entity.PostMessage = post.PostMessage;
                    entity.PostedDate = post.PostedDate;
                    entity.PostBy = post.PostBy;
                    entity.Id = item.Id;
                    entity.CommentBy = item.CommentBy;
                    entity.CommentMessage = item.CommentMessage;
                    entity.CommentedDate = item.PostedDate;
                    list.Add(entity);
                }
            }
            return list;
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        [HttpPost]
        [Route("Display")]
        public async Task<ActionResult<BlogModel>> Display(PagingParam pagingParam)
        {
            BlogModel blogModel = new BlogModel();
            
            //BlogModel blogModel = new BlogModel();
            blogModel.CommentPostVM = _CommentPostList.OrderBy(x=>x.Id).Skip((pagingParam.pageNumber - 1) * pagingParam.pageSize).Take(pagingParam.pageSize).ToList();
            double pageCount = (double)((decimal)_CommentPostList.Count() / Convert.ToDecimal(pagingParam.pageSize));
            blogModel.PageCount = (int)Math.Ceiling(pageCount);
            blogModel.CurrentPageIndex = pagingParam.pageNumber;
            return blogModel;
        }
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
