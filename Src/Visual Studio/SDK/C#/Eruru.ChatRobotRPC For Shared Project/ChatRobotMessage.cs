using System;

namespace Eruru.ChatRobotRPC {

	public class ChatRobotMessage {

		/// <summary>
		/// 来源
		/// </summary>
		public ChatRobotMessageType Type { get; set; }
		/// <summary>
		/// 机器人
		/// </summary>
		public long Robot { get; set; }
		/// <summary>
		/// 发送者
		/// </summary>
		public long QQ { get; set; }
		/// <summary>
		/// 来源群
		/// </summary>
		public long Group { get; set; }
		/// <summary>
		/// 内容
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// 与ID配合用于防撤回
		/// </summary>
		public long Number { get; set; }
		/// <summary>
		/// 与Number配合用于防撤回
		/// </summary>
		public long ID { get; set; }
		/// <summary>
		/// 发送时间
		/// </summary>
		public DateTime DateTime { get; set; }

		public ChatRobotMessage (ChatRobotMessageType type, long robot, long group, long qq, string text, long number, long id, DateTime dateTime) {
			Type = type;
			Robot = robot;
			QQ = qq;
			Group = group;
			Text = text;
			Number = number;
			ID = id;
			DateTime = dateTime;
		}
		public ChatRobotMessage (ChatRobotMessageType type, long robot, long group, long qq, string text, long number, long id)
			: this (type, robot, group, qq, text, number, id, DateTime.Now
		) {

		}

		/// <summary>
		/// 回复消息
		/// </summary>
		/// <param name="message"></param>
		public void Reply (string message) {
			Reply (message, ChatRobotSendMessageType.Text);
		}
		/// <summary>
		/// 回复消息
		/// </summary>
		/// <param name="message"></param>
		public void Reply (object message) {
			Reply (message?.ToString ());
		}

		/// <summary>
		/// 回复Json消息
		/// </summary>
		/// <param name="message"></param>
		public void ReplyJson (string message) {
			Reply (message, ChatRobotSendMessageType.Json);
		}
		/// <summary>
		/// 回复Json消息
		/// </summary>
		/// <param name="message"></param>
		public void ReplyJson (object message) {
			ReplyJson (message?.ToString ());
		}

		/// <summary>
		/// 回复Xml消息
		/// </summary>
		/// <param name="message"></param>
		public void ReplyXml (string message) {
			Reply (message, ChatRobotSendMessageType.Xml);
		}
		/// <summary>
		/// 回复Xml消息
		/// </summary>
		/// <param name="message"></param>
		public void ReplyXml (object message) {
			ReplyXml (message?.ToString ());
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="type">目标类型</param>
		/// <param name="robot">机器人</param>
		/// <param name="message">内容</param>
		/// <param name="group">目标群</param>
		/// <param name="qq">目标QQ</param>
		/// <param name="sendType">文本、Json、Xml消息</param>
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
		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="type">目标类型</param>
		/// <param name="robot">机器人</param>
		/// <param name="message">内容</param>
		/// <param name="group">目标群</param>
		/// <param name="qq">目标QQ</param>
		/// <param name="sendType">文本、Json、Xml消息</param>
		public static void Send (ChatRobotMessageType type, long robot, object message, long group = default, long qq = default, ChatRobotSendMessageType sendType = ChatRobotSendMessageType.Text) {
			Send (type, robot, message?.ToString (), group, qq, sendType);
		}

		/// <summary>
		/// 返回消息内容
		/// </summary>
		/// <returns></returns>
		public override string ToString () {
			return Text;
		}

		void Reply (string message, ChatRobotSendMessageType type) {
			Send (Type, Robot, message, Group, QQ, type);
		}

	}

}