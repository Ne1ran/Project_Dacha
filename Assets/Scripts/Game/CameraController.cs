using UnityEngine;

namespace Game
{
    public class CameraScript : MonoBehaviour
    {
        private Transform _transform = null!;
        private Transform _player = null!;
    
        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            LockOnPlayer();
        }

        private void Update()
        {
            _transform.LookAt(_player);
        }

        private void LockOnPlayer()
        {
            GameObject player = GameObject.Find("Player");
            if (player == null) {
                Debug.LogWarning("Player not found??");
                return;
            }
        
            _player = player.transform;
        
            Vector3 playerPosition = _player.position;
        
            Vector3 newPosition = playerPosition + new Vector3(0f, 2f, -2.5f);
            _transform.position = newPosition;
        }
    }
}
