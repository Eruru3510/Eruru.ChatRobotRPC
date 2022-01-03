package org.eruru.chatrobotrpc.enums;

public enum ChatRobotSendMessageType {

	text (0),
	json (1),
	xml (2);

	private final int value;

	ChatRobotSendMessageType (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public static ChatRobotSendMessageType get (int value) {
		for (ChatRobotSendMessageType item : ChatRobotSendMessageType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}