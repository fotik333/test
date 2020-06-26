using System;
using Interfaces;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameObjects
{
    public class Ball : MonoBehaviour
    {
        public Vector2 direction;
        public float radius;

        private float _speed;

        public event EventHandler<Collider2D> CollisionWithPaddle;
        public event EventHandler FlyAwayFromScreen;

        private IGamePlayManager _gamePlayManager;
        private GameManager _gameManager;

        public void Refresh()
        {
            transform.position = new Vector2(0, 0);

            var scale = Random.Range(.8f, 1.2f);
            transform.localScale = Vector2.one * scale;
            radius = scale / 2;

            _speed = Random.Range(8, 12);

            var signX = Random.Range(0.0f, 1.0f) > .5f ? 1 : -1;
            var signY = Random.Range(0.0f, 1.0f) > .5f ? 1 : -1;

            direction = new Vector2(Random.Range(0.35f, 0.65f) * signX, Random.Range(.85f, 1.15f) * signY).normalized;
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gamePlayManager = _gameManager.CurrentGamePlayManager;

            GetComponent<SpriteRenderer>().color = _gameManager.OptionsManager.GetColor();
        }
        
        private void FixedUpdate()
        {
            if (!_gamePlayManager.IsPlayingState) return;

            var tr = transform;

            tr.Translate(direction * (_speed * Time.deltaTime));

            var position = tr.position;

            if (position.x + radius >= GameManager.TopRight.x && direction.x > 0
                || position.x - radius <= GameManager.BottomLeft.x && direction.x < 0)
                direction.x = -direction.x;

            if (position.y - radius >= GameManager.TopRight.y && direction.y > 0
                || position.y + radius <= GameManager.BottomLeft.y && direction.y < 0)
            {
                FlyAwayFromScreen?.Invoke(this, null);
            }
        }

        public void SwitchYDirection(float offsetX)
        {
            direction.y = -direction.y;

            var signX = offsetX > 0 ? 1 : -1;
            var signY = direction.y > 0 ? 1 : -1;

            direction = new Vector2(Random.Range(0.35f, 0.65f) * signX, Random.Range(.85f, 1.15f) * signY).normalized;
        }

        public void OnCollisionWithPaddle(Collider2D e)
        {
            CollisionWithPaddle?.Invoke(this, e);
        }
    }
}