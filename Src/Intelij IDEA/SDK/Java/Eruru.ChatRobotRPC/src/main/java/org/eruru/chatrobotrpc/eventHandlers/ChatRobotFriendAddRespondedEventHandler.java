package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotFriendAddRespondedEventHandler {

	void invoke (boolean agree, long robot, long qq, String message);

}
