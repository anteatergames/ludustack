using System.Collections.Generic;

namespace LuduStack.Web.Models
{
    public class Select2SearchResultViewModel
    {
        public List<Select2SearchResultItemViewModel> Results { get; set; }

        public Select2SearchResultPaginationViewModel Pagination { get; set; }

        public Select2SearchResultViewModel()
        {
            Results = new List<Select2SearchResultItemViewModel>();
            Pagination = new Select2SearchResultPaginationViewModel();
        }
    }

    public class Select2SearchResultViewModel<T>
    {
        public List<T> Results { get; set; }

        public Select2SearchResultPaginationViewModel Pagination { get; set; }

        public Select2SearchResultViewModel()
        {
            Results = new List<T>();
            Pagination = new Select2SearchResultPaginationViewModel();
        }
    }

    public class Select2SearchResultItemViewModel
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }

    public class Select2UserSearchResultItemViewModel : Select2SearchResultItemViewModel
    {
        public string Location { get; set; }

        public string ProfileImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public string CreateDateText { get; set; }
    }

    public class Select2SearchResultPaginationViewModel
    {
        public bool More { get; set; }

        public Select2SearchResultPaginationViewModel()
        {
            More = false;
        }
    }
}