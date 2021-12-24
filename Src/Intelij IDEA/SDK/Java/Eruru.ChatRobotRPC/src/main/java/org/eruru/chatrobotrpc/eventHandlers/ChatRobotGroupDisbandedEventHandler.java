package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupDisbandedEventHandler {

	void invoke (long robot, long group, long qq);

}
