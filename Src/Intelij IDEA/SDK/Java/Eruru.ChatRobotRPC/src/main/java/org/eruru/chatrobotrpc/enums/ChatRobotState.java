package org.eruru.chatrobotrpc.enums;

public enum ChatRobotState {

	online (1),
	qMe (2),
	leave (3),
	busy (4),
	doNotDisturb (5),
	invisible (6);

	private final int value;

	ChatRobotState (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public ChatRobotState get (int value) {
		for (ChatRobotState item : ChatRobotState.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}