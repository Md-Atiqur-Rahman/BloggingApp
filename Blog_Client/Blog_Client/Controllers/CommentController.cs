using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blog_Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blog_Client.Controllers
{
    public class CommentController : Controller
    {
        ILogger<HomeController> _logger;
        IConfiguration _Configure;

        string apiBaseUrl;

        public CommentController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;

            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        #region Pagicnation
        // GET: Home
        public async Task<IActionResult> Display()
        {
            var model =await this.GetCustomers(1);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Display(int currentPageIndex)
        {
            return View(await this.GetCustomers(currentPageIndex));
        }

        private async Task<BlogModel> GetCustomers(int currentPage)
        {
            int maxRows = 100;

            BlogModel blogModel = new BlogModel();
            IEnumerable<CommentPostVM> comments = null;
            using (var client = new HttpClient())
            {
                
                PagingParam pagingParam = new PagingParam();
                pagingParam.pageSize = maxRows;
                pagingParam.pageNumber = currentPage;
                StringContent content = new StringContent(JsonConvert.SerializeObject(pagingParam), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: "Provide token after login");
                string endpoint = apiBaseUrl + "Comments/Display";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.IsSuccessStatusCode)
                    {
                        var readTask = Response.Content.ReadAsAsync<BlogModel>();
                        blogModel = readTask.Result;

                        List<CommentPostVM> list = new List<CommentPostVM>();
                        var post = blogModel.CommentPostVM.GroupBy(x => x.PostID).ToList();
                        foreach (var item in post)
                        {
                            var comnt = blogModel.CommentPostVM.Where(x => x.PostID == item.Key).GroupBy(x => x.Id).ToList();
                            foreach (var c in comnt) 
                            {
                                CommentPostVM entity = new CommentPostVM();
                                entity.PostMessage = item.FirstOrDefault().PostMessage;
                                entity.PostedDate = item.FirstOrDefault().PostedDate;
                                entity.PostBy = item.FirstOrDefault().PostBy;
                                entity.CommentMessage = c.FirstOrDefault().CommentMessage;
                                entity.CommentedDate = c.FirstOrDefault().CommentedDate;
                                entity.CommentBy = c.FirstOrDefault().CommentBy;
                                entity.Likes = blogModel.CommentPostVM.Where(x => x.PostID == item.Key && x.ReactStatus == 'L' && x.Id == c.Key).Count();
                                entity.Dislikes = blogModel.CommentPostVM.Where(x => x.PostID == item.Key && x.ReactStatus == 'D' && x.Id == c.Key).Count();
                                list.Add(entity);
                            }
                        }
                        blogModel.CommentPostVM = list;
                    }
                }

                return blogModel;

            }

        }
    
    #endregion
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            IEnumerable<CommentPostVM> comments = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                var responseTask = client.GetAsync("Comments");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CommentPostVM>>();
                    comments = readTask.Result;
                    ViewData["CurrentSort"] = sortOrder;
                    ViewData["CommentBy"] = String.IsNullOrEmpty(sortOrder) ? "CommentBy" : "";
                    ViewData["CommentMessage"] = String.IsNullOrEmpty(sortOrder) ? "CommentMessage" : "";
                    ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

                    if (searchString != null)
                    {
                        pageNumber = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }
                    ViewData["CurrentFilter"] = searchString;
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        comments = comments.Where(s => s.CommentBy.Contains(searchString)
                                               || s.CommentMessage.Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "CommentBy":
                            comments = comments.OrderByDescending(s => s.CommentBy);
                            break;
                        case "CommentMessage":
                            comments = comments.OrderBy(s => s.CommentMessage);
                            break;
                        case "date_desc":
                            comments = comments.OrderByDescending(s => s.PostedDate);
                            break;
                        default:
                            comments = comments.OrderBy(s => s.Id);
                            break;
                    }
                }
            }
            
            int pageSize = 3;
            return View(await PaginatedList<CommentPostVM>.CreateAsync(comments, pageNumber ?? 1, pageSize));
        }
      
        [HttpGet]
        public IActionResult Create()
        {
            
            string endpoint = apiBaseUrl + "/Posts";

           
            IEnumerable<Post> post = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                //HTTP GET
                var responseTask = client.GetAsync("Posts");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Post>>();
                   // var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                 // var  data = readTask.Result;
                    post = readTask.Result;
                    var s = post.Select(c => new SelectListItem
                    {
                        Value=c.Id.ToString(),
                        Text=c.PostMessage
                    });
                    ViewBag.post =s;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    post = Enumerable.Empty<Post>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(new Comment());

        }
        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            using (HttpClient client = new HttpClient())
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization= new System.Net.Http.Headers.AuthenticationHeaderValue(scheme:"Bearer",parameter:"Provide token after login");
                string endpoint = apiBaseUrl + "Comments";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //   TempData["Profile"] = JsonConvert.SerializeObject(comment);

                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, Response.StatusCode.ToString());
                        return View();

                    }

                }
            }
        }
    }
}
