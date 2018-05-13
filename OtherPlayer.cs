using System;
using SplashKitSDK;

public class OtherPlayer
{
    public string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public int Score { get; set; } = 0;
    public double Radius { get; set; } = 10;
    private Color PlayerColor;
    private int _NameWidth;

    public OtherPlayer(string name, double x, double y, double radius, int score, Color color)
    {
        Name = name;
        X = x;
        Y = y;
        Radius = radius;
        PlayerColor = color;
        Score = score;
        _NameWidth = SplashKit.TextWidth(name, "Arial", 10);
    }

    public void Draw()
    {
        SplashKit.FillCircle(PlayerColor, X, Y, Radius);
        SplashKit.DrawText(Name, Color.Red, X - _NameWidth / 2, Y - Radius - 8);
        SplashKit.DrawText(Convert.ToString(Score), Color.Black, X - 3, Y + Radius);
    }

    public Circle CollisionCircle
    {
        get
        {
            return SplashKit.CircleAt(X, Y, Radius);
        }
    }
}
