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
			Wait wait = Waits[id];
			wait.Result = result;
			wait.AutoResetEvent.Set ();
		}

		public string Get (long id) {
			if (!Waits.TryGetValue (id, out Wait wait)) {
				wait = new Wait ();
				Waits[id] = wait;
			}
			wait.AutoResetEvent.WaitOne ();
			return wait.Result;
		}

		public void Dispose () {
			foreach (var wait in Waits) {
				wait.Value.AutoResetEvent.Close ();
			}
		}

		class Wait {

			public AutoResetEvent AutoResetEvent = new AutoResetEvent (false);
			public string Result;

		}

	}

}