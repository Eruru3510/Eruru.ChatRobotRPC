package org.eruru.chatrobotrpc;

public interface ChatRobotOnGroupMessageRevoke {

	void invoke (long robotQQ, long group, long qq, long messageNumber, long messageID);

}