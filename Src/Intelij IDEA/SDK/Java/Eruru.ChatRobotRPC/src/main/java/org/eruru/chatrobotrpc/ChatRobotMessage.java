package org.eruru.chatrobotrpc;

import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;
import org.eruru.chatrobotrpc.enums.ChatRobotSendMessageType;

import java.util.Date;

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

	public long getId () {
		return id;
	}

	public void setId (long id) {
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
		reply (message, false);
	}

	public void reply (Object message, boolean isAnonymous) {
		reply (message == null ? null : message.toString (), isAnonymous);
	}

	public void reply (Object message) {
		reply (message, false);
	}

	public void replyJson (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.json, isAnonymous);
	}

	public void replyJson (String message) {
		replyJson (message, false);
	}

	public void replyJson (Object message, boolean isAnonymous) {
		replyJson (message == null ? null : message.toString (), isAnonymous);
	}

	public void replyJson (Object message) {
		replyJson (message, false);
	}

	public void replyXml (String message, boolean isAnonymous) {
		reply (message, ChatRobotSendMessageType.xml, isAnonymous);
	}

	public void replyXml (String message) {
		replyJson (message, false);
	}

	public void replyXml (Object message, boolean isAnonymous) {
		replyJson (message == null ? null : message.toString (), isAnonymous);
	}

	public void replyXml (Object message) {
		replyJson (message, false);
	}

	public static void send (ChatRobot chatRobot, ChatRobotMessageType type, long robot, String message, long group, long qq, boolean isAnonymous,
							 ChatRobotSendMessageType sendType
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

	@Override
	public String toString () {
		return text;
	}

	private void reply (String message, ChatRobotSendMessageType type, boolean isAnonymous) {
		send (chatRobot, this.type, robot, message, group, qq, isAnonymous, type);
	}

}