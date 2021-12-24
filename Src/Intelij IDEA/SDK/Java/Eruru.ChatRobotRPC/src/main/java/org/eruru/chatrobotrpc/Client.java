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

class Client implements Closeable {

	private boolean isConnected;
	private ReceivedEventHandler onReceived;
	private SentEventHandler onSent;
	private ChatRobotAction onDisconnected;
	private Charset charset = StandardCharsets.UTF_8;
	private int heartbeatPacketSendIntervalBySeconds = 60;
	private int receiveBufferSize = 1024;

	private final int packetHeadLength = 4;
	private final byte[] emptyBytes = new byte[0];
	private final Queue<Byte> buffer = new LinkedList<> ();

	private InputStream inputStream;
	private OutputStream outputStream;
	private Socket socket;
	private Thread heartbeatPacketThread;
	private int packetBodyLength = -1;
	private Date heartbeatPacketSendTime;

	public ReceivedEventHandler getOnReceived () {
		return onReceived;
	}

	public void setOnReceived (ReceivedEventHandler onReceived) {
		this.onReceived = onReceived;
	}

	public SentEventHandler getOnSent () {
		return onSent;
	}

	public void setOnSent (SentEventHandler onSent) {
		this.onSent = onSent;
	}

	public ChatRobotAction getOnDisconnected () {
		return onDisconnected;
	}

	public void setOnDisconnected (ChatRobotAction onDisconnected) {
		this.onDisconnected = onDisconnected;
	}

	public boolean isConnected () {
		return isConnected;
	}

	public Charset getCharset () {
		return charset;
	}

	public void setCharset (Charset charset) {
		this.charset = charset;
	}

	public int getHeartbeatPacketSendIntervalBySeconds () {
		return heartbeatPacketSendIntervalBySeconds;
	}

	public void setHeartbeatPacketSendIntervalBySeconds (int heartbeatPacketSendIntervalBySeconds) {
		this.heartbeatPacketSendIntervalBySeconds = heartbeatPacketSendIntervalBySeconds;
	}

	public int getReceiveBufferSize () {
		return receiveBufferSize;
	}

	public void setReceiveBufferSize (int receiveBufferSize) {
		this.receiveBufferSize = receiveBufferSize;
	}

	public void connect (String ip, int port) throws IOException {
		socket = new Socket ();
		socket.connect (new InetSocketAddress (ip, port));
		isConnected = true;
		heartbeatPacketSendTime = new Date ();
		inputStream = socket.getInputStream ();
		outputStream = socket.getOutputStream ();
		beginReceive ();
		heartbeatPacketThread = new Thread (() -> {
			try {
				beginHeartbeatPacket ();
			} catch (InterruptedException e) {
				e.printStackTrace ();
			}
		});
		heartbeatPacketThread.start ();
	}

	public void beginSend (byte[] data) {
		if (socket == null) {
			return;
		}
		heartbeatPacketSendTime = new Date ();
		byte[] buffer = toPacket (data);
		new Thread (() -> {
			try {
				outputStream.write (buffer, 0, buffer.length);
			} catch (Exception exception) {
				exception.printStackTrace ();
			}
		}).start ();
	}

	public void beginSend (String text) {
		if (socket == null) {
			return;
		}
		if (onSent != null) {
			onSent.invoke (text);
		}
		beginSend (text.getBytes (StandardCharsets.UTF_8));
	}

	public void Disconnect () throws IOException {
		if (socket == null) {
			return;
		}
		try {
			socket.close ();
		} finally {
			socket = null;
			buffer.clear ();
			packetBodyLength = -1;
			heartbeatPacketThread.stop ();
			if (onDisconnected != null) {
				onDisconnected.invoke ();
			}
		}
	}

	@Override
	public void close () throws IOException {
		Disconnect ();
	}

	private void beginReceive () {
		if (socket == null) {
			return;
		}
		new Thread (() -> {
			try {
				while (true) {
					if (socket == null) {
						return;
					}
					byte[] buffer = new byte[receiveBufferSize];
					int length = inputStream.read (buffer, 0, buffer.length);
					if (length == 0) {
						Disconnect ();
						return;
					}
					for (int i = 0; i < length; i++) {
						this.buffer.add (buffer[i]);
					}
					while (true) {
						if (packetBodyLength == -1) {
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
							byte[] bytes = new byte[packetBodyLength];
							for (int i = 0; i < bytes.length; i++) {
								bytes[i] = this.buffer.remove ();
							}
							packetBodyLength = -1;
							if (onReceived != null) {
								onReceived.invoke (bytes);
							}
						}
					}
				}
			} catch (Exception exception) {
				exception.printStackTrace ();
			}
		}).start ();
	}

	private void beginHeartbeatPacket () throws InterruptedException {
		while (true) {
			if (heartbeatPacketSendTime.getTime () <= new Date ().getTime () - heartbeatPacketSendIntervalBySeconds * 1000L) {
				heartbeatPacketSendTime = new Date ();
				beginSend (emptyBytes);
			}
			Thread.sleep (1000);
		}
	}

	private byte[] toPacket (byte[] data) {
		byte[] buffer = new byte[data.length + packetHeadLength];
		System.arraycopy (ChatRobotAPI.intToBytes (data.length), 0, buffer, 0, packetHeadLength);
		System.arraycopy (data, 0, buffer, packetHeadLength, data.length);
		return buffer;
	}

}