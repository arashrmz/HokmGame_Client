using System;

namespace HokmGame.Core.Hokm.Info
{
    public class TeamInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IPlayerInfo Player1 { get; set; }
        public IPlayerInfo Player2 { get; set; }
    }
}