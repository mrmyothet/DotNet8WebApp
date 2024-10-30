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
        private readonly string _dbConnectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConnectionString = _configuration.GetConnectionString("DbConnection")!;
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

        public IActionResult NewExpense()
        {
            return View();
        }

        public async Task<IActionResult> SaveExpense(ExpenseModel requestModel)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_dbConnectionString);
                await connection.OpenAsync();

                string query =
                    @"INSERT INTO [dbo].[Tbl_Expense]
           ([Title]
           ,[MoneySpent]
           ,[SpentDate]
           ,[Category]
           ,[IsDeleted])
     VALUES
           (@Title
           ,@MoneySpent
           ,@SpentDate
           ,@Category
           ,@IsDeleted)";

                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Title", requestModel.Title),
                    new SqlParameter("@MoneySpent", requestModel.MoneySpent),
                    new SqlParameter("@SpentDate", requestModel.SpentDate),
                    new SqlParameter("@Category", requestModel.Category),
                    new SqlParameter("@IsDeleted", requestModel.IsDeleted)
                };

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddRange(parameters.ToArray());
                int result = await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();

                if (result > 0)
                {
                    TempData["success"] = "Saving successful.";
                }

                if (result == 0)
                {
                    TempData["error"] = "Saving Failed.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Index");
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
