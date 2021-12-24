package org.eruru.chatrobotrpc.enums;

public enum ChatRobotGroupAddRequestType {

	request (1),
	inviteMe (2),
	memberInvite (3);

	private final int value;

	ChatRobotGroupAddRequestType (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public ChatRobotGroupAddRequestType get (int value) {
		for (ChatRobotGroupAddRequestType item : ChatRobotGroupAddRequestType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
