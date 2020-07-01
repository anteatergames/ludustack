using System;

namespace LuduStack.Application.ViewModels.Study
{
    public class StudyActivityViewModel : BaseViewModel
    {
        public Guid ActivityId { get; set; }

        public int Order { get; set; }
    }
}