package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupMemberBusinessCardChangedEventHandler {

	void invoke (long robot, long group, long qq, String businessCard);

}
