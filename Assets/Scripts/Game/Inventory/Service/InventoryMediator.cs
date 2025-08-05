using System;
using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.Inventory.Event;
using Game.Inventory.UI;
using Game.Player.Service;
using JetBrains.Annotations;
using MessagePipe;
using VContainer.Unity;

namespace Game.Inventory.Service
{
    [UsedImplicitly]
    public class InventoryMediator : IInitializable, IDisposable
    {
        private readonly ISubscriber<string, InventoryStatusEvent> _inventorySubscriber;
        private readonly UIService _uiService;
        private readonly PlayerService _playerService;

        private IDisposable? _disposable;

        private bool _ignore;
        private bool _inventoryOpened;

        public InventoryMediator(ISubscriber<string, InventoryStatusEvent> inventorySubscriber, UIService uiService, PlayerService playerService)
        {
            _inventorySubscriber = inventorySubscriber;
            _uiService = uiService;
            _playerService = playerService;
        }

        public void Initialize()
        {
            DisposableBagBuilder builder = DisposableBag.CreateBuilder();
            builder.Add(_inventorySubscriber.Subscribe(InventoryStatusEvent.INVENTORY_CHANGED, OnInventoryChanged));
            _disposable = builder.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnInventoryChanged(InventoryStatusEvent evt)
        {
            if (_ignore) {
                return;
            }
            
            if (_inventoryOpened) {
                HideInventoryDialogAsync().Forget();
            } else {
                ShowInventoryDialogAsync().Forget();
            }
        }

        private async UniTaskVoid ShowInventoryDialogAsync()
        {
            _ignore = true;
            _playerService.Player.ChangeMovementActive(false);
            InventoryDialog inventoryDialog = await _uiService.ShowDialogAsync<InventoryDialog>();
            await inventoryDialog.InitializeAsync();
            _inventoryOpened = true;
            _ignore = false;
        }

        private async UniTaskVoid HideInventoryDialogAsync()
        {
            _ignore = true;
            await _uiService.HideDialogAsync<InventoryDialog>();
            _playerService.Player.ChangeMovementActive(true);
            _inventoryOpened = false;
            _ignore = false;
        }
    }
}