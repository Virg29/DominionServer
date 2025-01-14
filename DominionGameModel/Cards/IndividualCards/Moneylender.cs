﻿using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class MoneylenderCard : AbstractActionCard
{
    public override string Name { get; } = "Moneylender";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "+You may trash a Copper from your hand for +$3.";

    public override CardEnum CardTypeId { get; } = CardEnum.Moneylender;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.TrashFromHand(game.Kingdom, CardEnum.Copper);

        player.State.AdditionalMoney += 3;
    }

    public override bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        if (!player.State.HaveInHand(CardEnum.Copper))
        {
            return false;
        }
        return true;
    }
}
