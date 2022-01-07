package org.eruru.chatrobotrpc;

import java.io.Closeable;
import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

class WaitSystem implements Closeable {

	private final Queue<Wait> waitPool = new LinkedList<> ();
	private final List<Wait> waits = new ArrayList<> ();
	private final Object getIDLock = new Object ();
	private final Object waitsLock = new Object ();

	private Long id = 0L;

	public long getID () {
		synchronized (getIDLock) {
			if (id == Long.MAX_VALUE) {
				id = 0L;
			}
			return id++;
		}
	}

	public void set (long id, String result) throws Exception {
		synchronized (waitsLock) {
			for (int i = 0; i < waits.size (); i++) {
				if (waits.get (i).id == id) {
					waits.get (i).result = result;
					waits.get (i).autoResetEvent.set ();
					waitPool.offer (waits.get (i));
					waits.remove (i);
					return;
				}
			}
			throw new Exception (String.format ("没有找到ID为%d的Wait", id));
		}
	}

	public String get (long id) throws InterruptedException {
		Wait wait;
		synchronized (waitsLock) {
			wait = waitPool.size () == 0 ? new Wait () : waitPool.poll ();
			wait.id = id;
			waits.add (wait);
		}
		wait.autoResetEvent.waitOne (60 * 1000);
		return wait.result;
	}

	@Override
	public void close () throws IOException {
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
		public AutoResetEvent autoResetEvent = new AutoResetEvent (false);
		public String result;

	}

}