package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupMemberLeftEventHandler {

	void invoke (boolean kick, long robot, long group, long qq, long operatorQQ);

}
