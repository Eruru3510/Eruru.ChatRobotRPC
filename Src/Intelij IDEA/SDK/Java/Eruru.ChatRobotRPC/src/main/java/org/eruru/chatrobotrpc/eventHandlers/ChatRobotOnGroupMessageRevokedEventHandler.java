package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotOnGroupMessageRevokedEventHandler {

	void invoke (long robotQQ, long group, long qq, long messageNumber, long messageID);

}