using System;
using System.Collections.Generic;
using System.Threading;

namespace Eruru.ChatRobotRPC {

	class WaitSystem : IDisposable {

		public int MillisecondsTimeout {

			get {
				return _MillisecondsTimeout;
			}

			set {
				_MillisecondsTimeout = value;
			}

		}

		readonly Queue<Wait> WaitPool = new Queue<Wait> ();
		readonly List<Wait> Waits = new List<Wait> ();
		readonly object GetIDLock = new object ();
		readonly object WaitsLock = new object ();

		long ID;
		int _MillisecondsTimeout = 60 * 1000;

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
				for (int i = 0; i < Waits.Count; i++) {
					if (Waits[i].ID == id) {
						Waits[i].Result = result;
						Waits[i].AutoResetEvent.Set ();
						WaitPool.Enqueue (Waits[i]);
						Waits.RemoveAt (i);
						return;
					}
				}
				throw new Exception (string.Format ("没有找到ID为{0}的Wait", id));
			}
		}

		public string Get (long id) {
			Wait wait;
			lock (WaitsLock) {
				if (WaitPool.Count == 0) {
					wait = new Wait (id);
				} else {
					wait = WaitPool.Dequeue ();
					wait.ID = id;
				}
				Waits.Add (wait);
			}
			wait.AutoResetEvent.WaitOne (MillisecondsTimeout);
			return wait.Result;
		}

		public void Dispose () {
			lock (WaitsLock) {
				ID = 0;
				foreach (var wait in Waits) {
					wait.AutoResetEvent.Close ();
					WaitPool.Enqueue (wait);
				}
				Waits.Clear ();
			}
		}

		class Wait {

			public long ID;
			public AutoResetEvent AutoResetEvent = new AutoResetEvent (false);
			public string Result;

			public Wait (long id) {
				ID = id;
			}

		}

	}

}