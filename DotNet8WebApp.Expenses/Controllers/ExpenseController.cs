using DotNet8WebApp.Expenses.Extensions;
using Microsoft.AspNetCore.Mvc;
using MyoThetDotNetCore.Expenses.Models;
using Newtonsoft.Json;

namespace DotNet8WebApp.Expenses.Controllers;

public class ExpenseController : Controller
{
    private readonly HttpClient _httpClient;

    public ExpenseController(IHttpClientFactory _httpClientFactory)
    {
        _httpClient = _httpClientFactory.CreateClient("ExpenseClient");
    }

    public async Task<IActionResult> IndexAsync()
    {
        List<ExpenseModel> lst = new List<ExpenseModel>();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/expense");
            //response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                string jsonStr = await response.Content.ReadAsStringAsync();
                lst = jsonStr.ToObject<List<ExpenseModel>>();
            }

            return View(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
