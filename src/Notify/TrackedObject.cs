namespace Notify
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a tracked object and wraps the original object.
    /// </summary>
    internal abstract class TrackedObject : IDisposable
    {
        public event Changed Changed;
        protected readonly object Tracked;

        protected TrackedObject(object tracked)
        {
            Tracked = tracked;
        }

        /// <summary>
        /// Factory method to create a correct subclass of <see cref="TrackedObject"/>.
        /// </summary>
        /// <param name="obj">The object to be tracked.</param>
        /// <returns>An instance of a subclass of <see cref="TrackedObject"/>.</returns>
        internal static TrackedObject Create(object obj)
        {
            return Create(obj, null);
        }

        internal static TrackedObject Create(object obj, HashSet<object> visited)
        {
            if (!IsValidObjectType(obj))
                throw new ArgumentException("null or invalid object type");

            if (IsClassExcluded(obj))
                return null;

            visited ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
            if (!visited.Add(obj))
                return null;

            TrackedObject trackedObject;
            if (obj is INotifyCollectionChanged && obj is INotifyPropertyChanged)
                trackedObject = new DualTrackedObject(obj);
            else if (obj is INotifyCollectionChanged)
                trackedObject = new CollectionChangedTrackObject(obj);
            else
                trackedObject = new PropertyChangedTrackedObject(obj);

            trackedObject.RegisterTrackedObject(visited);
            visited.Remove(obj);
            return trackedObject;
        }

        protected static bool IsValidObjectType(object obj)
        {
            return obj is INotifyPropertyChanged ||
                   (obj is INotifyCollectionChanged && obj is IEnumerable);
        }

        private static bool IsClassExcluded(object obj)
        {
            var attrs = obj.GetType().GetCustomAttributes(typeof(TrackClassAttribute), false);
            if (attrs.Length > 0)
            {
                var attr = (TrackClassAttribute)attrs[0];
                return attr.IsExcluded;
            }
            return false;
        }

        internal abstract void RegisterTrackedObject(HashSet<object> visited);

        internal abstract void UnregisterTrackedObject();

        protected void OnChange(Tracker tracker = null)
        {
            if (Changed != null) Changed(null);
        }

        public void Dispose()
        {
            Changed = null;
            UnregisterTrackedObject();
        }

        private class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
