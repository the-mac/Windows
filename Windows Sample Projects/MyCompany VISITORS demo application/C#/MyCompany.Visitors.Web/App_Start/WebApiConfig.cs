﻿
namespace MyCompany.Visitors.Web
{
    using System.Web.Http;

    /// <summary>
    /// WebApiConfig
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register Routes
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            // NoAuth routes are only for demos without internet access
            // this routes are not securized and all the requests are done with a dummy user
            config.Routes.MapHttpRoute(
                name: "DefaultApiNoAuth",
                routeTemplate: "noauth/api/{controller}",
                defaults: null
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}",
                defaults: null
            );

        }
    }
}
