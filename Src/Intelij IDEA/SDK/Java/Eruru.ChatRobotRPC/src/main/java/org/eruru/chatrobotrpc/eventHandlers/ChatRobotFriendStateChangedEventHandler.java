package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotFriendStateChangedEventHandler {

	void invoke (long robot, long qq, String state);

}
