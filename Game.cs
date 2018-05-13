using SplashKitSDK;
using System;
using System.Collections.Generic;

public class Game
{
    private Player _player;
    private Window _gameWindow;
    private List<RandomDots> _randomDots = new List<RandomDots>();
    public bool OnlineGame { get; private set; }
    public bool IsServer { get; private set; }
    public GamePeer ThisPeer { get; private set; }
    private string _otherPlayerMsg;

    private List<OtherPlayer> _otherPlayers = new List<OtherPlayer>();
    private List<string> _otherNetworkNames = new List<string>();
    private int NumOfRandomDotsOnScreen = 15;

    private string name = "";
    private double x = 0, y = 0, radius = 0;
    private int score = 0;
    private SoundEffect bgm = new SoundEffect("bgm", "bgm.wav");


    public bool Quit
    {
        get
        {
            return _player.Quit;
        }
    }

    public Game(Window window)
    {

        string answer;
        Console.Write("What is your name: ");
        string name = Console.ReadLine();

        do
        {
            Console.Write("Do you want to play it online? (Y/N) ");
            answer = Console.ReadLine();
        }
        while (answer.ToUpper() != "Y" && answer.ToUpper() != "N");

        if (answer.ToUpper() == "N") // Not online game, offline game
        {
            OnlineGame = false;
        }
        else if (answer.ToUpper() == "Y") // Online game
        {
            OnlineGame = true;
            Console.Write("Which port to run at: ");
            ushort port = Convert.ToUInt16(Console.ReadLine());
            GamePeer peer = new GamePeer(port) { Name = name };
            ThisPeer = peer;

            string isHost;
            do
            {
                Console.Write("Is this the host? (Y/N) ");
                isHost = Console.ReadLine();
            } while (isHost.ToUpper() != "Y" && isHost.ToUpper() != "N");

            if (isHost.ToUpper() == "N") // Not host server, select server to connect to
            {
                IsServer = false;
                MakeNewConnection(peer);
            }
            else if (isHost.ToUpper() == "Y") // Be the host server
            {
                IsServer = true;
            }
        }

        _gameWindow = window;

        Player Player = new Player(window, name);
        _player = Player;

        Console.WriteLine("Please switch back to game window!");
    }

    public void HandleInput()
    {
        _player.HandleInput();
        _player.StayOnWindow(_gameWindow);
    }

    public void Draw()
    {
        _player.Draw();

        if (_otherPlayers.Count > 0)
        {
            foreach (OtherPlayer op in _otherPlayers)
            {
                op.Draw();
            }
        }

        if (_randomDots.Count > 0)
        {
            foreach (RandomDots rd in _randomDots)
            {
                rd.Draw();
            }
        }
    }

    public void Update()
    {
        if (OnlineGame)
        {
            UpdateNetworkInfo();
            UpdateOtherPlayerInfo();
        }

        CheckCollisions();

        if (_randomDots.Count < NumOfRandomDotsOnScreen)
        {
            int tmp_small = 0;
            for (int i = 0; i < _randomDots.Count - 1; i++)
            {
                if (_randomDots[i].Radius < _player.Radius) { tmp_small++; }
            }

            if (tmp_small <= 2)
            {
                _randomDots.Add(new RandomDots(_gameWindow, Function.RdnInclusive(3, Convert.ToInt32(_player.Radius - 2))));
            }
            else
            {
                do
                {
                    _randomDots.Add(new RandomDots(_gameWindow, Function.RdnInclusive(Convert.ToInt32(_player.Radius + 1), Convert.ToInt32(_player.Radius + 20))));
                } while (_randomDots.Count < NumOfRandomDotsOnScreen);
            }
        }

        if (!bgm.IsPlaying)
        {
            PlayBgm();
        }
    }

    private void CheckCollisions()
    {
        List<RandomDots> rmRandomDots = new List<RandomDots>();
        foreach (OtherPlayer op in _otherPlayers)
        {
            if (_player.CollidedWithPlayer(op))
            {
                if (_player.Radius < op.Radius)
                {
                    _player.Score--;
                }
                else if (_player.Radius > op.Radius)
                {
                    _player.Score++;
                }
            }
        }

        foreach (RandomDots rd in _randomDots)
        {
            if (_player.CollidedWithRandomDots(rd))
            {
                rmRandomDots.Add(rd);
                if (rd.Radius > _player.Radius)
                {
                    _player.PlayBadSound();
                    if (_player.Radius - 5 > 5)
                    {
                        _player.Radius -= 5;
                    }
                    _player.Score -= 10;
                }
                else if (rd.Radius < _player.Radius)
                {
                    _player.PlayGoodSound();
                    _player.Score += 10;
                    _player.Radius += 1;
                }
            }
        }

        if (_randomDots.Count > 0)
        {
            foreach (RandomDots rd in rmRandomDots)
            {
                _randomDots.Remove(rd);
            }
        }
    }


    public void PlayBgm()
    {
        bgm.Play();
    }



    #region Network, OtherPlayer
    public void UpdateNetworkInfo()
    {
        if (IsServer)
        {
            SplashKit.AcceptAllNewConnections();
        }

        // ThisPeer.PrintNewMessages();

        _otherPlayerMsg = ThisPeer.GetNewMessages();

        UpdateOtherPlayer();

        //Console.WriteLine(ThisPeer.GetNewMessages());
        BroadcastMessage();
    }

    public void UpdateOtherPlayer()
    {
        if (_otherPlayerMsg != null && _otherPlayerMsg.Length > 0)
        {
            name = _otherPlayerMsg.Split(',')[0];
            x = Convert.ToDouble(_otherPlayerMsg.Split(',')[1]);
            y = Convert.ToDouble(_otherPlayerMsg.Split(',')[2]);
            radius = Convert.ToDouble(_otherPlayerMsg.Split(',')[3]);
            score = Convert.ToInt32(_otherPlayerMsg.Split(',')[4]);
            //Console.WriteLine($"{name},{x},{y},{radius},{score}");
        }

        if (name.Length > 0)
        {
            if (!_otherNetworkNames.Contains(name))
            {
                _otherNetworkNames.Add(name);
                _otherPlayers.Add(new OtherPlayer(name, x, y, radius, score, Color.Gray));
            }
        }
    }


    public void UpdateOtherPlayerInfo()
    {
        foreach (OtherPlayer op in _otherPlayers)
        {
            op.X = x;
            op.Y = y;
            op.Radius = radius;
            op.Score = score;
        }
    }

    private void BroadcastMessage()
    {
        ThisPeer.Broadcast($"{_player.Name},{_player.X},{_player.Y},{_player.Radius},{_player.Score}");
    }

    private void WaitForConnection()
    {
        while (!SplashKit.HasNewConnections())
        {
            SplashKit.AcceptAllNewConnections();
        }

        ThisPeer.CheckNewConnections();
    }

    private void MakeNewConnection(GamePeer peer)
    {
        string address;
        ushort port;

        Console.Write("Enter Host Server address: ");
        address = Console.ReadLine();

        Console.Write("Enter Host Server port: ");
        port = Convert.ToUInt16(Console.ReadLine());

        peer.ConnectToGamePeer(address, port);
    }

    #endregion
}
