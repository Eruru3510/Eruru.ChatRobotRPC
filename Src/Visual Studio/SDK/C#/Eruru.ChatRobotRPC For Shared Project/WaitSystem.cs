using System;
using System.Collections.Generic;
using System.Threading;

namespace Eruru.ChatRobotRPC {

	class WaitSystem : IDisposable {

		public int MillisecondsTimeout { get; set; } = 60 * 1000;

		readonly Dictionary<long, Wait> Waits = new Dictionary<long, Wait> ();
		readonly object GetIDLock = new object ();
		readonly object WaitsLock = new object ();

		long ID;

		public long GetID () {
			lock (GetIDLock) {
				if (ID >= long.MaxValue) {
					ID = 0;
				}
				return ID++;
			}
		}

		public void Set (long id, string result) {
			lock (WaitsLock) {
				Wait wait = Waits[id];
				wait.Result = result;
				wait.AutoResetEvent.Set ();
			}
		}

		public string Get (long id) {
			Wait wait;
			lock (WaitsLock) {
				if (!Waits.TryGetValue (id, out wait)) {
					wait = new Wait ();
					Waits[id] = wait;
				}
			}
			wait.AutoResetEvent.WaitOne (MillisecondsTimeout);
			return wait.Result;
		}

		public void Dispose () {
			lock (WaitsLock) {
				ID = 0;
				foreach (var wait in Waits) {
					wait.Value.AutoResetEvent.Close ();
				}
			}
		}

		class Wait {

			public AutoResetEvent AutoResetEvent = new AutoResetEvent (false);
			public string Result;

		}

	}

}