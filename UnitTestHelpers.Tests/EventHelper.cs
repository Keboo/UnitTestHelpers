using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UnitTestHelpers.Tests
{
    public static class EventHelper
    {
        public static Event WatchEvent<T>(this T @object, string eventName) where T : class
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            
            return WatchEvent<T>((object)@object, eventName);
        }

        public static Event WatchStaticEvent<T>(string eventName) where T : class
        {
            return WatchEvent<T>((object)null, eventName);
        }

        private static Event WatchEvent<T>(object source, string eventName)
        {
            var eventInfo = typeof(T).GetEvent(eventName) ?? source?.GetType().GetEvent(eventName);
            if (eventInfo == null) throw new ArgumentException($"Could not find event {eventName} on {typeof(T).FullName}");


            var invokeMethod = eventInfo.EventHandlerType.GetMethod(nameof(EventHandler.Invoke));
            if (invokeMethod == null) throw new MissingMethodException(eventInfo.EventHandlerType.FullName, nameof(EventHandler.Invoke));

            var eventInvokedMethod = typeof(Event).GetMethod(nameof(Event.EventInvoked));

            var rv = new Event();
            var instance = Expression.Constant(rv);
            var parameters = invokeMethod.GetParameters().Select(x => Expression.Parameter(x.ParameterType)).ToList();
            var convertedParams = parameters.Select(x => Expression.Convert(x, typeof(object)));
            var array = Expression.NewArrayInit(typeof(object), convertedParams);
            var methodInvoke = Expression.Call(instance, eventInvokedMethod, array);
            var eventHandler = Expression.Lambda(eventInfo.EventHandlerType, methodInvoke, parameters);
            //This expression is roughly equivalent to:
            //event += (p1, p2, ..., pn) => rv.EventInvoked(new[] {(object)p1, (object)p2, ..., (object)pn});
            eventInfo.AddEventHandler(source, eventHandler.Compile());

            return rv;
        }

        public class Event
        {
            private readonly List<EventInvocation> _invocations = new List<EventInvocation>();

            public void EventInvoked(object[] parameters)
            {
                _invocations.Add(new EventInvocation(parameters));
            }

            public bool Raised => _invocations.Any();

            public IReadOnlyList<EventInvocation> Invocations => _invocations.AsReadOnly();
        }

        public class EventInvocation
        {
            public IReadOnlyList<object> Values { get; }

            public EventInvocation(object[] values)
            {
                Values = values;
            }

            public TArgs GetEventArgs<TArgs>() where TArgs : EventArgs
            {
                return Values.Count == 2 ? Values[1] as TArgs : default(TArgs);
            }
        }
    }
}