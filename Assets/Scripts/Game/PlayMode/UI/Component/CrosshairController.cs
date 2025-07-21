using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PlayMode.UI.Component
{
    [RequireComponent(typeof(Image))]
    public class CrosshairController : MonoBehaviour
    {
        private Image _crosshairImage = null!;

        private void Awake()
        {
            _crosshairImage = this.RequireComponentInChildren<Image>("Crosshair");
        }

        public void ChangeColor(Color color)
        {
            _crosshairImage.color = color;
        }

        public UniTask Fade(bool instantly = false)
        {
            //todo neiran add dotween fade and etc
            _crosshairImage.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public UniTask Show(bool instantly = false)
        {
            _crosshairImage.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }
    }
}