using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;

namespace UnitTestHelpers.Tests
{
    [TestClass]
    public class MyEventsTests
    {
        [TestMethod]
        public void TestEventHandler()
        {
            //Arrange
            var sut = new MyEvents();
            var @event = sut.WatchEvent( nameof( MyEvents.EventHandler ) );
            var sender = new object();
            var e = new EventArgs();

            //Act
            sut.RaiseEventHandler( sender, e );

            //Assert
            Assert.IsTrue( @event.Raised );
            Assert.AreEqual( sender, @event.Invocations[0].Values[0] );
            Assert.AreEqual( e, @event.Invocations[0].GetEventArgs<EventArgs>() );
        }

        [TestMethod]
        public void TestEventHandlerWithArgs()
        {
            //Arrange
            var sut = new MyEvents();
            var @event = sut.WatchEvent( nameof( MyEvents.EventWithArgs ) );
            var sender = new object();
            var e = new CancelEventArgs();

            //Act
            sut.RaiseEventWithArgs( sender, e );

            //Assert
            Assert.IsTrue( @event.Raised );
            Assert.AreEqual( sender, @event.Invocations[0].Values[0] );
            Assert.AreEqual( e, @event.Invocations[0].GetEventArgs<CancelEventArgs>() );
        }

        [TestMethod]
        public void TestCustomEvent()
        {
            //Arrange
            var sut = new MyEvents();
            var @event = sut.WatchEvent( nameof( MyEvents.CustomEvent ) );

            //Act
            sut.RaiseCustomEvent( 42, true );

            //Assert
            Assert.IsTrue( @event.Raised );
            Assert.AreEqual( 42, @event.Invocations[0].Values[0] );
            Assert.AreEqual( true, @event.Invocations[0].Values[1] );
        }

        [TestMethod]
        public void TestActionEvent()
        {
            //Arrange
            var sut = new MyEvents();
            var @event = sut.WatchEvent( nameof( MyEvents.ActionEvent ) );

            //Act
            sut.RaiseActionEvent();

            //Assert
            Assert.IsTrue( @event.Raised );
            Assert.AreEqual( 1, @event.Invocations.Count );
        }

        [TestMethod]
        public void TestEventHandlerFromBaseClass()
        {
            //Arrange
            object sut = new MyEvents();
            var @event = sut.WatchEvent( nameof( MyEvents.EventHandler ) );

            //Act
            ( (MyEvents)sut ).RaiseEventHandler( null, EventArgs.Empty );

            //Assert
            Assert.IsTrue( @event.Raised );
        }

        [TestMethod]
        public void TestStaticEventHandler()
        {
            //Arrange
            var @event = EventHelper.WatchStaticEvent<MyEvents>( nameof( MyEvents.StaticEvent ) );

            //Act
            MyEvents.RaiseStaticEvent( null, EventArgs.Empty );

            //Assert
            Assert.IsTrue( @event.Raised );
        }
    }
}
