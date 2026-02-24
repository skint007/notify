// ReSharper disable InconsistentNaming
namespace Notify.Test
{
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Stubs;

    /// <summary>
    /// This contains test cases for previously known limitations that have been resolved.
    /// </summary>
    [TestClass] public class Limitation : BaseTest
    {
        [Timeout(2000)]
        [TestMethod]
        public void Should_handle_circular_references()
        {
            var p = new Person { Spouse = new Person() };
            p.Spouse.Spouse = p;
            Tracker.Track(p);

            p.Name += "(changed)";
            Assert.IsTrue(HasChange);

            p.Spouse.Name += "(changed)";
            Assert.IsTrue(HasChange);
        }

        [TestMethod]
        public void Should_fire_for_indexer()
        {
            var dummy = new IndexerDummy();
            Tracker.Track(dummy);
            dummy[0] = "see my change?";
            Assert.IsTrue(HasChange);
        }

        [TestMethod]
        public void Should_not_fire_if_class_is_excluded()
        {
            var dummy = new ExcludedDummy();
            Tracker.Track(dummy);
            dummy.Name = "changed";
            Assert.IsFalse(HasChange);
        }

        [TestMethod]
        public void Should_not_track_excluded_nested_property()
        {
            var holder = new ExcludedPropertyHolder
            {
                Excluded = new ExcludedDummy()
            };
            Tracker.Track(holder);
            holder.Excluded.Name = "changed";
            Assert.IsFalse(HasChange);
        }

        [Timeout(2000)]
        [TestMethod]
        public void Should_handle_circular_reference_via_collection()
        {
            var p = new Person
            {
                Friends = new ObservableCollection<Person>()
            };
            p.Friends.Add(p);
            Tracker.Track(p);

            p.Name += "(changed)";
            Assert.IsTrue(HasChange);
        }
    }
}
// ReSharper restore InconsistentNaming
