using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Eruru.ChatRobotRPC {

	class Client : IDisposable {

		public bool Connected { get; private set; }
		public Action<byte[]> OnReceived { get; set; }
		public ChatRobotAction OnDisconnected { get; set; }
		public Encoding Encoding { get; set; } = Encoding.UTF8;
		public long HeartbeatPacketSendIntervalBySeconds { get; set; } = 60;
		public Action<string> OnSent { get; set; }

		const int PacketHeadLength = 4;

		readonly byte[] EmptyBytes = new byte[0];
		readonly Queue<byte> Buffer = new Queue<byte> ();
		readonly int ReceiveBufferSize = 1024;

		Socket Socket;
		Thread HeartbeatPacketThread;
		int PacketBodyLength = -1;
		DateTime HeartbeatPacketSendTime;

		public void Connect (string ip, int port) {
			if (Socket == null) {
				Socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			}
			Socket.Connect (ip, port);
			HeartbeatPacketSendTime = DateTime.Now;
			BeginReceive ();
			HeartbeatPacketThread?.Abort ();
			HeartbeatPacketThread = new Thread (BeginHeartbeatPacket) { IsBackground = true };
			HeartbeatPacketThread.Start ();
		}

		public void BeginSend (byte[] data) {
			if (Socket == null) {
				return;
			}
			HeartbeatPacketSendTime = DateTime.Now;
			byte[] buffer = ToPacket (data);
			Socket.BeginSend (buffer, 0, buffer.Length, SocketFlags.None, asyncResult => {
				int length = Socket.EndSend (asyncResult, out SocketError socketError);
			}, null);
		}
		public void BeginSend (string text) {
			if (Socket == null) {
				return;
			}
			OnSent?.Invoke (text);
			BeginSend (Encoding.GetBytes (text));
		}

		public void Disconnect () {
			if (Socket == null) {
				return;
			}
			try {
				Socket.Shutdown (SocketShutdown.Both);
			} finally {
				Socket.Close ();
				Socket = null;
				Buffer.Clear ();
				PacketBodyLength = -1;
				OnDisconnected?.Invoke ();
			}
		}

		public void Dispose () {
			Disconnect ();
		}

		void BeginReceive () {
			if (Socket == null) {
				return;
			}
			byte[] buffer = new byte[ReceiveBufferSize];
			Socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, out SocketError socketError, asyncResult => {
				if (Socket == null) {
					return;
				}
				int length = Socket.EndReceive (asyncResult, out SocketError innerSocketError);
				if (length == 0) {
					Disconnect ();
					return;
				}
				for (int i = 0; i < length; i++) {
					Buffer.Enqueue (buffer[i]);
				}
				while (true) {
					if (PacketBodyLength == -1) {
						if (Buffer.Count < PacketHeadLength) {
							break;
						}
						byte[] bytes = new byte[PacketHeadLength];
						for (int i = 0; i < bytes.Length; i++) {
							bytes[i] = Buffer.Dequeue ();
						}
						PacketBodyLength = BitConverter.ToInt32 (bytes, 0);
					} else {
						if (Buffer.Count < PacketBodyLength) {
							break;
						}
						byte[] bytes = new byte[PacketBodyLength];
						for (int i = 0; i < bytes.Length; i++) {
							bytes[i] = Buffer.Dequeue ();
						}
						PacketBodyLength = -1;
						OnReceived?.Invoke (bytes);
					}
				}
				BeginReceive ();
			}, null);
		}

		void BeginHeartbeatPacket () {
			while (true) {
				if (HeartbeatPacketSendTime <= DateTime.Now.AddSeconds (-HeartbeatPacketSendIntervalBySeconds)) {
					HeartbeatPacketSendTime = DateTime.Now;
					BeginSend (EmptyBytes);
				}
				Thread.Sleep (1000);
			}
		}

		byte[] ToPacket (byte[] data) {
			byte[] buffer = new byte[data.Length + PacketHeadLength];
			Array.Copy (BitConverter.GetBytes (data.Length), buffer, PacketHeadLength);
			Array.Copy (data, 0, buffer, PacketHeadLength, data.Length);
			return buffer;
		}

	}

}