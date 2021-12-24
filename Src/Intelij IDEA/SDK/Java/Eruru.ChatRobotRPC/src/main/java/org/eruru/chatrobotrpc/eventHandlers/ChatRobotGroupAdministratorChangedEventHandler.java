package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupAdministratorChangedEventHandler {

	void invoke (boolean enable, long robot, long group, long qq);

}
