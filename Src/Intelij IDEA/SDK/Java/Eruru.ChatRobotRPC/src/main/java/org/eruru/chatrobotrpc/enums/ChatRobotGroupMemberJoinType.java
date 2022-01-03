package org.eruru.chatrobotrpc.enums;

public enum ChatRobotGroupMemberJoinType {

	approve (1),
	iJoin (2),
	invite (3);

	private final int value;

	ChatRobotGroupMemberJoinType (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public static ChatRobotGroupMemberJoinType get (int value) {
		for (ChatRobotGroupMemberJoinType item : ChatRobotGroupMemberJoinType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}