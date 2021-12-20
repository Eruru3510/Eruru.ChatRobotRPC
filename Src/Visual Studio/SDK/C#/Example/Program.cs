using Eruru.ChatRobotRPC;
using System;

namespace Example {

	class Program {

		static void Main (string[] args) {
			Console.Title = string.Empty;
			ChatRobotAPI.OnReceivedMessage = message => {//收到消息
				if (message.Type == ChatRobotMessageType.Friend) {//如果是好友消息
					message.Reply ($"{ChatRobotAPI.GetName (message.Robot, message.QQ)}发送了：{message}");//复读
				}
			};
			ChatRobotAPI.Connect ("127.0.0.1", 19730, "root", "root");
			Console.ReadLine ();//保持运行
		}

	}

}