package org.eruru.chatrobotrpc.informations;

/**
 * 状态信息
 */
public class ChatRobotStateInformation {

	private long qq;
	private String name;
	private String state;
	private String messageSpeed;
	private int receiveMessageNumber;
	private int sendMessageNumber;
	private String onlineTime;

	/**
	 * 机器人QQ
	 */
	public long getQQ () {
		return qq;
	}

	/**
	 * 机器人QQ
	 */
	public void setQQ (long qq) {
		this.qq = qq;
	}

	/**
	 * 昵称
	 */
	public String getName () {
		return name;
	}

	/**
	 * 昵称
	 */
	public void setName (String name) {
		this.name = name;
	}

	/**
	 * 在线状态
	 */
	public String getState () {
		return state;
	}

	/**
	 * 在线状态
	 */
	public void setState (String state) {
		this.state = state;
	}

	/**
	 * 消息速度
	 */
	public String getMessageSpeed () {
		return messageSpeed;
	}

	/**
	 * 消息速度
	 */
	public void setMessageSpeed (String messageSpeed) {
		this.messageSpeed = messageSpeed;
	}

	/**
	 * 接收消息数
	 */
	public int getReceiveMessageNumber () {
		return receiveMessageNumber;
	}

	/**
	 * 接收消息数
	 */
	public void setReceiveMessageNumber (int receiveMessageNumber) {
		this.receiveMessageNumber = receiveMessageNumber;
	}

	/**
	 * 发送消息数
	 */
	public int getSendMessageNumber () {
		return sendMessageNumber;
	}

	/**
	 * 发送消息数
	 */
	public void setSendMessageNumber (int sendMessageNumber) {
		this.sendMessageNumber = sendMessageNumber;
	}

	/**
	 * 在线时间
	 */
	public String getOnlineTime () {
		return onlineTime;
	}

	/**
	 * 在线时间
	 */
	public void setOnlineTime (String onlineTime) {
		this.onlineTime = onlineTime;
	}

}