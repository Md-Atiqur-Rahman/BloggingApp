using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Client.Models
{
    public class DTO
    {
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
       
        public int PostID { get; set; }
        public virtual Post Post { get; set; }

    }
    public class CommentPostVM
    {

        public int Id { get; set; }
        public string CommentMessage { get; set; }
        public string CommentBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CommentedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PostedDate { get; set; }
        public int PostID { get; set; }
        public string PostMessage { get; set; }
        public string PostBy { get; set; }
        public bool Reacts { get; set; }
        public int StatusId { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public char ReactStatus { get; set; }


    }
    public class PagingParam
    {

        public int? pageNumber { get; set; }
        public int pageSize { get; set; }
    }

    public class BlogModel
    {
        ///<summary>
        /// Gets or sets Customers.
        ///</summary>
        public List<CommentPostVM> CommentPostVM { get; set; }

        ///<summary>
        /// Gets or sets CurrentPageIndex.
        ///</summary>
        public int CurrentPageIndex { get; set; }

        ///<summary>
        /// Gets or sets PageCount.
        ///</summary>
        public int PageCount { get; set; }
    }
}
