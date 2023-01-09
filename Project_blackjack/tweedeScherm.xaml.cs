using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Threading;


namespace Project_blackjack
{

   
    public partial class tweedeScherm : Window
    {


        // private List<string> kaarten = new List<string>();


        //Randoms
        Random RdmKaarten = new Random();
        Random Rdmgetal = new Random();




        //stringbuilders
        //StringBuilder sb = new StringBuilder();
        //StringBuilder sbBank = new StringBuilder();
        StringBuilder puntenS = new StringBuilder();
        StringBuilder sbHistoriek = new StringBuilder();


        int spelerPunten = 0;
        int bankPunten = 0;

        int ronde = 0;

        public string spelerNaam;

        private double inzet;
        private double kapitaal;

        

        private int seconden =0;
        private int wachtseconden = 0;
        
        private int xAs = 0;
        private int xAsBank = 0;
        bool dubbeldown;
        private int aasTeller;



        DispatcherTimer timer;
        DispatcherTimer wachtTimer;

        Image back;


        List<string> historiekLijst = new List<string>();

        List<string> gebruikteKaarten = new List<string>();

        private string[,] kaarten = new string[,] { 
            

            { "schoppen 2", "schoppen 3", "schoppen 4", "schoppen 5", "schoppen 6", "schoppen 7", "schoppen 8", "schoppen 9", "schoppen 10", "schoppen Boer", "schoppen Vrouw", "schoppen Heer", "schoppen Aas" }, 
            { "harten 2", "harten 3", "harten 4", "harten 5", "harten 6", "harten 7", "harten 8","harten 9","harten 10", "harten Vrouw", "harten Heer", "harten Boer", "harten Aas"},
            { "klaveren 2", "klaveren 3", "klaveren 4","klaveren 5", "klaveren 6", "klaveren 7", "klaveren 8","klaveren 9", "klaveren 10", "klaveren Boer", "klaveren Vrouw", "klaveren Heer", "klaveren Aas"}, 
            { "ruiten 2","ruiten 3","ruiten 4","ruiten 5","ruiten 6","ruiten 7", "ruiten 8","ruiten 9","ruiten 10", "ruiten Boer", "ruiten Vrouw", "ruiten Heer", "ruiten Aas" } 


    };


        
        public tweedeScherm()

            
        {
            InitializeComponent();



            double.TryParse(InzetTxtBx.Text, out inzet);

            double.TryParse(LblKapitaal.Text, out kapitaal);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;


            //Timer voor datum
            DispatcherTimer secondTimer = new DispatcherTimer();
            secondTimer.Tick += SecondTimer_Tick;
            secondTimer.Interval = new TimeSpan(0,0, 1);
            secondTimer.Start();


            //Timer om aantal seconden te wachten tijdens de kaarten worden geschud
            wachtTimer = new DispatcherTimer();
            wachtTimer.Interval = TimeSpan.FromSeconds(1);
            wachtTimer.Tick += WachtTimer_Tick;
            


            lblDate.Content = DateTime.Now.ToString();



        }

        /// <summary>
        /// Ik maak hier een methode om de int wachtseconden te verhogen met 1 
        /// en wanneer de seconden gelijk zijn aan 3 stopt de timer en wordt de label onzichtbaar
        /// </summary>

        private void WachtTimer_Tick(object? sender, EventArgs e)
        {
            wachtseconden++;

            
            if (wachtseconden == 3)
            {
                wachtTimer.Stop();

                
                kaartenSchudden.Visibility = Visibility.Hidden;
            }
            
        }







        private void SecondTimer_Tick(object? sender, EventArgs e)
        {
            lblDate.Content = DateTime.Now.ToString("HH: mm: ss");
        }



        /// <summary>
        /// Er wordt na elke seconden een kaart gedeeld en wanneer de seconden aan 10 gelijk wordt 
        /// start er een nieuwe ronde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            seconden++;

            

            if (seconden == 1)
            {
                GeefKaart(true);
            } else if (seconden == 2)
            {
                GeefKaart(true);
            }
            else if (seconden == 3)
            {
                GeefKaart(false);
                BtnHit.IsEnabled = true;
                BtnStand.IsEnabled = true;
                BtnDoubleDown.IsEnabled = true;
                
            } else if(seconden == 4)
            {
                back = new Image();
                back.Stretch = Stretch.UniformToFill;
                back.Width = 70;

                back.Source = new BitmapImage(new Uri(@"/Kaarten/back.jpg", UriKind.RelativeOrAbsolute));

                canvasBank.Children.Add(back);
                back.Margin = new Thickness(0 + 20, 0, 0, 0);
                
                back.Width = 130;
                timer.Stop();



            }
            else
            {


                BtnStand.IsEnabled = false;
                BtnHit.IsEnabled = false;



                gewonnenLabel.Visibility = Visibility.Visible;

                
                


                if (seconden == 10)
                {
                    nieuweRonde();


                    if (kapitaal <= 0)
                    {
                        blut();
                    }


                }



            }







        }

