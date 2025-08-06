using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.PlayMode.UI.Screen;
using JetBrains.Annotations;

namespace Game.PlayMode.Service
{
    [UsedImplicitly]
    public class PlayModeService
    {
        private readonly UIService _uiService;

        public PlayModeService(UIService uiService)
        {
            _uiService = uiService;
        }

        public PlayModeScreen PlayModeScreen { get; private set; }
        
        public async UniTask<PlayModeScreen> CreatePlayModeScreen()
        {
            PlayModeScreen playModeScreen = await _uiService.ShowDialogAsync<PlayModeScreen>();
            await playModeScreen.InitializeAsync();
            PlayModeScreen = playModeScreen;
            return playModeScreen;
        }
    }
}