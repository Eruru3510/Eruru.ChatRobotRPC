package org.eruru.chatrobotrpc;

import com.alibaba.fastjson.JSONObject;
import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;
import org.eruru.chatrobotrpc.eventHandlers.*;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.Date;

public class ChatRobot {

	private final WaitSystem waitSystem = new WaitSystem ();
	private final Client client;

	private ChatRobotOnReceivedEventHandler onReceived;
	private ChatRobotOnSentEventHandler onSent;
	private ChatRobotOnReceivedMessageEventHandler onReceivedMessage;
	private ChatRobotOnGroupMessageRevokedEventHandler onGroupMessageRevoked;
	private ChatRobotAction onDisconnected;
	private boolean useAsyncReceive = true;

	public String getProtocolVersion () {
		return "1.0.0.1";
	}

	public ChatRobotOnReceivedEventHandler getOnReceived () {
		return onReceived;
	}

	public void setOnReceived (ChatRobotOnReceivedEventHandler onReceived) {
		this.onReceived = onReceived;
	}

	public ChatRobotOnSentEventHandler getOnSent () {
		return onSent;
	}

	public void setOnSent (ChatRobotOnSentEventHandler onSent) {
		this.onSent = onSent;
	}

	public ChatRobotOnReceivedMessageEventHandler getOnReceivedMessage () {
		return onReceivedMessage;
	}

	public void setOnReceivedMessage (ChatRobotOnReceivedMessageEventHandler onReceivedMessage) {
		this.onReceivedMessage = onReceivedMessage;
	}

	public ChatRobotOnGroupMessageRevokedEventHandler getOnGroupMessageRevoked () {
		return onGroupMessageRevoked;
	}

	public void setOnGroupMessageRevoked (ChatRobotOnGroupMessageRevokedEventHandler onGroupMessageRevoked) {
		this.onGroupMessageRevoked = onGroupMessageRevoked;
	}

	public ChatRobotAction getOnDisconnected () {
		return onDisconnected;
	}

	public void setOnDisconnected (ChatRobotAction onDisconnected) {
		this.onDisconnected = onDisconnected;
	}

	public int getHeartbeatPacketSendIntervalBySeconds () {
		return client.getHeartbeatPacketSendIntervalBySeconds ();
	}

	public void setHeartbeatPacketSendIntervalBySeconds (int heartbeatPacketSendIntervalBySeconds) {
		client.setHeartbeatPacketSendIntervalBySeconds (heartbeatPacketSendIntervalBySeconds);
	}

	public boolean isUseAsyncReceive () {
		return useAsyncReceive;
	}

	public void setUseAsyncReceive (boolean useAsyncReceive) {
		this.useAsyncReceive = useAsyncReceive;
	}

	public ChatRobot () {
		client = new Client () {{
			setOnReceived (bytes -> {
				if (useAsyncReceive) {
					new Thread (() -> received (bytes)).start ();
					return;
				}
				received (bytes);
			});
			setOnSent (text -> {
				if (onSent != null) {
					onSent.invoke (text);
				}
			});
			setOnDisconnected (() -> {
				if (onDisconnected != null) {
					onDisconnected.invoke ();
				}
			});
		}};
	}

	public void connect (String ip, int port, String account, String password) throws IOException {
		client.connect (ip, port);
		clientBeginSend (new JSONObject () {{
			put ("Type", "Login");
			put ("Account", account);
			put ("Password", password);
		}});
	}

	public void sendFriendMessage (long robotQQ, long qq, String message) {
		sendFriendMessage ("SendFriendMessage", robotQQ, qq, message);
	}

	public void sendFriendJsonMessage (long robotQQ, long qq, String message) {
		sendFriendMessage ("SendFriendJsonMessage", robotQQ, qq, message);
	}

	public void sendFriendXmlMessage (long robotQQ, long qq, String message) {
		sendFriendMessage ("SendFriendXmlMessage", robotQQ, qq, message);
	}

	public void sendGroupMessage (long robotQQ, long group, String message, boolean isAnonymous) {
		sendGroupMessage ("SendGroupMessage", robotQQ, group, message, isAnonymous);
	}

	public void sendGroupMessage (long robotQQ, long group, String message) {
		sendGroupMessage (robotQQ, group, message, false);
	}

