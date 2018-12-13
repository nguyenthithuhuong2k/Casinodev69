﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WebCasino.Entities;

namespace WebCasino.Service.Abstract
{
	public interface ITransactionService
	{
		Task<IEnumerable<Transaction>> GetAllTransactionsTable();

        Task<IEnumerable<Transaction>> ListByContainingText(string searchText, int page = 1, int pageSize = 10);

		Task<int> TotalContainingText(string searchText);

		Task<int> Total();

		Task<IEnumerable<Transaction>> GetUserTransactions(string userId);

        Task<Transaction> RetrieveUserTransaction(string id);

        Task<IEnumerable<Transaction>> GetTransactionByType(string transactionTypeName);

		Task<Transaction> AddWinTransaction(string userId, double originalAmount, string description);

		Task<Transaction> AddStakeTransaction(string userId, double originalAmount, string description);

		Task<Transaction> AddWithdrawTransaction(string userId, string cardId, double originalAmount, string description);

		Task<Transaction> AddDepositTransaction(string userId, string cardId, double originalAmount, string description);

        Task<IEnumerable<Transaction>> RetrieveUserTransaction(string id, int page = 1, int pageSize = 10);

        Task<IEnumerable<Transaction>> RetrieveUserSearchTransaction(string searchText, string id, int page = 1, int pageSize = 10);

       

    }
}