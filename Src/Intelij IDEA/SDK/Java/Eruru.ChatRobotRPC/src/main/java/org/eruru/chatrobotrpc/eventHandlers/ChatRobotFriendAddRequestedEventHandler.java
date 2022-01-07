package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotFriendAddRequestedEventHandler {

	void invoke (long robot, long qq, String message);

}
