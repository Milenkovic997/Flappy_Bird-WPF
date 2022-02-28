using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FlappyBird_WPF
{
    public partial class MainWindow : Window
    {
        private int _score;
        private int _birdImageChange = 0;
        private double _birdAcceleration = 70;
        private bool playing = true;
        private bool directionBottom = true;
        MediaPlayer player = new MediaPlayer();
        DispatcherTimer gameTime = new DispatcherTimer();
        RotateTransform rotateTransform = new RotateTransform();

        public MainWindow()
        {
            InitializeComponent();

            Random r = new Random();
            int firstPipePosition = r.Next(-350, -50);
            int secondPipePosition = r.Next(-350, -50);
            spPipe1.Margin = new Thickness(-100, firstPipePosition, 0, 0);
            spPipe2.Margin = new Thickness(400, secondPipePosition, 0, 0);

            gameTime.Tick += gameTime_Tick;
            gameTime.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }
        private void gameTime_Tick(object? sender, EventArgs e)
        {
            checkScore();
            checkDeath();
            changeBirdImage();

            if (directionBottom && _birdAcceleration < 0.5)
            {
                imgPlayer.Margin = new Thickness(64, imgPlayer.Margin.Top + 5, 0, 0);

                rotateTransform.CenterX = 27;
                rotateTransform.CenterY = 16;
                rotateTransform.Angle = 15;
                imgPlayer.RenderTransform = rotateTransform;
            }
            else
            {
                _birdAcceleration = _birdAcceleration / 1.08;
                imgPlayer.Margin = new Thickness(64, imgPlayer.Margin.Top - _birdAcceleration, 0, 0);

                rotateTransform.CenterX = 27;
                rotateTransform.CenterY = 16;
                rotateTransform.Angle = -15;
                imgPlayer.RenderTransform = rotateTransform;
            }

            Random r = new Random();
            int pipePosition = r.Next(-350, -50);

            imgGround.Margin = new Thickness(imgGround.Margin.Left - 2, 591, 0, 0);
            if (imgGround.Margin.Left < -400) imgGround.Margin = new Thickness(0, 591, 0, 0);

            spPipe1.Margin = new Thickness(spPipe1.Margin.Left - 4, spPipe1.Margin.Top, 0, 0);
            if (spPipe1.Margin.Left < -1100)
            {
                spPipe1.Margin = new Thickness(-100, pipePosition, 0, 0);
            }

            spPipe2.Margin = new Thickness(spPipe2.Margin.Left - 4, spPipe2.Margin.Top, 0, 0);
            if (spPipe2.Margin.Left < -1100)
            {
                spPipe2.Margin = new Thickness(-100, pipePosition, 0, 0);
            }
        }

        private void changeBirdImage()
        {
            _birdImageChange++;
            if (_birdImageChange == 10) imgPlayer.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"/sprites/yellowbird-downflap.png"));
            else if (_birdImageChange == 20) imgPlayer.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"/sprites/yellowbird-midflap.png"));
            else if (_birdImageChange == 30) imgPlayer.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"/sprites/yellowbird-upflap.png"));
            else if (_birdImageChange > 30) _birdImageChange = 0;
        }

        private void checkDeath()
        {
            if (spPipe1.Margin.Left <= -725 && spPipe1.Margin.Left >= -930)
            {
                if (imgPlayer.Margin.Top <= (spPipe1.Margin.Top + 420) || imgPlayer.Margin.Top >= (spPipe1.Margin.Top + 550))
                {
                    death();
                }
            }
            else if (spPipe2.Margin.Left <= -725 && spPipe2.Margin.Left >= - 930)
            {
                if (imgPlayer.Margin.Top <= (spPipe2.Margin.Top + 420) || imgPlayer.Margin.Top >= (spPipe2.Margin.Top + 550))
                {
                    death();
                }
            }
            else if(imgPlayer.Margin.Top >= 550) death();
        }
        private void death()
        {
            DoubleAnimation ani = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            whiteScreen.BeginAnimation(OpacityProperty, ani);

            imgGameOver.Visibility = Visibility.Visible;
            gameTime.Stop();
            player.Open(new Uri(Directory.GetCurrentDirectory() + @"/audio/die.wav"));
            player.Play();
            playing = false;
        }

        private void checkScore()
        {
            if (spPipe1.Margin.Left == -820 || spPipe2.Margin.Left == -820)
            {
                player.Open(new Uri(Directory.GetCurrentDirectory() + @"/audio/point.wav"));
                player.Play();

                _score++;
                if (_score == 1000) _score = 999;

                int firstNumber = _score / 100;
                int secondNumber = _score/10 % 10;
                int thirdNumber = _score % 10;

                imgFirstNumber.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"/sprites/" + firstNumber + ".png"));
                imgSecondNumber.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"/sprites/" + secondNumber + ".png"));
                imgThirdNumber.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"/sprites/" + thirdNumber + ".png"));

                if(firstNumber == 0) imgFirstNumber.Visibility = Visibility.Collapsed;
                else imgFirstNumber.Visibility = Visibility.Visible;

                if (secondNumber == 0) imgSecondNumber.Visibility = Visibility.Collapsed;
                else imgSecondNumber.Visibility = Visibility.Visible;
            }
        }

        // EXIT BUTTON FUNCTIONS
        private void btnExit_MouseEnter(object sender, MouseEventArgs e)
        {
            btnExit.Background = Brushes.Red;
        }
        private void btnExit_MouseLeave(object sender, MouseEventArgs e)
        {
            btnExit.Background = new SolidColorBrush(Color.FromArgb(0xFF, 29, 29, 29));
        }
        private void btnExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        private void dragFrameFunction(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // KEY PRESS
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (playing)
            {
                if (e.Key == Key.Up) directionBottom = false;
                imgStartGame.Visibility = Visibility.Collapsed;
                gameTime.Start();
                _birdAcceleration = 5;
            }
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            directionBottom = true;
        }

        // MOUSE PRESS
        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (playing)
            {
                directionBottom = false;
                imgStartGame.Visibility = Visibility.Collapsed;
                gameTime.Start();
                _birdAcceleration = 5;
            }
        }
        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            directionBottom = true;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Top = this.Top;
            mw.Left = this.Left;
            mw.Show();
            this.Close();
        }
    }
}