package org.eruru.chatrobotrpc;

interface OnReceivedEventHandler {

	void invoke (byte[] bytes);

}