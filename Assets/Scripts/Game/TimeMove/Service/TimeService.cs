using Game.TimeMove.Event;
using Game.TimeMove.Model;
using Game.TimeMove.Repo;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.TimeMove.Service
{
    [UsedImplicitly]
    public class TimeService : IInitializable
    {
        private readonly TimeRepo _timeRepo;
        private readonly IPublisher<string, TimeChangeEvent> _timeChangePublisher; 
        private readonly IPublisher<string, DayFinishedEvent> _dayFinishedPublisher;

        public TimeService(TimeRepo timeRepo, IPublisher<string, TimeChangeEvent> timeChangePublisher, IPublisher<string, DayFinishedEvent> dayFinishedPublisher)
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
            
            _timeRepo.Save(new(Constants.Constants.START_DAY_TIME));
        }
        
        public bool TryPassTime(int minutes)
        {
            TimeModel timeModel = _timeRepo.Require();
            if (timeModel.CurrentTime + minutes > Constants.Constants.END_DAY_TIME) {
                return false;
            }

            timeModel.CurrentTime += minutes;
            _timeRepo.Save(timeModel);

            if (timeModel.CurrentTime >= Constants.Constants.END_DAY_TIME) {
                _dayFinishedPublisher.Publish(DayFinishedEvent.DAY_FINISHED, new());
            }
            
            _timeChangePublisher.Publish(TimeChangeEvent.PASSED, new(minutes, timeModel.CurrentTime));
            Debug.Log($"Time passed for {minutes}. Current time: {timeModel.CurrentTime}");
            return true;
        }

        public int GetTime()
        {
            return _timeRepo.Require().CurrentTime;
        }
    }
}