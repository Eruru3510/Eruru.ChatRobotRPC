package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupNameChangedEventHandler {

	void invoke (long robot, long group, String name);

}
