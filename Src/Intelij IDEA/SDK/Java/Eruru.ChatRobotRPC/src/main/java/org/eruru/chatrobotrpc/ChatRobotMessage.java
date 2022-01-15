package org.eruru.chatrobotrpc;

import javafx.util.Pair;
import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;
import org.eruru.chatrobotrpc.enums.ChatRobotSendMessageType;

import java.util.Date;
import java.util.List;

/**
 * 聊天机器人消息
 */
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

	/**
	 * 所属实例
	 */
	public ChatRobot getChatRobot () {
		return chatRobot;
	}

	/**
	 * 所属实例
	 */
	public void setChatRobot (ChatRobot chatRobot) {
		this.chatRobot = chatRobot;
	}

	/**
	 * 来源
	 */
	public ChatRobotMessageType getType () {
		return type;
	}

	/**
	 * 来源
	 */
	public void setType (ChatRobotMessageType type) {
		this.type = type;
	}

	/**
	 * 机器人
	 */
	public long getRobot () {
		return robot;
	}

	/**
	 * 机器人
	 */
	public void setRobot (long robot) {
		this.robot = robot;
	}

	/**
	 * 发送者
	 */
	public long getQQ () {
		return qq;
	}

	/**
	 * 发送者
	 */
	public void setQQ (long qq) {
		this.qq = qq;
	}

	/**
	 * 来源群
	 */
	public long getGroup () {
		return group;
	}

	/**
	 * 来源群
	 */
	public void setGroup (long group) {
		this.group = group;
	}

	/**
	 * 内容
	 */
	public String getText () {
		return text;
	}

	/**
	 * 内容
	 */
	public void setText (String text) {
		this.text = text;
	}

	/**
	 * 与ID配合用于防撤回
	 */
	public long getNumber () {
		return number;
	}

	/**
	 * 与ID配合用于防撤回
	 */
	public void setNumber (long number) {
		this.number = number;
	}

	/**
	 * 与Number配合用于防撤回
	 */
	public long getID () {
		return id;
	}

	/**
	 * 与Number配合用于防撤回
	 */
	public void setID (long id) {
		this.id = id;
	}

	/**
	 * 接收时间
	 */
	public Date getReceivedTime () {
		return receivedTime;
	}

	/**
	 * 接收时间
	 */
	public void setReceivedTime (Date receivedTime) {
		this.receivedTime = receivedTime;
	}

	/**
	 * 完整构造聊天机器人消息
	 *
	 * @param chatRobot    调用实例
	 * @param type         消息类型
	 * @param robot        响应机器人
	 * @param group        来源群
	 * @param qq           来源QQ
	 * @param text         内容
	 * @param number       消息序号
	 * @param id           消息ID
	 * @param receivedTime 接收消息时间
	 */
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

	/**
	 * 构造聊天机器人消息，无需指定接收时间，默认为new Date ()
	 *
	 * @param chatRobot 调用实例
	 * @param type      消息类型
	 * @param robot     响应机器人
	 * @param group     来源群
	 * @param qq        来源QQ
	 * @param text      内容
	 * @param number    消息序号
	 * @param id        消息ID
	 */
	public ChatRobotMessage (ChatRobot chatRobot, ChatRobotMessageType type, long robot, long group, long qq, String text, long number, long id) {
		this (chatRobot, type, robot, group, qq, text, number, id, new Date ());
	}

	/**
	 * 回复消息
	 *
	 * @param message     内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void reply (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.text, isAnonymous);
	}

	/**
	 * 回复消息
	 *
	 * @param message 内容
	 */
	public void reply (String message) {
		reply (message, ChatRobotSendMessageType.text, false);
	}

	/**
	 * 回复消息
	 *
	 * @param message     内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void reply (Object message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.text, isAnonymous);
	}

	/**
	 * 回复消息
	 *
	 * @param message 内容
	 */
	public void reply (Object message) {
		reply (message, ChatRobotSendMessageType.text, false);
	}

	/**
	 * 回复消息
	 *
	 * @param format      格式化字符串
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 * @param args        可变长参数
	 */
	public void reply (String format, boolean isAnonymous, Object... args) {
		reply (format, ChatRobotSendMessageType.text, isAnonymous, args);
	}

	/**
	 * 回复消息
	 *
	 * @param format 格式化字符串
	 * @param args   可变长参数
	 */
	public void reply (String format, Object... args) {
		reply (format, ChatRobotSendMessageType.text, false, args);
	}

	/**
	 * 回复Json消息
	 *
	 * @param message     内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void replyJson (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.json, isAnonymous);
	}

	/**
	 * 回复Json消息
	 *
	 * @param message 内容
	 */
	public void replyJson (String message) {
		reply (message, ChatRobotSendMessageType.json, false);
	}

	/**
	 * 回复Json消息
	 *
	 * @param message     内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void replyJson (Object message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.json, isAnonymous);
	}

	/**
	 * 回复Json消息
	 *
	 * @param message 内容
	 */
	public void replyJson (Object message) {
		reply (message, ChatRobotSendMessageType.json, false);
	}

	/**
	 * 回复Json消息
	 *
	 * @param format      格式化字符串
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 * @param args        可变长参数
	 */
	public void replyJson (String format, boolean isAnonymous, Object... args) {
		reply (format, ChatRobotSendMessageType.json, isAnonymous, args);
	}

	/**
	 * 回复Json消息
	 *
	 * @param format 格式化字符串
	 * @param args   可变长参数
	 */
	public void replyJson (String format, Object... args) {
		reply (format, ChatRobotSendMessageType.json, false, args);
	}

	/**
	 * 回复Xml消息
	 *
	 * @param message     内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void replyXml (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.xml, isAnonymous);
	}

	/**
	 * 回复Xml消息
	 *
	 * @param message 内容
	 */
	public void replyXml (String message) {
		reply (message, ChatRobotSendMessageType.xml, false);
	}

	/**
	 * 回复Xml消息
	 *
	 * @param message     内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void replyXml (Object message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.xml, isAnonymous);
	}

	/**
	 * 回复Xml消息
	 *
	 * @param message 内容
	 */
	public void replyXml (Object message) {
		reply (message, ChatRobotSendMessageType.xml, false);
	}

	/**
	 * 回复Xml消息
	 *
	 * @param format      格式化字符串
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 * @param args        可变长参数
	 */
	public void replyXml (String format, boolean isAnonymous, Object... args) {
		reply (format, ChatRobotSendMessageType.xml, isAnonymous, args);
	}

	/**
	 * 回复Xml消息
	 *
	 * @param format 格式化字符串
	 * @param args   可变长参数
	 */
	public void replyXml (String format, Object... args) {
		reply (format, ChatRobotSendMessageType.xml, false, args);
	}

	/**
	 * 是否为语音消息
	 */
	public ChatRobotVoiceMessageResult isVoice () {
		return ChatRobotAPI.isVoiceMessage (text);
	}

	/**
	 * 消息中是否包含图片
	 *
	 * @return 是否包含, 提取出来的图片GUID
	 */
	public Pair<Boolean, List<String>> containsPicture () {
		return ChatRobotAPI.containsPictureInMessage (text);
	}

	/**
	 * 消息中是否包含艾特
	 *
	 * @return 是否包含, 被艾特的人
	 */
	public Pair<Boolean, List<Long>> containsAt () {
		return ChatRobotAPI.containsAtInMessage (text);
	}

	/**
	 * 是否为闪照消息
	 */
	public boolean isFlashPicture () {
		return ChatRobotAPI.isFlashPictureMessage (text);
	}

	/**
	 * 发送消息
	 *
	 * @param chatRobot   调用实例
	 * @param type        目标类型
	 * @param robot       机器人
	 * @param message     内容
	 * @param group       目标群
	 * @param qq          目标QQ
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 * @param sendType    文本、Json、Xml消息
	 */
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

	/**
	 * 发送消息
	 *
	 * @param chatRobot   调用实例
	 * @param type        目标类型
	 * @param robot       机器人
	 * @param message     内容
	 * @param group       目标群
	 * @param qq          目标QQ
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 * @param sendType    文本、Json、Xml消息
	 */
	public static void send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, Object message, long group, long qq,
							 boolean isAnonymous, ChatRobotSendMessageType sendType
	) {
		send (chatRobot, type, robot, message == null ? null : message.toString (), group, qq, isAnonymous, sendType);
	}

	/**
	 * 发送消息
	 *
	 * @param chatRobot   调用实例
	 * @param type        目标类型
	 * @param robot       机器人
	 * @param format      格式化字符串
	 * @param group       目标群
	 * @param qq          目标QQ
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 * @param sendType    文本、Json、Xml消息
	 * @param args        可变长参数
	 */
	public static void send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, String format, long group, long qq,
							 boolean isAnonymous, ChatRobotSendMessageType sendType, Object... args
	) {
		send (chatRobot, type, robot, String.format (format, args), group, qq, isAnonymous, sendType);
	}

	/**
	 * 3
	 * 返回消息内容（ChatRobotMessage.Text）
	 */
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