using SplashKitSDK;

public class RandomDots
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; set; }

    public Color MainColor;
    public RandomDots(Window gameWindow, double radius)
    {
        MainColor = Color.RandomRGB(200);
        Radius = radius;

        X = Function.RdnInclusive(1, gameWindow.Width - 10);
        Y = Function.RdnInclusive(1, gameWindow.Height - 10);
    }

    public void Draw()
    {
        SplashKit.FillCircle(MainColor, X, Y, Radius);
    }

    public Circle CollisionCircle
    {
        get
        {
            return SplashKit.CircleAt(X, Y, Radius);
        }
    }
}
