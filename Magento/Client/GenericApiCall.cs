using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;

namespace IllustratorMagentoConsole.Magento.Client
{
    internal class GenericApiCall
    {
        private string baseURL;
        //private IAuthenticator authenticator;

        public GenericApiCall(string baseURL, IAuthenticator authenticator)
        {
            this.baseURL = baseURL;
            //this.authenticator = authenticator;
        }
        public GenericApiCall(string baseURL, string username, string password)
        {
            this.baseURL = baseURL;
            //authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(username, password);
        }
        public string Request(RestSharp.Method method, string endPoint, Dictionary<string, object> headers, Dictionary<string, object> parameters, Dictionary<string, object> queryParameters, string body)
        {
            RestClient client = new RestClient(baseURL);
            RestRequest request = new RestRequest(endPoint, method);
            foreach (var key in headers.Keys)
            {
                if (headers[key].GetType().ToString().StartsWith("System.Collections.Generics.List"))
                {
                    request.AddHeader(key, JsonConvert.SerializeObject(headers[key]));
                }
                else
                {
                    request.AddHeader(key, headers[key].ToString());
                }
            }
            foreach (var key in parameters.Keys)
            {
                if (key == "application/json")
                {
                    request.AddParameter(key, parameters[key], ParameterType.RequestBody);
                }
                else
                {
                    request.AddParameter(key, (string)parameters[key]);
                }
            }
            foreach (var key in queryParameters.Keys)
            {
                if (headers[key].GetType().ToString().StartsWith("System.Collections.Generics.List"))
                {
                    request.AddQueryParameter(key, JsonConvert.SerializeObject(queryParameters[key]));
                }
                else
                {
                    request.AddQueryParameter(key, queryParameters[key].ToString());
                }
            }
            var response = client.Execute(request);
            try
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Content;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";

        }
    }
}
