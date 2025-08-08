using System;
using Game.TimeMove.Event;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.TimeMove.Service
{
    [UsedImplicitly]
    public class EndDayService : IInitializable, IDisposable
    {
        private readonly ISubscriber<string, DayFinishedEvent> _dayFinishedSubscriber;

        private IDisposable? _disposable;
        
        public EndDayService(ISubscriber<string, DayFinishedEvent> dayFinishedSubscriber)
        {
            _dayFinishedSubscriber = dayFinishedSubscriber;
        }

        public void Initialize()
        {
            _disposable = _dayFinishedSubscriber.Subscribe(DayFinishedEvent.DAY_FINISHED, OnDayFinished);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnDayFinished(DayFinishedEvent evt)
        {
            // todo neiran impl day finish
            Debug.LogWarning("Day finished!");
        }
    }
}