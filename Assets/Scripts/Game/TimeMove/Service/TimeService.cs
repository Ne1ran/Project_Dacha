using Core.Attributes;
using Game.TimeMove.Event;
using Game.TimeMove.Model;
using Game.TimeMove.Repo;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.TimeMove.Service
{
    [Service]
    public class TimeService : IInitializable
    {
        private readonly TimeRepo _timeRepo;
        private readonly IPublisher<string, TimeChangeEvent> _timeChangePublisher;
        private readonly IPublisher<string, DayChangedEvent> _dayFinishedPublisher;

        public TimeService(TimeRepo timeRepo,
                           IPublisher<string, TimeChangeEvent> timeChangePublisher,
                           IPublisher<string, DayChangedEvent> dayFinishedPublisher)
        {
            _timeRepo = timeRepo;
            _timeChangePublisher = timeChangePublisher;
            _dayFinishedPublisher = dayFinishedPublisher;
        }

        public void Initialize()
        {
            if (_timeRepo.Exists()) {
                return;
            }

            _timeRepo.Save(new(Constants.Constants.START_DAY_TIME, 0));
        }

        public void PassTime(int minutes)
        {
            if (minutes < 0) {
                Debug.LogWarning("Can't pass time in backwards!");
                return;
            }

            TimeModel timeModel = _timeRepo.Require();
            int newTime = Mathf.Min(timeModel.CurrentMinutes + minutes, Constants.Constants.END_DAY_TIME);
            int diff = newTime - timeModel.CurrentMinutes;
            timeModel.CurrentMinutes = newTime;

            if (timeModel.CurrentMinutes >= Constants.Constants.END_DAY_TIME) {
                timeModel.CurrentDay++;
                timeModel.CurrentMinutes = 0;
                _dayFinishedPublisher.Publish(DayChangedEvent.DAY_FINISHED, new(timeModel.CurrentDay));
                Debug.Log($"Day passed! Current time: {timeModel.CurrentMinutes}");
            }

            _timeChangePublisher.Publish(TimeChangeEvent.PASSED, new(diff, newTime));
            Debug.Log($"Time passed for {minutes}. Current time: {timeModel.CurrentMinutes}");
            _timeRepo.Save(timeModel);
        }

        public void EndDay()
        {
            TimeModel timeModel = _timeRepo.Require();
            int diff = Constants.Constants.END_DAY_TIME - timeModel.CurrentMinutes;

            timeModel.CurrentDay++;
            timeModel.CurrentMinutes = 0;
            _dayFinishedPublisher.Publish(DayChangedEvent.DAY_FINISHED, new(timeModel.CurrentDay));
            _timeChangePublisher.Publish(TimeChangeEvent.PASSED, new(diff, timeModel.CurrentMinutes));
        }

        public void StartDay()
        {
            TimeModel timeModel = _timeRepo.Require();
            _dayFinishedPublisher.Publish(DayChangedEvent.DAY_STARTED, new(timeModel.CurrentDay));
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
            return timeModel.CurrentDay * Constants.Constants.END_DAY_TIME + timeModel.CurrentDay;
        }
    }
}