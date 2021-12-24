package org.eruru.chatrobotrpc.enums;

public enum ChatRobotRequestType {

	ignore (30),
	agree (10),
	refuse (20);

	private final int value;

	ChatRobotRequestType (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public ChatRobotRequestType get (int value) {
		for (ChatRobotRequestType item : ChatRobotRequestType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
