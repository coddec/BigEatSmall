using SplashKitSDK;

public class Program
{
    public static void Main()
    {
        Window gameWindow = new Window("Big Eat Small", 900, 900);
        Game game = new Game(gameWindow);

        do
        {
            SplashKit.ProcessEvents();
            gameWindow.Clear(Color.AliceBlue);
            game.HandleInput();
            game.Update();
            game.Draw();
            gameWindow.Refresh(60);
        } while (!game.Quit);
    }
}


