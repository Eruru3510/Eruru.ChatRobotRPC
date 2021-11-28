package org.eruru.chatrobotrpc;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.LinkedList;
import java.util.Queue;

class Client {

	private final Queue<Byte> buffer = new LinkedList<> ();
	private final int packetHeadLength = 4;

	private OnReceived onReceived;
	private  OnSent onSent;
	private ChatRobotAction onDisconnected;
	private InputStream inputStream;
	private OutputStream outputStream;
	private int packetBodyLength = -1;
	private Socket socket;

	public void setOnReceived (OnReceived onReceived) {
		this.onReceived = onReceived;
	}

	public OnReceived getOnReceived () {
		return onReceived;
	}

	public OnSent getOnSent () {
		return onSent;
	}

	public void setOnSent (OnSent onSent) {
		this.onSent = onSent;
	}

	public void setOnDisconnected (ChatRobotAction onDisconnected) {
		this.onDisconnected = onDisconnected;
	}

	public ChatRobotAction getOnDisconnected () {
		return onDisconnected;
	}

	public void connect (String ip, int port) throws IOException {
		if (socket == null) {
			socket = new Socket ();
		}
		socket.connect (new InetSocketAddress (ip, port));
		inputStream = socket.getInputStream ();
		outputStream = socket.getOutputStream ();
		beginReceive ();
	}

	public void send (byte[] data) throws IOException {
		byte[] buffer = toPacket (data);
		outputStream.write (buffer, 0, buffer.length);
	}

	public void send (String text) throws IOException {
		if(onSent!=null){
			onSent.invoke (text);
		}
		send (text.getBytes (StandardCharsets.UTF_8));
	}

	public void beginSend (byte[] data) {
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
		if(onSent!=null){
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
			if (onDisconnected != null) {
				onDisconnected.invoke ();
			}
		}
	}

	private void beginReceive () {
		new Thread (() -> {
			try {
				byte[] buffer = new byte[1024];
				int length = inputStream.read (buffer, 0, buffer.length);
				if (length == 0) {
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
				beginReceive ();
			} catch (Exception exception) {
				exception.printStackTrace ();
			}
		}).start ();
	}

	private byte[] toPacket (byte[] data) {
		byte[] buffer = new byte[data.length + packetHeadLength];
		System.arraycopy (ChatRobotAPI.intToBytes (data.length), 0, buffer, 0, packetHeadLength);
		System.arraycopy (data, 0, buffer, packetHeadLength, data.length);
		return buffer;
	}

}