        /// <summary>
        /// Punten worden gelijk aan 0 en kaarte  worden geschud
        /// </summary>
        private void nieuweRonde()
        {
            timer.Stop();

            schudden(kaarten);


            gewonnenLabel.Visibility = Visibility.Hidden;

            canvasBank.Children.Clear();
            canvasSpeler.Children.Clear();
            BtnDeel.Visibility = Visibility.Visible;
            BtnDeel.IsEnabled = false;

            BtnStand.Visibility = Visibility.Hidden;
            BtnHit.Visibility = Visibility.Hidden;
            BtnDoubleDown.Visibility = Visibility.Hidden;

            InzetTxtBx.IsEnabled = true;

            spelerPunten = 0;
            bankPunten = 0;
            aasTeller = 0;

            PuntenSpeler.Text = "0";
            PuntenBank.Text = "0";

            seconden = 0;
            wachtseconden = 0;

            xAs = 0;
            xAsBank = 0;

            gebruikteKaarten.Clear();
        }


        private void nieuweSpel_Click(object sender, RoutedEventArgs e)
        {
            
            kapitaal = 100;
            LblKapitaal.Text = "100";

            InzetTxtBx.Text = "";
            inzet = 0;

            canvasBank.Children.Clear();
            canvasSpeler.Children.Clear();

            BtnDeel.Visibility = Visibility.Visible;
            nieuweSpel.Visibility = Visibility.Hidden;
            BtnDoubleDown.Visibility = Visibility.Hidden;
            BtnDeel.IsEnabled = false;
            BtnPlaats.IsEnabled = true;

            InzetTxtBx.IsEnabled = true;

            spelerPunten = 0;
            bankPunten = 0;
            aasTeller = 0;

            PuntenSpeler.Text = "0";
            PuntenBank.Text = "0";

            seconden = 0;
            wachtseconden=0;

            xAs = 0;
            xAsBank = 0;

            gebruikteKaarten.Clear();

        }

        private void BtnDeel_Click(object sender, RoutedEventArgs e)
        {
            {
                


                BackgroundImage.Source = new BitmapImage(new Uri(@"/images/table.jpg", UriKind.RelativeOrAbsolute));
                sNaam.Text = spelerNaam.ToString();




                BtnStand.Visibility = Visibility.Visible; 
                BtnStand.IsEnabled = false; 


                BtnDeel.Visibility = Visibility.Hidden;
                BtnHit.Visibility = Visibility.Visible;
                BtnHit.IsEnabled = false;

                BtnDoubleDown.Visibility = Visibility.Visible;
                BtnDoubleDown.IsEnabled = false;
                //LabelSpeler.Visibility = Visibility.Visible;
                //LabelBank.Visibility = Visibility.Visible;


                sNaam.Visibility = Visibility.Visible;
                bNaam.Visibility = Visibility.Visible;

                PuntenSpeler.Visibility = Visibility.Visible;
                PuntenBank.Visibility = Visibility.Visible;




                LblKapitaal.Text = $"{(kapitaal-inzet).ToString()}";
                InzetTxtBx.IsEnabled = false;
                


                

                timer.Start();
                


                
                
                




 

            }
        }
        

        private int checkAas(bool persoon)
        {

            aasTeller++;

            if (persoon)
            {


                



                if (spelerPunten + 11 > 21)
                {
                    spelerPunten += 1;
                }
                else
                {
                    spelerPunten += 11;
                }
                return spelerPunten;

            }

            else
            {

                if (bankPunten + 11 > 21)
                {
                    bankPunten += 1;
                }
                else
                {
                    bankPunten += 11;
                }
                return bankPunten;

            }

            


        }


        private void BtnHit_Click(object sender, RoutedEventArgs e)

