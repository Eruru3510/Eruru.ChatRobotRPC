package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupAnonymousSwitchedEventHandler {

	void invoke (boolean enable, long robot, long group, long qq);

}
