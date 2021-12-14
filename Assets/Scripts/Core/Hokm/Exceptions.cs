using System;

namespace HokmGame.Core.Hokm
{
    public class InvalidPlayException : Exception
    {
        public InvalidPlayException() : this("Invalid card play.")
        {
        }


        public InvalidPlayException(string error) : base(error)
        {

        }
    }
}