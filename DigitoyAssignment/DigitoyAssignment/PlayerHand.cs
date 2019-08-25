using System.Collections;
using System.Collections.Generic;
using System;

namespace DigitoyAssignment
{
    /// <summary>
    /// Her oyuncunun eline gelen desteyi taslara, okeye ve sahte okeye gore diziyor. Seriler, ciftler veya gruplar olusturarak farazi bir puanlama yapiyor. 
    /// </summary>
    public class PlayerHand //TODO monobehaviour yok 
    {
        
        private List<int> currentPlayerHand=new List<int>(); //Bunu fonk icerisinde hesaplamalarimiz sirasinda kullanacagiz. 
        private List<int> firstDealtPlayerHand = new List<int>(); //Bu kopya ise en son sonuclari ve oyuncu elini gostermek icin kullanilacak. 
        private List<int> okeyPieces = new List<int>(); //Elimize gelen okeyleri buraya atacagiz.
        private List<int> doublesList = new List<int>(); //Ciftleri sakladigimiz liste
        private List<int> mainGroupList = new List<int>(); //Yaptigimiz sarı iki, mavi iki gibi gruplari koydugumuz liste. 
        private List<int> mainSeriesList = new List<int>(); //Sırali ayni renk serileri koydugumuz liste. 

        private int playerNumber = 0;
        private int defaultFakeOkeyNumber = 52; 
        private int totalPointsEarned = 0; 
        private int minimumAmountOfPiecesForAConsecutiveSeries = 3; //default, sıralı 3 rakam gelmedigi surece puan vermeyecegiz , anlamlı saymayacagiz ilk denemede. Daha sonra ikili yani pere de bakılacak.  
        private int minimumAmountOfPiecesForSameNumberDiffColorGroups = 3; //default, 3 adet aynı rakam, farklı renk gelmedigi surece puan vermeyecegiz , anlamlı saymayacagiz ilk denemede. Daha sonra ikili yani pere de bakılacak.  
        private bool isPlayerGoingForDoubles = false; //Cifte gitme durumu 

        private PointCalculatorForWin myPointCalculator = new PointCalculatorForWin(); //Gerekli puan hesabi icin kullanilan sinif. 

        /// <summary>
        /// Bu constructor ile oyuncuya elini, okey bilgisini, ve sirasini veriyoruz. 
        /// </summary>
        /// <param name="numberOfPlayer"></param>
        /// <param name="pieces"></param>
        /// <param name="okeyNumber"></param>
        public PlayerHand(int numberOfPlayer, List<int> pieces, int okeyNumber)
        {
            currentPlayerHand = pieces; 
            firstDealtPlayerHand = pieces;
            playerNumber = numberOfPlayer +1;
            PlaceFirstHand(okeyNumber);
        }
        
        /// <summary>
        /// Taslar yerlestiriliyor. Okey varsa belirleniyor, sahte okey varsa olması gereken sayi ataniyor, arkasindan seri, gerup ve ciftlere ayriliyor. 
        /// </summary>
        /// <param name="okeyNumber"></param>
        public void PlaceFirstHand(int okeyNumber)
        {  
            WriteListsToScreen("Dagitilan el", currentPlayerHand);//Gerekli adımları, sonucu ve dogru calistigini gosterme amacli konsol komutları goreceksiniz. 

            for (int i = 0; i < currentPlayerHand.Count; i++)
            {
                if (currentPlayerHand[i] == okeyNumber) //Okey ve sahte okey cekersek ona gore elimizi dizecegiz
                {
                    okeyPieces.Add(okeyNumber); //Cıkarıyoruz dizmeye baslamadan once. Dizilirken gerekli gorulen perlerin yanına konulacak ya da dizim sonrası bir yer bulunacak. 
                    currentPlayerHand.RemoveAt(i);
                }
                else if (currentPlayerHand[i] == defaultFakeOkeyNumber) //52 rakamı sahte okay belirtiyor, cekilen okeyin numarasının alması saglaniyor asagida, oyuncun elindeki sahte okey artık bu numaraya ceviriliyor. 
                {
                    currentPlayerHand[i] = okeyNumber;
                }
            }
            SortAndSeperateSeries(minimumAmountOfPiecesForAConsecutiveSeries); //Seri olusturmak icin. 
            FindSameNumberDifferenColorGroups(minimumAmountOfPiecesForSameNumberDiffColorGroups);//Aynı numara farklı renk grup olusturmak icin. 
            ManageAndPointsForRemainingHand(); //Kalan taslar tekrar kontrol ediliyor, ikili seri ve grup yapiliyor, okeyle birlestiriliyor, cifte gitmenin kazanma olasiligina bakiliyor. 
        }

