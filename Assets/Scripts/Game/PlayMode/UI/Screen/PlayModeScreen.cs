using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.PlayMode.UI.Component;
using UnityEngine;

namespace Game.PlayMode.UI.Screen
{
    [PrefabPath("UI/Dialogs/PlayMode/PlayModeScreen")]
    public class PlayModeScreen : MonoBehaviour
    {
        [ComponentBinding("Crosshair")]
        private CrosshairController _crosshairController;

        public UniTask ShowCrosshair(bool instantly = false)
        {
            return _crosshairController.Show(true);
        }
        
        public UniTask FadeCrosshair(bool instantly = false)
        {
            return _crosshairController.Fade(true);
        }
        
        public void SetColor(Color color)
        {
            _crosshairController.ChangeColor(color);
        }
    }
}