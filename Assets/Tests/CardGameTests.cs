using System.Collections;
using System.Collections.Generic;
using HokmGame.Core.CardGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CardGameTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void Card_ToString_A_Of_Club()
    {
        Card card = new Card(Suit.Club, Rank.Ace);
        Assert.AreEqual(card.ToString(), "A of Club");
    }

    [Test]

    public void Card_ToString_Test()
    {

        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j < 14; j++)
            {
                Card card = new Card((Suit)i, (Rank)j);
                try
                {

                    Assert.AreEqual(card.ToString(), card.GetHashCode());
                }
                catch (System.Exception e)
                {
                    Assert.Fail(e.Message + $"\tRank: {card.Rank}\tSuit: {card.Suit}");
                    throw;
                }

            }
        }

    }

}
