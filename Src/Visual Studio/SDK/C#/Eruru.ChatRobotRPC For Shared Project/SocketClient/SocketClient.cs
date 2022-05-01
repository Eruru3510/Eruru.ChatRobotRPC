using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
#if NETCOREAPP || NET5_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace Eruru.ChatRobotRPC {

	class SocketClient : IDisposable {

		public SocketClientState State {

			get {
				return _State;
			}

			set {
				_State = value;
			}

		}
		public bool UseAsyncOnReceived {

			get {
				return _UseAsyncOnReceived;
			}

			set {
				_UseAsyncOnReceived = value;
			}

		}
		public SocketClientReceivedEventHandler OnReceived { get; set; }
		public SocketClientSendEventHandler OnSend { get; set; }
		public SocketClientDisconnected OnDisconnected { get; set; }
		/// <summary>
		/// Seconds
		/// </summary>
		public int HeartbeatInterval {

			get {
				return _HeartbeatInterval;
			}

			set {
				_HeartbeatInterval = value;
			}

		}
		public int BufferLength {

			get {
				return _BufferLength;
			}

			set {
				_BufferLength = value;
			}

		}
		public bool DebugMode {

			get {
				return _DebugMode;
			}

			set {
				_DebugMode = value;
			}

		}

		const int PacketHeaderLength = 4;

#if NET5_0_OR_GREATER
		readonly byte[] EmptyBytes = Array.Empty<byte> ();
#else
		readonly byte[] EmptyBytes = new byte[0];
#endif
		readonly Queue<byte> Buffer = new Queue<byte> ();
		readonly object Lock = new object ();

		Socket Socket;
		Thread HeartbeatThread;
		int PacketBodyLength = -1;
		DateTime HeartbeatSendTime;
		SocketClientState _State = SocketClientState.NotConnected;
		bool _UseAsyncOnReceived = true;
		int _HeartbeatInterval = 60;
		int _BufferLength = 1024;
		bool _DebugMode;

		public void Connect (string ip, int port) {
			lock (Lock) {
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
			try {
				HeartbeatSendTime = DateTime.Now;
				byte[] buffer = ToPacket (bytes);
				if (bytes.Length > 0) {
					if (OnSend != null) {
						OnSend (bytes);
					}
				}
				Socket.BeginSend (buffer, 0, buffer.Length, SocketFlags.None, asyncResult => {
					try {
						SocketError socketError;
						Socket.EndSend (asyncResult, out socketError);
					} catch (ObjectDisposedException) {

					}
				}, null);
			} catch (ObjectDisposedException) {

			}
		}

		public void Disconnect () {
			lock (Lock) {
				try {
					Socket.Shutdown (SocketShutdown.Both);
				} finally {
					Socket.Close ();
					Buffer.Clear ();
					PacketBodyLength = -1;
#if NETCOREAPP || NET5_0_OR_GREATER
					HeartbeatThread.Interrupt ();
#else
					HeartbeatThread.Abort ();
#endif

					if (OnDisconnected != null) {
						OnDisconnected ();
					}
				}
			}
		}

		public void Dispose () {
			Disconnect ();
		}

		static byte[] ToPacket (byte[] bytes) {
			byte[] buffer = new byte[bytes.Length + PacketHeaderLength];
			Array.Copy (BitConverter.GetBytes (bytes.Length), buffer, PacketHeaderLength);
			Array.Copy (bytes, 0, buffer, PacketHeaderLength, bytes.Length);
			return buffer;
		}

		void BeginReceive () {
			try {
				byte[] buffer = new byte[BufferLength];
				SocketError socketError;
				Socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, out socketError, asyncResult => {
					try {
						SocketError innerSocketError;
						int length = Socket.EndReceive (asyncResult, out innerSocketError);
						if (length < 1) {
							Disconnect ();
							return;
						}
						for (int i = 0; i < length; i++) {
							Buffer.Enqueue (buffer[i]);
						}
						while (true) {
							if (PacketBodyLength < 0) {
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
								if (PacketBodyLength > 0) {
									byte[] bytes = new byte[PacketBodyLength];
									for (int i = 0; i < bytes.Length; i++) {
										bytes[i] = Buffer.Dequeue ();
									}
									PerformOnReceived (bytes);
								} else {
									PerformOnReceived (EmptyBytes);
								}
								PacketBodyLength = -1;
							}
						}
						BeginReceive ();
					} catch (ObjectDisposedException) {

					}
				}, null);
			} catch (ObjectDisposedException) {

			}
		}

		void BeginHeartbeat () {
			try {
				while (State == SocketClientState.Connected) {
					Thread.Sleep (1000);
					if (HeartbeatSendTime <= DateTime.Now.AddSeconds (-HeartbeatInterval)) {
						if (DebugMode) {
							Console.WriteLine ("{0} 发送心跳包", DateTime.Now);
						}
						SendAsync (EmptyBytes);
					}
				}
			} catch (ThreadAbortException) {

			} catch (ThreadInterruptedException) {

			}
		}

		void PerformOnReceived (byte[] bytes) {
			if (OnReceived != null) {
				if (UseAsyncOnReceived) {
#if NETCOREAPP || NET5_0_OR_GREATER
					Task.Run (() => OnReceived (bytes));
#else
					OnReceived.BeginInvoke (bytes, asyncResult => OnReceived.EndInvoke (asyncResult), null);
#endif
					return;
				}
				OnReceived (bytes);//todo 如果有阻塞方法就会导致无法继续接收数据，这里必须得是异步的
			}
		}

	}

}