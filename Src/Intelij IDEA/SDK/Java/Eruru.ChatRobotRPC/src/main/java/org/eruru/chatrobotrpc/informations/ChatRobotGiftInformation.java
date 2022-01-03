package org.eruru.chatrobotrpc.informations;

public class ChatRobotGiftInformation {

	private int id;
	private String name;
	private int value;
	private int getTimestamp;
	private int expirationTimestamp;

	public int getID () {
		return id;
	}

	public void setID (int id) {
		this.id = id;
	}

	public String getName () {
		return name;
	}

	public void setName (String name) {
		this.name = name;
	}

	public int getValue () {
		return value;
	}

	public void setValue (int value) {
		this.value = value;
	}

	public int getGetTimestamp () {
		return getTimestamp;
	}

	public void setGetTimestamp (int getTimestamp) {
		this.getTimestamp = getTimestamp;
	}

	public int getExpirationTimestamp () {
		return expirationTimestamp;
	}

	public void setExpirationTimestamp (int expirationTimestamp) {
		this.expirationTimestamp = expirationTimestamp;
	}
}
