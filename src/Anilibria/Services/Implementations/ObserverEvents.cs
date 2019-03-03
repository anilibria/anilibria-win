using System;
using System.Collections.Generic;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Observer service.
	/// </summary>
	public static class ObserverEvents {

		private static readonly Dictionary<string , ICollection<Action>> m_EventSubscribers = new Dictionary<string , ICollection<Action>> ();

		/// <summary>
		/// Subscribe on event.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="action">Action.</param>
		public static void SubscribeOnEvent ( string eventName , Action action ) {
			if ( !m_EventSubscribers.ContainsKey ( eventName ) ) m_EventSubscribers.Add ( eventName , new List<Action> () );

			m_EventSubscribers[eventName].Add ( action );
		}

		/// <summary>
		/// Unsubscribe from event.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="action">Action.</param>
		public static void UnsubscribeFromEvent ( string eventName , Action action ) {
			if ( !m_EventSubscribers.ContainsKey ( eventName ) ) return;

			m_EventSubscribers[eventName].Remove ( action );
		}

		/// <summary>
		/// Fire event.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		public static void FireEvent ( string eventName ) {
			if ( !m_EventSubscribers.ContainsKey ( eventName ) ) return;

			foreach ( var subscriber in m_EventSubscribers[eventName] ) subscriber ();
		}

	}

}
