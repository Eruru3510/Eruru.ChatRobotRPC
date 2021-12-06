package org.eruru.chatrobotrpc;

import java.util.Date;

public class ChatRobotMessage {

	private ChatRobotMessageType type;
	private long robotQQ;
	private long qq;
	private long group;
	private String text;
	private long number;
	private long id;
	private Date date;

	public ChatRobotMessage (ChatRobotMessageType type, long robotQQ, String text, long number, long id, long qq, long group) {
		this.setType (type);
		this.setRobotQQ (robotQQ);
		this.setQQ (qq);
		this.setGroup (group);
		this.setText (text);
		this.setNumber (number);
		this.setId (id);
		this.setDate (new Date ());
	}

	public ChatRobotMessageType getType () {
		return type;
	}

	public void setType (ChatRobotMessageType type) {
		this.type = type;
	}

	public long getRobotQQ () {
		return robotQQ;
	}

	public void setRobotQQ (long robotQQ) {
		this.robotQQ = robotQQ;
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

	public Date getDate () {
		return date;
	}

	public void setDate (Date date) {
		this.date = date;
	}

	public void Reply (String message) {
		reply (message, false);
	}

	public void replyJson (String message) {
		reply (message, true);
	}

	private void reply (String message, boolean isJson) {
		switch (getType ()) {
			case Friend:
				if (isJson) {
					ChatRobotAPI.sendFriendJsonMessage (robotQQ, qq, message);
					break;
				}
				ChatRobotAPI.sendFriendMessage (robotQQ, qq, message);
				break;
			case Group:
				if (isJson) {
					ChatRobotAPI.sendGroupJsonMessage (robotQQ, group, message);
					break;
				}
				ChatRobotAPI.sendGroupMessage (robotQQ, group, message);
				break;
		}
	}

}
