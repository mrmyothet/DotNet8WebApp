namespace MyoThetDotNetCore.Expenses.Models;

public class ExpenseModel
{
    public int ExpenseId { get; set; }

    public string Title { get; set; }

    public decimal MoneySpent { get; set; }

    public DateTime SpentDate { get; set; }

    public string Category { get; set; }

    public bool IsDeleted { get; set; }
}
