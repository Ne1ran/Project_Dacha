using System;
using Core.Notifications.Model;
using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Notifications.Component
{
    [NeedBinding("pfNotification")]
    public class NotificationController : MonoBehaviour
    {
        [ComponentBinding("Icon")]
        private Image _notificationIcon = null!;
        [ComponentBinding("Title")]
        private TextMeshProUGUI _titleText = null!;
        [ComponentBinding("Description")]
        private TextMeshProUGUI _descriptionText = null!;
        [ComponentBinding]
        private RectTransform _notificationRectTransform = null!;

        public event Action<NotificationModel>? OnNotificationShowed;

        private NotificationModel _model = null!;

        public void Initialize(NotificationModel model)
        {
            _model = model;

            TitleText = model.Title;
            DescriptionText = model.Message;
            ImageIcon = model.Icon;
            Alignment = model.Alignment;

            ActivateNotificationAsync().Forget();
        }

        private async UniTaskVoid ActivateNotificationAsync()
        {
            Debug.LogWarning($"Showing notification={_model.Id}");
            await UniTask.WaitForSeconds(_model.ShowTime);
            Debug.LogWarning($"Showed notification={_model.Id}");
            ShowEnded();
        }

        private void ShowEnded()
        {
            OnNotificationShowed?.Invoke(_model);
        }

        private string TitleText
        {
            set => _titleText.text = value;
        }

        private string DescriptionText
        {
            set => _descriptionText.text = value;
        }

        private Sprite? ImageIcon
        {
            set
            {
                _notificationIcon.SetActive(value != null);
                _notificationIcon.sprite = value;
            }
        }

        private NotificationAlignment Alignment
        {
            set
            {
                Vector2 positionVector;
                switch (value) {
                    case NotificationAlignment.LOWER_LEFT: {
                        positionVector = Vector2.zero;
                        break;
                    }
                    case NotificationAlignment.LOWER_RIGHT: {
                        positionVector = new(1f, 0f);

                        break;
                    }
                    case NotificationAlignment.UPPER_LEFT: {
                        positionVector = new(0f, 1f);

                        break;
                    }
                    case NotificationAlignment.UPPER_RIGHT: {
                        positionVector = Vector2.one;
                        break;
                    }
                    default:
                        Debug.LogWarning($"Alignment format not found={value.ToString()}");
                        positionVector = Vector2.zero;
                        break;
                }

                _notificationRectTransform.pivot = positionVector;
                _notificationRectTransform.anchorMin = positionVector;
                _notificationRectTransform.anchorMax = positionVector;
            }
        }
    }
}