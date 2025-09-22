using System;
using Core.Attributes;
using Game.Calendar.Event;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Calendar.Service
{
    [Service]
    public class EndDayService : IInitializable, IDisposable
    {
        private readonly ISubscriber<string, DayChangedEvent> _dayFinishedSubscriber;

        private IDisposable? _disposable;
        
        public EndDayService(ISubscriber<string, DayChangedEvent> dayFinishedSubscriber)
        {
            _dayFinishedSubscriber = dayFinishedSubscriber;
        }

        public void Initialize()
        {
            _disposable = _dayFinishedSubscriber.Subscribe(DayChangedEvent.DAY_FINISHED, OnDayFinished);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnDayFinished(DayChangedEvent evt)
        {
            // todo neiran impl day finish
            Debug.LogWarning("Day finished!");
        }
    }
}