        {

            BtnDoubleDown.IsEnabled = false;
           

            GeefKaart(true);


           


            if (spelerPunten >21)
            {


                gewonnen(false);
                timer.Start();

     
            }
    


        }


        

        
        private void BtnStand_Click(object sender, RoutedEventArgs e)
            {

            aasTeller = 0;

                timer.Start();



                BtnStand.IsEnabled = false;
                BtnDoubleDown.IsEnabled = false;
                BtnHit.IsEnabled = false;


                

                while (bankPunten < 17)
                {

                    GeefKaart(false);

                }



                if (bankPunten > 21)
                {



                    gewonnen(true);



                }



                else if (spelerPunten == bankPunten)
                {
                    gewonnenLabel.Content = "Push ";
                    gewonnenLabel.Visibility = Visibility.Visible;

                    LblKapitaal.Text = kapitaal.ToString();

                    //sbHistoriek.Append($"{inzet} - {spelerPunten} / {bankPunten}\n ");
                    lblHistoriek.Content = $"{ronde}: {inzet} - {spelerPunten} / {bankPunten} ";
                }



                else
                {

                    if (spelerPunten > bankPunten)
                    {


                        gewonnen(true);

                    }


                    else
                    {


                        gewonnen(false);

                        

                    }

                }




            


        }



        


