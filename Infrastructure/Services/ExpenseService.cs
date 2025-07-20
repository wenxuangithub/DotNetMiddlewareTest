using Application.DTOs.Expenses;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateExpenseAsync(Guid userId, CreateExpenseRequest request)
    {
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = request.Amount,
            Category = request.Category,
            Description = request.Description,
            Date = request.Date
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense.Id;
    }

    public async Task<IEnumerable<ExpenseResponse>> GetUserExpensesAsync(Guid userId)
    {
        return await _context.Expenses
            .Where(e => e.UserId == userId)
            .Select(e => new ExpenseResponse
            {
                Id = e.Id,
                Amount = e.Amount,
                Category = e.Category,
                Description = e.Description,
                Date = e.Date
            }).ToListAsync();
    }

    public async Task<ExpenseResponse?> GetExpenseByIdAsync(Guid userId, Guid expenseId)
    {
        var e = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
        if (e == null) return null;

        return new ExpenseResponse
        {
            Id = e.Id,
            Amount = e.Amount,
            Category = e.Category,
            Description = e.Description,
            Date = e.Date
        };
    }

    public async Task<bool> UpdateExpenseAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
        if (expense == null) return false;

        expense.Amount = request.Amount;
        expense.Category = request.Category;
        expense.Description = request.Description;
        expense.Date = request.Date;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteExpenseAsync(Guid userId, Guid expenseId)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
        if (expense == null) return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }
}
