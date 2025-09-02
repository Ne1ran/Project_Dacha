using System;
using System.Globalization;
using Core.Resources.Binding.Attributes;
using Game.Common.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Game.Common.Controller
{
    [PrefabPath("UI/Prefabs/pfSliderSelector")]
    public class SliderSelectorComponent : MonoBehaviour
    {
        [ComponentBinding("Slider")]
        private readonly Slider _slider = null!;
        [ComponentBinding("DescriptionText")]
        private readonly TextMeshProUGUI _descriptionText = null!;
        [ComponentBinding("LeftSliderText")]
        private readonly TextMeshProUGUI _leftSliderText = null!;
        [ComponentBinding("RightSliderText")]
        private readonly TextMeshProUGUI _rightSliderText = null!;
        [ComponentBinding("CurrentValueText")]
        private readonly TextMeshProUGUI _currentValueText = null!;
        [ComponentBinding("AcceptButton")]
        private readonly Button _acceptButton = null!;
        [ComponentBinding("RejectButton")]
        private readonly Button _rejectButton = null!;
        [ComponentBinding("Background")]
        private readonly ClickableControl _backgroundClickable = null!;

        private float _currentValue;

        private float _minValue;
        private float _maxValue;
        private float _valuesDifference;
        private float _stepsCount;
        private int _roundDigits;

        public event Action? OnBackPressed;
        public event Action<float>? OnAcceptPressed;

        private void Awake()
        {
            _acceptButton.onClick.AddListener(OnAcceptClicked);
            _rejectButton.onClick.AddListener(OnGoBack);
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            _backgroundClickable.OnClick += OnGoBack;
        }
        
        private void OnSliderValueChanged(float newValue)
        {
            float steppedValue = Mathf.CeilToInt(newValue * _stepsCount) / _stepsCount;
            _slider.SetValueWithoutNotify(steppedValue);
            _currentValue = steppedValue;
            CurrentValueText = RealValue;
        }

        private void OnDestroy()
        {
            _acceptButton.onClick.RemoveListener(OnAcceptClicked);
            _rejectButton.onClick.RemoveListener(OnGoBack);
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            _backgroundClickable.OnClick -= OnGoBack;
        }

        public void Initialize(SliderSelectorModel sliderSelectorModel)
        {
            _minValue = sliderSelectorModel.MinValue;
            _maxValue = sliderSelectorModel.MaxValue;
            _roundDigits = sliderSelectorModel.RoundDigits;
            _stepsCount = sliderSelectorModel.StepsCount;
            _currentValue = sliderSelectorModel.StartValue / _maxValue;
            
            MinValue = _minValue;
            MaxValue = _maxValue;
            CurrentValueText = RealValue;
        }

        private void OnAcceptClicked()
        {
            OnAcceptPressed?.Invoke(RealValue);
        }

        private void OnGoBack()
        {
            OnBackPressed?.Invoke();
        }

        private float RealValue => Mathf.Clamp(_currentValue * _maxValue, _minValue, _maxValue);

        private float CurrentValueText
        {
            set => _currentValueText.text = Math.Round(value, _roundDigits).ToString(CultureInfo.InvariantCulture);
        }

        private float MinValue
        {
            set => _leftSliderText.text = value.ToString(CultureInfo.InvariantCulture);
        }

        private float MaxValue
        {
            set => _rightSliderText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}