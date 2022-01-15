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
		int _MillisecondsTimeout = 10 * 1000;

		public long GetID () {
			lock (GetIDLock) {
				if (ID >= 100000) {
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
						return;
					}
				}
				throw new KeyNotFoundException (string.Format ("没有找到ID为{0}的Wait，Result：{1}", id, result));
			}
		}

		public string Get (long id) {
			Wait wait;
			lock (WaitsLock) {
				wait = WaitPool.Count == 0 ? new Wait () : WaitPool.Dequeue ();
				wait.ID = id;
				wait.Result = null;
				wait.AutoResetEvent.Reset ();
				Waits.Add (wait);
			}
			try {
				if (!wait.AutoResetEvent.WaitOne (MillisecondsTimeout)) {
					throw new TimeoutException (string.Format ("ID为{0}的Wait等待超时", id));
				}
			} finally {
				lock (WaitsLock) {
					WaitPool.Enqueue (wait);
					Waits.Remove (wait);
				}
			}
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
			public string Result;
			public AutoResetEvent AutoResetEvent = new AutoResetEvent (false);

		}

	}

}