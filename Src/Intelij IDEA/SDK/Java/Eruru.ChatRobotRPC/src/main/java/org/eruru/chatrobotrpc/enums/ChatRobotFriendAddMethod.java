package org.eruru.chatrobotrpc.enums;

public enum ChatRobotFriendAddMethod {

	allowAny (0),
	needValidation (1),
	needRightAnswer (3),
	needAnswerQuestion (4),
	alreadyFriend (99);

	private final int value;

	ChatRobotFriendAddMethod (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public static ChatRobotFriendAddMethod get (int value) {
		for (ChatRobotFriendAddMethod item : ChatRobotFriendAddMethod.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
