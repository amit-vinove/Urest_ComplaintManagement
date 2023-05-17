using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace UrestComplaintManagement
{
    public static class GlobaVariables
    {
        public static HttpClient WebApiClient = new HttpClient();

        static GlobaVariables()
        {
            WebApiClient.BaseAddress = new Uri("https://urest.in/complaint/swagger/");
            WebApiClient.DefaultRequestHeaders.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
    }
}