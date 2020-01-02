using ASPNetCoreMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ASPNetCoreMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // ToDo: Look into the ITask issue commented in the IndexAsync method
        // ToDo: Introduce a Processor class (based on an Interface) and move all of the logic that calls the ASPNetCoreWebApi to that class; will allow us to write unit tests for these Controller methods
        // ToDo: Write custom exception type to override to StackTrace.ToString() to prevent stack traces from leaking in non-Development scenarios
        // ToDo: Add logging statements in the public methods

        private readonly ILogger<HomeController> _logger;
        private readonly string _apiUrlTasks;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _apiUrlTasks = "https://localhost:44309/Tasks";
        }

        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                // Wanted to use Models.ITask but the JsonConvert.DeserializeObject throws an exception: JsonSerializationException: Could not create an instance of type ASPNetCoreMVC.Models.ITask. Type is an interface or abstract class and cannot be instantiated
                // May be able to use one or more of the following articles
                // https://skrift.io/articles/archive/bulletproof-interface-deserialization-in-jsonnet/
                // https://stackoverflow.com/questions/2254872/using-json-net-converters-to-deserialize-properties
                // https://stackoverflow.com/questions/5780888/casting-interfaces-for-deserialization-in-json-net
                List<Models.Task> taskList = new List<Models.Task>();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(_apiUrlTasks))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        taskList = JsonConvert.DeserializeObject<List<Models.Task>>(apiResponse);
                    }
                }
                return View(taskList);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(IndexAsync)} threw an exception: {ex}");
            }

        }

        public ViewResult AddTask() => View();

        public async Task<IActionResult> HideCompletedTasks()
        {
            try
            {
                List<Models.Task> taskList = new List<Models.Task>();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(_apiUrlTasks))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        taskList = JsonConvert.DeserializeObject<List<Models.Task>>(apiResponse);
                    }
                }

                List<Models.Task> taskListToShow = new List<Models.Task>();
                foreach (Models.Task task in taskList)
                {
                    if (task.IsComplete == false)
                    {
                        taskListToShow.Add(task);
                    }
                }

                return View("Index", taskListToShow);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(HideCompletedTasks)} threw an exception: {ex}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(Models.Task task)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string serializedTask = JsonConvert.SerializeObject(task);
                    StringContent content = new StringContent(serializedTask, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(_apiUrlTasks, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AddTask)} threw an exception: {ex}");
            }
        }

        public async Task<IActionResult> UpdateTask(int id)
        {
            try
            {
                Models.Task task = new Models.Task();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{_apiUrlTasks}/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        task = JsonConvert.DeserializeObject<Models.Task>(apiResponse);
                    }
                }
                return View(task);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(UpdateTask)} threw an exception: {ex}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(Models.Task task)
        {
            try
            {
                Models.Task receivedTask = new Models.Task();
                using (var httpClient = new HttpClient())
                {
                    string serializedTask = JsonConvert.SerializeObject(task);
                    StringContent content = new StringContent(serializedTask, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync($"{_apiUrlTasks}/{task.Id}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(UpdateTask)} threw an exception: {ex}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                Models.Task task = new Models.Task();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{_apiUrlTasks}/{taskId}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        task = JsonConvert.DeserializeObject<Models.Task>(apiResponse);
                    }
                }

                string serializedTask = JsonConvert.SerializeObject(task);
                StringContent content = new StringContent(serializedTask, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_apiUrlTasks}/{taskId}")
                    {
                        Content = content
                    };

                    using (var response = await httpClient.SendAsync(requestMessage))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (apiResponse != "true")
                        {
                            throw new Exception($"Error trying to complete the task - \"{apiResponse}\"");
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(DeleteTask)} threw an exception: {ex}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CompleteTask(int taskId)
        {
            try
            {
                Models.Task task = new Models.Task();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{_apiUrlTasks}/{taskId}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        task = JsonConvert.DeserializeObject<Models.Task>(apiResponse);
                    }

                    task.IsComplete = true;

                    string serializedTask = JsonConvert.SerializeObject(task);
                    StringContent content = new StringContent(serializedTask, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync($"{_apiUrlTasks}/{task.Id}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (apiResponse != "true")
                        {
                            throw new Exception($"Error trying to complete the task - \"{apiResponse}\"");
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CompleteTask)} threw an exception: {ex}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
