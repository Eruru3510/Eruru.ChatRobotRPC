using Eruru.ChatRobotRPC;
using Eruru.TextCommand;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace ConsoleApp2 {

	class Program {

		static readonly ChatRobot ChatRobot = new ChatRobot ();
		static readonly object Lock = new object ();

		static void Main (string[] args) {
			Console.Title = string.Empty;
			TextCommandSystem<ChatRobotMessage> textCommandSystem = new TextCommandSystem<ChatRobotMessage> ();
			textCommandSystem.Register<Program> ();
			textCommandSystem.MatchParameterType = false;
			ChatRobot.OnReceived = message => {
				WriteLine ($"收到消息：{message}");
			};
			ChatRobot.OnSend = message => {
				WriteLine ($"发送消息：{message}", ConsoleColor.Green);
			};
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
			};
			ChatRobot.OnDisconnected = () => {
				WriteLine ("连接断开");
				Connect ();
			};
			Connect ();
			Console.ReadLine ();
		}

		static void Connect () {
			try {
				WriteLine ("开始连接");
				ChatRobot.Connect ("localhost", 19730, "root", "root");
				WriteLine ("连接成功");
			} catch (SocketException socketException) {
				WriteLine (socketException, ConsoleColor.Red);
				Connect ();
			} catch (TimeoutException timeoutException) {
				Console.WriteLine ("连接成功，但是响应登录请求超时");
				Console.WriteLine (timeoutException);
				Connect ();
			} catch (Exception exception) {
				WriteLine (exception, ConsoleColor.Red);
			}
		}

		static void WriteLine (object text, ConsoleColor consoleColor = ConsoleColor.White) {
			lock (Lock) {
				ConsoleColor oldConsoleColor = Console.ForegroundColor;
				Console.ForegroundColor = consoleColor;
				Console.WriteLine (text);
				Console.ForegroundColor = oldConsoleColor;
			}
		}

		[TextCommand ("测试")]
		static void Test (ChatRobotMessage message) {
			message.Reply (message);
		}

	}

}