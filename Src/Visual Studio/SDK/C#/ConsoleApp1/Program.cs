using Eruru.ChatRobotRPC;
using Eruru.TextCommand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			ChatRobot.OnSend = message => Console.WriteLine ($"发送消息：{message}");
			ChatRobot.OnReceivedMessage = message => {
				string text = message.Text;
				switch (message.Type) {
					case ChatRobotMessageType.Friend:
						break;
					case ChatRobotMessageType.Group:
						if (File.ReadAllLines ("D:/Group.txt").Contains (message.Group.ToString ())) {
							break;
						}
						return;
					default:
						return;
				}
				textCommandSystem.Execute (text, message);
				if (message.IsFlashPicture ()) {
					ChatRobot.SendFriendMessage (message.Robot, 1633756198, ChatRobot.GroupPictureToFriendPicture (message.Robot, ChatRobotAPI.FlashPictureToPicture (text)));
				}
				if (message.ContainsPicture (out List<string> guids)) {
					ChatRobot.SendFriendMessage (message.Robot, 1633756198, string.Join ("\n", guids.ConvertAll (guid => ChatRobot.GroupPictureToFriendPicture (message.Robot, guid))));
				}
				if (message.IsVoice (out _, out string identifyResult)) {
					ChatRobot.SendFriendMessage (message.Robot, 1633756198, $"语音识别结果：{identifyResult}");
				}
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
				ChatRobot.Connect (File.ReadAllText ("D:/IP.txt"), 19730, "root", "root");
				Console.WriteLine ("连接成功");
			} catch (SocketException socketException) {
				Console.WriteLine (socketException);
				Connect ();
			} catch (Exception exception) {
				Console.WriteLine (exception);
			}
		}

		[TextCommand ("测试")]
		static void Test (ChatRobotMessage message) {

		}

	}

}