package org.eruru.chatrobotrpc;

import java.io.Closeable;

class AutoResetEvent implements Closeable {

	private final Object monitor = new Object ();
	private volatile boolean open;

	public AutoResetEvent (boolean initialState) {
		this.open = initialState;
	}

	public boolean waitOne (long millisecondsTimeout) throws InterruptedException {
		synchronized (monitor) {
			monitor.wait (millisecondsTimeout);
			boolean isOpen = open;
			open = false;
			return isOpen;
		}
	}

	public void set () {
		synchronized (monitor) {
			open = true;
			monitor.notify ();
		}
	}

	public void reset () {
		synchronized (monitor) {
			open = false;
		}
	}

	@Override
	public void close () {

	}

}