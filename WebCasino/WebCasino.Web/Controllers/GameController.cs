﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasino.Service.Abstract;
using WebCasino.Web.Models.GameViewModels;
using WebCasino.Web.Utilities.Wrappers;

namespace WebCasino.Web.Controllers
{
    [Authorize(Roles = "Player")]
    public class GameController : Controller
    {
        private readonly IGameService gameService;
        private readonly ITransactionService transactionService;
        private readonly IUserWrapper userWrapper;

        public GameController(IGameService gameService, ITransactionService transactionService, IUserWrapper userWrapper)
        {
            this.gameService = gameService;
            this.transactionService = transactionService;
            this.userWrapper = userWrapper;
        }

        public IActionResult Index()
        {
            var board = this.gameService.GenerateBoard(4, 3);
            var model = this.gameService.GameResults(board);
            var dto = new GameViewModel()
            {
                Board = board,
                WinCoef = model.WinCoefficient
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Bet(GameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = this.userWrapper.GetUserId(HttpContext.User);
                await this.transactionService.AddStakeTransaction(userId, model.BetAmount, $"Stake at slot game {model.GameBoardRows} x {model.GameBoardCols}");
                var board = this.gameService.GenerateBoard(4, 3);
                var gameModel = this.gameService.GameResults(board);
                if(gameModel.WinCoefficient > 0)
                {
                    await this.transactionService.AddWinTransaction(userId, gameModel.WinCoefficient * model.BetAmount, $"Win at slot game {model.GameBoardRows} x {model.GameBoardCols}");
                }
                var dto = new GameViewModel()
                {
                    Board = board,
                    WinCoef = gameModel.WinCoefficient,
                    BetAmount = model.BetAmount
                };

                return PartialView("_GameBoardPartial", dto);

            }
            return View("index", "game");
        }

    }
}