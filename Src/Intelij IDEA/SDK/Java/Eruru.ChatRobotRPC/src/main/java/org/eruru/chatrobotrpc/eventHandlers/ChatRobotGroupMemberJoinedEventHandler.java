package org.eruru.chatrobotrpc.eventhandlers;

import org.eruru.chatrobotrpc.enums.ChatRobotGroupMemberJoinType;

public interface ChatRobotGroupMemberJoinedEventHandler {

	void invoke (ChatRobotGroupMemberJoinType type, long robot, long group, long qq, long operatorQQ);

}
