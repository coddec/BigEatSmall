using System;
using SplashKitSDK;

public class Player
{
    public int Score { get; set; } = 0;
    public string Name { get; set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Radius { get; set; } = 10;
    public Circle c;
    private Color PlayerColor;

    private int Boost = 0;
    private int Speed = 5;
    private bool PlayerWrapAroundTheWindow = false;
    private int _NameWidth;
    private SoundEffect GoodSound = new SoundEffect("pop", "pop.wav");
    private SoundEffect BadSound = new SoundEffect("beep", "beep.wav");

    public Player(Window gameWindow, string name)
    {
        X = gameWindow.Width / 2;
        Y = gameWindow.Height / 2;
        Name = name;

        PlayerColor = SplashKit.RandomRGBColor(200);

        _NameWidth = SplashKit.TextWidth(name, "Arial", 10);
    }


    public void Draw()
    {
        SplashKit.FillCircle(PlayerColor, X, Y, Radius);
        c = SplashKit.CircleAt(X, Y, Radius);

        SplashKit.DrawText(Name, Color.Black, X - _NameWidth / 2, Y - Radius - 8);
        SplashKit.DrawText(Convert.ToString(Score), Color.Black, X - 3, Y + Radius);

        //SplashKit.DrawText($"X:{X}, Y:{Y}", Color.Black, 5, 20);

    }

    public bool Quit { get; private set; }

    public void HandleInput()
    {
        if (SplashKit.KeyReleased(SplashKitSDK.KeyCode.IKey))
        {
            if (Boost <= 5)
            {
                Boost++;
                if (Boost >= 6)
                {
                    Boost = 1;
                }
            }
            switch (Boost)
            {
                case 1:
                    Speed = 5;
                    break;

                case 2:
                    Speed = 10;
                    break;

                case 3:
                    Speed = 15;
                    break;

                case 4:
                    Speed = 20;
                    break;

                case 5:
                    Speed = 50;
                    break;
            }
        }

        if (SplashKit.KeyDown(SplashKitSDK.KeyCode.EscapeKey))
        {
            Quit = true;
        }

        if (SplashKit.QuitRequested())
        {
            Quit = true;
        }

        if (SplashKit.KeyDown(SplashKitSDK.KeyCode.LeftKey) || SplashKit.KeyDown(SplashKitSDK.KeyCode.AKey))
        {
            X -= Speed;
        }

        if (SplashKit.KeyDown(SplashKitSDK.KeyCode.RightKey) || SplashKit.KeyDown(SplashKitSDK.KeyCode.DKey))
        {
            X += Speed;
        }

        if (SplashKit.KeyDown(SplashKitSDK.KeyCode.UpKey) || SplashKit.KeyDown(SplashKitSDK.KeyCode.WKey))
        {
            Y -= Speed;
        }

        if (SplashKit.KeyDown(SplashKitSDK.KeyCode.DownKey) || SplashKit.KeyDown(SplashKitSDK.KeyCode.SKey))
        {
            Y += Speed;
        }
    }

    public void StayOnWindow(Window gameWindow)
    {

        int windowWidth = gameWindow.Width;
        int windowHeight = gameWindow.Height;

        if (!PlayerWrapAroundTheWindow)
        {
            //Player stops at Boundary
            if (X - Radius <= 0)
            {
                X = Radius;
            }

            if (X + Radius >= (windowWidth))
            {
                X = windowWidth - Radius;
            }

            if (Y - Radius <= 0)
            {
                Y = Radius;
            }

            if (Y + Radius >= windowHeight)
            {
                Y = windowHeight - Radius;
            }
            //Player stops at Boundary
        }
        else
        {
            //Player wrap around the screen
            if (X + Radius < 0)
            {
                X = windowWidth;
            }

            if (X > windowWidth)
            {
                X = -Radius;
            }

            if (Y + Radius < 0)
            {
                Y = windowHeight;
            }

            if (Y > windowHeight)
            {
                Y = -Radius;
            }
            //Player wrap around the screen
        }
    }

    public bool CollidedWithRandomDots(RandomDots randomDots)
    {
        return SplashKit.CirclesIntersect(c, randomDots.CollisionCircle);
    }

    public bool CollidedWithPlayer(Player player)
    {
        //TODO: chang it, make it work
        return false;
    }

    public bool CollidedWithPlayer(OtherPlayer op)
    {
        return SplashKit.CirclesIntersect(c, op.CollisionCircle);
    }

    public void PlayGoodSound()
    {
        GoodSound.Play();
    }

    public void PlayBadSound()
    {
        BadSound.Play();
    }
}
