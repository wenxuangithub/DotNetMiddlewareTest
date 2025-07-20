using Application.DTOs.Expenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExpenseService
    {
        Task<Guid> CreateExpenseAsync(Guid userId, CreateExpenseRequest request);
        Task<IEnumerable<ExpenseResponse>> GetUserExpensesAsync(Guid userId);
        Task<ExpenseResponse?> GetExpenseByIdAsync(Guid userId, Guid expenseId);
        Task<bool> UpdateExpenseAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request);
        Task<bool> DeleteExpenseAsync(Guid userId, Guid expenseId);
    }
}
