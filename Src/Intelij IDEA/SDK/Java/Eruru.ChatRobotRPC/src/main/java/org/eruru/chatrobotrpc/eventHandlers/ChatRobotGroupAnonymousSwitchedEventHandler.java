package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupAnonymousSwitchedEventHandler {

	void invoke (boolean enable, long robot, long group, long qq);

}