        private void blut()
        {
            
            kapitaal = 0;
            LblKapitaal.Text = "blut";
            timer.Stop();

            sNaam.Visibility = Visibility.Hidden;
            bNaam.Visibility = Visibility.Hidden;

            PuntenSpeler.Visibility = Visibility.Hidden;
            PuntenBank.Visibility = Visibility.Hidden;

            BtnDeel.Visibility = Visibility.Hidden;
            BtnPlaats.IsEnabled = false;
            nieuweSpel.Visibility = Visibility.Visible;
            MessageBox.Show("Je bent blut, start een nieuwe spel");

        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Isspeler">Speler of niet</param>
        private void GeefKaart(bool Isspeler)
        {

            int randomKaartSpeler = RdmKaarten.Next(0, 4);
            int randomGetalSpeler = Rdmgetal.Next(0, 13);

            string kaart1 = kaarten[randomKaartSpeler, randomGetalSpeler];


            

            //maakt elke kaart uniek
            while (gebruikteKaarten.Contains(kaart1))
            {
                randomKaartSpeler = RdmKaarten.Next(0, 4);
                randomGetalSpeler = Rdmgetal.Next(0, 13);
                kaart1 = kaarten[randomKaartSpeler, randomGetalSpeler];
            }

            gebruikteKaarten.Add(kaart1);

            string[] lijst = kaart1.Split(" ");


            if (Isspeler)
            {



                if (lijst[1].Length <= 2)
                {
                    int getal = int.Parse(lijst[1].ToString());
                    spelerPunten += getal;


                    GeneerKaartImg(kaart1, true);
                   // GeneerKaartImg(kaart1, true, 20);
                    //spelerKaart1.Source = new BitmapImage(new Uri(@"/Kaarten/" + kaart1 + ".png", UriKind.RelativeOrAbsolute));
                    //spelerKaart2.Source = new BitmapImage(new Uri(@"/Kaarten/" + kaart1 + ".png", UriKind.RelativeOrAbsolute));



                }

                else
                {

                    if (lijst[1].ToLower() == "aas")
                    {

                        checkAas(true);
                        GeneerKaartImg(kaart1, true);
                    }

                    else
                    {
                        spelerPunten += 10;
                        GeneerKaartImg(kaart1, true);
                    }


                }

                if (spelerPunten > 21 && aasTeller > 0)
                {
                    spelerPunten -= 10 * aasTeller;
                    aasTeller = 0;
                }

                //LabelSpeler.Content = sb.Append($"{kaarten[randomKaartSpeler, randomGetalSpeler]}\n").ToString();
                PuntenSpeler.Text = spelerPunten.ToString();

            }

            else
            {


                if (lijst[1].Length <= 2)
                {

                    int getal = int.Parse(lijst[1].ToString());
                    bankPunten += getal;
                    GeneerKaartImg(kaart1, false);

                }


                else
                {




                    if (lijst[1].ToLower() == "aas")
                    {

                        checkAas(false);
                        GeneerKaartImg(kaart1, false);
                    }
                    else
                    {

                        bankPunten += 10;
                        GeneerKaartImg(kaart1, false);


                    }


                }

                if (bankPunten > 21 && aasTeller > 0)
                {
                    bankPunten -= 10 ;
                    aasTeller = 0;
                }

                //LabelBank.Content = sbBank.Append($"{kaarten[randomKaartSpeler, randomGetalSpeler]}\n").ToString();
                PuntenBank.Text = bankPunten.ToString();
                

                


            }






        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="kaart">Kaart waarde in string value</param>
        /// 
        private void GeneerKaartImg(string kaart, bool isSpeler)
        {





            //spelerKaart1.Source = new BitmapImage(new Uri(@"/Kaarten/" + kaart + ".png", UriKind.RelativeOrAbsolute));
            Image image = new Image();
           
                //image.Margin = new Thickness(0 + xAs, 0, 0, 0);
                image.Stretch = Stretch.UniformToFill;
                image.Width = 70;
            

            


                image.Source = new BitmapImage(new Uri(@"/Kaarten/" + kaart + ".png", UriKind.RelativeOrAbsolute));


            
                

            if (isSpeler)
            {
                canvasSpeler.Children.Add(image);
                image.Margin = new Thickness(0 + xAs, 0, 0, 0);
                xAs += 20;
                image.Width = 130;
            }
            else
            {
                canvasBank.Children.Add(image);
                image.Margin = new Thickness(0 + xAsBank, 0, 0, 0);
                image.Width = 130;
                xAsBank += 20;

            }


                
        }


        private void gewonnen(bool gewonnen)
        {



            ronde++;

            //sbHistoriek.Append($"{inzet} - {spelerPunten} / {bankPunten}\n ");
            lblHistoriek.Content = $"Bedrag: {inzet} - Spelerpunten {spelerPunten} / Bankpunten {bankPunten}";
            //historiekLijst.Append($"{ronde}: {inzet} - {spelerPunten} / {bankPunten}");
            //timer.Start();
            historiekLijst.Add($"Ronde {ronde}: Bedrag: {inzet} - Spelerpunten: {spelerPunten} / Bankpunten {bankPunten}");
            

            if (gewonnen)
            {
                //LblKapitaal.IsEnabled = false;
                //InzetTxtBx.IsEnabled = true;
                kapitaal += inzet * 2;
                LblKapitaal.Text = kapitaal.ToString();
                gewonnenLabel.Content = "Gewonnen!!!";
                gewonnenLabel.Foreground = new SolidColorBrush(Colors.Green);
                // gewonnenLabel.Visibility = Visibility.Hidden;



            }

            else
            {
                //LblKapitaal.IsEnabled = false;
                //InzetTxtBx.IsEnabled = true;
                kapitaal -= inzet;
                LblKapitaal.Text = kapitaal.ToString();
                gewonnenLabel.Content = "Verloren!!!";
                gewonnenLabel.Foreground = new SolidColorBrush(Colors.Red);
                // gewonnenLabel.Visibility = Visibility.Hidden;




            }


        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {


            double.TryParse(InzetTxtBx.Text, out inzet);

            double.TryParse(LblKapitaal.Text, out kapitaal);

            if (inzet < kapitaal*0.1)
            {
                
                MessageBox.Show("Inzet moet minimaal 10% zijn van het kapitaal");

            }
            else
            {
                BtnDeel.IsEnabled=true; 
            }
        }

        private void lblHistoriek_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            historiekLijst.Reverse();

            foreach (string item in historiekLijst)
            {
                sbHistoriek.AppendLine(item);
            }


            MessageBox.Show(sbHistoriek.ToString());
            sbHistoriek.Clear();

        }


        /// <summary>
        /// Veranderd de index van de kaarten in een lijst
        /// </summary>
        /// <param name="kaarten">Gegeven kaarten lijst</param>
        /// <returns>Geeft de nieuwe kaarte lijst terug</returns>
        private string[,] schudden(string[,] kaarten)
        {
            Random random = new Random();
            int indexKaarten;
            int indexY;
            int indexGetal;
            int index2Y;
            string temp;
            

            for (int i = 0; i < 100; i++)
            {
                indexKaarten = random.Next(0, 4);
                indexGetal = random.Next(0, 4);

                indexY = random.Next(0, 13);
                index2Y = random.Next(0, 13);
                temp = kaarten[indexKaarten, indexY];
                kaarten[indexKaarten, indexY] = kaarten[indexGetal, index2Y];
                kaarten[indexGetal, index2Y] = temp;
            }

            wachtTimer.Start();
            kaartenSchudden.Visibility = Visibility.Visible;
            kaartenSchudden.Content = "Kaarten Worden Geschud";
            


            return kaarten;
        }

        private void BtnDoubleDown_Click(object sender, RoutedEventArgs e)
        {
            BtnDoubleDown.IsEnabled = false;

            if (inzet/2 >=0)
            {
                inzet = inzet * 2;

                dubbeldown = true;

                GeefKaart(true);

                
                

                if (spelerPunten > 21)
                {
                    gewonnen(false);
                    timer.Start();
                }
                else
                {
                    BtnStand_Click(sender, e);



                }


            }



            else
            {
                MessageBox.Show("Onvoldoende inzet");
            }
            




            



        }
    }
}
