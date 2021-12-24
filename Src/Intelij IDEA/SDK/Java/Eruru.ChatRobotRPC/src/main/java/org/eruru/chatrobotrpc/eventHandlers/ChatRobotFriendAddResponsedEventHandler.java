package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotFriendAddResponsedEventHandler {

	void invoke (boolean agree, long robot, long qq, String message);

}
