using System;

namespace JetpackRunner
{
    public class Game
    {
        public int uplift = 0;
        public int gravity = 8;
        public double speed;
        public Random random = new Random();
        public bool gameover = false;

        public int score = 0;

        public void InitGame()
        {
            gameover = false;
            score = 0;
            speed = 2;
        }

        public void IncreaseSpeed()
        {
            speed += 0.01;
        }

    }
}
