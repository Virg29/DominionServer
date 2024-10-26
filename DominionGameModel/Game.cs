﻿using GameModel.Infrastructure;
using System.Diagnostics;

namespace GameModel
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<IPlayer> Players { get; set; } = new();

        public Kingdom Kingdom { get; set; }

        public IPlayer CurrentPlayer { get; private set; }

        public int Turn { get; private set; }
        public List<LogEntry> Logs { get; private set; } = new();

        public Game(List<IPlayer> players, Kingdom kingdom)
        {
            Id = Guid.NewGuid();
            Kingdom = kingdom;


            foreach (var player in players)
            {
                Players.Add(player);
                player.State.SetDefaultState();
            }

            Players.Shuffle();
        }

        public async Task StartGame()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Turn = 0;
            int playerTurnCounter = 0;
            GameEndType? gameEndType = null;
            while (gameEndType == null)
            {
                if (playerTurnCounter % Players.Count == 0)
                {
                    Turn++;
                }

                try
                {
                    
                    CurrentPlayer = Players[playerTurnCounter % Players.Count];
                    await CurrentPlayer.PlayTurnAsync(this);

                    CurrentPlayer.State.EndTurn();

                }
                catch (Exception e)
                {
                    foreach (var log in Logs)
                    {
                        Console.WriteLine($"{log.Turn} {log.PlayerName} {log.MessageType} {log.Args}");
                    }
                    Console.WriteLine(e);
                }

                gameEndType = Kingdom.IsGameOver();
                playerTurnCounter++;

            }

            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

            
            EndGame(gameEndType.Value);
        }

        private void EndGame(GameEndType gameEndType)
        {
            var gameResult = GetGameResult(gameEndType);
            Console.WriteLine($"{gameEndType} Winner: {gameResult.WinnerName}");
            foreach (var player in Players)
            {
                player.GameEnded(gameResult);
            }
        }

        public void Dispose()
        {
            foreach (var player in Players)
            {
                player.GameEnded(GetGameResult(GameEndType.Error));
            }
        }

        private GameEndDto GetGameResult(GameEndType gameEndType)
        {
            var gameEndDto = new GameEndDto();
            gameEndDto.GameEndType = gameEndType;
            gameEndDto.Turn = Turn;
            gameEndDto.Players = Players
                .OrderBy(p => p.State.VictoryPoints)
                .Select((p, i) => new PlayerVictoryDto()
                {
                    Name = p.Name,
                    Place = i + 1,
                    VictoryPoints = p.State.VictoryPoints
                })
                .ToList();
            gameEndDto.WinnerName = Players[0].Name;

            return gameEndDto;
        }

        public void AddLog(IPlayer player, BaseMessage message)
        {
            Logs.Add(new LogEntry()
            {
                Turn = Turn,
                PlayerName = player.Name,
                MessageType = message is BuyMessage ? MessageType.Buy : MessageType.Play,
                Args = message.Args
            });
        }
    }
}
