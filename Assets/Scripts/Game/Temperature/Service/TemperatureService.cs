using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Model;
using Game.Calendar.Service;
using Game.Difficulty.Model;
using Game.Temperature.Descriptor;
using Game.Temperature.Model;
using UnityEngine.Rendering;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Temperature.Service
{
    [Service]
    public class TemperatureService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly TimeService _timeService;
        private readonly CalendarService _calendarService;

        public TemperatureService(IDescriptorService descriptorService, TimeService timeService, CalendarService calendarService)
        {
            _descriptorService = descriptorService;
            _timeService = timeService;
            _calendarService = calendarService;
        }

        public float GetDailyAverageTemperature()
        {
            TimeModel today = _timeService.GetToday();
            return _calendarService.GetAverageTemperature(today.CurrentDay, today.CurrentMonth);
        }

        public float GetDayTemperature()
        {
            TimeModel today = _timeService.GetToday();
            return _calendarService.GetDayTemperature(today.CurrentDay, today.CurrentMonth);
        }

        public float GetNightTemperature()
        {
            TimeModel today = _timeService.GetToday();
            return _calendarService.GetNightTemperature(today.CurrentDay, today.CurrentMonth);
        }

        public float GetCurrentTemperature()
        {
            TimeModel now = _timeService.GetToday();
            TemperatureModel temperatureModel = _calendarService.GetTemperatureModel(now.CurrentDay, now.CurrentMonth);
            TemperatureDistributionDescriptor temperatureDistributionDescriptor = _descriptorService.Require<TemperatureDistributionDescriptor>();
            TemperatureDistributionModelDescriptor distributionModelDescriptor =
                    temperatureDistributionDescriptor.FindByPlaceType(DachaPlaceType.Middle);
            SerializedDictionary<int, float> distribution = distributionModelDescriptor.TemperatureDistribution;

            float minTemperature = temperatureModel.NightTemperature;
            float maxTemperature = temperatureModel.DayTemperature;

            int minutes = _timeService.GetTimeMinutes();
            return EvaluateTemperatureAtMinutes(minutes, distribution, minTemperature, maxTemperature);
        }

        public TemperatureModel GetTemperatureModel()
        {
            TimeModel today = _timeService.GetToday();
            return _calendarService.GetTemperatureModel(today.CurrentDay, today.CurrentMonth);
        }

        private float EvaluateTemperatureAtMinutes(int minutes,
                                                   SerializedDictionary<int, float> distribution,
                                                   float minTemperature,
                                                   float maxTemperature)
        {
            float distributionValue = GetInterpolatedDistributionValue(minutes, distribution);
            return Mathf.Lerp(minTemperature, maxTemperature, distributionValue);
        }

        private float GetInterpolatedDistributionValue(int minutes, SerializedDictionary<int, float> distribution)
        {
            if (distribution.Count == 0) {
                return 0f;
            }

            List<int> hours = new(distribution.Keys);
            if (hours.Count == 1) {
                return distribution[hours[0]];
            }

            hours.Sort();

            int prevHour = hours[0];
            int nextHour = hours[0];

            float hour = Mathf.Repeat(minutes / 60f, 24f);

            foreach (int h in hours) {
                if (h <= hour) {
                    prevHour = h;
                }

                if (h > hour) {
                    nextHour = h;
                    break;
                }
            }

            if (nextHour <= prevHour) {
                nextHour = hours[0];
            }

            float prevHourValue = distribution[prevHour];
            float nextHourValue = distribution[nextHour];

            int hourDiff = nextHour - prevHour;
            float delta = hour - prevHour;
            float t = Mathf.Clamp01(delta / hourDiff);

            float sinusGraphPoint = (1f - Mathf.Cos(Mathf.PI * t)) * 0.5f;
            return prevHourValue * (1f - sinusGraphPoint) + nextHourValue * sinusGraphPoint;
        }
    }
}