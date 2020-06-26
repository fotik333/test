using System;
using System.Collections;
using GameObjects;
using Interfaces;
using UnityEngine;

namespace Managers
{
    public class GamePlayManager : MonoBehaviour, IGamePlayManager
    {
        public event EventHandler<int> CounterUpdated;
        public event EventHandler<int> RecordUpdated;
        public event EventHandler Destroyed;

        public GameObject menu;
        public GameObject[] objectsToBeActivatedOnStart;
        
        public bool IsWaitingState => _currentState == GameState.Waiting;
        public bool IsStartingState => _currentState == GameState.Starting;
        public bool IsPlayingState => _currentState == GameState.Playing;
        
        private Ball _ball;

        private GameState _currentState = 0;

        private int _counter, _record;

        private GameManager _gameManager;
        
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gameManager.OnGamePlayManagerCreated(this);

            foreach (var obj in objectsToBeActivatedOnStart)
            {
                obj.SetActive(true);
            }

            var ballPrefab = Resources.Load<Ball>("Prefabs/Ball");
            _ball = Instantiate(ballPrefab);

            _ball.CollisionWithPaddle += OnBallCollisionWithPaddle;
            _ball.FlyAwayFromScreen += OnBallFlyAwayFromScreen;
            _ball.gameObject.SetActive(false);

            var paddlePrefab = Resources.Load<Paddle>("Prefabs/Paddle");
            
            var paddle1 = Instantiate(paddlePrefab);
            paddle1.Init(true);
            
            var paddle2 = Instantiate(paddlePrefab);
            paddle2.Init(false);

            _record = _gameManager.Record;
            RecordUpdated?.Invoke(this, _record);
        }
        
        public void StartGame()
        {
            menu.SetActive(false);
            
            _currentState = GameState.Starting;
            _ball.Refresh();
            _ball.gameObject.SetActive(true);

            _counter = 0;
            CounterUpdated?.Invoke(this, _counter);

            StartCoroutine(BallAppearing());
        }

        public void StopGame()
        {
            menu.SetActive(true);
            _currentState = GameState.Waiting;
            _ball.gameObject.SetActive(false);
        }

        private void OnBallFlyAwayFromScreen(object sender, EventArgs args)
        {
            StopGame();
        }

        private void OnBallCollisionWithPaddle(object sender, Collider2D other)
        {
            var paddleTransform = other.transform;
            var paddlePosition = paddleTransform.position;
            var paddleX = paddlePosition.x;
            var paddleY = paddlePosition.y;
            var paddleHeight = paddleTransform.localScale.y;

            if (paddleY < 0 && _ball.transform.position.y - _ball.radius < GameManager.BottomLeft.y + paddleHeight &&
                _ball.direction.y < 0
                || paddleY > 0 && _ball.transform.position.y + _ball.radius > GameManager.TopRight.y - paddleHeight &&
                _ball.direction.y > 0)
                return;

            _counter++;
            CounterUpdated?.Invoke(this, _counter);

            if (_record < _counter)
            {
                _record = _counter;
                RecordUpdated?.Invoke(this, _record);
            }

            _ball.SwitchYDirection(_ball.transform.position.x - paddleX);
        }

        private IEnumerator BallAppearing()
        {
            _counter = 0;
            CounterUpdated?.Invoke(this, _counter);

            yield return new WaitForSeconds(1);

            _currentState = GameState.Playing;
        }

        private void OnDestroy()
        {
            if (_ball)
            {
                _ball.CollisionWithPaddle -= OnBallCollisionWithPaddle;
                _ball.FlyAwayFromScreen -= OnBallFlyAwayFromScreen;
            }

            Destroyed?.Invoke(this, null);
        }
    }
}