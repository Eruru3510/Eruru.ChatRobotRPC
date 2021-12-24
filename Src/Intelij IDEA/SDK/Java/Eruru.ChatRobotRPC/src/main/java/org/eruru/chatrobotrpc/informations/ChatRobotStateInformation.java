package org.eruru.chatrobotrpc.informations;

public class ChatRobotStateInformation {

	private  long qq;
	private  String name;
	private  String state;
	private  String messageSpeed;
	private  int receiveMessageNumber;
	private  int  sendMessageNumber;
	private  String onlineTime;

	public long getQq () {
		return qq;
	}

	public void setQq (long qq) {
		this.qq = qq;
	}

	public String getName () {
		return name;
	}

	public void setName (String name) {
		this.name = name;
	}

	public String getState () {
		return state;
	}

	public void setState (String state) {
		this.state = state;
	}

	public String getMessageSpeed () {
		return messageSpeed;
	}

	public void setMessageSpeed (String messageSpeed) {
		this.messageSpeed = messageSpeed;
	}

	public int getReceiveMessageNumber () {
		return receiveMessageNumber;
	}

	public void setReceiveMessageNumber (int receiveMessageNumber) {
		this.receiveMessageNumber = receiveMessageNumber;
	}

	public int getSendMessageNumber () {
		return sendMessageNumber;
	}

	public void setSendMessageNumber (int sendMessageNumber) {
		this.sendMessageNumber = sendMessageNumber;
	}

	public String getOnlineTime () {
		return onlineTime;
	}

	public void setOnlineTime (String onlineTime) {
		this.onlineTime = onlineTime;
	}
}
