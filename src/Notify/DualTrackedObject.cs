namespace Notify
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A composite object wrapping both <see cref="CollectionChangedTrackObject"/>
    /// and <see cref="PropertyChangedTrackedObject"/> to support tracked object
    /// implementing both <see cref="INotifyPropertyChanged"/> and <see cref="INotifyCollectionChanged"/>.
    /// </summary>
    internal class DualTrackedObject : TrackedObject
    {
        private TrackedObject _propertyChangedObject;
        private TrackedObject _collectionChangedObject;

        public DualTrackedObject(object tracked) : base(tracked) 
        {
        }

        internal override void RegisterTrackedObject(HashSet<object> visited)
        {
            _propertyChangedObject = new PropertyChangedTrackedObject(Tracked);
            _propertyChangedObject.RegisterTrackedObject(visited);
            _propertyChangedObject.Changed += OnChange;

            _collectionChangedObject = new CollectionChangedTrackObject(Tracked);
            _collectionChangedObject.RegisterTrackedObject(visited);
            _collectionChangedObject.Changed += OnChange;
        }

        internal override void UnregisterTrackedObject()
        {
            _propertyChangedObject.UnregisterTrackedObject();
            _collectionChangedObject.UnregisterTrackedObject();
        }
    }
}
