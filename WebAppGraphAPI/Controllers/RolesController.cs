﻿using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebAppGraphAPI.Utils;

namespace WebAppGraphAPI.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {

        /// <summary>
        /// Gets a list of <see cref="Role"/> objects from Graph.
        /// </summary>
        /// <returns>A view with the list of <see cref="Role"/> objects.</returns>
        public ActionResult Index()
        {
            //Get the access token as we need it to make a call to the Graph API
            string accessToken = AuthUtils.GetAuthToken(Request, HttpContext);
            if (accessToken == null)
            {
                //
                // The user needs to re-authorize.  Show them a message to that effect.
                //
                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View();
            }

            //Setup GRaph API connection and get a list of roles
            Guid ClientRequestId = Guid.NewGuid();
            GraphSettings graphSettings = new GraphSettings();
            graphSettings.ApiVersion = GraphConfiguration.GraphApiVersion;
            GraphConnection graphConnection = new GraphConnection(accessToken, ClientRequestId, graphSettings);

            PagedResults<Role> pagedResults = graphConnection.List<Role>(null, new FilterGenerator());

            return View(pagedResults.Results);
        }

        /// <summary>
        /// Gets details of a single <see cref="Role"/> Graph.
        /// </summary>
        /// <returns>A view with the details of a single <see cref="Role"/>.</returns>
        public ActionResult Details(string objectId)
        {
            //Get the access token as we need it to make a call to the Graph API
            string accessToken = AuthUtils.GetAuthToken(Request, HttpContext);
            if (accessToken == null)
            {
                //
                // The user needs to re-authorize.  Show them a message to that effect.
                //
                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View();
            }

            // Setup Graph API connection and get single Role
            Guid ClientRequestId = Guid.NewGuid();
            GraphSettings graphSettings = new GraphSettings();
            graphSettings.ApiVersion = GraphConfiguration.GraphApiVersion;
            GraphConnection graphConnection = new GraphConnection(accessToken, ClientRequestId, graphSettings);

            Role contact = graphConnection.Get<Role>(objectId);
            return View(contact);
        }

        /// <summary>
        /// Gets a list of <see cref="User"/> objects that are members of a give <see cref="Role"/>.
        /// </summary>
        /// <param name="objectId">Unique identifier of the <see cref="Role"/>.</param>
        /// <returns>A view with the list of <see cref="User"/> objects.</returns>
        public ActionResult GetMembers(string objectId)
        {
            //Get the access token as we need it to make a call to the Graph API
            string accessToken = AuthUtils.GetAuthToken(Request, HttpContext);
            if (accessToken == null)
            {
                //
                // The user needs to re-authorize.  Show them a message to that effect.
                //
                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View();
            }

            // Setup Graph API connection and get Role members
            Guid ClientRequestId = Guid.NewGuid();
            GraphSettings graphSettings = new GraphSettings();
            graphSettings.ApiVersion = GraphConfiguration.GraphApiVersion;
            GraphConnection graphConnection = new GraphConnection(accessToken, ClientRequestId, graphSettings);

            Role role = graphConnection.Get<Role>(objectId);
            IList<GraphObject> members = graphConnection.GetAllDirectLinks(role, LinkProperty.Members);

            IList<User> users = new List<User>();

            foreach (GraphObject obj in members)
            {
                if (obj is User)
                {
                    users.Add((User)obj);
                }
            }

            return View(users);
        }
    }
}