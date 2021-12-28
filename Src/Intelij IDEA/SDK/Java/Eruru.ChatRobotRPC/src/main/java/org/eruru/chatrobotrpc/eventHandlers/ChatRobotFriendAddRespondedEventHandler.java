package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotFriendAddRespondedEventHandler {

	void invoke (boolean agree, long robot, long qq, String message);

}
