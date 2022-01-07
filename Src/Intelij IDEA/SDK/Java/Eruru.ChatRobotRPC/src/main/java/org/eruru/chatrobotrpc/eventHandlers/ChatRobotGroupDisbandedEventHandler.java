package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupDisbandedEventHandler {

	void invoke (long robot, long group, long qq);

}
