using System;
using System.Collections.Generic;

namespace Anilibria.GlobalState {

	/// <summary>
	/// Launch parameter.
	/// </summary>
	public static class LaunchParameters {

		private static string m_Arguments;

		/// <summary>
		/// Launch arguments.
		/// </summary>
		public static string Arguments
		{
			get => m_Arguments;
		}

		private static List<Action<string>> m_Subscribers = new List<Action<string>> ();


		public static void SetArguments ( string arguments ) {
			m_Arguments = arguments;

			foreach ( var subscriber in m_Subscribers ) subscriber ( arguments );
		}

		/// <summary>
		/// Add new subscriber.
		/// </summary>
		/// <param name="subscriber">Subscriber handler.</param>
		public static void AddSubscriber ( Action<string> subscriber ) => m_Subscribers.Add ( subscriber );

	}

}
