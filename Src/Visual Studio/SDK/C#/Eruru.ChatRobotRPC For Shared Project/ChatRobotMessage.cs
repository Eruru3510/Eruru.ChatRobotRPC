using System;

namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 聊天机器人消息
	/// </summary>
	public class ChatRobotMessage {

		/// <summary>
		/// 所属实例
		/// </summary>
		public ChatRobot ChatRobot { get; set; }
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
		/// 接收时间
		/// </summary>
		public DateTime ReceivedTime { get; set; }

		/// <summary>
		/// 完整构造聊天机器人消息
		/// </summary>
		/// <param name="chatRobot">调用实例</param>
		/// <param name="type">消息类型</param>
		/// <param name="robot">响应机器人</param>
		/// <param name="group">来源群</param>
		/// <param name="qq">来源QQ</param>
		/// <param name="text">内容</param>
		/// <param name="number">消息序号</param>
		/// <param name="id">消息ID</param>
		/// <param name="receivedTime">接收消息时间</param>
		public ChatRobotMessage (ChatRobot chatRobot, ChatRobotMessageType type, long robot, long group, long qq, string text, long number, long id,
			DateTime receivedTime
		) {
			ChatRobot = chatRobot;
			Type = type;
			Robot = robot;
			QQ = qq;
			Group = group;
			Text = text;
			Number = number;
			ID = id;
			ReceivedTime = receivedTime;
		}
		/// <summary>
		/// 构造聊天机器人消息，无需指定接收时间，默认为DateTime.Now
		/// </summary>
		/// <param name="chatRobot">调用实例</param>
		/// <param name="type">消息类型</param>
		/// <param name="robot">响应机器人</param>
		/// <param name="group">来源群</param>
		/// <param name="qq">来源QQ</param>
		/// <param name="text">内容</param>
		/// <param name="number">消息序号</param>
		/// <param name="id">消息ID</param>
		public ChatRobotMessage (ChatRobot chatRobot, ChatRobotMessageType type, long robot, long group, long qq, string text, long number, long id)
			: this (chatRobot, type, robot, group, qq, text, number, id, DateTime.Now
		) {

		}

		/// <summary>
		/// 回复消息
		/// </summary>
		/// <param name="message">内容</param>
		/// 	/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void Reply (string message, bool isAnonymous = false) {
			Reply (message, ChatRobotSendMessageType.Text, isAnonymous);
		}
		/// <summary>
		/// 回复消息
		/// </summary>
		/// <param name="message">内容</param>
		/// 	/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void Reply (object message, bool isAnonymous = false) {
			Reply (message?.ToString (), isAnonymous);
		}

		/// <summary>
		/// 回复Json消息
		/// </summary>
		/// <param name="message">内容</param>
		/// 	/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void ReplyJson (string message, bool isAnonymous = false) {
			Reply (message, ChatRobotSendMessageType.Json, isAnonymous);
		}
		/// <summary>
		/// 回复Json消息
		/// </summary>
		/// <param name="message">内容</param>
		/// 	/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void ReplyJson (object message, bool isAnonymous = false) {
			ReplyJson (message?.ToString (), isAnonymous);
		}

		/// <summary>
		/// 回复Xml消息
		/// </summary>
		/// <param name="message">内容</param>
		/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void ReplyXml (string message, bool isAnonymous = false) {
			Reply (message, ChatRobotSendMessageType.Xml, isAnonymous);
		}
		/// <summary>
		/// 回复Xml消息
		/// </summary>
		/// <param name="message">群内</param>
		/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void ReplyXml (object message, bool isAnonymous = false) {
			ReplyXml (message?.ToString (), isAnonymous);
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="chatRobot">调用实例</param>
		/// <param name="type">目标类型</param>
		/// <param name="robot">机器人</param>
		/// <param name="message">内容</param>
		/// <param name="group">目标群</param>
		/// <param name="qq">目标QQ</param>
		/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		/// <param name="sendType">文本、Json、Xml消息</param>
		public static void Send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, string message, long group = default, long qq = default,
			bool isAnonymous = false, ChatRobotSendMessageType sendType = ChatRobotSendMessageType.Text
		) {
			switch (type) {
				case ChatRobotMessageType.Friend:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendFriendMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendFriendJsonMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendFriendXmlMessage (robot, qq, message);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				case ChatRobotMessageType.GroupTemp:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendGroupTempMessage (robot, group, qq, message);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendGroupTempJsonMessage (robot, group, qq, message);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendGroupTempXmlMessage (robot, group, qq, message);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				case ChatRobotMessageType.DiscussTemp:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendDiscussTempMessage (robot, group, qq, message);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendDiscussTempJsonMessage (robot, group, qq, message);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendDiscussTempXmlMessage (robot, group, qq, message);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				case ChatRobotMessageType.WebpageTemp:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendWebpageTempMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendWebpageTempJsonMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendWebpageTempXmlMessage (robot, qq, message);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				case ChatRobotMessageType.FriendVerificationReply:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendFriendVerificationReplyMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendFriendVerificationReplyJsonMessage (robot, qq, message);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendFriendVerificationReplyXmlMessage (robot, qq, message);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				case ChatRobotMessageType.Group:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendGroupMessage (robot, group, message, isAnonymous);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendGroupJsonMessage (robot, group, message, isAnonymous);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendGroupXmlMessage (robot, group, message, isAnonymous);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				case ChatRobotMessageType.Discuss:
					switch (sendType) {
						case ChatRobotSendMessageType.Text:
							chatRobot.SendDiscussMessage (robot, group, message);
							break;
						case ChatRobotSendMessageType.Json:
							chatRobot.SendDiscussJsonMessage (robot, group, message);
							break;
						case ChatRobotSendMessageType.Xml:
							chatRobot.SendDiscussXmlMessage (robot, group, message);
							break;
						default:
							throw new NotImplementedException ($"{type}.{sendType}");
					}
					break;
				default:
					throw new NotImplementedException (type.ToString ());
			}
		}
		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="chatRobot">调用实例</param>
		/// <param name="type">目标类型</param>
		/// <param name="robot">机器人</param>
		/// <param name="message">内容</param>
		/// <param name="group">目标群</param>
		/// <param name="qq">目标QQ</param>
		/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		/// <param name="sendType">文本、Json、Xml消息</param>
		public static void Send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, object message, long group = default, long qq = default,
			bool isAnonymous = false,
			ChatRobotSendMessageType sendType = ChatRobotSendMessageType.Text
		) {
			Send (chatRobot, type, robot, message?.ToString (), group, qq, isAnonymous, sendType);
		}

		/// <summary>
		/// 返回消息内容（ChatRobotMessage.Text）
		/// </summary>
		/// <returns></returns>
		public override string ToString () {
			return Text;
		}

		void Reply (string message, ChatRobotSendMessageType type, bool isAnonymous) {
			Send (ChatRobot, Type, Robot, message, Group, QQ, isAnonymous, type);
		}

	}

}