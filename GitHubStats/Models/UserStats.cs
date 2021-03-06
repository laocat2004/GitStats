﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// GitHubStats.Models.UserStats
namespace GitHubStats.Models
{
    public class UserStats
    {
       public List<RepoStats> Repositories { get; set; }

       public User  User { get; set; }
        public int TotalDownload { get; set; }

        public int TotalStar { get; set; }
        public int TotalReleases { get; set; }

        public int TotalStars { get; set; }

        public long DiskUsage { get; set; }

       

    }
}