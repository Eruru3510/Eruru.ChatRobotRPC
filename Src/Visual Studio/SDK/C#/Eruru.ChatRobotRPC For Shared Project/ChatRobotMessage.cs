using System;

namespace Eruru.ChatRobotAPI {

	public class ChatRobotMessage {

		public ChatRobotMessageType Type { get; set; }
		public long Robot { get; set; }
		public long QQ { get; set; }
		public long Group { get; set; }
		public string Text { get; set; }
		public long Number { get; set; }
		public long ID { get; set; }
		public DateTime DateTime { get; }

		public ChatRobotMessage (ChatRobotMessageType type, long robot, long group, long qq, string text, long number, long iD) {
			Type = type;
			Robot = robot;
			QQ = qq;
			Group = group;
			Text = text;
			Number = number;
			ID = iD;
			DateTime = DateTime.Now;
		}

		public void Reply (string message) {
			Reply (message, ChatRobotSendMessageType.Text);
		}
		public void Reply (object message) {
			Reply (message?.ToString ());
		}

		public void ReplyJson (string message) {
			Reply (message, ChatRobotSendMessageType.Json);
		}
		public void ReplyJson (object message) {
			ReplyJson (message?.ToString ());
		}

		public void ReplyXml (string message) {
			Reply (message, ChatRobotSendMessageType.Xml);
		}
		public void ReplyXml (object message) {
			ReplyXml (message?.ToString ());
		}

		public static void Send (ChatRobotMessageType type, long robot, string message, long group = default, long qq = default, ChatRobotSendMessageType sendType = ChatRobotSendMessageType.Text) {
			switch (type) {
				case ChatRobotMessageType.Friend:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							ChatRobotAPI.SendFriendMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Json:
							ChatRobotAPI.SendFriendJsonMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Xml:
							ChatRobotAPI.SendFriendXmlMessage (robot, qq, message);
							break;
						default:
							throw new NotImplementedException (sendType.ToString ());
					}
					break;
				case ChatRobotMessageType.Group:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							ChatRobotAPI.SendGroupMessage (robot, group, message);
							break;
						case ChatRobotSendMessageType.Json:
							ChatRobotAPI.SendGroupJsonMessage (robot, group, message);
							break;
						case ChatRobotSendMessageType.Xml:
							ChatRobotAPI.SendGroupXmlMessage (robot, group, message);
							break;
						default:
							throw new NotImplementedException (sendType.ToString ());
					}
					break;
				case ChatRobotMessageType.Discuss:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							ChatRobotAPI.SendDiscussMessage (robot, group, message);
							break;
						case ChatRobotSendMessageType.Json:
							ChatRobotAPI.SendDiscussJsonMessage (robot, group, message);
							break;
						case ChatRobotSendMessageType.Xml:
							ChatRobotAPI.SendDiscussXmlMessage (robot, group, message);
							break;
						default:
							throw new NotImplementedException (sendType.ToString ());
					}
					break;
				default:
					throw new NotImplementedException (type.ToString ());
			}
		}
		public static void Send (ChatRobotMessageType type, long robot, object message, long group = default, long qq = default, ChatRobotSendMessageType sendType = ChatRobotSendMessageType.Text) {
			Send (type, robot, message?.ToString (), group, qq, sendType);
		}

		void Reply (string message, ChatRobotSendMessageType type) {
			Send (Type, Robot, message, Group, QQ, type);
		}

	}

}