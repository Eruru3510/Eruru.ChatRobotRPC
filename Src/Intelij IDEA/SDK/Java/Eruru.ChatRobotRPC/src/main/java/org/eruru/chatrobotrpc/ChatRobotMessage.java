package org.eruru.chatrobotrpc;

import javafx.util.Pair;
import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;
import org.eruru.chatrobotrpc.enums.ChatRobotSendMessageType;

import java.util.Date;
import java.util.List;

public class ChatRobotMessage {

	private ChatRobot chatRobot;
	private ChatRobotMessageType type;
	private long robot;
	private long qq;
	private long group;
	private String text;
	private long number;
	private long id;
	private Date receivedTime;

	public ChatRobot getChatRobot () {
		return chatRobot;
	}

	public void setChatRobot (ChatRobot chatRobot) {
		this.chatRobot = chatRobot;
	}

	public ChatRobotMessageType getType () {
		return type;
	}

	public void setType (ChatRobotMessageType type) {
		this.type = type;
	}

	public long getRobot () {
		return robot;
	}

	public void setRobot (long robot) {
		this.robot = robot;
	}

	public long getQQ () {
		return qq;
	}

	public void setQQ (long qq) {
		this.qq = qq;
	}

	public long getGroup () {
		return group;
	}

	public void setGroup (long group) {
		this.group = group;
	}

	public String getText () {
		return text;
	}

	public void setText (String text) {
		this.text = text;
	}

	public long getNumber () {
		return number;
	}

	public void setNumber (long number) {
		this.number = number;
	}

	public long getID () {
		return id;
	}

	public void setID (long id) {
		this.id = id;
	}

	public Date getReceivedTime () {
		return receivedTime;
	}

	public void setReceivedTime (Date receivedTime) {
		this.receivedTime = receivedTime;
	}

	public ChatRobotMessage (ChatRobot chatRobot, ChatRobotMessageType type, long robot, long group, long qq, String text, long number, long id, Date receivedTime) {
		this.chatRobot = chatRobot;
		this.type = type;
		this.robot = robot;
		this.qq = qq;
		this.group = group;
		this.text = text;
		this.number = number;
		this.id = id;
		this.receivedTime = receivedTime;
	}

	public ChatRobotMessage (ChatRobot chatRobot, ChatRobotMessageType type, long robot, long group, long qq, String text, long number, long id) {
		this (chatRobot, type, robot, group, qq, text, number, id, new Date ());
	}

	public void reply (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.text, isAnonymous);
	}

	public void reply (String message) {
		reply (message, ChatRobotSendMessageType.text, false);
	}

	public void reply (Object message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.text, isAnonymous);
	}

	public void reply (Object message) {
		reply (message, ChatRobotSendMessageType.text, false);
	}

	public void reply (String format, boolean isAnonymous, Object... args) {
		reply (format, ChatRobotSendMessageType.text, isAnonymous, args);
	}

	public void reply (String format, Object... args) {
		reply (format, ChatRobotSendMessageType.text, false, args);
	}

	public void replyJson (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.json, isAnonymous);
	}

	public void replyJson (String message) {
		reply (message, ChatRobotSendMessageType.json, false);
	}

	public void replyJson (Object message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.json, isAnonymous);
	}

	public void replyJson (Object message) {
		reply (message, ChatRobotSendMessageType.json, false);
	}

	public void replyJson (String format, boolean isAnonymous, Object... args) {
		reply (format, ChatRobotSendMessageType.json, isAnonymous, args);
	}

	public void replyJson (String format, Object... args) {
		reply (format, ChatRobotSendMessageType.json, false, args);
	}

	public void replyXml (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.xml, isAnonymous);
	}

	public void replyXml (String message) {
		reply (message, ChatRobotSendMessageType.xml, false);
	}

	public void replyXml (Object message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.xml, isAnonymous);
	}

	public void replyXml (Object message) {
		reply (message, ChatRobotSendMessageType.xml, false);
	}

	public void replyXml (String format, boolean isAnonymous, Object... args) {
		reply (format, ChatRobotSendMessageType.xml, isAnonymous, args);
	}

	public void replyXml (String format, Object... args) {
		reply (format, ChatRobotSendMessageType.xml, false, args);
	}

	/// <summary>
	/// 是否为语音消息
	/// </summary>
	/// <param name="message">消息文本</param>
	/// <param name="guid">提取出来的语音GUID</param>
	/// <param name="identifyResult">提取出来的语音识别结果</param>
	/// <returns></returns>
	public ChatRobotVoiceMessageResult isVoice () {
		return ChatRobotAPI.isVoiceMessage (text);
	}

	/// <summary>
	/// 消息中是否包含图片
	/// </summary>
	/// <param name="message">消息文本</param>
	/// <param name="guids">提取出来的图片GUID</param>
	/// <returns></returns>
	public Pair<Boolean, List<String>> containsPicture () {
		return ChatRobotAPI.containsPictureInMessage (text);
	}

	/// <summary>
	/// 是否为闪照消息
	/// </summary>
	/// <param name="message">消息文本</param>
	/// <returns></returns>
	public boolean isFlashPicture () {
		return ChatRobotAPI.isFlashPictureMessage (text);
	}

	public static void send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, String message, long group, long qq,
							 boolean isAnonymous, ChatRobotSendMessageType sendType
	) {
		switch (type) {
			case friend:
				switch (sendType) {
					case text:
						chatRobot.sendFriendMessage (robot, qq, message);
						break;
					case json:
						chatRobot.sendFriendJsonMessage (robot, qq, message);
						break;
					case xml:
						chatRobot.sendFriendXmlMessage (robot, qq, message);
						break;
					default:
						new Exception (String.format ("%s.%s", type, sendType)).printStackTrace ();
						break;
				}
				break;
			case group:
				switch (sendType) {
					case text:
						chatRobot.sendGroupMessage (robot, group, message, isAnonymous);
						break;
					case json:
						chatRobot.sendGroupJsonMessage (robot, group, message, isAnonymous);
						break;
					case xml:
						chatRobot.sendGroupXmlMessage (robot, group, message, isAnonymous);
						break;
					default:
						new Exception (String.format ("%s.%s", type, sendType)).printStackTrace ();
						break;
				}
				break;
			default:
				new Exception (String.format ("%s.%s", type, sendType)).printStackTrace ();
				break;
		}
	}

	public static void send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, Object message, long group, long qq,
							 boolean isAnonymous, ChatRobotSendMessageType sendType
	) {
		send (chatRobot, type, robot, message == null ? null : message.toString (), group, qq, isAnonymous, sendType);
	}

	public static void send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, String format, long group, long qq,
							 boolean isAnonymous, ChatRobotSendMessageType sendType, Object... args
	) {
		send (chatRobot, type, robot, String.format (format, args), group, qq, isAnonymous, sendType);
	}

	@Override
	public String toString () {
		return text;
	}

	private void reply (String message, ChatRobotSendMessageType type, boolean isAnonymous) {
		send (chatRobot, this.type, robot, message, group, qq, isAnonymous, type);
	}

	private void reply (Object message, ChatRobotSendMessageType type, boolean isAnonymous) {
		send (chatRobot, this.type, robot, message == null ? null : message.toString (), group, qq, isAnonymous, type);
	}

	private void reply (String format, ChatRobotSendMessageType type, boolean isAnonymous, Object... args) {
		send (chatRobot, this.type, robot, String.format (format, args), group, qq, isAnonymous, type);
	}

}