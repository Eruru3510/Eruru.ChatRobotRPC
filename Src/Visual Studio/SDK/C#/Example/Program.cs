using Eruru.ChatRobotRPC;
using System;
using System.Net.Sockets;

namespace Example {

	class Program {

		static readonly ChatRobot ChatRobot = new ChatRobot ();

		static void Main (string[] args) {
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {//全局捕获未捕获异常，方便排查
				Console.WriteLine ($"{DateTime.Now} {nameof (sender)} = {sender}, {nameof (Exception)} = {e.ExceptionObject}");
			};
			Console.Title = string.Empty;
			ChatRobot.OnReceivedMessage = message => {//当收到消息
				if (message.Type == ChatRobotMessageType.Friend) {//如果是好友消息
					message.Reply ($"{ChatRobot.GetName (message.Robot, message.QQ)}发送了：{message}");//复读
				}
			};
			ChatRobot.OnDisconnected = () => {//当与机器人框架RPC插件断开了连接
				Console.WriteLine ("连接断开");
				Connect ();//重连
			};
			Connect ();//连接机器人框架RPC插件
			Console.ReadLine ();//保持运行
		}

		static void Connect () {
			try {
				Console.WriteLine ("开始连接");
				ChatRobot.Connect ("127.0.0.1", 19730, "root", "root");
				Console.WriteLine ("连接成功");
			} catch (SocketException socketException) {
				Console.WriteLine ("连接失败");
				Console.WriteLine (socketException);
				Connect ();
			}
		}

	}

}