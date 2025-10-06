using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Descriptor;
using Game.Calendar.Event;
using Game.Calendar.Model;
using Game.Calendar.Repo;
using Game.Difficulty.Model;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Calendar.Service
{
    [Service]
    public class TimeService : IInitializable
    {
        private readonly TimeRepo _timeRepo;
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, TimeChangeEvent> _timeChangePublisher;
        private readonly IPublisher<string, DayChangedEvent> _dayFinishedPublisher;

        public TimeService(TimeRepo timeRepo,
                           IPublisher<string, TimeChangeEvent> timeChangePublisher,
                           IPublisher<string, DayChangedEvent> dayFinishedPublisher,
                           IDescriptorService descriptorService)
        {
            _timeRepo = timeRepo;
            _timeChangePublisher = timeChangePublisher;
            _dayFinishedPublisher = dayFinishedPublisher;
            _descriptorService = descriptorService;
        }

        public void Initialize()
        {
            if (_timeRepo.Exists()) {
                return;
            }

            _timeRepo.Save(new(Constants.Constants.StartDayTime, 1, 4, 0));
        }

        public void PassTime(int minutes)
        {
            if (minutes < 0) {
                Debug.LogWarning("Can't pass time in backwards!");
                return;
            }

            TimeModel timeModel = _timeRepo.Require();
            int newTime = Mathf.Min(timeModel.CurrentMinutes + minutes, Constants.Constants.EndDayTime);
            int diff = newTime - timeModel.CurrentMinutes;
            timeModel.CurrentMinutes = newTime;

            int savedCurrentDay = timeModel.CurrentDay;

            if (timeModel.CurrentMinutes >= Constants.Constants.EndDayTime) {
                PassDay(timeModel, savedCurrentDay);
            }

            _timeChangePublisher.Publish(TimeChangeEvent.PASSED, new(diff, newTime));
            Debug.Log($"Time passed for {minutes}. Current time: {timeModel.CurrentMinutes}");
            _timeRepo.Save(timeModel);
        }

        public TimeModel EndDay()
        {
            TimeModel timeModel = _timeRepo.Require();
            int diff = Constants.Constants.EndDayTime - timeModel.CurrentMinutes;

            int savedCurrentDay = timeModel.CurrentDay;
            PassDay(timeModel, savedCurrentDay);
            _timeChangePublisher.Publish(TimeChangeEvent.PASSED, new(diff, timeModel.CurrentMinutes));
            return timeModel;
        }

        private void PassDay(TimeModel timeModel, int currentDay)
        {
            CalendarDescriptor calendarDescriptor = _descriptorService.Require<CalendarDescriptor>();
            CalendarMonthModel currentMonthDescriptor = calendarDescriptor.FindByType(DachaPlaceType.Middle, (MonthType) timeModel.CurrentMonth);
            if (currentMonthDescriptor.DaysCount <= currentDay) {
                timeModel.CurrentDay = 0;
                if (timeModel.CurrentMonth >= 12) {
                    timeModel.CurrentMonth = 1;
                    timeModel.CurrentYear += 1;
                } else {
                    timeModel.CurrentMonth++;
                }
            }

            timeModel.CurrentDay++;
            timeModel.CurrentMinutes = 0;
            _dayFinishedPublisher.Publish(DayChangedEvent.DAY_FINISHED, new(timeModel.CurrentDay, 1));
            Debug.Log($"Day passed! Current time: {timeModel.CurrentMinutes}");
        }

        public void StartDay()
        {
            TimeModel timeModel = _timeRepo.Require();
            _dayFinishedPublisher.Publish(DayChangedEvent.DAY_STARTED, new(timeModel.CurrentDay, 0));
        }

        public void SetTime(int month, int day)
        {
            TimeModel timeModel = _timeRepo.Require();
            timeModel.CurrentMonth = month;
            timeModel.CurrentDay = day;
        }

        public void SetMonth(int month)
        {
            TimeModel timeModel = _timeRepo.Require();
            timeModel.CurrentMonth = month;
        }

        public void SetDay(int day)
        {
            TimeModel timeModel = _timeRepo.Require();
            timeModel.CurrentDay = day;
        }

        public int GetTimeMinutes()
        {
            return _timeRepo.Require().CurrentMinutes;
        }

        public int GetTimeDays()
        {
            return _timeRepo.Require().CurrentDay;
        }

        public int GetPassedGlobalTime()
        {
            TimeModel timeModel = _timeRepo.Require();
            return timeModel.CurrentDay * Constants.Constants.EndDayTime + timeModel.CurrentDay;
        }

        public TimeModel GetToday()
        {
            TimeModel timeModel = _timeRepo.Require();
            return new(0, timeModel.CurrentDay, timeModel.CurrentMonth, 0);
        }
    }
}