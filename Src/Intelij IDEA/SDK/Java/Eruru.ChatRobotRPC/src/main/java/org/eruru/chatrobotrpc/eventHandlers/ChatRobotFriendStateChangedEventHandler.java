package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotFriendStateChangedEventHandler {

	void invoke (long robot, long qq, String state);

}
