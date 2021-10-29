using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Options
{
    public class UpdateBulkOptions : BulkOptions
    {
        public string[] MatchOnColumnNames { get; set; }
    }
}