        /// <summary>
        /// Seri, yani ardisik gelen aynı renkli numaralari grupluyor. 
        /// </summary>
        /// <param name="minNumberOfPiecesForSeries"></param>
        private void SortAndSeperateSeries(int minNumberOfPiecesForSeries)
        {        
            currentPlayerHand.Sort();
            List<int> seriesList = new List<int>();
          
            for (int i = currentPlayerHand.Count - 1; 0 <= i; i--)
            {
            
                //Renkler numaralar ile tanımlı oldugu ve baska renklerle seri olmayacagindan asagidaki ifade bu durumu engelleniyor. 
                if (currentPlayerHand[i] == 0 || currentPlayerHand[i] == 13 || currentPlayerHand[i] == 26 || currentPlayerHand[i] == 39)
                {
                    continue;

                }else if(i > 0 && currentPlayerHand[i] == currentPlayerHand[i - 1]) //Ard arda aynı taslar gelirse aynı tastan aynı seride iki tane olmasnı engellemek icin. 
                {
                    continue;
                }
                else if (i > 0 && currentPlayerHand[i] - currentPlayerHand[i - 1] == 1) //Ardisik rakamlar gecici bir listeye ekleniyor. 
                {
                    if (seriesList.Count == 0)
                    {
                        seriesList.Add(currentPlayerHand[i]);

                    }
                    seriesList.Add(currentPlayerHand[i - 1]);
                }
                else
                {
                    if (seriesList.Count >= minNumberOfPiecesForSeries)
                    {
                      
                        if (seriesList.Count == 2 && okeyPieces.Count > 0)//Bu durum sadece en son elimizde kalan taslarla bir peri 3'lu seriye cevirme durumunda ise yarayacak. 2'li seri kontrolu icin bir daha cagrildiginda. 
                        {
                            seriesList.Add(okeyPieces[0]);
                            currentPlayerHand.Add(okeyPieces[0]);
                            okeyPieces.RemoveAt(0);
                            totalPointsEarned += myPointCalculator.PoitsForGroupsAndSeries(seriesList);
                        }
                        else
                        {
                            totalPointsEarned += myPointCalculator.PoitsForGroupsAndSeries(seriesList);
                        }
                    
                        for(int j=0; j<seriesList.Count; j++) //Bulduğumuz serileri daha sonra ekranda gostermek uzere ve bir daha kullanilip bozulmasını onleme amacli oyuncu elinden cikarmak icin global bir "seriler" listesine aliyoruz. 
                        {
                            mainSeriesList.Add(seriesList[j]);
                        }
                        
                    }
                    seriesList.Clear();
                }
            }
            RemoveGroupedPiecesFromUnsortedHand(currentPlayerHand, mainSeriesList); //Bir sonraki asama icin bu siralayip kullandigimiz taslarimizi ana tas listemizden cikariyoruz. 
        }
        /// <summary>
        /// Bosta kalan taslardan grup yani ayni numara farkli renklileri belirleme. 
        /// </summary>
        /// <param name="minNumberOfPiecesForGroups"></param>
        private void FindSameNumberDifferenColorGroups(int minNumberOfPiecesForGroups)
        {
            List<int> tempGroupList = new List<int>();
            currentPlayerHand.Sort();
           
            for (int i = currentPlayerHand.Count - 1; 0 <= i; i--)
            {
                tempGroupList.Clear();

                for (int j = 1; j < i; j++)
                {
                    //Aynı numaralı farlı renkli taslar arasinda 13 ve katlari kadar sayi farki olacagindan asagidaki sekilde aynı numaralari belirliyoruz ve sonuc olarak yine gecici ve global grup listelerine atiyoruz.
                    if (i > 0 && i - j >=0 && currentPlayerHand[i] != currentPlayerHand[i - 1] && (currentPlayerHand[i] - currentPlayerHand[i - j]) % 13 == 0)
                    {
                        if (mainGroupList != null && !mainGroupList.Contains(currentPlayerHand[i]) && !mainGroupList.Contains(currentPlayerHand[i-j]) && !tempGroupList.Contains(i-j))
                        {

                            if (tempGroupList.Count == 0)
                            {
                                tempGroupList.Add(currentPlayerHand[i]);
                            }
                            tempGroupList.Add(currentPlayerHand[i - j]); 
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (tempGroupList.Count >= minNumberOfPiecesForGroups)
                {
                    if (tempGroupList.Count == 2 && okeyPieces.Count > 0) //Serileri bulma kisimindaki ile ayni amac. 
                    {
                        tempGroupList.Add(okeyPieces[0]);
                        currentPlayerHand.Add(okeyPieces[0]);
                        okeyPieces.RemoveAt(0);
                        totalPointsEarned += myPointCalculator.PoitsForGroupsAndSeries(tempGroupList);
                    }
                    else
                    {
                        totalPointsEarned += myPointCalculator.PoitsForGroupsAndSeries(tempGroupList);
                    }
                    for (int y = 0; y < tempGroupList.Count; y++)
                    {
                        mainGroupList.Add(tempGroupList[y]); //Daha sonra oyuncuya gosterebilmek icin tum gruplamalar saklaniyor. Ve aynı zamanda yine elmizde bosta kalanlari gorebilmek adina gruplaniyor. 
                    }
                }
            }         
            RemoveGroupedPiecesFromUnsortedHand(currentPlayerHand, mainGroupList);//Bir sonraki asama icin bu gruplayip kullandigimiz taslarimizi ana tas listemizden cikariyoruz. 
        } 

        /// <summary>
        /// Bir listenin icerigini diger listeden cikarmak icin kullaniliyor. Dizilen taslarin bir daha kullanilmamasini onlemek icin ana tas havuzumuzdan cikarilmasina sebep oluyor. Ya da ekranda kalan taslari gorebilmek icin kullaniliyor. 
        /// </summary>
        /// <param name="listToBeExtractedFrom"></param>
        /// <param name="listToBeExtracted"></param>
        private void RemoveGroupedPiecesFromUnsortedHand(List<int> listToBeExtractedFrom, List<int> listToBeExtracted) 
        {
            for(int i=0; i< listToBeExtracted.Count; i++)
            {
                listToBeExtractedFrom.Remove(listToBeExtracted[i]);
            }
        }
        /// <summary>
        /// Bosta kalan per veya okeyleri bulur, onları gruplar veya seriller. aynı zamanda oyuncunun cifte gidip gitmemesi gerektigine bakar.
        /// </summary>
        private void ManageAndPointsForRemainingHand()
        {  
            SortAndSeperateSeries(2); //Uclu ve uzeri seriler yapıldıktan sonra elimizde kalanlara ilerisi icin veya okey ile birlestirmek icin perli seri kontrolü. 
            FindSameNumberDifferenColorGroups(2); //Uclu ve uzeri gruplar yapıldıktan sonra elimizde kalanlara ilerisi icin veya okey ile birlestirmek icin perli seri kontrolü. 
            CalculateIfPlayerShouldGoDoubles();//Eger elimzideki ciftler daha cok puan kazanditiyorsa cifte gitmesi durumundaki puani soyler. 
            CheckUnusedOkey(); //Kullanamadigimiz okayin puanlamasi icin. 
            PrepareListsForResultScreen();  //El icerik bilgisi ve puanlari getirir ekrana. 
        }

        private void CheckUnusedOkey()
        {
            if (okeyPieces.Count > 0)
            {
                for (int i = 0; i < okeyPieces.Count; i++)
                {
                    totalPointsEarned += myPointCalculator.PointsForHavingTheOkeyPiece(); //Okeyi hic bir perle gruplayamadıysak bile kendisi bir per olusturabilecegi icin puana katılıyor. 
                }
            }
        }
        /// <summary>
        /// Aldigimiz ilk eli, arkasindan olusturdugumuz seri ve gruplari gosterip, kalan taslari soyler ve puani belirtir. Cifte gidilme durumu oldugunda onu da ayrica belirtir.
        /// </summary>
        private void PrepareListsForResultScreen()
        {
            //Yarattigimiz seri ve gruplari gostermek ve elimize gelen ilk elden cikarip bu sayede kalan taslari gostermek. 
            RemoveGroupedPiecesFromUnsortedHand(firstDealtPlayerHand, mainSeriesList);
            RemoveGroupedPiecesFromUnsortedHand(firstDealtPlayerHand, mainGroupList);

            if (isPlayerGoingForDoubles)
            {
                RemoveGroupedPiecesFromUnsortedHand(firstDealtPlayerHand, doublesList);
                WriteListsToScreen("Cifte gidiyor ", doublesList);
                WriteListsToScreen("Elde Kalan", firstDealtPlayerHand);
                if (okeyPieces.Count > 0)
                {
                    WriteListsToScreen("Yerlestirilmemis okey tespit edildi, puanlamaya uygun sekilde eklendi ", okeyPieces);
                }
            }
            else
            {
                WriteListsToScreen("Sıralı", mainSeriesList);
                WriteListsToScreen("Gruplu", mainGroupList);
                WriteListsToScreen("Elde Kalan", firstDealtPlayerHand);
                if (okeyPieces.Count > 0)
                {
                    WriteListsToScreen("Yerlestirilmemis okey tespit edildi, puanlamaya uygun sekilde eklendi ", okeyPieces);
                }
            }
            Console.WriteLine("\n"+ "Player " + playerNumber + " scored " + totalPointsEarned + " points" + " has " + firstDealtPlayerHand.Count + " unsorted pieces");
        }

        private void WriteListsToScreen(string listName, List<int> listToShowOnScreen)
        {
            Console.WriteLine("\n" + listName);
            for(int i=0; i<listToShowOnScreen.Count; i++)
            {
                Console.Write(listToShowOnScreen[i] + "-");
            }
        }

        /// <summary>
        /// Elimizdeki ciftler ile kazanma sansimizin daha yuksek olup olmadigina bakiliyor. 
        /// </summary>
        private void CalculateIfPlayerShouldGoDoubles()
        {
            for (int i = 0; i < firstDealtPlayerHand.Count; i++)
            {
                FindDoubles(firstDealtPlayerHand[i]); //Ciftleri bulup bir listeye atıyoruz  
            }

            if (doublesList.Count >= 6) //Bu en az 3 adet ciftimiz var demek bu da bir avantaj olacagi icin bir puan verilmeli.Eger su an ki punimizdan yuksek bir puansa oyuncu cifte gitmeli. Eski puani silinmeli ve cifte gore puan verilmeli. 
            {
                int doublePoints = myPointCalculator.PointsForDoubles(doublesList);

                if (doublePoints > totalPointsEarned)
                {
                    totalPointsEarned = myPointCalculator.PointsForDoubles(doublesList);
                    isPlayerGoingForDoubles = true;
                }
            }
        }

        private void FindDoubles(int numberToCheck)
        {
            List<int> doubles = currentPlayerHand.FindAll(i => i == numberToCheck);
            if (doubles.Count > 1 && !doublesList.Contains(numberToCheck))
            {
                doublesList.AddRange(doubles);
            }
            else
            {
                doubles.Clear();
            }
        }
        /// <summary>
        /// Gets the total point of Player.
        /// </summary>
        /// <returns></returns>
        public int GetPointOfPlayer()
        {
            return totalPointsEarned;
        }

        /// <summary>
        /// How many pieces left after series , groups and doubles are removed.
        /// </summary>
        /// <returns></returns>
        public int GetRemainingPieceQuantity()
        {
            return currentPlayerHand.Count;
        }
        /// <summary>
        /// If it is Player "1" or Player "2" etc...
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfPlayer()
        {
            return playerNumber;
        }
    }
    
}
