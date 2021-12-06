using Eruru.ChatRobotAPI;
using System;

namespace Example {

	class Program {

		static void Main (string[] args) {
			Console.Title = string.Empty;
			ChatRobotAPI.OnReceivedMessage = message => {
				message.Reply ($"{ChatRobotAPI.GetName (message.Robot, message.QQ)}发送了：{message}");
			};
			ChatRobotAPI.Connect ("127.0.0.1", 19730, "root", "root");
			Console.ReadLine ();
		}

	}

}