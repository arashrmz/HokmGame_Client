using System;

namespace HokmGame.Core.Hokm.Info
{
    public class PlayerInfo : IPlayerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public interface IPlayerInfo
    {
        Guid Id { get; }
        string Name { get; }
    }
}