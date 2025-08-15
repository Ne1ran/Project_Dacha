using Core.Attributes;
using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.PlayMode.UI.Screen;

namespace Game.PlayMode.Service
{
    [Service]
    public class PlayModeService
    {
        private readonly UIService _uiService;

        public PlayModeService(UIService uiService)
        {
            _uiService = uiService;
        }

        public PlayModeScreen PlayModeScreen { get; private set; } = null!;
        
        public async UniTask<PlayModeScreen> CreatePlayModeScreen()
        {
            PlayModeScreen playModeScreen = await _uiService.ShowDialogAsync<PlayModeScreen>();
            await playModeScreen.InitializeAsync();
            PlayModeScreen = playModeScreen;
            return playModeScreen;
        }
    }
}