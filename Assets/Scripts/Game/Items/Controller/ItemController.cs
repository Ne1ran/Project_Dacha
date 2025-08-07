using Game.Utils;
using UnityEngine;

namespace Game.Items.Controller
{
    public class ItemController : MonoBehaviour
    {
        private Rigidbody _rigidbody = null!;

        private void Awake()
        {
            _rigidbody = this.RequireComponent<Rigidbody>();
        }

        public bool IsKinematic
        {
            get => _rigidbody.isKinematic;
            set => _rigidbody.isKinematic = value;
        }
    }
}