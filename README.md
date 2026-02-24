[![Code Shelter](https://www.codeshelter.co/static/badges/badge-flat.svg)](https://www.codeshelter.co/)

## Original Project
- [Notify](http://buunguyen.github.com/notify)

## About Notify
A library that simplifies change tracking for `INotifyPropertyChanged` and `INotifyCollectionChanged` data sources. Track a root object and Notify will recursively monitor it and all reachable properties and collection elements, firing a single `Changed` event whenever anything changes.

## Using Notify
Install via NuGet:
```
dotnet add package Notify.ChangeTracking
```

Create a `Tracker` instance to track your objects and handle its `Changed` event:
```csharp
var tracker = new Tracker().Track(root1, root2);
tracker.Changed += _ => EnableSave();

// ...sometime later
tracker.Dispose(); // stop bothering me
```

That's it! The unit tests include detailed usage of the library. Go take a look and have fun being notified of changes.

## Features

- Recursively tracks `INotifyPropertyChanged` and `INotifyCollectionChanged` objects
- Handles circular references without stack overflow
- Tracks indexer change notifications
- Supports objects implementing both `INotifyPropertyChanged` and `INotifyCollectionChanged`
- Correctly handles duplicate elements in collections
- Class-level exclusion via `[TrackClass(IsExcluded = true)]`
- Property-level control via `[TrackMember]` and `[TrackMember(IsExcluded = true)]`
- Configurable explicit marking with `[TrackClass(RequireExplicitMarking = true)]`

## Changes from original version

- **Upgraded to .NET 10** with SDK-style projects
- **Fixed circular reference handling** - objects with circular property or collection references no longer cause a stack overflow
- **Fixed indexer tracking** - changes to indexer properties now correctly fire change notifications
- **Added class exclusion** - apply `[TrackClass(IsExcluded = true)]` to prevent a class from being tracked
- **Fixed collection clear with duplicates** - clearing a collection containing the same object multiple times now properly disposes all tracked objects
