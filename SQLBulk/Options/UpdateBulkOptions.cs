using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Options
{
    public class UpdateBulkOptions : BulkOptions
    {
        /// <summary>
        /// Column names for merge conditions. Uses AND operator. See Merge statetent documentation.
        /// </summary>
        public string[] MatchOnColumnNames { get; set; }

        /// <summary>
        /// Custom condition for merge statements. Use d as destination table and s as source table.
        /// Example: s.Id = d.Id
        /// </summary>
        public string CustomMergeCondition { get; set; }
    }
}
