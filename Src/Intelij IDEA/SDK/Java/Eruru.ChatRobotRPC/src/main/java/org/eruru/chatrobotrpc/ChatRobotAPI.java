package org.eruru.chatrobotrpc;

import com.alibaba.fastjson.JSONObject;

import java.io.IOException;
import java.nio.charset.StandardCharsets;

public class ChatRobotAPI {

	private static final ReturnSystem returnSystem = new ReturnSystem ();

	private static ChatRobotOnReceived onReceived;
	private static ChatRobotOnSent onSent;
	private static ChatRobotOnGroupMessageRevoke onGroupMessageRevoke;
	private static ChatRobotOnReceivedMessage onReceivedMessage;
	private static ChatRobotAction onDisconnected;
	private static final Client client = new Client () {{
		setOnReceived (ChatRobotAPI::onReceived);
		setOnSent (ChatRobotAPI::Sent);
		setOnDisconnected (ChatRobotAPI::onDisconnected);
	}};
	private static long id;

	public static ChatRobotOnReceived getOnReceived () {
		return onReceived;
	}

	public static void setOnReceived (ChatRobotOnReceived onReceived) {
		ChatRobotAPI.onReceived = onReceived;
	}

	public static ChatRobotOnSent getOnSent () {
		return onSent;
	}

	public static void setOnSent (ChatRobotOnSent onSent) {
		ChatRobotAPI.onSent = onSent;
	}

	public static void setOnGroupMessageRevoke (ChatRobotOnGroupMessageRevoke onGroupMessageRevoke) {
		ChatRobotAPI.onGroupMessageRevoke = onGroupMessageRevoke;
	}

	public static ChatRobotOnGroupMessageRevoke getOnGroupMessageRevoke () {
		return onGroupMessageRevoke;
	}

	public static void setOnReceivedMessage (ChatRobotOnReceivedMessage onReceivedMessage) {
		ChatRobotAPI.onReceivedMessage = onReceivedMessage;
	}

	public static ChatRobotOnReceivedMessage getOnReceivedMessage () {
		return onReceivedMessage;
	}

	public static void setOnDisconnected (ChatRobotAction onDisconnected) {
		ChatRobotAPI.onDisconnected = onDisconnected;
	}

	public static ChatRobotAction getOnDisconnected () {
		return onDisconnected;
	}

	public static void connect (String ip, int port, String account, String password) throws IOException {
		client.connect (ip, port);
		client.beginSend (new JSONObject () {{
			put ("Type", "Login");
			put ("Account", account);
			put ("Password", password);
		}}.toJSONString ());
	}

	public static void sendGroupMessage (long robotQQ, long group, String message) {
		sendGroupMessage ("SendGroupMessage", robotQQ, group, message);
	}

	public static void sendGroupJsonMessage (long robotQQ, long group, String message) {
		sendGroupMessage ("SendGroupJsonMessage", robotQQ, group, message);
	}

	public static void sendFriendMessage (long robotQQ, long qq, String message) {
		sendFriendMessage ("SendFriendMessage", robotQQ, qq, message);
	}

	public static void sendFriendJsonMessage (long robotQQ, long qq, String message) {
		sendFriendMessage ("SendFriendJsonMessage", robotQQ, qq, message);
	}

	public static String getGroupName (long robotQQ, long group) {
		long id = ChatRobotAPI.id++;
		client.beginSend (new JSONObject () {{
			put ("Type", "GetGroupName");
			put ("ID", id);
			put ("RobotQQ", robotQQ);
			put ("Group", group);
		}}.toJSONString ());
		return returnSystem.get (id);
	}

	public static void disconnect () throws IOException {
		client.Disconnect ();
	}

	private static void Sent (String text){
		if(onSent!=null){
			onSent.invoke (text);
		}
	}

	 static byte[] intToBytes (int i) {
		byte[] result = new byte[4];
		result[3] = (byte) ((i >>> 24) & 0xFF);
		result[2] = (byte) ((i >>> 16) & 0xFF);
		result[1] = (byte) ((i >>> 8) & 0xFF);
		result[0] = (byte) (i & 0xFF);
		return result;
	}

	 static int bytesToInt (byte[] bytes) {
		int value = 0;
		for (int i = 0; i < 4; i++) {
			int shift = i * 8;
			value += (bytes[i] & 0xFF) << shift;
		}
		return value;
	}

	private static void onReceived (byte[] bytes) {
		new Thread (() -> {
			String text = new String (bytes, StandardCharsets.UTF_8);
			if (onReceived!=null) {
				onReceived.invoke (text);
			}
			JSONObject jsonObject = JSONObject.parseObject (text);
			switch (jsonObject.getString ("Type")) {
				case "Return":
					returnSystem.add (jsonObject.getLong ("ID"), jsonObject.getString ("Result"));
					break;
				case "Message": {
					String subType = jsonObject.getString ("SubType");
					switch (subType) {
						case "Friend":
							if (onReceivedMessage != null) {
								onReceivedMessage.invoke (new ChatRobotMessage (
										ChatRobotMessageType.Friend,
										jsonObject.getLong ("Robot"),
										jsonObject.getString ("Message"),
										jsonObject.getLong ("MessageNumber"),
										jsonObject.getLong ("MessageID"),
										jsonObject.getLong ("QQ"),
										0
								));
							}
							break;
						case "Group":
							if (onReceivedMessage!=null) {
								onReceivedMessage.invoke (new ChatRobotMessage (
										ChatRobotMessageType.Group,
										jsonObject.getLong ("Robot"),
										jsonObject.getString ("Message"),
										jsonObject.getLong ("MessageNumber"),
										jsonObject.getLong ("MessageID"),
										jsonObject.getLong ("QQ"),
										jsonObject.getLong ("Group")
								));
							}
							break;
						case "GroupMessageRevoke":
							if (onGroupMessageRevoke != null) {
								onGroupMessageRevoke.invoke (
										jsonObject.getLong ("Robot"),
										jsonObject.getLong ("Group"),
										jsonObject.getLong ("QQ"),
										jsonObject.getLong ("MessageNumber"),
										jsonObject.getLong ("MessageID")
								);
							}
							break;
						default:
							 new Exception (subType).printStackTrace ();
					}
					break;
				}
			}
		}).start ();
	}

	private static void onDisconnected () {
		if (onDisconnected != null) {
			onDisconnected.invoke ();
		}
	}

	private static void sendGroupMessage (String type, long robotQQ, long group, String message) {
		client.beginSend (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robotQQ);
			put ("Group", group);
			put ("Message", message);
		}}.toJSONString ());
	}

	private static void sendFriendMessage (String type, long robotQQ, long qq, String message) {
		client.beginSend (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robotQQ);
			put ("QQ", qq);
			put ("Message", message);
		}}.toJSONString ());
	}

}