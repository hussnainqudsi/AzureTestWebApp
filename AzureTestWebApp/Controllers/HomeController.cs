using AzureTestWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureTestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly string url = "http://localhost:7071/api/TestingNewAzureFunction";
        //private readonly string url = "https://hqtest.azurewebsites.net/api/HQTESTFunction1";
        private readonly string url = "https://testingazure096.azurewebsites.net/TestingNewAzureFunction";
        private readonly ApplicationDbContext dbContext;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> CallingAzureApp()
        {
            WebRequest webRequest;
            webRequest = WebRequest.Create(url);
            webRequest.Method = "get";
            webRequest.ContentType = "application/json;charset=UTF-8";
            WebResponse webResponse = await webRequest.GetResponseAsync();
            Stream stream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            reader.Close();
            

            return Content(result);
        }
        public async Task<IActionResult> PostCallingAzureApp()
        {
            string result = "";
            try
            {
                var a = dbContext.Student.FirstOrDefault();
                //Student st = new Student()
                //{
                //    Class = "16",
                //    Name = "Muhammad Hussnain Qudsi",
                //    RollNumber = "F16-BSCS-079"
                //};

                string name = "Hussnain";
                var json = JsonConvert.SerializeObject(name);
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Method = "post";
                webRequest.ContentType = "application/json";
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                webRequest.ContentLength = bytes.Length;

                Stream stream = await webRequest.GetRequestStreamAsync();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();

                WebResponse webResponse = await webRequest.GetResponseAsync();
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                result = streamReader.ReadToEnd();
                streamReader.Close();
                var aaa = JsonConvert.DeserializeObject<Student>(result);
            }
            catch (Exception ex)
            {

                result = "Exception";
            }


            return Content(result);
        }
        public async Task<string> ConsumingApi()
        {
            string url = "http://localhost:7071/api/TestingNewAzureFunction";
            string result = "";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url))
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
            return result;
        }
        public async Task<string> PostAPI()
        {
            string result = string.Empty;
            string url = "http://localhost:7071/api/TestingNewAzureFunction";
            Student data = new Student()
            {
                FirstName = "16",
                LastName = "F16-BSBS-078",
                Email = "Hussnain Qudsi"
            };
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var res = await httpClient.PostAsync(url, content))
                {
                    result = await res.Content.ReadAsStringAsync();
                    Student dataRes = JsonConvert.DeserializeObject<Student>(result);
                }
            }
            //WebRequest webRequest = WebRequest.Create(url);
            //webRequest.Method = "post";
            //webRequest.ContentType = "application/json;charset=UTF-8";
            //var data = JsonConvert.SerializeObject(st);
            //var bytes = Encoding.UTF8.GetBytes(data);
            //webRequest.ContentLength = bytes.Length;

            //Stream stream = await webRequest.GetRequestStreamAsync();
            //stream.Write(bytes, 0, bytes.Length);
            //stream.Close();

            //var response = await webRequest.GetResponseAsync();
            //var dataa = response.GetResponseStream();

            //var streamReader = new StreamReader(dataa);
            //var resut = await streamReader.ReadToEndAsync();
            return result;
        }
    }
}
