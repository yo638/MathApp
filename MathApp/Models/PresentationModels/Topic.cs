using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathApp.Models.DbModels
{
    public partial class Topic
    {
        [NotMapped]
        public string UpdatedTimeAgo { get; set; }
        [NotMapped]
        public string Password { get; set; }

        public void SetUpdatedTimeAgoProperty()
        {
            int timespan = (DateTime.Now.Date - UpdateDate.Date).Days;
            if (timespan == 0) UpdatedTimeAgo = "updated today";
            else if (timespan == 1) UpdatedTimeAgo = "updated yesterday";
            else if (timespan < 7) UpdatedTimeAgo = "updated " + timespan.ToString() + " days ago";
            else if (timespan < 30) UpdatedTimeAgo = "updated " + (timespan / 7).ToString() + " weeks ago";
            else if (timespan < 365) UpdatedTimeAgo = "updated " + (timespan / 30).ToString() + " months ago";
            else UpdatedTimeAgo = "updated " + (timespan / 365).ToString() + " years ago";
        }

    }
}
