package org.eruru.chatrobotrpc.informations;

/**
 * 礼物信息
 */
public class ChatRobotGiftInformation {

	private int id;
	private String name;
	private int value;
	private int getTimestamp;
	private int expirationTimestamp;

	/**
	 * 礼物ID
	 */
	public int getID () {
		return id;
	}

	/**
	 * 礼物ID
	 */
	public void setID (int id) {
		this.id = id;
	}

	/**
	 * 礼物名称
	 */
	public String getName () {
		return name;
	}

	/**
	 * 礼物名称
	 */
	public void setName (String name) {
		this.name = name;
	}

	/**
	 * 礼物价值
	 */
	public int getValue () {
		return value;
	}

	/**
	 * 礼物价值
	 */
	public void setValue (int value) {
		this.value = value;
	}

	/**
	 * 礼物获得时间戳
	 */
	public int getGetTimestamp () {
		return getTimestamp;
	}

	/**
	 * 礼物获得时间戳
	 */
	public void setGetTimestamp (int getTimestamp) {
		this.getTimestamp = getTimestamp;
	}

	/**
	 * 礼物过期时间戳
	 */
	public int getExpirationTimestamp () {
		return expirationTimestamp;
	}

	/**
	 * 礼物过期时间戳
	 */
	public void setExpirationTimestamp (int expirationTimestamp) {
		this.expirationTimestamp = expirationTimestamp;
	}

}
