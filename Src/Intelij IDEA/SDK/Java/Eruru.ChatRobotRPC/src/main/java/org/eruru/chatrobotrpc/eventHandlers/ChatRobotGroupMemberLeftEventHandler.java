package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupMemberLeftEventHandler {

	void invoke (boolean kick, long robot, long group, long qq, long operatorQQ);

}
