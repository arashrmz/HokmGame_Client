using System;
using HokmGame.Core.Hokm.Info;

namespace HokmGame.Core.Hokm
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IPlayer Player1 { get; set; }
        public IPlayer Player2 { get; set; }

    }
    public static class TeamExtenstions
    {
        public static TeamInfo ToTeamInfo(this Team team)
        {
            return new TeamInfo
            {
                Id = team.Id,
                Name = team.Name,
                Player1 = team.Player1.ToPlayerInfo(),
                Player2 = team.Player2.ToPlayerInfo()
            };
        }
    }
}