package org.eruru.chatrobotrpc;

import javax.naming.NameNotFoundException;
import java.io.Closeable;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;
import java.util.concurrent.TimeoutException;

class WaitSystem implements Closeable {

	private final Queue<Wait> waitPool = new LinkedList<> ();
	private final List<Wait> waits = new ArrayList<> ();
	private final Object getIDLock = new Object ();
	private final Object waitsLock = new Object ();

	private long millisecondsTimeout = 10 * 1000;
	private Long id = 0L;

	public long getMillisecondsTimeout () {
		return millisecondsTimeout;
	}

	public void setMillisecondsTimeout (long millisecondsTimeout) {
		this.millisecondsTimeout = millisecondsTimeout;
	}

	public long getID () {
		synchronized (getIDLock) {
			if (id == 100000) {
				id = 0L;
			}
			return id++;
		}
	}

	public void set (long id, String result) throws Exception {
		synchronized (waitsLock) {
			for (Wait wait : waits) {
				if (wait.id == id) {
					wait.result = result;
					wait.autoResetEvent.set ();
					return;
				}
			}
			throw new NameNotFoundException (String.format ("没有找到ID为%d的Wait，Result：%s", id, result));
		}
	}

	public String get (long id) throws InterruptedException, TimeoutException {
		Wait wait;
		synchronized (waitsLock) {
			wait = waitPool.size () == 0 ? new Wait () : waitPool.poll ();
			wait.id = id;
			wait.result = null;
			wait.autoResetEvent.reset ();
			waits.add (wait);
		}
		try {
			if (!wait.autoResetEvent.waitOne (millisecondsTimeout)) {
				throw new TimeoutException (String.format ("ID为%d的Wait等待超时", id));
			}
		} finally {
			synchronized (waitsLock) {
				waitPool.offer (wait);
				waits.remove (wait);
			}
		}
		return wait.result;
	}

	@Override
	public void close () {
		synchronized (waitsLock) {
			for (Wait wait : waits) {
				wait.autoResetEvent.close ();
				waitPool.offer (wait);
			}
			waits.clear ();
		}
	}

	static class Wait {

		public long id;
		public String result;
		public AutoResetEvent autoResetEvent = new AutoResetEvent (false);

	}

}