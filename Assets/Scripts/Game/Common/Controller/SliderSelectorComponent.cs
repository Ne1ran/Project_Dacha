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
    [NeedBinding("UI/Prefabs/pfSliderSelector")]
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

        private float _minValue;
        private float _maxValue;

        private float _stepValue;
        private int _stepsCount;

        private int _roundDigits;
        private float _currentValue;

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
            _currentValue = GetRealValue(newValue);
            float sliderValue = GetSliderValue(_currentValue);
            _slider.SetValueWithoutNotify(sliderValue);
            CurrentValueText = _currentValue;
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
            _stepValue = sliderSelectorModel.StepValue;
            _stepsCount = (int) Mathf.Round((_maxValue - _minValue) / _stepValue) + 1;

            MinValue = _minValue;
            MaxValue = _maxValue;
            CurrentValueText = GetRealValue(GetSliderValue(sliderSelectorModel.StartValue));
        }

        private float GetRealValue(float sliderValue)
        {
            int stepIndex = (int) Mathf.Round(sliderValue * (_stepsCount - 1));
            stepIndex = Mathf.Clamp(stepIndex, 0, _stepsCount - 1);
            return _minValue + stepIndex * _stepValue;
        }

        private float GetSliderValue(float realValue)
        {
            int stepIndex = (int) Mathf.Round((realValue - _minValue) / _stepValue);
            stepIndex = Mathf.Clamp(stepIndex, 0, _stepsCount - 1);
            return (float) stepIndex / (_stepsCount - 1);
        }

        private void OnAcceptClicked()
        {
            OnAcceptPressed?.Invoke(_currentValue);
        }

        private void OnGoBack()
        {
            OnBackPressed?.Invoke();
        }

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