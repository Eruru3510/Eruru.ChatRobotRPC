package org.eruru.chatrobotrpc;

import java.io.Closeable;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

class WaitSystem implements Closeable {

	private final Map<Long, Wait> waits = new HashMap<> ();
	private final Object getIDLock = new Object ();

	private Long id = 0L;

	public long getID () {
		synchronized (getIDLock) {
			if (id == Long.MAX_VALUE) {
				id = 0L;
			}
			return id++;
		}
	}

	public void set (long id, String result) {
		Wait wait = waits.get (id);
		wait.Result = result;
		wait.AutoResetEvent.set ();
	}

	public String get (long id) throws InterruptedException {
		Wait wait = waits.get (id);
		if (wait == null) {
			wait = new Wait ();
			waits.put (id, wait);
		}
		wait.AutoResetEvent.waitOne ();
		return wait.Result;
	}

	@Override
	public void close () throws IOException {
		for (Map.Entry<Long, Wait> wait : waits.entrySet ()) {
			wait.getValue ().AutoResetEvent.close ();
		}
	}

	static class Wait {

		public AutoResetEvent AutoResetEvent = new AutoResetEvent (false);
		public String Result;

	}

}