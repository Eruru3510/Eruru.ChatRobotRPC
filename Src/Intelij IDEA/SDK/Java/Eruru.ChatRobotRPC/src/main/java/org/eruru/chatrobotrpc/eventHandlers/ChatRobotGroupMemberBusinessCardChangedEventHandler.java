package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupMemberBusinessCardChangedEventHandler {

	void invoke (long robot, long group, long qq, String businessCard);

}
