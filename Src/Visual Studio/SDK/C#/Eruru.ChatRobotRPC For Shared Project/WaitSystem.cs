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

		int ID;
		int _MillisecondsTimeout = 10 * 1000;

		public int GetID () {
			lock (GetIDLock) {
				if (ID > 100000) {
					ID = 0;
				}
				return ID++;
			}
		}

		public void Set (int id, string result) {
			lock (WaitsLock) {
				Wait wait = Waits.Find (item => item.ID == id);
				if (wait == null) {
					wait = NewWait (id);
					Waits.Add (wait);
				}
				wait.Result = result;
				wait.Signal = true;
				Monitor.PulseAll (WaitsLock);
				//throw new KeyNotFoundException (string.Format ("没有找到ID为{0}的Wait，Result：{1}", id, result));
			}
		}

		public string Get (int id) {
			lock (WaitsLock) {
				Wait wait = Waits.Find (item => item.ID == id);
				if (wait == null) {
					wait = NewWait (id);
					Waits.Add (wait);
				}
				while (true) {
					if (wait.Signal) {
						break;
					}
					if (!Monitor.Wait (WaitsLock, MillisecondsTimeout)) {
						throw new TimeoutException (string.Format ("ID为{0}的Wait等待超时", id));
					}
				}
				WaitPool.Enqueue (wait);
				Waits.Remove (wait);
				if (Waits.Count <= 0) {
					Monitor.PulseAll (WaitsLock);
				}
				return wait.Result;
			}
		}

		public void Dispose () {
			lock (WaitsLock) {
				ID = 0;
				while (Waits.Count > 0) {
					foreach (var wait in Waits) {
						wait.Signal = true;
					}
					Monitor.PulseAll (WaitsLock);
					if (!Monitor.Wait (WaitsLock, MillisecondsTimeout)) {
						throw new TimeoutException ();
					}
				}
			}
		}

		Wait NewWait (int id) {
			lock (WaitsLock) {
				Wait wait;
				if (WaitPool.Count == 0) {
					wait = new Wait ();
				} else {
					wait = WaitPool.Dequeue ();
					wait.Result = null;
					wait.Signal = false;
				}
				wait.ID = id;
				return wait;
			}
		}

		class Wait {

			public long ID;
			public string Result;
			public bool Signal;

		}

	}

}