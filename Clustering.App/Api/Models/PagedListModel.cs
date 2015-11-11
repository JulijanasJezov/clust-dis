using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api.Models
{
    public class PagedListModel<T>
    {
        public int Total { get; set; }

        private int _pageNumber;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = value < 1 ? 1 : value;
            }
        }

        private int _itemsPerPage { get; set; }
        public int ItemsPerPage
        {
            get
            {
                return _itemsPerPage < 0 ? Total : _itemsPerPage;
            }
            set
            {
                _itemsPerPage = value;
            }
        }

        public int FromNumber
        {
            get
            {
                return Total > 0 ? StartIndex + 1 : 0;
            }
        }

        public int ToNumber
        {
            get
            {
                var to = StartIndex + ItemsPerPage;
                return Results.Count() == ItemsPerPage ? to : to - (ItemsPerPage - Results.Count());
            }
        }

        [JsonIgnore]
        public int StartIndex
        {
            get
            {
                return ItemsPerPage * (PageNumber - 1);
            }
        }

        public bool PreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool NextPage
        {
            get { return ToNumber < Total; }
        }

        public string OrderBy { get; set; }
        public string Direction { get; set; }

        public string FilterQuery { get; set; }

        public IEnumerable<T> Results { get; set; }
    }
}
