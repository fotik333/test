using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using GameObjects;
using Interfaces;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class GamePlayNetworkManager : NetworkManager, IGamePlayManager
    {
        public event EventHandler Destroyed, Disconnected;

        public GameObject menu;

        public bool IsWaitingState => _currentState == GameState.Waiting;
        public bool IsStartingState => _currentState == GameState.Starting;
        public bool IsPlayingState => _currentState == GameState.Playing;
        
        private Ball _ball;

        private GameState _currentState = GameState.Waiting;

        private GameManager _gameManager;

        private bool _isClientReadyToPlay;

        private NetworkManagerHUD _hud;

        private static string LocalIPAddress()
        {
            var localIP = "0.0.0.0";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork) continue;
                localIP = ip.ToString();
                break;
            }
            
            return localIP;
        }
        
        public override void Start()
        {
            base.Start();
            singleton.networkAddress = LocalIPAddress();

            _gameManager = GameManager.Instance;
            _gameManager.OnGamePlayManagerCreated(this);

            _hud = gameObject.GetComponent<NetworkManagerHUD>();

            menu.SetActive(false);

            NetworkClient.RegisterHandler<HostCompleteGameMessage>(OnHostCompleteGameMessage, false);
            NetworkClient.RegisterHandler<ServerStartMessage>(OnServerStartMessage, false);
            NetworkServer.RegisterHandler<ClientReadyToPlayMessage>(OnClientReadyToPlayMessageReceived, false);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _hud.showGUI = false;
            menu.SetActive(true);

            NetworkServer.SendToAll(new ServerStartMessage());
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            TryToStartSession();
        }

        private void OnServerStartMessage(ServerStartMessage msg)
        {
            TryToStartSession();
        }

        private void TryToStartSession()
        {
            if (menu.activeSelf) return;

            _hud.showGUI = false;
            menu.SetActive(true);
        }

        private void OnHostCompleteGameMessage(HostCompleteGameMessage msg)
        {
            CompeteGame();
        }

        private void OnClientReadyToPlayMessageReceived(ClientReadyToPlayMessage msg)
        {
            _isClientReadyToPlay = true;

            if (IsStartingState) HostStartGame();
        }

        public void StartGame()
        {
            if (numPlayers == 1) return;

            menu.SetActive(false);
            _currentState = GameState.Starting;

            if (numPlayers == 0)
            {
                NetworkClient.Send(new ClientReadyToPlayMessage());
                return;
            }

            if (numPlayers < 2) return;

            if (_isClientReadyToPlay) HostStartGame();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            var paddle = Instantiate(playerPrefab);
            paddle.GetComponent<Paddle>().Init(numPlayers == 0);
            NetworkServer.AddPlayerForConnection(conn, paddle);
        }

        private void HostStartGame()
        {
            if (!_ball) InitBall();

            _ball.Refresh();
            _ball.gameObject.SetActive(true);

            StartCoroutine(StartPlaying());
        }

        private void InitBall()
        {
            var ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "NetworkBall"));
            NetworkServer.Spawn(ball);

            _ball = ball.GetComponent<Ball>();
            _ball.CollisionWithPaddle += OnBallCollisionWithPaddle;
            _ball.FlyAwayFromScreen += OnBallFlyAwayFromScreen;
        }

        private IEnumerator StartPlaying()
        {
            yield return new WaitForSeconds(1);
            _currentState = GameState.Playing;
        }

        public void StopGame()
        {
            CompeteGame();
            _ball.gameObject.SetActive(false);
            _isClientReadyToPlay = false;

            NetworkServer.SendToAll(new HostCompleteGameMessage());
        }

        private void CompeteGame()
        {
            menu.gameObject.SetActive(true);
            _currentState = GameState.Waiting;
        }

        private void OnBallCollisionWithPaddle(object sender, Collider2D other)
        {
            var paddleTransform = other.transform;
            var paddlePosition = paddleTransform.position;
            var paddleX = paddlePosition.x;
            var paddleY = paddlePosition.y;
            var paddleHeight = paddleTransform.localScale.y;

            if (paddleY < 0 && _ball.transform.position.y - _ball.radius <
                GameManager.BottomLeft.y + paddleHeight &&
                _ball.direction.y < 0
                || paddleY > 0 && _ball.transform.position.y + _ball.radius >
                GameManager.TopRight.y - paddleHeight &&
                _ball.direction.y > 0)
                return;

            _ball.SwitchYDirection(_ball.transform.position.x - paddleX);
        }

        private void OnBallFlyAwayFromScreen(object sender, EventArgs args)
        {
            StopGame();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            Disconnected?.Invoke(this, null);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Disconnected?.Invoke(this, null);
        }

        public override void OnDestroy()
        {
            if (_ball)
            {
                _ball.CollisionWithPaddle -= OnBallCollisionWithPaddle;
                _ball.FlyAwayFromScreen -= OnBallFlyAwayFromScreen;
            }

            NetworkClient.Disconnect();
            NetworkClient.Shutdown();

            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();

            Destroyed?.Invoke(this, null);
        }
    }
}