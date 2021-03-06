﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using GitHubStats.Models;

namespace GitHubStats.Controllers
{
    public class BadgesController : Controller
    {
        const string badgeTemplate = "https://img.shields.io/badge/{0}-{1}-{2}.svg";
        // GET: Badges
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult UserBadge(string login = "")
        {
            if (login == "") throw new Exception("Login Missing");

            var client = GetClientForRequest(this);
            var user=client.User.Get(login).Result;
            var userStats = StatsController.GetUserStats(this, user);

            return View(userStats );

        }
        public ActionResult RepositoryDownloads( long id=0)
        {
            if (id == 0) throw new Exception("Repository Id Missing");
            string url = "";
            GitHubClient client = GetClientForRequest(this);
            var total = 0;
            var repository = client.Repository.Get(id).Result;
            foreach (var rel in client.Repository.Release.GetAll(id).Result)
            {
                foreach (var asset in rel.Assets)
                {
                    total += asset.DownloadCount;

                }
            }

            url = string.Format(badgeTemplate, "downloads", total, "orange");


            return DownloadFile(url, "repositoryDownload.svg", true);

        }

        public static  GitHubClient GetClientForRequest(Controller context)
        {

            GitHubClient client = new GitHubClient(new ProductHeaderValue(global::GitHubStats.Controllers.AppConfig.Current.AppName));
            var accessToken = context.Session["OAuthToken"] as string ?? ConfigurationManager.AppSettings["AnonymousToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                client.Credentials = new Credentials(accessToken);
            }
            else
            {


                var appUsername = ConfigurationManager.AppSettings["AppUsername"];
                var appPassword = ConfigurationManager.AppSettings["AppPassword"];
                if (!string.IsNullOrEmpty(appUsername) && !string.IsNullOrEmpty(appPassword))
                {
                    var basicAuth = new Credentials(appUsername, appPassword);
                    client.Credentials = basicAuth;
                }
            }

            return client;
        }

        private ActionResult DownloadFile(string url,string filename, bool inline)
        {
         

            WebClient client = new WebClient();
            var bytes=client.DownloadData(url);
          
            string contentType = MimeMapping.GetMimeMapping(filename);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = inline,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(bytes, contentType);
        }
    }

}