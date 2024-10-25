﻿using GameModel;

namespace Dominion.SocketIoServer.Dtos
{
    public class GameStateDto
    {
        public Guid GameId { get; set; }

        public List<PlayerDto> Players { get; set; } = new();
        public Kingdom Kingdom { get; set; }

        public string PlayerId { get; set; }
        public int Turn { get; set; }
        public CurrentPlayerStateDto PlayerState { get; set; }

        public GameStateDto() { }

        public GameStateDto(Game game)
        {
            GameId = game.Id;
            Players = game.Players.Select(p => new PlayerDto(p)).ToList();
            PlayerId = game.CurrentPlayer.Id;
            PlayerState = new CurrentPlayerStateDto(game.CurrentPlayer.State);
            Kingdom = game.Kingdom;
            Turn = game.Turn;
        }
    }
}
