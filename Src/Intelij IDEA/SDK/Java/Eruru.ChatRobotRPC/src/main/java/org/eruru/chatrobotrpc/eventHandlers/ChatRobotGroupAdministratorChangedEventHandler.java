package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupAdministratorChangedEventHandler {

	void invoke (boolean enable, long robot, long group, long qq);

}
