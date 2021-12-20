using Eruru.ChatRobotRPC;
using Eruru.Html;
using Eruru.Http;
using Eruru.TextCommand;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = string.Empty;
			TextCommandSystem<ChatRobotMessage> textCommandSystem = new TextCommandSystem<ChatRobotMessage> ();
			textCommandSystem.Register<Program> ();
			textCommandSystem.MatchParameterType = false;
			ChatRobotAPI.OnReceived = message => Console.WriteLine ($"收到消息：{message}");
			ChatRobotAPI.OnSent = message => Console.WriteLine ($"发送消息：{message}");
			ChatRobotAPI.OnReceivedMessage = message => {
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
			ChatRobotAPI.OnReceivedFriendAddResponse = (agree, robot, qq, message) => {
				Console.WriteLine ($"{agree} {robot} {qq} {message}");
			};
			ChatRobotAPI.OnDisconnected = () => Console.WriteLine ("连接断开");
			Console.WriteLine ("开始连接");
			ChatRobotAPI.Connect ("127.0.0.1", 19730, "root", "root");
			Console.WriteLine ("连接成功");
			Console.ReadLine ();
		}

		[TextCommand ("测试")]
		static void Test (ChatRobotMessage message) {

		}

	}

}