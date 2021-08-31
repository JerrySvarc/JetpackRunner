using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace JetpackRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        ImageBrush playerSprite = new ImageBrush();
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush zapperSprite = new ImageBrush();

        Game game = new Game();

        int[] obstaclePosition = { 20, 111, 200 };

        private double RunningSpeed;
        private double ZapperSpeed;

        Rect playerHitBox;
        Rect groundHitBox;
        Rect obstacleHitBox;
        Rect obstacleHitBox2;
        Rect roofHitBox;


        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            backgroundSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BackdropMain.png"));

            background.Fill = backgroundSprite;
            background2.Fill = backgroundSprite;

            InitWindow();
            gameTimer.Start();
        }


        public void InitWindow()
        {
            RunAnimation(0);
            ZapperAnimation(0);
            SetStage();

            game.InitGame();
            scoreText.Content = "Score: " + game.score;
            gameTimer.Start();
        }


        private void GameEngine(object sender, EventArgs e)
        {
            game.IncreaseSpeed();
            ZapperSpeed += .5;
            ZapperSpeed %= 3;
            ZapperAnimation(ZapperSpeed);

            Canvas.SetTop(player, Canvas.GetTop(player) + (game.gravity + game.uplift));
            Canvas.SetLeft(background, Canvas.GetLeft(background) - game.speed);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - game.speed);
            Canvas.SetLeft(zapper1, Canvas.GetLeft(zapper1) - game.speed);
            Canvas.SetLeft(zapper2, Canvas.GetLeft(zapper2) - game.speed);

            scoreText.Content = "Score: " + game.score;

            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 10, player.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);
            roofHitBox = new Rect(Canvas.GetLeft(roof), Canvas.GetTop(roof), roof.Width, roof.Height);
            obstacleHitBox = new Rect(Canvas.GetLeft(zapper1), Canvas.GetTop(zapper1), zapper1.Width, zapper1.Height);
            obstacleHitBox2 = new Rect(Canvas.GetLeft(zapper2), Canvas.GetTop(zapper2), zapper2.Width, zapper2.Height);


            CheckCollision();
            DrawNew();

            if (game.gameover)
            {
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BarryDead.png"));
                scoreText.Content += "   Press Space to retry";
            }
        }

        private void DrawNew()
        {
            if (Canvas.GetLeft(background) < -1262)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background2) + background2.Width);
            }

            if (Canvas.GetLeft(background2) < -1262)
            {
                Canvas.SetLeft(background2, Canvas.GetLeft(background) + background.Width);
            }

            if (Canvas.GetLeft(zapper1) < -50)
            {
                Canvas.SetLeft(zapper1, 950 + game.random.Next(0, 40));
                Canvas.SetTop(zapper1, obstaclePosition[game.random.Next(0, obstaclePosition.Length)]);
                game.score += 1;
            }

            if (Canvas.GetLeft(zapper2) < -50)
            {
                Canvas.SetLeft(zapper2, 950 + game.random.Next(0, 40));
                Canvas.SetTop(zapper2, obstaclePosition[game.random.Next(0, obstaclePosition.Length)]);
                game.score += 1;
            }
        }

        private void RunAnimation(double i)
        {
            switch (i)
            {
                case 0:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BarryRun1.png"));
                    break;
                case 1:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BarryRun2.png"));
                    break;
                case 2:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BarryRun3.png"));
                    break;
            }
            player.Fill = playerSprite;
        }

        void ZapperAnimation(double i)
        {
            switch (i)
            {
                case 0:
                    zapperSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Zapper1.png"));
                    break;
                case 1:
                    zapperSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Zapper2.png"));
                    break;
                case 2:
                    zapperSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Zapper3.png"));
                    break;
                case 3:
                    zapperSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Zapper4.png"));
                    break;
            }
            zapper1.Fill = zapperSprite;
            zapper2.Fill = zapperSprite;
        }

        void CheckCollision()
        {
            if (playerHitBox.IntersectsWith(roofHitBox))
            {
                game.gravity = 0;
                game.uplift = 0;
            }
            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                game.gravity = 0;
                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);
                RunningSpeed += .5;
                RunningSpeed %= 3;
                RunAnimation(RunningSpeed);
            }

            if (playerHitBox.IntersectsWith(obstacleHitBox) || playerHitBox.IntersectsWith(obstacleHitBox2))
            {
                game.gameover = true;
                gameTimer.Stop();
            }
        }

        void SetStage()
        {
            Canvas.SetLeft(background, 0);
            Canvas.SetLeft(background2, 1262);

            Canvas.SetLeft(player, 67);
            Canvas.SetTop(player, 329);

            Canvas.SetLeft(zapper1, 581);
            Canvas.SetTop(zapper1, 200);

            Canvas.SetLeft(zapper2, 1101);
            Canvas.SetTop(zapper2, 22);
        }


        void Canvas_KeyUp(object sender, KeyEventArgs keyboard)
        {
            if (keyboard.Key == Key.Space)
            {
                game.uplift = 0;
                game.gravity = 8;
            }
        }

        void Canvas_KeyDown(object sender, KeyEventArgs keyboard)
        {
            if (keyboard.Key == Key.Space && !game.gameover)
            {
                game.gravity = 4;
                game.uplift = -10;
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BarryFly.png"));
                if (playerHitBox.IntersectsWith(roofHitBox))
                {
                    game.gravity = 0;
                    game.uplift = 0;
                }

            }
            else if (keyboard.Key == Key.Space && game.gameover)
            {
                InitWindow();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs mouse)
        {

            if (mouse.ButtonState == MouseButtonState.Pressed && !game.gameover)
            {
                game.gravity = 4;
                game.uplift = -10;
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/BarryFly.png"));

                if (playerHitBox.IntersectsWith(roofHitBox))
                {
                    game.gravity = 0;
                    game.uplift = 0;

                }
            }
            else if (mouse.ButtonState == MouseButtonState.Pressed && game.gameover)
            {
                InitWindow();
            }
        }

        private void MyCanvas_MouseUp(object sender, MouseButtonEventArgs mouse)
        {
            if (mouse.ButtonState == MouseButtonState.Released)
            {
                game.uplift = 0;
                game.gravity = 8;
            }
        }

    }
}
