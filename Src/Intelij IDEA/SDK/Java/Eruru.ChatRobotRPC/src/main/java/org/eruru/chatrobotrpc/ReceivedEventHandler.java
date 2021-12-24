package org.eruru.chatrobotrpc;

interface ReceivedEventHandler {

	void invoke (byte[] bytes);

}