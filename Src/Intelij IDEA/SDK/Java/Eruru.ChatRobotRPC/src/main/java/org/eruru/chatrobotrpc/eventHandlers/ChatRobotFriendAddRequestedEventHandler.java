package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotFriendAddRequestedEventHandler {

	void invoke (long robot, long qq, String message);

}
