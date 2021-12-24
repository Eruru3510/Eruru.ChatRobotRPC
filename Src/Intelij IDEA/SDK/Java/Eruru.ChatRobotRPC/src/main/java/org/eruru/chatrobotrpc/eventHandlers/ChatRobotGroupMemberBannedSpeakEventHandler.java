package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupMemberBannedSpeakEventHandler {

	void invoke (boolean enable, long robot, long group, long qq, long operatorQQ, int seconds);

}
