using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Eruru.ChatRobotRPC {

	class SocketClient : IDisposable {

		public SocketClientState State { get; set; } = SocketClientState.NotConnected;
		public bool UseAsyncOnReceived { get; set; } = true;
		public SocketClientReceivedEventHandler OnReceived { get; set; }
		public SocketClientSendEventHandler OnSend { get; set; }
		public SocketClientDisconnected OnDisconnected { get; set; }
		/// <summary>
		/// Seconds
		/// </summary>
		public int HeartbeatInterval { get; set; } = 60;
		public int BufferLength { get; set; } = 1024;

		const int PacketHeaderLength = 4;

		readonly byte[] EmptyBytes = new byte[0];
		readonly Queue<byte> Buffer = new Queue<byte> ();
		readonly object SocketLock = new object ();

		Socket Socket;
		Thread HeartbeatThread;
		int PacketBodyLength = -1;
		DateTime HeartbeatSendTime;

		public void Connect (string ip, int port) {
			lock (SocketLock) {
				try {
					State = SocketClientState.Connecting;
					Socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					Socket.Connect (ip, port);
					State = SocketClientState.Connected;
					HeartbeatSendTime = DateTime.Now;
					HeartbeatThread = new Thread (BeginHeartbeat) { IsBackground = true };
					HeartbeatThread.Start ();
					BeginReceive ();
				} catch {
					State = SocketClientState.NotConnected;
					Socket.Close ();
					throw;
				}
			}
		}

		public void SendAsync (byte[] bytes) {
			HeartbeatSendTime = DateTime.Now;
			byte[] buffer = ToPacket (bytes);
			if (bytes.Length > 0) {
				OnSend?.Invoke (bytes);
			}
			Socket.BeginSend (buffer, 0, buffer.Length, SocketFlags.None, asyncResult => {
				Socket.EndSend (asyncResult, out SocketError socketError);
			}, null);
		}

		public void Disconnect () {
			lock (SocketLock) {
				try {
					Socket.Shutdown (SocketShutdown.Both);
				} finally {
					Socket.Close ();
					Buffer.Clear ();
					PacketBodyLength = -1;
					HeartbeatThread.Abort ();
					OnDisconnected?.Invoke ();
				}
			}
		}

		public void Dispose () {
			Disconnect ();
		}

		void BeginReceive () {
			byte[] buffer = new byte[BufferLength];
			Socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, out SocketError socketError, asyncResult => {
				int length = Socket.EndReceive (asyncResult, out SocketError innerSocketError);
				if (length < 1) {
					Disconnect ();
					return;
				}
				for (int i = 0; i < length; i++) {
					Buffer.Enqueue (buffer[i]);
				}
				while (true) {
					if (PacketBodyLength == -1) {
						if (Buffer.Count < PacketHeaderLength) {
							break;
						}
						byte[] bytes = new byte[PacketHeaderLength];
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
						PerformOnReceived (bytes);
					}
				}
				BeginReceive ();
			}, null);
		}

		void BeginHeartbeat () {
			try {
				while (State == SocketClientState.Connected) {
					Thread.Sleep (1000);
					if (HeartbeatSendTime <= DateTime.Now.AddSeconds (-HeartbeatInterval)) {
						SendAsync (EmptyBytes);
					}
				}
			} catch (ThreadAbortException) {

			}
		}

		byte[] ToPacket (byte[] data) {
			byte[] buffer = new byte[data.Length + PacketHeaderLength];
			Array.Copy (BitConverter.GetBytes (data.Length), buffer, PacketHeaderLength);
			Array.Copy (data, 0, buffer, PacketHeaderLength, data.Length);
			return buffer;
		}

		void PerformOnReceived (byte[] bytes) {
			if (OnReceived != null) {
				if (UseAsyncOnReceived) {
					OnReceived.BeginInvoke (bytes, asyncResult => OnReceived.EndInvoke (asyncResult), null);
					return;
				}
				OnReceived.Invoke (bytes);
			}
		}

	}

}