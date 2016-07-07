using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace UnitTestHelpers.Tests
{
    public static class PropertyChangedHelper
    {
        public static IPropertyChanges<T> WatchPropertyChanges<T>(this INotifyPropertyChanged propertyChanged, string propertyName)
        {
            if (propertyChanged == null) throw new ArgumentNullException(nameof(propertyChanged));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            return new PropertyChangedEnumerable<T>(propertyChanged, propertyName);
        }

        private class PropertyChangedEnumerable<T> : IPropertyChanges<T>
        {
            private readonly List<T> _values = new List<T>();
            private readonly Func<T> _getPropertyValue;
            private readonly string _propertyName;
            private readonly List<Tuple<Func<T, bool>, EventWaitHandle>> _WaitHandles = new List<Tuple<Func<T, bool>, EventWaitHandle>>();

            public PropertyChangedEnumerable(INotifyPropertyChanged propertyChanged, string propertyName)
            {
                _propertyName = propertyName;

                const BindingFlags flags = BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public;
                var propertyInfo = propertyChanged.GetType().GetProperty(propertyName, flags);
                if (propertyInfo == null) throw new ArgumentException($"Could not find public propert getter for {propertyName} on {propertyChanged.GetType().FullName}");

                var instance = Expression.Constant(propertyChanged);
                var propertyExpression = Expression.Property(instance, propertyInfo);
                _getPropertyValue = Expression.Lambda<Func<T>>(propertyExpression).Compile();

                propertyChanged.PropertyChanged += OnPropertyChanged;
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.Equals(_propertyName, e.PropertyName, StringComparison.Ordinal))
                {
                    var value = _getPropertyValue();
                    _values.Add(value);
                    _WaitHandles.ForEach(t =>
                    {
                        if (t.Item1(value)) t.Item2.Set();
                    });
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public WaitHandle WaitForChange()
            {
                return WaitFor(x => true);
            }

            public WaitHandle WaitFor(Func<T, bool> predicate)
            {
                EventWaitHandle mre = new ManualResetEvent(false);
                _WaitHandles.Add(Tuple.Create(predicate, mre));
                return mre;
            }
        }
    }

    public interface IPropertyChanges<out T> : IEnumerable<T>
    {
        WaitHandle WaitForChange();
        WaitHandle WaitFor(Func<T, bool> predicate);
    }
}