namespace HokmGame.Core.Hokm
{
    [System.Serializable]
    public enum PlayerPosition
    {
        Team1Player1,
        Team2Player1,
        Team1Player2,
        Team2Player2
    }

    public static class PlayerPositions
    {
        public static readonly PlayerPosition[] All = new PlayerPosition[]{
            PlayerPosition.Team1Player1,
            PlayerPosition.Team2Player1,
            PlayerPosition.Team1Player2,
            PlayerPosition.Team2Player2
        };

        public static bool IsTeam1(this PlayerPosition position)
        {
            return position == PlayerPosition.Team1Player1 || position == PlayerPosition.Team1Player2;
        }

        //returns the position of teammate
        public static PlayerPosition GetTeammatePosition(this PlayerPosition position)
        {
            if (position.IsTeam1())
            {
                return position == PlayerPosition.Team1Player1 ? PlayerPosition.Team1Player2 : PlayerPosition.Team1Player1;
            }
            else
            {
                return position == PlayerPosition.Team2Player1 ? PlayerPosition.Team2Player2 : PlayerPosition.Team2Player1;
            }
        }
    }
}