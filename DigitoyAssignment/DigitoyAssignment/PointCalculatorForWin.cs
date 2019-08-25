using System;
using System.Collections.Generic;

namespace DigitoyAssignment
{
    /// <summary>
    /// Kazanma ihtimali en yuksek olani belirleyebilme amcali yaratilmis bir puan sistemi sinifi
    /// Her seri veya grup yapildiginda ya da cifte gidilmesi gerekip gerekmedigini hesaplarken bu sinif fonk ve degiskenleri araciligiyla oyuncu ellerindeki toplam puan hesaplaniyor. 
    /// </summary>
    public class PointCalculatorForWin
    {
        private int pointsForDouble = 2; //Bir cift icin verilen puan, oyuncu cifte gitmeli mi gitmemeli mi kismi hesaplanirken ve sonrasinda kullaniliyor. 
        private int pointsForTwoPieceGroupOrSeries = 1; //En yuksek sansi olani hesaplayabilmemiz icin perleri de yani ikili seri ve gruplari da puana tabi tutuyoruz. 
        private int pointsForOkeyPiece = 1; //Hic bir per veya seri ile gruplayamadigimiz okeyimizin getirdigi puandir. 

        public int PoitsForGroupsAndSeries(List<int> listToCalculatePointsFor)
        {
            if (listToCalculatePointsFor.Count == 2) //İkili puanı 
            {
                return pointsForTwoPieceGroupOrSeries;
            }
            else
            {
                return listToCalculatePointsFor.Count; //Grup veya seri uzunlugu kadar puan 
            }

        }

        public int PointsForDoubles(List<int> listToCalculatePointsFor)
        {
            return (listToCalculatePointsFor.Count/2) * pointsForDouble; //Cift puani 
        }

        public int PointsForHavingTheOkeyPiece()
        {
            return pointsForOkeyPiece; //PlayerHand icerisinde per varsa zaten okey bu peri uclemek icin kullanılıyor. Eger kullanilmamissa da bu okey bir per yapmak veya bir seri veya gruba katılmak icin kullanılır. Her kosulda zaten elde edilen puan bir fazlalascagi icin bu sekilde ilerlenmistir. 
        }
    }
}
