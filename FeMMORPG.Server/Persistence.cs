using System;
using System.Net;
using FeMMORPG.Synchronization;
using Newtonsoft.Json;

namespace FeMMORPG.Server
{
    public class Persistence
    {
        private WebClient client;
        private const string BaseUri = "http://localhost:64651/";

        public Persistence()
        {
            this.client = new WebClient();
        }

        public User GetUser(string id)
        {
            return sendRequest<User>($"users/{id}", Methods.Get);
        }

        //public User FindUser(string username)
        //{
        //    var users = sendRequest<List<User>>($"users?id={username}", Methods.Get);
        //    return users.SingleOrDefault();
        //}

        public User SaveUser(User user)
        {
            return sendRequest<User>($"users/{user.Id}", Methods.Put, user);
        }

        private T sendRequest<T>(string resource, Methods method, object data = null)
        {
            this.client.Headers[HttpRequestHeader.ContentType] = "application/json";
            var url = BaseUri + resource;
            var dataString = data != null ? JsonConvert.SerializeObject(data) : "";

            Console.WriteLine($">>> Web {method.ToString()}: {resource} with data: {dataString}");

            string response = "";
            switch (method)
            {
                case Methods.Get:
                    response = client.DownloadString(url);
                    break;
                case Methods.Post:
                    response = client.UploadString(url, "POST", dataString);
                    break;
                case Methods.Put:
                    response = client.UploadString(url, "PUT", dataString);
                    break;
                case Methods.Delete:
                    response = client.UploadString(url, "DELETE", dataString);
                    break;
                default:
                    break;
            }
            var ret = JsonConvert.DeserializeObject<T>(response);
            return ret;
        }

        private enum Methods
        {
            Get,
            Post,
            Put,
            Delete
        }

    }
}
