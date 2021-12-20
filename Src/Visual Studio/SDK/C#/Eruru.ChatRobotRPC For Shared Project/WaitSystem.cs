using System;
using System.Collections.Generic;
using System.Threading;

namespace Eruru.ChatRobotRPC {

	class WaitSystem : IDisposable {

		readonly Dictionary<long, Wait> Waits = new Dictionary<long, Wait> ();
		readonly object GetIDLock = new object ();

		long ID;

		public long GetID () {
			lock (GetIDLock) {
				if (ID == long.MaxValue) {
					ID = 0;
				}
				return ID++;
			}
		}

		public void Set (long id, string result) {
			Waits[id].Result = result;
			Waits[id].AutoResetEvent.Set ();
		}

		public string Get (long id) {
			Wait wait = new Wait ();
			Waits[id] = wait;
			wait.AutoResetEvent.WaitOne ();
			return wait.Result;
		}

		public void Dispose () {
			for (int i = 0; i < Waits.Count; i++) {
				Waits[i].AutoResetEvent.Close ();
			}
		}

		class Wait {

			public AutoResetEvent AutoResetEvent = new AutoResetEvent (false);
			public string Result;

		}

	}

}