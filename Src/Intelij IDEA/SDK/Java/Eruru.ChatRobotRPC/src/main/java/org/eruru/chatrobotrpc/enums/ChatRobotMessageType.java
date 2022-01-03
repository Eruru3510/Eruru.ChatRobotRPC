package org.eruru.chatrobotrpc.enums;

public enum ChatRobotMessageType {

	friend (1),
	groupTemp (2),
	discussTemp (3),
	webpageTemp (4),
	friendVerificationReply (5),
	group (6),
	discuss (7);

	private final int value;

	ChatRobotMessageType (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public static ChatRobotMessageType get (int value) {
		for (ChatRobotMessageType item : ChatRobotMessageType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}