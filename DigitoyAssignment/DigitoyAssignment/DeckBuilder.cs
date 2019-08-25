
using System.Collections.Generic;
using System;

namespace DigitoyAssignment
{
    /// <summary>
    /// 106 taslik destemizi yaratiyor ve karistiriyor. 
    /// </summary>
    public class DeckBuilder
    {
        private List<int> mainDeckBeforeDeal = new List<int>();

        private int numberOfPiecesInDeck = 53; 
        private int numberOfShuffleDeckRepeat = 100;

        public List<int> CreateShuffledGameDeck()
        {
            //Burada yapmamız gereken şey 106 adet taş yaratmak, random bir şekilde dizmek, 2 x53'lük deck yaratmak şeklinde. 13x4 taş ve 1 adet sahte okay şeklinde. Sahte okey ise 52 olacak. 
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < numberOfPiecesInDeck; j++)
                {
                    mainDeckBeforeDeal.Add(j);
                }
            }
            ShuffleWholeDeck();
            return mainDeckBeforeDeal;
        }
        //Bu sekilde karıstırmayı tercih ettim gercege yakın olsun diye. 
        private void ShuffleWholeDeck()
        {
            int x = 0;
            int y = 0;
            int firstRandomNum = 0;
            int secondRandomNum = 0;
            var random = new Random();

            for (int i = 0; i < numberOfShuffleDeckRepeat; i++)
            {
                firstRandomNum = random.Next(0, mainDeckBeforeDeal.Count);
                x = mainDeckBeforeDeal[firstRandomNum];
                secondRandomNum = random.Next(0, mainDeckBeforeDeal.Count);
                y = mainDeckBeforeDeal[secondRandomNum];
                mainDeckBeforeDeal[secondRandomNum] = x;
                mainDeckBeforeDeal[firstRandomNum] = y;
            }

        }
    }
}
