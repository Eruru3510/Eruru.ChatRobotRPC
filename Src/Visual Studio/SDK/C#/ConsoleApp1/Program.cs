using Eruru.ChatRobotRPC;
using Eruru.TextCommand;
using System;
using System.Net.Sockets;

namespace ConsoleApp1 {

	class Program {

		static readonly ChatRobot ChatRobot = new ChatRobot ();

		static void Main (string[] args) {
			Console.Title = string.Empty;
			TextCommandSystem<ChatRobotMessage> textCommandSystem = new TextCommandSystem<ChatRobotMessage> ();
			textCommandSystem.Register<Program> ();
			textCommandSystem.MatchParameterType = false;
			ChatRobot.OnReceived = message => Console.WriteLine ($"收到消息：{message}");
			ChatRobot.OnSent = message => Console.WriteLine ($"发送消息：{message}");
			ChatRobot.OnReceivedMessage = message => {
				switch (message.Type) {
					case ChatRobotMessageType.Friend:
						break;
					case ChatRobotMessageType.Group:
						if (message.Text.Contains (ChatRobotCode.At (message.Robot))) {
							message.Text = message.Text.Replace (ChatRobotCode.At (message.Robot), string.Empty);
							break;
						}
						return;
					default:
						return;
				}
				textCommandSystem.Execute (message.Text, message);
			};
			ChatRobot.OnReceivedFriendAddResponse = (agree, robot, qq, message) => {
				Console.WriteLine ($"{agree} {robot} {qq} {message}");
			};
			ChatRobot.OnDisconnected = () => {
				Console.WriteLine ("连接断开");
				Connect ();
			};
			Connect ();
			Console.ReadLine ();
		}

		static void Connect () {
			try {
				Console.WriteLine ("开始连接");
				ChatRobot.Connect ("127.0.0.1", 19730, "root", "root");
			} catch (SocketException socketException) {
				Console.WriteLine (socketException);
				Connect ();
				return;
			}
			Console.WriteLine ("连接成功");
		}

		[TextCommand ("测试")]
		static void Test (ChatRobotMessage message) {
			Console.WriteLine (message.ChatRobot.GetFriendAge (message.Robot, 1633756198));
		}

	}

}