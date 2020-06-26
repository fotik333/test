using Interfaces;
using Managers;
using UnityEngine;

namespace GameObjects
{
    public class Paddle : MonoBehaviour
    {
        private Camera _camera;
        private float _length;
    
        private IGamePlayManager _gamePlayManager;
        private IPaddleBehaviour _paddleBehaviour;
    
        public void Init(bool isBottom)
        {
            transform.position = isBottom ? new Vector2(0, GameManager.BottomLeft.y + transform.localScale.y) : new Vector2(0, GameManager.TopRight.y - transform.localScale.y);
        }

        private void Start()
        {
            _camera = Camera.main;
            _length = transform.localScale.x;

            _gamePlayManager = GameManager.Instance.CurrentGamePlayManager;
            _paddleBehaviour = GetComponent<IPaddleBehaviour>();
        }

        private void FixedUpdate()
        {
            if (!Input.GetMouseButton(0) || _gamePlayManager.IsWaitingState || !_paddleBehaviour.IsLocalPlayer()) return;
        
            var mousePositionX = _camera.ScreenPointToRay(Input.mousePosition).origin.x;
            float offset, newPositionX;
        
            if (mousePositionX <= (offset = GameManager.BottomLeft.x + _length / 2))
            {
                newPositionX = offset;
            }
            else if (mousePositionX >= (offset = GameManager.TopRight.x - _length / 2))
            {
                newPositionX = offset;  
            }
            else
            {
                newPositionX = mousePositionX;                
            }

            transform.position = new Vector3(newPositionX, transform.position.y, -1);
        }
    }
}
