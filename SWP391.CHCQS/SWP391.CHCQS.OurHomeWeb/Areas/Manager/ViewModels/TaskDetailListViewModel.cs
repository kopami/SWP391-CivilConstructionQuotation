﻿using SWP391.CHCQS.OurHomeWeb.Models;

namespace SWP391.CHCQS.OurHomeWeb.Areas.Manager.ViewModels
{
    public class TaskDetailListViewModel
    {
        public string? QuoteId { get; set; }
        public string? TaskId { get; set; }
        public string? TaskName { get; set; }
        public decimal? Price { get; set; }
        public KeyValuePair<string, string> Note { get; set; }
    }
}
