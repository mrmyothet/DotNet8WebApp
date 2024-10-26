using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyoThetDotNetCore.Expenses.Models;
using Newtonsoft.Json;

namespace MyoThetDotNetCore.Expenses.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                SqlConnection connection = new(_configuration.GetConnectionString("DbConnection"));
                await connection.OpenAsync();

                string query =
                    @"SELECT ExpenseId, Title, MoneySpent, SpentDate, Category, IsDeleted
                      FROM Tbl_Expense
                      WHERE IsDeleted=@IsDeleted";

                List<SqlParameter> parameters = new() { new("@IsDeleted", false) };

                SqlCommand command = new(query, connection);
                command.Parameters.AddRange(parameters.ToArray());

                SqlDataAdapter adapter = new(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                await connection.CloseAsync();

                string jsonStr = JsonConvert.SerializeObject(dt);
                var lst = JsonConvert.DeserializeObject<List<ExpenseModel>>(jsonStr);

                return View(lst);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}
