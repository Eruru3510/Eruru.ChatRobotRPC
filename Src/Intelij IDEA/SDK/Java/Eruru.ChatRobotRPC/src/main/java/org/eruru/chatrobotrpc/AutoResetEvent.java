package org.eruru.chatrobotrpc;

import java.io.Closeable;

class AutoResetEvent implements Closeable {

	private final Object monitor = new Object ();
	private volatile boolean open;

	public AutoResetEvent (boolean initialState) {
		this.open = initialState;
	}

	public void waitOne (long millisecondsTimeout) throws InterruptedException {
		synchronized (monitor) {
			while (!open) {
				monitor.wait (millisecondsTimeout);
			}
			open = false;
		}
	}

	public void set () {
		synchronized (monitor) {
			open = true;
			monitor.notify ();
		}
	}

	public void reset () {
		open = false;
	}

	@Override
	public void close () {

	}

}