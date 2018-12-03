﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebCasino.DataContext;
using WebCasino.Service.Abstract;
using WebCasino.Service.DTO.Canvas;

namespace WebCasino.Service
{
	public class AdminDashboardService : IAdminDashboard
	{
		private readonly CasinoContext dbContext;

		public AdminDashboardService(CasinoContext dbContext)
		{
			this.dbContext = dbContext ?? throw new NullReferenceException();
		}

		public async Task<MonthsTransactionsModel> GetMonthsTransactions(DateTime timePeriod, string transactionType, int monthCount)
		{
			var dbQuery = await this.dbContext.Transactions
				.Include(tt => tt.TransactionType)
				.Where(t => t.TransactionType.Name == transactionType)
				.ToListAsync();

			var resultModel = new MonthsTransactionsModel();

			//CHECK FOR MONTH NUMBER !!
			for (int i = timePeriod.Month - monthCount; i <= timePeriod.Month; i++)
			{
				var monthly = new MonthVallueModel();

				var valueFilter = dbQuery
				.Where(d => d.CreatedOn.Value.Month == i).Count();

				monthly.MonthValue = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3).ToUpper();
				monthly.Value = valueFilter;

				resultModel.ValuesByMonth.Add(monthly);
			}

			return resultModel;
		}

		public async Task<int> GetTotaTransactionsByTypeCount(string transactionType)
		{
			var totalWins = await this.dbContext
				.Transactions.Include(tt => tt.TransactionType)
				.Where(t => t.TransactionType.Name == transactionType)
				.CountAsync();

			return totalWins;
		}

		public async Task<CyrrencyDaylyWin> GetTransactionsCurrencyDaylyWins(int day)
		{
			var allTransactionsQuery = await this.dbContext
				.Transactions
				.Include(tt => tt.TransactionType)
				.Include(u => u.User.Wallet.Currency)
				.ToListAsync();

			var daylyTotalUsd = allTransactionsQuery
				.Where(tt => tt.TransactionType.Name == "Win")
				.Where(td => td.CreatedOn.Value.Month == DateTime.Now.Month
							&& td.CreatedOn.Value.Day == day)
				.Select(t => t.NormalisedAmount).Sum();

			var daylyWinsBGN = allTransactionsQuery
				.Where(tt => tt.TransactionType.Name == "Win")
				.Where(c => c.User.Wallet.Currency.Name == "BGN")
				.Where(td => td.CreatedOn.Value.Month == DateTime.Now.Month
							&& td.CreatedOn.Value.Day == day)
				.Select(t => t.OriginalAmount).Sum();

			var daylyWinsUSD = allTransactionsQuery
				.Where(tt => tt.TransactionType.Name == "Win")
				.Where(c => c.User.Wallet.Currency.Name == "USD")
				.Where(td => td.CreatedOn.Value.Month == DateTime.Now.Month
							&& td.CreatedOn.Value.Day == DateTime.Now.Day)
				.Select(t => t.OriginalAmount).Sum();

			var daylyWinsGBP = allTransactionsQuery
				.Where(tt => tt.TransactionType.Name == "Win")
				.Where(c => c.User.Wallet.Currency.Name == "GBP")
				.Where(td => td.CreatedOn.Value.Month == DateTime.Now.Month
							&& td.CreatedOn.Value.Day == day)
				.Select(t => t.OriginalAmount).Sum();

			var daylyWinsEUR = allTransactionsQuery
				.Where(tt => tt.TransactionType.Name == "Win")
				.Where(c => c.User.Wallet.Currency.Name == "EUR")
				.Where(td => td.CreatedOn.Value.Month == DateTime.Now.Month
							&& td.CreatedOn.Value.Day == day)
				.Select(t => t.OriginalAmount).Sum();

			var resultModel = new CyrrencyDaylyWin()
			{
				DaylyTotalUSD = daylyTotalUsd,
				DaylyWinsBGN = daylyWinsBGN,
				DaylyWinsEUR = daylyWinsEUR,
				DaylyWinsGBP = daylyWinsGBP,
				DaylyWinsUSD = daylyWinsUSD
			};

			return resultModel;
		}
	}
}