	public void sendGroupJsonMessage (long robotQQ, long group, String message, boolean isAnonymous) {
		sendGroupMessage ("SendGroupJsonMessage", robotQQ, group, message, isAnonymous);
	}

	public void sendGroupJsonMessage (long robotQQ, long group, String message) {
		sendGroupJsonMessage (robotQQ, group, message, false);
	}

	public void sendGroupXmlMessage (long robotQQ, long group, String message, boolean isAnonymous) {
		sendGroupMessage ("SendGroupXmlMessage", robotQQ, group, message, isAnonymous);
	}

	public void sendGroupXmlMessage (long robotQQ, long group, String message) {
		sendGroupXmlMessage (robotQQ, group, message, false);
	}

	public String GetName (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetName");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	public String getGroupName (long robotQQ, long group) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetGroupName");
			put ("RobotQQ", robotQQ);
			put ("Group", group);
		}});
	}

	public void disconnect () throws IOException {
		client.Disconnect ();
		waitSystem.close ();
	}

	private void received (byte[] bytes) {
		String text = new String (bytes, StandardCharsets.UTF_8);
		try {
			if (onReceived != null) {
				onReceived.invoke (text);
			}
			JSONObject jsonObject = JSONObject.parseObject (text);
			String type = jsonObject.getString ("Type");
			switch (type) {
				default:
					throw new Exception (String.format ("未知的消息类型：%s", type));
				case "Protocol": {
					String targetProtocolVersion = jsonObject.getString ("Version");
					if (!targetProtocolVersion.equals (getProtocolVersion ())) {
						throw new Exception (String.format ("SDK协议版本：%s 与机器人框架插件的协议版本：%s 不符", getProtocolVersion (), targetProtocolVersion));
					}
					break;
				}
				case "Return":
					waitSystem.set (jsonObject.getLong ("ID"), jsonObject.getString ("Result"));
					break;
				case "Message": {
					if (onReceivedMessage != null) {
						onReceivedMessage.invoke (new ChatRobotMessage (
								this,
								ChatRobotAPI.enumParse (ChatRobotMessageType.class, jsonObject.getString ("SubType")),
								jsonObject.getLong ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLong ("QQ"),
								jsonObject.getString ("Message"),
								jsonObject.getLong ("MessageNumber"),
								jsonObject.getLong ("MessageID")
						));
					}
					break;
				}
				case "GroupMessageRevoke":
					if (onGroupMessageRevoked != null) {
						onGroupMessageRevoked.invoke (
								jsonObject.getLong ("Robot"),
								jsonObject.getLong ("Group"),
								jsonObject.getLong ("QQ"),
								jsonObject.getLong ("MessageNumber"),
								jsonObject.getLong ("MessageID")
						);
					}
					break;
			}
		} catch (Exception exception) {
			System.out.printf ("%s 处理消息：%s 时出现异常：%s\n", new Date (), text, exception);
			exception.printStackTrace ();
		}
	}

	private void sendGroupMessage (String type, long robotQQ, long group, String message, boolean isAnonymous) {
		clientBeginSend (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robotQQ);
			put ("Group", group);
			put ("Message", message);
			put ("IsAnonymous", isAnonymous);
		}});
	}

	private void sendFriendMessage (String type, long robotQQ, long qq, String message) {
		clientBeginSend (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robotQQ);
			put ("QQ", qq);
			put ("Message", message);
		}});
	}

	private void sendGroupTempMessage (String type, long robotQQ, long group, long qq, String message) {
		clientBeginSend (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robotQQ);
			put ("Group", group);
			put ("QQ", qq);
			put ("Message", message);
		}});
	}

	long waitSystemSend (JSONObject jsonObject) {
		long id = waitSystem.getID ();
		jsonObject.put ("ID", id);
		clientBeginSend (jsonObject);
		return id;
	}

	<T> T waitSystemConvert (Class<T> type, String result) {
		return (T) result;
	}

	<T> T waitSystemGet (Class<T> type, JSONObject jsonObject) {
		try {
			return waitSystemConvert (type, waitSystem.get (waitSystemSend (jsonObject)));
		} catch (InterruptedException e) {
			e.printStackTrace ();
		}
		return null;
	}

	private void clientBeginSend (JSONObject jsonObject) {
		client.beginSend (jsonObject.toJSONString ());
	}

}