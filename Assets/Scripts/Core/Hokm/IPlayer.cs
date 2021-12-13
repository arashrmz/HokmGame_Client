using System.Collections.Generic;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm.Info;
using HokmGame.Core.Hokm.Score;

namespace HokmGame.Core.Hokm
{
    public interface IPlayer : IPlayerInfo
    {
        Task ReceiveHandAsync(IEnumerable<Card> cards);
        Task<Suit> CallTrumpSuitAsync();
        Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit);
        Task<string> InformTrickOutcomeAsync(TrickOutcome outcome);
        Task<string> BanterAsync();
        Task NewGameAsync(MatchScore currentMatchScore, PlayerPosition caller);
        Task<string> GameFinished(GameOutcome outcome, GameScore score);
        Task NewMatchAsync(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition yourPosition);
        Task MatchFinished(MatchScore score);
    }

    public static class IPlayerExtensions
    {
        public static IPlayerInfo ToPlayerInfo(this IPlayer player)
        {
            return new PlayerInfo() { Id = player.Id, Name = player.Name };
        }
    }
}
