using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blog_Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blog_Client.Controllers
{
    public class PostController : Controller
    {
        ILogger<HomeController> _logger;
        IConfiguration _Configure;

        string apiBaseUrl;

        public PostController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;

            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }
        public async Task<IActionResult> Index(string sortOrder,string currentFilter,string searchString,int? pageNumber)
        {
            IEnumerable<Post> post = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                var responseTask = client.GetAsync("Posts");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Post>>();
                    post = readTask.Result;
                    ViewData["CurrentSort"] = sortOrder;
                    ViewData["PostBy"] = String.IsNullOrEmpty(sortOrder) ? "post_By" : "";
                    ViewData["PostMessage"] = String.IsNullOrEmpty(sortOrder) ? "post_Message" : "";
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
                        post = post.Where(s => s.PostBy.Contains(searchString)
                                               || s.PostMessage.Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "post_By":
                            post = post.OrderByDescending(s => s.PostBy);
                            break;
                        case "post_Message":
                            post = post.OrderBy(s => s.PostMessage);
                            break;
                        case "date_desc":
                            post = post.OrderByDescending(s => s.PostedDate);
                            break;
                        default:
                            post = post.OrderBy(s => s.Id);
                            break;
                    }
                }
            }
            int pageSize = 3;
            return View(await PaginatedList<Post>.CreateAsync(post, pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        public ActionResult Post()
        {
          //  UserInfo user = JsonConvert.DeserializeObject<UserInfo>(Convert.ToString(TempData["Profile"]));
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> Index(UserInfo user)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        //        string endpoint = apiBaseUrl + "/login";

        //        using (var Response = await client.PostAsync(endpoint, content))
        //        {
        //            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                TempData["Profile"] = JsonConvert.SerializeObject(user);

        //                return RedirectToAction("Profile");

        //            }
        //            else
        //            {
        //                ModelState.Clear();
        //                ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
        //                return View();

        //            }

        //        }
        //    }
        //}
        [HttpGet]
        public IActionResult Create()
        {

            return View(new Post());

        }
        [HttpPost]
        public async Task<IActionResult> Create(Post comment)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Posts";

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

        [HttpGet]
        public IActionResult Edit(long Id)
        {
            Post post = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                var responseTask = client.GetAsync("Posts/"+ Id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Post>();
                    post = readTask.Result;
                }
            }
            return View(post);
            //return View(new Post());

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Post comment)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Posts/"+comment.Id;

                using (var Response = await client.PutAsync(endpoint, content))
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

        [HttpGet]
        public IActionResult Delete(long Id)
        {
            IEnumerable<Post> post = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                var responseTask = client.DeleteAsync("Posts/" + Id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");

        }
    }
}
