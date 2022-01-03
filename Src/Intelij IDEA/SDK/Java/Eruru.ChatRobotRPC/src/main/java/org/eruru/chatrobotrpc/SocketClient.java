package org.eruru.chatrobotrpc;

import org.eruru.chatrobotrpc.eventHandlers.ChatRobotAction;

import java.io.Closeable;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.util.Date;
import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.SynchronousQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

class SocketClient implements Closeable {

	private SocketClientState state;
	private ReceivedEventHandler onReceived;
	private SendEventHandler onSend;
	private ChatRobotAction onDisconnected;
	private Charset charset = StandardCharsets.UTF_8;
	private int heartbeatInterval = 60;
	private int bufferLength = 1024 * 1024;

	private final int packetHeadLength = 4;
	private final byte[] emptyBytes = new byte[0];
	private final Queue<Byte> buffer = new LinkedList<> ();
	private final Object lock = new Object ();

	private InputStream inputStream;
	private OutputStream outputStream;
	private Socket socket;
	private Thread heartbeatThread;
	private int packetBodyLength = -1;
	private Date heartbeatSendTime;
	private boolean UseAsyncOnReceived = true;
	private ThreadPoolExecutor threadPoolExecutor = new ThreadPoolExecutor (0, Integer.MAX_VALUE, 60L, TimeUnit.SECONDS, new SynchronousQueue<> ());

	public SocketClientState getState () {
		return state;
	}

	public ReceivedEventHandler getOnReceived () {
		return onReceived;
	}

	public void setOnReceived (ReceivedEventHandler onReceived) {
		this.onReceived = onReceived;
	}

	public SendEventHandler getOnSend () {
		return onSend;
	}

	public void setOnSend (SendEventHandler onSend) {
		this.onSend = onSend;
	}

	public ChatRobotAction getOnDisconnected () {
		return onDisconnected;
	}

	public void setOnDisconnected (ChatRobotAction onDisconnected) {
		this.onDisconnected = onDisconnected;
	}

	public Charset getCharset () {
		return charset;
	}

	public void setCharset (Charset charset) {
		this.charset = charset;
	}

	public int getHeartbeatInterval () {
		return heartbeatInterval;
	}

	public void setHeartbeatInterval (int heartbeatInterval) {
		this.heartbeatInterval = heartbeatInterval;
	}

	public int getBufferLength () {
		return bufferLength;
	}

	public void setBufferLength (int bufferLength) {
		this.bufferLength = bufferLength;
	}

	public boolean isUseAsyncOnReceived () {
		return UseAsyncOnReceived;
	}

	public void setUseAsyncOnReceived (boolean useAsyncOnReceived) {
		UseAsyncOnReceived = useAsyncOnReceived;
	}

	public void connect (String ip, int port) throws IOException {
		synchronized (lock) {
			try {
				state = SocketClientState.Connecting;
				socket = new Socket ();
				socket.connect (new InetSocketAddress (ip, port));
				state = SocketClientState.Connected;
				heartbeatSendTime = new Date ();
				inputStream = socket.getInputStream ();
				outputStream = socket.getOutputStream ();
				heartbeatThread = new Thread (this::beginHeartbeat);
				heartbeatThread.start ();
				beginReceive ();
			} catch (Exception exception) {
				state = SocketClientState.NotConnected;
				throw exception;
			}
		}
	}

	public void sendAsync (byte[] bytes) {
		heartbeatSendTime = new Date ();
		byte[] buffer = toPacket (bytes);
		if (bytes.length > 0) {
			if (onSend != null) {
				onSend.invoke (bytes);
			}
		}
		threadPoolExecutor.execute (() -> {
			try {
				outputStream.write (buffer, 0, buffer.length);
			} catch (IOException e) {
				e.printStackTrace ();
			}
		});
	}

	public void Disconnect () throws IOException {
		synchronized (lock) {
			try {
				socket.close ();
			} finally {
				buffer.clear ();
				packetBodyLength = -1;
				heartbeatThread.stop ();
				if (onDisconnected != null) {
					onDisconnected.invoke ();
				}
			}
		}
	}

	@Override
	public void close () throws IOException {
		Disconnect ();
	}

	private void beginReceive () {
		new Thread (() -> {
			try {
				while (state == SocketClientState.Connected) {
					byte[] buffer = new byte[bufferLength];
					int length = inputStream.read (buffer, 0, buffer.length);
					if (length < 1) {
						Disconnect ();
						return;
					}
					for (int i = 0; i < length; i++) {
						this.buffer.add (buffer[i]);
					}
					while (true) {
						if (packetBodyLength < 0) {
							if (this.buffer.size () < packetHeadLength) {
								break;
							}
							byte[] bytes = new byte[packetHeadLength];
							for (int i = 0; i < bytes.length; i++) {
								bytes[i] = this.buffer.remove ();
							}
							packetBodyLength = ChatRobotAPI.bytesToInt (bytes);
						} else {
							if (this.buffer.size () < packetBodyLength) {
								break;
							}
							if (packetBodyLength > 0) {
								byte[] bytes = new byte[packetBodyLength];
								for (int i = 0; i < bytes.length; i++) {
									bytes[i] = this.buffer.remove ();
								}
								performOnReceived (bytes);
							} else {
								performOnReceived (emptyBytes);
							}
							packetBodyLength = -1;
						}
					}
				}
			} catch (IOException exception) {
				exception.printStackTrace ();
			}
		}).start ();
	}

	private void beginHeartbeat () {
		try {
			while (state == SocketClientState.Connected) {
				if (heartbeatSendTime.getTime () <= new Date ().getTime () - heartbeatInterval * 1000L) {
					sendAsync (emptyBytes);
				}
				Thread.sleep (1000);
			}
		} catch (InterruptedException exception) {
			exception.printStackTrace ();
		}
	}

	private byte[] toPacket (byte[] bytes) {
		byte[] buffer = new byte[bytes.length + packetHeadLength];
		System.arraycopy (ChatRobotAPI.intToBytes (bytes.length), 0, buffer, 0, packetHeadLength);
		System.arraycopy (bytes, 0, buffer, packetHeadLength, bytes.length);
		return buffer;
	}

	private void performOnReceived (byte[] bytes) {
		if (onReceived != null) {
			if (UseAsyncOnReceived) {
				threadPoolExecutor.execute (() -> onReceived.invoke (bytes));
				return;
			}
			onReceived.invoke (bytes);
		}
	}

}