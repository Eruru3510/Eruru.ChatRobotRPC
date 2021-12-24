package org.eruru.chatrobotrpc.enums;

public enum ChatRobotGender {

	male (0),
	female (1),
	hide (255);

	private final int value;

	ChatRobotGender (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public ChatRobotGender get (int value) {
		for (ChatRobotGender item : ChatRobotGender.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
