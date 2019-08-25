using System.Collections;
using System.Collections.Generic;
using System;

namespace DigitoyAssignment
{
    /// <summary>
    /// Oyun yoneticisi; oyunu baslatan, dizaynlayan , oyunculari yaratan ve kazanani belirleyen sinif.
    /// </summary>
    public class MyGameManager
    {
        private DeckBuilder deckBuilder = new DeckBuilder(); 

        //Design degiskenleri
        private int numberOfPlayers = 4;
        private int numberOfPiecesPerPlayer = 14; //İlk oyuncuya ise 15 adet dagitiliyor. 
        private int defaultFakeOkeyDesignationNumber = 52;

        private List<PlayerHand> playerHands = new List<PlayerHand>(); //Oyuncuları temsil eden elleri duzenleyen sinif. Sinif icerisinde daha detayli anlatilmistir. 
        private List<int> currentCreatedDeck; //Oyuncuya dagitilan deste. 

        private int currentOkeyPieceNumber = 0;
        private int gosterge = 0;

        public void StartGame()
        {
            currentCreatedDeck = deckBuilder.CreateShuffledGameDeck(); //Once deckimizi hazirliyoruz. 
            ChooseOkeyAndGosterge(); //Okey belirliyoruz. 
            DealPiecesToPlayers(); //Oyunculara ellerini dagitiyoruz. 
            ShowWinnerAndPoints(); //Kazanma sansi en yuksek olani belirliyoruz. 
        }

        /// <summary>
        /// Once gosterge sonrasinda da okey tasi belirleniyor
        /// </summary>
        private void ChooseOkeyAndGosterge() //Burada okeyi belirleyip sahte okeye atıyoruz. 
        {
            
            var Random = new Random();//Gosterge belirlerken jokeri secmemek adına bu sekilde ilerliyoruz. 
            gosterge = Random.Next(0, defaultFakeOkeyDesignationNumber);
            //Gosterge tasını en sona atttık cekilmemesi icin.
            currentCreatedDeck.Remove(gosterge);
            currentCreatedDeck.Add(gosterge);

            AssignOkeyPiece(gosterge);
        }
        /// <summary>
        /// Okey tasi belirleniyor
        /// </summary>
        /// <param name="assignedGosterge"></param>
        private void AssignOkeyPiece(int assignedGosterge) 
        {
            //Gosterge mesela Sarı 13 olursa burada 12 olarak temsil edilecek ama okey Sarı 1 olmalı, dolayısıyla numara olarak bir arttirip 13 yapamayiz, 0 yaparak sari 1 yapmaliyiz gibi. 
            if (assignedGosterge == 12){currentOkeyPieceNumber = 0;}
            else if (assignedGosterge == 25){currentOkeyPieceNumber = 13;}
            else if (assignedGosterge == 38){currentOkeyPieceNumber = 26;}
            else if (assignedGosterge == 51){currentOkeyPieceNumber = 39;}
            else{currentOkeyPieceNumber = assignedGosterge + 1;}

            Console.WriteLine("Kurallar ve puanlama icin Readme.dosyasına bakınız");
            Console.WriteLine("Okey "+ currentOkeyPieceNumber);   
        }

        /// <summary>
        /// Oyuncu elleri yaratiliyor ve taslar dagitiliyor PlayerHand sinifi ile dizilmek uzere 
        /// </summary>
        private void DealPiecesToPlayers()
        { 
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (i == 0)
                {
                    //İlk oyuncuya bir adet fazla tas dagitiliyor. 
                    //Oyuncuya elini veriyoruz, okayin de ne oldugu belirtiyoruz buna gore dizmesi icin.
                    playerHands.Add(new PlayerHand(i, currentCreatedDeck.GetRange(0, numberOfPiecesPerPlayer + 1), currentOkeyPieceNumber));
                    currentCreatedDeck.RemoveRange(0, numberOfPiecesPerPlayer + 1);
                }
                else
                {
                    playerHands.Add(new PlayerHand(i, currentCreatedDeck.GetRange(0, numberOfPiecesPerPlayer), currentOkeyPieceNumber));
                    currentCreatedDeck.RemoveRange(0, numberOfPiecesPerPlayer);
                }
            }
        }
        /// <summary>
        /// Belirli bir algoritmaya gore hesaplanan puanlari oyunculardan cekiyor, gerekirse ellerinde kalan tas sayisini da hesaba katarak kazananı tahmin ediyor bu bilgiler isiginda.
        /// </summary>
        private void ShowWinnerAndPoints()
        {
            List<PlayerHand> winningPlayers = new List<PlayerHand>();
            int highestScore = 0;

            for (int i= playerHands.Count-1 ; 0 <=i; i--)
            {
                if (playerHands[i].GetPointOfPlayer() > highestScore)
                {
                    highestScore = playerHands[i].GetPointOfPlayer();

                    if(winningPlayers.Count == 0)
                    {
                        winningPlayers.Add(playerHands[i]);
                    }else
                    {
                        winningPlayers.Clear();
                        winningPlayers.Add(playerHands[i]);
                    }  
                }else if(playerHands[i].GetPointOfPlayer() == highestScore)
                {
                    if(winningPlayers.Count != 0)
                    {
                        winningPlayers.Add(playerHands[i]);
                    }
                }
            }
            //Eger aynı puana sahip birden fazla oyuncu varsa elinde en az karti bulunan oyuncu daha yuksek kazanma sansina sahip sayiliyor. 
            if(winningPlayers.Count > 1)
            {
                PlayerHand winingHandWithSamePointDiffRemainCard = null;
                int remainingCardInHand = 16;

                for(int i=0; i<winningPlayers.Count; i++)
                {
                    if (winningPlayers[i].GetRemainingPieceQuantity() < remainingCardInHand)
                    {
                        winingHandWithSamePointDiffRemainCard = winningPlayers[i];
                        remainingCardInHand = winingHandWithSamePointDiffRemainCard.GetRemainingPieceQuantity();
                    }
                }

                AnnounceWinner(winingHandWithSamePointDiffRemainCard);
            }else
            {
                AnnounceWinner(winningPlayers[0]);
            }
        }

        private void AnnounceWinner(PlayerHand winningPlayerHand)
        {
            Console.WriteLine("\nPlayer " + winningPlayerHand.GetNumberOfPlayer() + " has the highest chance to win" ); 
        }
    }
}
  
