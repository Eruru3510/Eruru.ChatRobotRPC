package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupNameChangedEventHandler {

	void invoke (long robot, long group, String name);

}
