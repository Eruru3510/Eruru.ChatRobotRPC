package org.eruru.chatrobotrpc;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import javafx.util.Pair;
import org.eruru.chatrobotrpc.enums.ChatRobotFriendAddMethod;
import org.eruru.chatrobotrpc.enums.ChatRobotGroupAddRequestType;
import org.eruru.chatrobotrpc.enums.ChatRobotGroupMemberJoinType;
import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;
import org.eruru.chatrobotrpc.eventhandlers.*;
import org.eruru.chatrobotrpc.informations.*;

import javax.naming.AuthenticationException;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.Date;
import java.util.concurrent.TimeoutException;

public class ChatRobot {

	/**
	 * SDK版本
	 */
	public static final String version = "1.0.5";
	/**
	 * 协议版本
	 */
	public static final String protocolVersion = "1.0.0.5";

	private ChatRobotAction onDisconnected;
	private ChatRobotReceivedEventHandler onReceived;
	private ChatRobotSendEventHandler onSend;
	private ChatRobotReceivedMessageEventHandler onReceivedMessage;
	private ChatRobotGroupAddRequestedEventHandler onReceivedGroupAddRequest;
	private ChatRobotFriendAddRespondedEventHandler onReceivedFriendAddResponse;
	private ChatRobotFriendAddRequestedEventHandler onReceivedFriendAddRequest;
	private ChatRobotGroupMessageRevokedEventHandler onGroupMessageRevoked;
	private ChatRobotGroupAnonymousSwitchedEventHandler onGroupAnonymousSwitched;
	private ChatRobotGroupNameChangedEventHandler onGroupNameChanged;
	private ChatRobotGroupBannedSpeakEventHandler onGroupBannedSpeak;
	private ChatRobotGroupAdministratorChangedEventHandler onGroupAdministratorChanged;
	private ChatRobotGroupMemberBusinessCardChangedEventHandler onGroupMemberBusinessCardChanged;
	private ChatRobotGroupMemberLeftEventHandler onGroupMemberLeft;
	private ChatRobotGroupMemberBannedSpeakEventHandler onGroupMemberBannedSpeak;
	private ChatRobotGroupMemberJoinedEventHandler onGroupMemberJoined;
	private ChatRobotGroupDisbandedEventHandler onGroupDisbanded;
	private ChatRobotFriendStateChangedEventHandler onFriendStateChanged;
	private ChatRobotWasRemovedByFriendEventHandler onWasRemovedByFriend;
	private ChatRobotOtherEventEventHandler onReceivedOtherEvent;

	private final WaitSystem waitSystem = new WaitSystem ();
	private final SocketClient socketClient;
	private final Charset charset = StandardCharsets.UTF_8;

	/**
	 * 心跳包发送间隔（秒）
	 */
	public int getHeartbeatInterval () {
		return socketClient.getHeartbeatInterval ();
	}

	/**
	 * 心跳包发送间隔（秒）
	 */
	public void setHeartbeatInterval (int heartbeatInterval) {
		socketClient.setHeartbeatInterval (heartbeatInterval);
	}

	/**
	 * 是否使用异步处理所有消息
	 */
	public boolean isUseAsyncReceive () {
		return socketClient.isUseAsyncOnReceived ();
	}

	/**
	 * 是否使用异步处理所有消息
	 */
	public void setUseAsyncReceive (boolean useAsyncReceive) {
		socketClient.setUseAsyncOnReceived (useAsyncReceive);
	}

	/**
	 * 与机器人框架RPC插件断开了连接
	 */
	public ChatRobotAction getOnDisconnected () {
		return onDisconnected;
	}

	/**
	 * 与机器人框架RPC插件断开了连接
	 */
	public void setOnDisconnected (ChatRobotAction onDisconnected) {
		this.onDisconnected = onDisconnected;
	}

	/**
	 * 收到消息（底层协议消息）
	 */
	public ChatRobotReceivedEventHandler getOnReceived () {
		return onReceived;
	}

	/**
	 * 收到消息（底层协议消息）
	 */
	public void setOnReceived (ChatRobotReceivedEventHandler onReceived) {
		this.onReceived = onReceived;
	}

	/**
	 * 发送消息（底层协议消息）
	 */
	public ChatRobotSendEventHandler getOnSend () {
		return onSend;
	}

	/**
	 * 发送消息（底层协议消息）
	 */
	public void setOnSend (ChatRobotSendEventHandler onSend) {
		this.onSend = onSend;
	}

	/**
	 * 收到消息（好友或群等，统一入口，可用ChatRobot.UseAsyncReceive决定是否异步处理所有消息）
	 */
	public ChatRobotReceivedMessageEventHandler getOnReceivedMessage () {
		return onReceivedMessage;
	}

	/**
	 * 收到消息（好友或群等，统一入口，可用ChatRobot.UseAsyncReceive决定是否异步处理所有消息）
	 */
	public void setOnReceivedMessage (ChatRobotReceivedMessageEventHandler onReceivedMessage) {
		this.onReceivedMessage = onReceivedMessage;
	}

	/**
	 * 收到群添加请求（使用ChatRobot.HandleGroupAddRequest处理）
	 */
	public ChatRobotGroupAddRequestedEventHandler getOnReceivedGroupAddRequest () {
		return onReceivedGroupAddRequest;
	}

	/**
	 * 收到群添加请求（使用ChatRobot.HandleGroupAddRequest处理）
	 */
	public void setOnReceivedGroupAddRequest (ChatRobotGroupAddRequestedEventHandler onReceivedGroupAddRequest) {
		this.onReceivedGroupAddRequest = onReceivedGroupAddRequest;
	}

	/**
	 * 收到好友添加响应
	 */
	public ChatRobotFriendAddRespondedEventHandler getOnReceivedFriendAddResponse () {
		return onReceivedFriendAddResponse;
	}

	/**
	 * 收到好友添加响应
	 */
	public void setOnReceivedFriendAddResponse (ChatRobotFriendAddRespondedEventHandler onReceivedFriendAddResponse) {
		this.onReceivedFriendAddResponse = onReceivedFriendAddResponse;
	}

	/**
	 * 收到好友添加请求（使用ChatRobot.HandleFriendAddRequest处理）
	 */
	public ChatRobotFriendAddRequestedEventHandler getOnReceivedFriendAddRequest () {
		return onReceivedFriendAddRequest;
	}

	/**
	 * 收到好友添加请求（使用ChatRobot.HandleFriendAddRequest处理）
	 */
	public void setOnReceivedFriendAddRequest (ChatRobotFriendAddRequestedEventHandler onReceivedFriendAddRequest) {
		this.onReceivedFriendAddRequest = onReceivedFriendAddRequest;
	}

	/**
	 * 群消息撤回
	 */
	public ChatRobotGroupMessageRevokedEventHandler getOnGroupMessageRevoked () {
		return onGroupMessageRevoked;
	}

	/**
	 * 群消息撤回
	 */
	public void setOnGroupMessageRevoked (ChatRobotGroupMessageRevokedEventHandler onGroupMessageRevoked) {
		this.onGroupMessageRevoked = onGroupMessageRevoked;
	}

	/**
	 * 群匿名开关
	 */
	public ChatRobotGroupAnonymousSwitchedEventHandler getOnGroupAnonymousSwitched () {
		return onGroupAnonymousSwitched;
	}

	/**
	 * 群匿名开关
	 */
	public void setOnGroupAnonymousSwitched (ChatRobotGroupAnonymousSwitchedEventHandler onGroupAnonymousSwitched) {
		this.onGroupAnonymousSwitched = onGroupAnonymousSwitched;
	}

	/**
	 * 群名改变
	 */
	public ChatRobotGroupNameChangedEventHandler getOnGroupNameChanged () {
		return onGroupNameChanged;
	}

	/**
	 * 群名改变
	 */
	public void setOnGroupNameChanged (ChatRobotGroupNameChangedEventHandler onGroupNameChanged) {
		this.onGroupNameChanged = onGroupNameChanged;
	}

	/**
	 * 群禁言
	 */
	public ChatRobotGroupBannedSpeakEventHandler getOnGroupBannedSpeak () {
		return onGroupBannedSpeak;
	}

	/**
	 * 群禁言
	 */
	public void setOnGroupBannedSpeak (ChatRobotGroupBannedSpeakEventHandler onGroupBannedSpeak) {
		this.onGroupBannedSpeak = onGroupBannedSpeak;
	}

	/**
	 * 群管理员改变
	 */
	public ChatRobotGroupAdministratorChangedEventHandler getOnGroupAdministratorChanged () {
		return onGroupAdministratorChanged;
	}

	/**
	 * 群管理员改变
	 */
	public void setOnGroupAdministratorChanged (ChatRobotGroupAdministratorChangedEventHandler onGroupAdministratorChanged) {
		this.onGroupAdministratorChanged = onGroupAdministratorChanged;
	}

	/**
	 * 群成员名片改变
	 */
	public ChatRobotGroupMemberBusinessCardChangedEventHandler getOnGroupMemberBusinessCardChanged () {
		return onGroupMemberBusinessCardChanged;
	}

	/**
	 * 群成员名片改变
	 */
	public void setOnGroupMemberBusinessCardChanged (ChatRobotGroupMemberBusinessCardChangedEventHandler onGroupMemberBusinessCardChanged) {
		this.onGroupMemberBusinessCardChanged = onGroupMemberBusinessCardChanged;
	}

	/**
	 * 群成员离开
	 */
	public ChatRobotGroupMemberLeftEventHandler getOnGroupMemberLeft () {
		return onGroupMemberLeft;
	}

	/**
	 * 群成员离开
	 */
	public void setOnGroupMemberLeft (ChatRobotGroupMemberLeftEventHandler onGroupMemberLeft) {
		this.onGroupMemberLeft = onGroupMemberLeft;
	}

	/**
	 * 群成员禁言
	 */
	public ChatRobotGroupMemberBannedSpeakEventHandler getOnGroupMemberBannedSpeak () {
		return onGroupMemberBannedSpeak;
	}

	/**
	 * 群成员禁言
	 */
	public void setOnGroupMemberBannedSpeak (ChatRobotGroupMemberBannedSpeakEventHandler onGroupMemberBannedSpeak) {
		this.onGroupMemberBannedSpeak = onGroupMemberBannedSpeak;
	}

	/**
	 * 群成员加入
	 */
	public ChatRobotGroupMemberJoinedEventHandler getOnGroupMemberJoined () {
		return onGroupMemberJoined;
	}

	/**
	 * 群成员加入
	 */
	public void setOnGroupMemberJoined (ChatRobotGroupMemberJoinedEventHandler onGroupMemberJoined) {
		this.onGroupMemberJoined = onGroupMemberJoined;
	}

	/**
	 * 群被解散
	 */
	public ChatRobotGroupDisbandedEventHandler getOnGroupDisbanded () {
		return onGroupDisbanded;
	}

	/**
	 * 群被解散
	 */
	public void setOnGroupDisbanded (ChatRobotGroupDisbandedEventHandler onGroupDisbanded) {
		this.onGroupDisbanded = onGroupDisbanded;
	}

	/**
	 * 好友状态改变
	 */
	public ChatRobotFriendStateChangedEventHandler getOnFriendStateChanged () {
		return onFriendStateChanged;
	}

	/**
	 * 好友状态改变
	 */
	public void setOnFriendStateChanged (ChatRobotFriendStateChangedEventHandler onFriendStateChanged) {
		this.onFriendStateChanged = onFriendStateChanged;
	}

	/**
	 * 被好友删除
	 */
	public ChatRobotWasRemovedByFriendEventHandler getOnWasRemovedByFriend () {
		return onWasRemovedByFriend;
	}

	/**
	 * 被好友删除
	 */
	public void setOnWasRemovedByFriend (ChatRobotWasRemovedByFriendEventHandler onWasRemovedByFriend) {
		this.onWasRemovedByFriend = onWasRemovedByFriend;
	}

	/**
	 * 其他事件
	 */
	public ChatRobotOtherEventEventHandler getOnReceivedOtherEvent () {
		return onReceivedOtherEvent;
	}

	/**
	 * 其他事件
	 */
	public void setOnReceivedOtherEvent (ChatRobotOtherEventEventHandler onReceivedOtherEvent) {
		this.onReceivedOtherEvent = onReceivedOtherEvent;
	}

	public ChatRobot () {
		socketClient = new SocketClient () {{
			setOnReceived (bytes -> socketClient_onReceived (bytes));
			setOnSend (bytes -> {
				if (onSend != null) {
					onSend.invoke (new String (bytes, charset));
				}
			});
			setOnDisconnected (() -> {
				if (onDisconnected != null) {
					onDisconnected.invoke ();
				}
			});
		}};
	}

	/**
	 * 连接机器人框架RPC插件（使用ChatRobot.OnDisconnected事件获知断开连接）
	 *
	 * @throws IOException             连接失败
	 * @throws AuthenticationException 账号或密码错误
	 */
	public void connect (String ip, int port, String account, String password) throws AuthenticationException, IOException, InterruptedException, TimeoutException {
		socketClient.connect (ip, port);
		if (Boolean.FALSE.equals (waitSystemGetSafe (boolean.class, new JSONObject () {{
			put ("Type", "Login");
			put ("Account", account);
			put ("Password", password);
		}}))) {
			throw new AuthenticationException ("账号或密码错误");
		}
	}

	/**
	 * 断开与机器人框架RPC插件的连接
	 */
	public void disconnect () {
		socketClient.Disconnect ();
		waitSystem.close ();
	}

	/**
	 * TEA加密
	 *
	 * @param content 需加密的内容
	 */
	public String teaEncryption (String content, String key) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "TEAEncryption");
			put ("Content", content);
			put ("Key", key);
		}});
	}

	/**
	 * TEA解密
	 *
	 * @param content 需解密的内容
	 */
	public String teaDecryption (String content, String key) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "TEADecryption");
			put ("Content", content);
			put ("Key", key);
		}});
	}

	/**
	 * 查询我的群礼物 QQMini Pro才可用 返回礼物数量
	 *
	 * @param robot 机器人QQ
	 * @return 礼物列表
	 */
	public ChatRobotGiftInformation[] queryGroupGiftInformations (long robot) {
		return waitSystemGet (ChatRobotGiftInformation[].class, new JSONObject () {{
			put ("Type", "QueryGroupGiftInformations");
			put ("Robot", robot);
		}});
	}

	/**
	 * 撤回群消息
	 *
	 * @param robot         机器人QQ
	 * @param group         需撤回消息群号
	 * @param messageNumber 需撤回消息序号
	 * @param messageID     需撤回消息ID
	 */
	public String revokeGroupMessage (long robot, long group, long messageNumber, long messageID) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "RevokeGroupMessage");
			put ("Robot", robot);
			put ("Group", group);
			put ("MessageNumber", messageNumber);
			put ("MessageID", messageID);
		}});
	}

	/**
	 * 抽取群礼物
	 *
	 * @param robot 机器人QQ
	 * @param group 目标群号
	 */
	public Long drawGroupGift (long robot, long group) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "DrawGroupGift");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 处理好友添加请求
	 *
	 * @param robot           机器人QQ
	 * @param qq              请求添加好友人QQ
	 * @param treatmentMethod 处理方式
	 * @param information     拒绝添加好友 附加信息
	 */
	public void handleFriendAddRequest (long robot, long qq, ChatRobotGroupAddRequestType treatmentMethod, String information) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "HandleFriendAddRequest");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("TreatmentMethod", treatmentMethod.getValue ());
			put ("Information", information);
		}});
	}

	/**
	 * 处理群添加请求
	 *
	 * @param robot           机器人QQ
	 * @param requestType     请求类型
	 * @param qq              申请入群 被邀请人 （当请求类型为某人被邀请时这里为邀请人QQ）
	 * @param group           收到请求群号
	 * @param sign            需要处理请求的标记
	 * @param treatmentMethod 处理方式
	 * @param information     拒绝入群 附加信息
	 */
	public void handleGroupAddRequest (long robot, ChatRobotGroupAddRequestType requestType, long qq, long group, long sign,
									   ChatRobotGroupAddRequestType treatmentMethod, String information
	) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "HandleGroupAddRequest");
			put ("Robot", robot);
			put ("RequestType", requestType.getValue ());
			put ("QQ", qq);
			put ("Group", group);
			put ("Tag", sign);
			put ("TreatmentMethod", treatmentMethod.getValue ());
			put ("Information", information);
		}});
	}

	/**
	 * 创建一个讨论组 成功返回讨论组ID 失败返回原因
	 *
	 * @param robot 机器人QQ
	 * @param qq    被邀请对象QQ
	 */
	public String createDiscuss (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "CreateDiscuss");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 登录指定QQ，应确保QQ号码在列表中已存在
	 *
	 * @param robot 机器人QQ
	 */
	public void loginRobot (long robot) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "LoginRobot");
			put ("Robot", robot);
		}});
	}

	/**
	 * 调用一次点一下，成功返回空，失败返回理由如：每天最多给他点十个赞哦，调用此Api时应注意频率，每人每日可被赞10次，每日每Q最多可赞50人
	 *
	 * @param robot 机器人QQ
	 * @param qq    填写被赞人QQ
	 */
	public String like (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "Like");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 发布群公告，调用此API应保证响应QQ为管理员 成功返回空,失败返回错误信息
	 *
	 * @param robot   机器人QQ
	 * @param group   欲发布公告的群号
	 * @param title   公告标题
	 * @param content 公告内容
	 */
	public String publishGroupAnnouncement (long robot, long group, String title, String content) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "PublishGroupAnnouncement");
			put ("Robot", robot);
			put ("Group", group);
			put ("Title", title);
			put ("Content", content);
		}});
	}

	/**
	 * QQ群作业发布
	 *
	 * @param robot   机器人QQ
	 * @param group   需要发布的群号
	 * @param name    作业名
	 * @param title   作业标题
	 * @param content 作业内容
	 */
	public String publishGroupJob (long robot, long group, String name, String title, String content) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "PublishGroupJob");
			put ("Robot", robot);
			put ("Group", group);
			put ("Name", name);
			put ("Title", title);
			put ("Content", content);
		}});
	}

	/**
	 * 发送好友JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      好友的QQ号
	 * @param message 信息内容
	 */
	public void sendFriendJsonMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendFriendJsonMessage", robot, qq, message);
	}

	/**
	 * 发送好友XML消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      好友的QQ号
	 * @param message 信息内容
	 */
	public void sendFriendXmlMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendFriendXmlMessage", robot, qq, message);
	}

	/**
	 * 向好友发起窗口抖动，调用此Api腾讯会限制频率
	 *
	 * @param robot 机器人QQ
	 * @param qq    接收抖动消息的QQ
	 */
	public boolean sendFriendWindowJitter (long robot, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SendFriendWindowJitter");
			put ("Robot", robot);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 发送好友普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      好友的QQ号
	 * @param message 信息内容
	 */
	public void sendFriendMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendFriendMessage", robot, qq, message);
	}

	/**
	 * 发送好友验证回复JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendFriendVerificationReplyJsonMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendFriendVerificationReplyJsonMessage", robot, qq, message);
	}

	/**
	 * 发送好友验证回复XML消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendFriendVerificationReplyXmlMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendFriendVerificationReplyXmlMessage", robot, qq, message);
	}

	/**
	 * 发送好友验证回复普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendFriendVerificationReplyMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendFriendVerificationReplyMessage", robot, qq, message);
	}

	/**
	 * 好友语音上传并发送 （成功返回真 失败返回假） QQMini Pro才可用
	 *
	 * @param robot  机器人QQ
	 * @param qq     要发送的QQ号
	 * @param base64 语音字节集数据（AMR Silk编码）
	 */
	public boolean sendFriendVoice (long robot, long qq, String base64) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SendFriendVoice");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("Data", base64);
		}}));
	}

	/**
	 * 好友语音上传并发送 （成功返回真 失败返回假） QQMini Pro才可用
	 *
	 * @param robot 机器人QQ
	 * @param qq    要发送的QQ号
	 * @param bytes 语音字节集数据（AMR Silk编码）
	 */
	public boolean sendFriendVoice (long robot, long qq, byte[] bytes) {
		return sendFriendVoice (robot, qq, Base64.getEncoder ().encodeToString (bytes));
	}

	/**
	 * 发送群JSON消息
	 *
	 * @param robot       机器人QQ
	 * @param group       要发送消息的群号
	 * @param message     信息内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void sendGroupJsonMessage (long robot, long group, String message, boolean isAnonymous) {
		sendGroupMessage ("SendGroupJsonMessage", robot, group, message, isAnonymous);
	}

	/**
	 * 发送群JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param group   要发送消息的群号
	 * @param message 信息内容
	 */
	public void sendGroupJsonMessage (long robot, long group, String message) {
		sendGroupJsonMessage (robot, group, message, false);
	}

	/**
	 * 发送群XML消息
	 *
	 * @param robot       机器人QQ
	 * @param group       要发送消息的群号
	 * @param message     信息内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void sendGroupXmlMessage (long robot, long group, String message, boolean isAnonymous) {
		sendGroupMessage ("SendGroupXmlMessage", robot, group, message, isAnonymous);
	}

	/**
	 * 发送群XML消息
	 *
	 * @param robot   机器人QQ
	 * @param group   要发送消息的群号
	 * @param message 信息内容
	 */
	public void sendGroupXmlMessage (long robot, long group, String message) {
		sendGroupXmlMessage (robot, group, message, false);
	}

	/**
	 * 送群礼物 成功返回真 失败返回假
	 *
	 * @param robot 机器人QQ
	 * @param group 需送礼物群号
	 * @param qq    赠予礼物对象
	 * @param gift  礼物id
	 */
	public boolean sendGroupGift (long robot, long group, long qq, long gift) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SendGroupGift");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
			put ("Gift", gift);
		}}));
	}

	/**
	 * 发送群临时JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param group   对方所在群号
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendGroupTempJsonMessage (long robot, long group, long qq, String message) {
		sendGroupTempMessage ("SendGroupTempJsonMessage", robot, group, qq, message);
	}

	/**
	 * 发送群临时XML消息
	 *
	 * @param robot   机器人QQ
	 * @param group   对方所在群号
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendGroupTempXmlMessage (long robot, long group, long qq, String message) {
		sendGroupTempMessage ("SendGroupTempXmlMessage", robot, group, qq, message);
	}

	/**
	 * 发送群临时普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param group   对方所在群号
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendGroupTempMessage (long robot, long group, long qq, String message) {
		sendGroupTempMessage ("SendGroupTempMessage", robot, group, qq, message);
	}

	/**
	 * QQ群签到（成功返回真 失败返回假
	 *
	 * @param robot   机器人QQ
	 * @param group   QQ群号
	 * @param place   签到地名（Pro可自定义）
	 * @param content 想发表的内容
	 */
	public boolean sendGroupSignIn (long robot, long group, String place, String content) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SendGroupSignIn");
			put ("Robot", robot);
			put ("Group", group);
			put ("Place", place);
			put ("Content", content);
		}}));
	}

	/**
	 * 发送群普通文本消息
	 *
	 * @param robot       机器人QQ
	 * @param group       要发送消息的群号
	 * @param message     信息内容
	 * @param isAnonymous 是否匿名（仅Pro有效）
	 */
	public void sendGroupMessage (long robot, long group, String message, boolean isAnonymous) {
		sendGroupMessage ("SendGroupMessage", robot, group, message, isAnonymous);
	}

	/**
	 * 发送群普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param group   要发送消息的群号
	 * @param message 信息内容
	 */
	public void sendGroupMessage (long robot, long group, String message) {
		sendGroupMessage (robot, group, message, false);
	}

	/**
	 * 向服务器发送原始封包（成功返回服务器返回的包 失败返回空）
	 *
	 * @param robot 机器人QQ
	 * @param data  封包内容
	 */
	public String sendData (long robot, String data) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "SendData");
			put ("Robot", robot);
			put ("Data", data);
		}});
	}

	/**
	 * 发送讨论组临时JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param group   对方所在讨论组号
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendDiscussTempJsonMessage (long robot, long group, long qq, String message) {
		sendGroupTempMessage ("SendDiscussTempJsonMessage", robot, group, qq, message);
	}

	/**
	 * 发送讨论组JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param group   讨论组号
	 * @param message 信息内容
	 */
	public void sendDiscussJsonMessage (long robot, long group, String message) {
		sendGroupMessage ("SendDiscussJsonMessage", robot, group, message, false);
	}

	/**
	 * 发送讨论组临时XML消息
	 *
	 * @param robot   机器人QQ
	 * @param group   对方所在讨论组号
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendDiscussTempXmlMessage (long robot, long group, long qq, String message) {
		sendGroupTempMessage ("SendDiscussTempXmlMessage", robot, group, qq, message);
	}

	/**
	 * 发送讨论组XML消息
	 *
	 * @param robot   机器人QQ
	 * @param group   讨论组号
	 * @param message 信息内容
	 */
	public void sendDiscussXmlMessage (long robot, long group, String message) {
		sendGroupMessage ("SendDiscussXmlMessage", robot, group, message, false);
	}

	/**
	 * 发送讨论组临时普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param group   对方所在讨论组号
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendDiscussTempMessage (long robot, long group, long qq, String message) {
		sendGroupTempMessage ("SendDiscussTempMessage", robot, group, qq, message);
	}

	/**
	 * 发送讨论组普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param group   讨论组号
	 * @param message 信息内容
	 */
	public void sendDiscussMessage (long robot, long group, String message) {
		sendGroupMessage ("SendDiscussMessage", robot, group, message, false);
	}

	/**
	 * 发送网页临时JSON消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendWebpageTempJsonMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendWebpageTempJsonMessage", robot, qq, message);
	}

	/**
	 * 发送网页临时XML消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendWebpageTempXmlMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendWebpageTempXmlMessage", robot, qq, message);
	}

	/**
	 * 发送网页临时普通文本消息
	 *
	 * @param robot   机器人QQ
	 * @param qq      对方的QQ号
	 * @param message 信息内容
	 */
	public void sendWebpageTempMessage (long robot, long qq, String message) {
		sendFriendMessage ("SendWebpageTempMessage", robot, qq, message);
	}

	/**
	 * 把好友图片的GUID转换成群聊可用的GUID
	 *
	 * @param robot   机器人QQ
	 * @param picture 例：[pic={30055346-3524074609-441EE15D1D802AA41D0396A7C303CD93}.jpg]
	 */
	public String friendPictureToGroupPicture (long robot, String picture) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "FriendPictureToGroupPicture");
			put ("Robot", robot);
			put ("Picture", picture);
		}});
	}

	/**
	 * 通过连接加入讨论组
	 *
	 * @param robot 机器人QQ
	 * @param url   讨论组链接
	 */
	public String joinDiscussByURL (long robot, String url) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "JoinDiscussByUrl");
			put ("Robot", robot);
			put ("Url", url);
		}});
	}

	/**
	 * 请求禁用插件自身
	 */
	public void disablePlugin () {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "DisablePlugin");
		}});
	}

	/**
	 * 取得机器人网页操作用参数Bkn或G_tk Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getBkn (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetBkn");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取得机器人网页操作用的Clientkey Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getClientKey (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetClientKey");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取得机器人网页操作用的Cookies Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getCookies (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetCookies");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取得机器人网页操作用的P_skey Pro可用
	 *
	 * @param robot      机器人QQ
	 * @param domainName t.qq.com；qzone.qq.com；qun.qq.com；ke.qq.com
	 */
	public String getPSKey (long robot, String domainName) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetPSKey");
			put ("Robot", robot);
			put ("DomainName", domainName);
		}});
	}

	/**
	 * 获取会话SessionKey密钥 Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getSessionKey (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetSessionKey");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取得机器人网页操作用参数长Bkn或长G_tk Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getLongBkn (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetLongBkn");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取得机器人网页操作用的长Clientkey Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getLongClientKey (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetLongClientKey");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取得机器人网页操作用参数长Ldw Pro可用
	 *
	 * @param robot 机器人QQ
	 */
	public String getLongLdw (long robot) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetLongLdw");
			put ("Robot", robot);
		}});
	}

	/**
	 * 查询对象或自身群聊等级
	 *
	 * @param robot 机器人QQ
	 * @param group 查询群号
	 * @param qq    需查询对象或机器人QQ
	 */
	public String getMemberGroupChatLevel (long robot, long group, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetMemberGroupChatLevel");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取当前框架内部时间戳
	 */
	public Long getCurrentTimeStamp () {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetCurrentTimeStamp");
		}});
	}

	/**
	 * 获取等级 活跃天数 升级剩余活跃天数
	 *
	 * @param robot 机器人QQ
	 * @return 是否成功, 等级信息
	 */
	public Pair<Boolean, ChatRobotLevelInformation> tryGetLevel (long robot) {
		return waitSystemGet (boolean.class, ChatRobotLevelInformation.class, new JSONObject () {{
			put ("Type", "TryGetLevel");
			put ("Robot", robot);
		}});
	}

	/**
	 * 查询对象或自身QQ达人天数（返回实际天数 失败返回-1）
	 *
	 * @param robot 机器人QQ
	 * @param qq    需查询对象或机器人QQ
	 */
	public Long getFriendQQMasterDays (long robot, long qq) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetFriendQQMasterDays");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取Q龄 成功返回Q龄 失败返回-1
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public Long getFriendQAge (long robot, long qq) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetFriendQAge");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取好友备注姓名（成功返回备注，失败或无备注返回空）
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象好友QQ
	 */
	public String getFriendNotes (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFriendNotes");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 返回好友等级
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public Long getFriendLevel (long robot, long qq) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetFriendLevel");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取个人说明
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public String getFriendPersonalDescription (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFriendPersonalDescription");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取个人签名
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public String getFriendPersonalSignature (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFriendPersonalSignature");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取好友信息
	 *
	 * @param robot 机器人QQ
	 */
	public ChatRobotFriendListInformation[] getFriendListInformations (long robot) {
		return waitSystemGet (ChatRobotFriendListInformation[].class, new JSONObject () {{
			put ("Type", "GetFriendListInformations");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取 成功返回年龄 失败返回-1
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public Long getFriendAge (long robot, long qq) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetFriendAge");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 查询对象是否在线
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象QQ
	 */
	public boolean isFriendOnline (long robot, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsFriendOnline");
			put ("Robot", robot);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 获取好友资料 此方式为http，调用时应自行注意控制频率
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 * @return 是否成功, 好友信息
	 */
	public Pair<Boolean, ChatRobotFriendInformation> tryGetFriendInformation (long robot, long qq) {
		return waitSystemGet (boolean.class, ChatRobotFriendInformation.class, new JSONObject () {{
			put ("Type", "TryGetFriendInformation");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取对象性别 1男 2女 未知或失败返回-1
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public Long getFriendGender (long robot, long qq) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetFriendGender");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取邮箱，获取对象QQ资料内邮箱栏为邮箱时返回
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public String getFriendEmail (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFriendEmail");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 查询对象在线状态 返回 #状态_ 常量 离线或隐身都返回#状态_隐身
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象QQ
	 */
	public String getFriendOnlineState (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFriendOnlineState");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 取好友账号
	 *
	 * @param robot 机器人QQ
	 */
	public long[] getFriends (long robot) {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetFriends");
			put ("Robot", robot);
		}});
	}

	/**
	 * 获取机器人状态信息，成功返回真，失败返回假
	 *
	 * @param robot 机器人QQ
	 * @return 是否成功, 返回状态信息
	 */
	public Pair<Boolean, ChatRobotStateInformation> tryGetRobotStateInformation (long robot) {
		return waitSystemGet (boolean.class, ChatRobotStateInformation.class, new JSONObject () {{
			put ("Type", "TryGetRobotStateInformation");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取框架版本号
	 */
	public String getFrameVersionNumber () {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFrameVersionNumber");
		}});
	}

	/**
	 * 取框架版本名，返回QQMini Air或QQMini Pro
	 */
	public String getFrameVersionName () {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFrameVersionName");
		}});
	}

	/**
	 * 取框架离线QQ账号
	 */
	public long[] getOfflineRobots () {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetOfflineRobots");
		}});
	}

	/**
	 * 取框架所有QQ账号 包括未登录以及登录失败的QQ
	 */
	public long[] getRobots () {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetRobots");
		}});
	}

	/**
	 * 取框架日志
	 */
	public String getFrameLog () {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetFrameLog");
		}});
	}

	/**
	 * 取框架在线可用的QQ账号
	 */
	public long[] getOnlineRobots () {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetOnlineRobots");
		}});
	}

	/**
	 * 取对象好友添加验证方式 返回常量 #验证_
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象QQ
	 */
	public ChatRobotFriendAddMethod getTargetFriendAddMethod (long robot, long qq) {
		return waitSystemGet (ChatRobotFriendAddMethod.class, new JSONObject () {{
			put ("Type", "GetTargetFriendAddMethod");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 群号转群ID
	 */
	public Long getGroupID (long group) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetGroupID");
			put ("Group", group);
		}});
	}

	/**
	 * 取群成员列表信息
	 *
	 * @param robot 机器人QQ
	 * @param group 指定群号
	 * @return 群成员列表信息, 群成员信息
	 */
	public Pair<ChatRobotGroupMemberListInformation, ChatRobotGroupMemberInformation[]> getGroupMemberListInformation (long robot, long group) {
		return waitSystemGet (ChatRobotGroupMemberListInformation.class, ChatRobotGroupMemberInformation[].class, new JSONObject () {{
			put ("Type", "GetGroupMemberListInformation");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 取对象群名片
	 *
	 * @param robot 机器人QQ
	 * @param group QQ群号
	 * @param qq    欲取得群名片的群成员QQ号
	 */
	public String getGroupMemberBusinessCard (long robot, long group, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetGroupMemberBusinessCard");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
		}});
	}

	/**
	 * 根据群号+QQ判断指定群员是否被禁言 获取失败的情况下亦会返回假
	 *
	 * @param robot 机器人QQ
	 * @param group 要查询的群号
	 * @param qq    要查询的QQ号
	 */
	public boolean isGroupMemberBanSpeak (long robot, long group, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsGroupMemberBanSpeak");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 取群成员账号
	 *
	 * @param robot 机器人QQ
	 * @param group 指定群号
	 */
	public long[] getGroupMembers (long robot, long group) {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetGroupMembers");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 取群公告
	 *
	 * @param robot 机器人QQ
	 * @param group 欲取得公告的群号
	 */
	public String getGroupAnnouncement (long robot, long group) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetGroupAnnouncement");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 取群管理员QQ（包含群主）
	 *
	 * @param robot 机器人QQ
	 * @param group 指定群号
	 */
	public long[] getGroupAdministrators (long robot, long group) {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetGroupAdministrators");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 群ID转群号
	 */
	public Long getGroupQQ (long id) {
		return waitSystemGet (Long.class, new JSONObject () {{
			put ("Type", "GetGroupQQ");
			put ("GroupID", id);
		}});
	}

	/**
	 * 取群号
	 *
	 * @param robot 机器人QQ
	 */
	public long[] getGroups (long robot) {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetGroups");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取群信息
	 *
	 * @param robot 机器人QQ
	 */
	public ChatRobotGroupInformation[] getGroupInformations (long robot) {
		return waitSystemGet (ChatRobotGroupInformation[].class, new JSONObject () {{
			put ("Type", "GetGroupInformations");
			put ("Robot", robot);
		}});
	}

	/**
	 * 取群名
	 *
	 * @param robot 机器人QQ
	 * @param group QQ群号
	 */
	public String getGroupName (long robot, long group) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetGroupName");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 查询对象群当前人数和上限人数
	 *
	 * @param robot 机器人QQ
	 * @param group 需查询的群号
	 * @return 群当前人数, 群人数上限
	 */
	public Pair<Integer, Integer> getGroupMemberNumber (long robot, long group) {
		return waitSystemGet (int.class, int.class, new JSONObject () {{
			put ("Type", "GetGroupMemberNumber");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 取群是否支持匿名
	 */
	public boolean isGroupAnonymousEnabled (long robot, long group) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsGroupAnonymousEnabled");
			put ("Robot", robot);
			put ("Group", group);
		}}));
	}

	/**
	 * 取讨论组成员QQ
	 *
	 * @param robot   机器人QQ
	 * @param discuss 讨论组号
	 */
	public long[] getDiscussMembers (long robot, long discuss) {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetDiscussMembers");
			put ("Robot", robot);
			put ("Discuss", discuss);
		}});
	}

	/**
	 * 取讨论组号
	 *
	 * @param robot 机器人QQ
	 */
	public long[] getDiscusss (long robot) {
		return waitSystemGet (long[].class, new JSONObject () {{
			put ("Type", "GetDiscusss");
			put ("Robot", robot);
		}});
	}

	/**
	 * 通过讨论组号获取加群连接
	 *
	 * @param robot   机器人QQ
	 * @param discuss 需执行的讨论组ID
	 */
	public String getDiscussURL (long robot, long discuss) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetDiscussURL");
			put ("Robot", robot);
			put ("Discuss", discuss);
		}});
	}

	/**
	 * 取讨论组名称
	 *
	 * @param robot   机器人QQ
	 * @param discuss 需执行的讨论组ID
	 */
	public String getDiscussName (long robot, long discuss) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetDiscussName");
			put ("Robot", robot);
			put ("Discuss", discuss);
		}});
	}

	/**
	 * 根据图片码取得图片下载连接
	 *
	 * @param robot 机器人QQ
	 * @param group 指定的群号或讨论组号,临时会话和好友不填
	 * @param code  例如[pic={xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}.jpg]
	 */
	public String getImageURL (long robot, long group, String code) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetImageURL");
			put ("Robot", robot);
			put ("Group", group);
			put ("Code", code);
		}});
	}

	/**
	 * 取语音下载地址
	 *
	 * @param robot 机器人QQ
	 * @param code  例如[Voi={xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx}.amr]
	 */
	public String getVoiceURL (long robot, String code) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetVoiceURL");
			put ("Robot", robot);
			put ("Code", code);
		}});
	}

	/**
	 * 取用户名
	 *
	 * @param robot 机器人QQ
	 * @param qq    欲取得的QQ的号码
	 */
	public String getName (long robot, long qq) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GetName");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 把群图片的GUID转换成好友可用的GUID
	 *
	 * @param robot   机器人QQ
	 * @param picture 例：[pic={441EE15D-1D80-2AA4-1D03-96A7C303CD93}.jpg]
	 */
	public String groupPictureToFriendPicture (long robot, String picture) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "GroupPictureToFriendPicture");
			put ("Robot", robot);
			put ("Picture", picture);
		}});
	}

	/**
	 * 删除好友 成功返回真 失败返回假
	 *
	 * @param robot 机器人QQ
	 * @param qq    好友QQ号
	 */
	public boolean removeFriend (long robot, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "RemoveFriend");
			put ("Robot", robot);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 把好友删除为单项，或从对方列表删除自己 Pro可用
	 *
	 * @param robot        机器人QQ
	 * @param qq           欲操作的目标
	 * @param operatorType 1为在对方的列表删除我(双向) 2为在我的列表删除对方(单项) 默认为2
	 */
	public boolean removeFriendByOneWay (long robot, long qq, long operatorType) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "RemoveFriendByOneWay");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("OperatorType", operatorType);
		}}));
	}

	/**
	 * 删除框架帐号列表内指定QQ，不可在执行登录过程中删除QQ否则有几率引起错误 QQMini Pro才可用
	 *
	 * @param robot 机器人QQ
	 */
	public void removeRobot (long robot) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "RemoveRobot");
			put ("Robot", robot);
		}});
	}

	/**
	 * 上传群或讨论组图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
	 *
	 * @param robot  机器人QQ
	 * @param group  要上传的群号或讨论组号
	 * @param base64 图片base64文本
	 */
	public String uploadGroupChatImage (long robot, long group, String base64) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "UploadGroupChatImage");
			put ("Robot", robot);
			put ("Group", group);
			put ("Data", base64);
		}});
	}

	/**
	 * 上传群或讨论组图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
	 *
	 * @param robot 机器人QQ
	 * @param group 要上传的群号或讨论组号
	 * @param bytes 图片字节集数据
	 */
	public String uploadGroupChatImage (long robot, long group, byte[] bytes) {
		return uploadGroupChatImage (robot, group, Base64.getEncoder ().encodeToString (bytes));
	}

	/**
	 * 成功返回真 失败返回假 Pro可用
	 *
	 * @param robot    机器人QQ
	 * @param group    要发送的群号
	 * @param filePath 文件路径
	 */
	public boolean uploadGroupFile (long robot, long group, String filePath) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "UploadGroupFile");
			put ("Robot", robot);
			put ("Group", group);
			put ("Path", filePath);
		}}));
	}

	/**
	 * 上传QQ语音，成功返回语音GUID 失败返回空
	 *
	 * @param robot  机器人QQ
	 * @param group  要上传的群号
	 * @param base64 语音字节集数据（AMR Silk编码）
	 */
	public String uploadGroupChatVoice (long robot, long group, String base64) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "UploadGroupChatVoice");
			put ("Robot", robot);
			put ("Group", group);
			put ("Data", base64);
		}});
	}

	/**
	 * 上传QQ语音，成功返回语音GUID 失败返回空
	 *
	 * @param robot 机器人QQ
	 * @param group 要上传的群号
	 * @param bytes 语音字节集数据（AMR Silk编码）
	 */
	public String uploadGroupChatVoice (long robot, long group, byte[] bytes) {
		return uploadGroupChatVoice (robot, group, Base64.getUrlEncoder ().encodeToString (bytes));
	}

	/**
	 * 上传好友或临时会话图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
	 *
	 * @param robot  机器人QQ
	 * @param qq     要上传的QQ号
	 * @param base64 图片base64文本
	 */
	public String uploadPrivateChatImage (long robot, long qq, String base64) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "UploadPrivateChatImage");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("Data", base64);
		}});
	}

	/**
	 * 上传好友或临时会话图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
	 *
	 * @param robot 机器人QQ
	 * @param qq    要上传的QQ号
	 * @param bytes 图片字节集数据
	 */
	public String uploadPrivateChatImage (long robot, long qq, byte[] bytes) {
		return uploadPrivateChatImage (robot, qq, Base64.getEncoder ().encodeToString (bytes));
	}

	/**
	 * 主动加好友 成功返回真 失败返回假 当对象设置需要正确回答问题或不允许任何人添加时无条件失败 QQMini Pro才可用
	 *
	 * @param robot   机器人QQ
	 * @param qq      要添加的好友QQ号
	 * @param message 加好友提交的理由
	 */
	public String requestAddFriend (long robot, long qq, String message) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "RequestAddFriend");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("Message", message);
		}});
	}

	/**
	 * 申请加群.为了避免广告、群发行为。出现验证码时将会直接失败不处理 QQMini Pro才可用
	 *
	 * @param robot   机器人QQ
	 * @param group   欲申请加入的群号
	 * @param message 附加理由，可留空（需回答正确问题时，请填写问题答案）
	 */
	public String requestAddGroup (long robot, long group, String message) {
		return waitSystemGet (String.class, new JSONObject () {{
			put ("Type", "RequestAddGroup");
			put ("Robot", robot);
			put ("Group", group);
			put ("Message", message);
		}});
	}

	/**
	 * 获取机器人QQ是否被屏蔽消息发送状态（真屏蔽 假未屏蔽）
	 *
	 * @param robot 机器人QQ
	 */
	public boolean isMaskSendMessage (long robot) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsMaskSendMessage");
			put ("Robot", robot);
		}}));
	}

	/**
	 * 取得插件自身启用状态，启用真 禁用假
	 */
	public boolean isEnable () {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsEnable");
		}}));
	}

	/**
	 * 是否QQ好友（双向） 好友返回真 非好友或获取失败返回假
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象QQ
	 */
	public boolean isFriend (long robot, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsFriend");
			put ("Robot", robot);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 查询群是否支持群私聊功能 Pro可用
	 *
	 * @param robot 机器人QQ
	 * @param group 要查询的群号
	 */
	public boolean isAllowGroupTempMessage (long robot, long group) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsAllowGroupTempMessage");
			put ("Robot", robot);
			put ("Group", group);
		}}));
	}

	/**
	 * 查询对方是否允许网页咨询发起的临时会话消息（非讨论组和群临时）
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象QQ
	 */
	public boolean isAllowWebpageTempMessage (long robot, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "IsAllowWebpageTempMessage");
			put ("Robot", robot);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 向框架帐号列表添加一个Q.当该Q已存在时则覆盖密码 QQMini Pro才可用
	 *
	 * @param robot     机器人QQ
	 * @param password  机器人密码
	 * @param autoLogin 运行框架时是否自动登录该Q.若添加后需要登录该Q则需要通过API登录账号操作
	 */
	public boolean addRobot (long robot, String password, boolean autoLogin) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "AddRobot");
			put ("Robot", robot);
			put ("Password", password);
			put ("AutoLogin", autoLogin);
		}}));
	}

	/**
	 * 退群
	 *
	 * @param robot 机器人QQ
	 * @param group 欲退出的群号
	 */
	public void removeGroup (long robot, long group) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "RemoveGroup");
			put ("Robot", robot);
			put ("Group", group);
		}});
	}

	/**
	 * 退出讨论组
	 *
	 * @param robot   机器人QQ
	 * @param discuss 需退出的讨论组ID
	 */
	public void removeDiscuss (long robot, long discuss) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "RemoveDiscuss");
			put ("Robot", robot);
			put ("Discuss", discuss);
		}});
	}

	/**
	 * 令指定QQ下线，应确保QQ号码已在列表中且在线
	 *
	 * @param robot 机器人QQ
	 */
	public void logoutRobot (long robot) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "LogoutRobot");
			put ("Robot", robot);
		}});
	}

	/**
	 * 邀请对象加入讨论组 成功返回空 失败返回理由
	 *
	 * @param robot   机器人QQ
	 * @param discuss 需执行的讨论组ID
	 * @param qq      被邀请对象QQ
	 */
	public boolean inviteFriendJoinDiscuss (long robot, long discuss, long qq) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "InviteFriendJoinDiscuss");
			put ("Robot", robot);
			put ("Discuss", discuss);
			put ("QQ", qq);
		}}));
	}

	/**
	 * 管理员邀请对象入群，频率过快会失败
	 *
	 * @param robot 机器人QQ
	 * @param group 被邀请加入的群号
	 * @param qq    被邀请人QQ号码
	 */
	public void inviteFriendJoinGroupByAdministrator (long robot, long group, long qq) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "InviteFriendJoinGroupByAdministrator");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
		}});
	}

	/**
	 * 非管理员邀请对象入群，频率过快会失败
	 *
	 * @param robot 机器人QQ
	 * @param group 被邀请加入的群号
	 * @param qq    被邀请人QQ号码
	 */
	public void inviteFriendJoinGroupNonAdministrator (long robot, long group, long qq) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "InviteFriendJoinGroupNonAdministrator");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
		}});
	}

	/**
	 * 上传封面（通过读入字节集方式）成功真 失败假
	 *
	 * @param robot  机器人QQ
	 * @param base64 图片base64文本
	 */
	public boolean setCover (long robot, String base64) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetCover");
			put ("Robot", robot);
			put ("Data", base64);
		}}));
	}

	/**
	 * 上传封面（通过读入字节集方式）成功真 失败假
	 *
	 * @param robot 机器人QQ
	 * @param bytes 图片数据
	 */
	public boolean setCover (long robot, byte[] bytes) {
		return setCover (robot, Base64.getEncoder ().encodeToString (bytes));
	}

	/**
	 * 设置个人签名
	 *
	 * @param robot             机器人QQ
	 * @param personalSignature 签名
	 */
	public void setPersonalSignature (long robot, String personalSignature) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetPersonalSignature");
			put ("Robot", robot);
			put ("PersonalSignature", personalSignature);
		}});
	}

	/**
	 * 修改好友备注姓名
	 *
	 * @param robot 机器人QQ
	 * @param qq    需获取对象好友QQ
	 * @param notes 需要修改的备注姓名
	 */
	public void setFriendNotes (long robot, long qq, String notes) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetFriendNotes");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("Notes", notes);
		}});
	}

	/**
	 * 将好友拉入黑名单或解除
	 *
	 * @param robot  机器人QQ
	 * @param qq     要拉黑的好友QQ号
	 * @param enable 真拉黑,假取消拉黑
	 */
	public void setFriendBlacklist (long robot, long qq, boolean enable) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetFriendBlacklist");
			put ("Robot", robot);
			put ("QQ", qq);
			put ("Enable", enable);
		}});
	}

	/**
	 * 设置本机器人好友验证方式，可重复调用 Pro可用
	 *
	 * @param robot              机器人QQ
	 * @param VerificationMethod 验证方式
	 * @param question           需要回答的问题,不需要可空
	 * @param answer             设置的问题答案,不需要可空
	 */
	public boolean setFriendAuthenticationMethod (long robot, ChatRobotFriendAddMethod VerificationMethod, String question, String answer) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetFriendAuthenticationMethod");
			put ("Robot", robot);
			put ("VerificationMethod", VerificationMethod.getValue ());
			put ("Question", question);
			put ("Answer", answer);
		}}));
	}

	/**
	 * 设置机器人性别
	 *
	 * @param robot  机器人QQ
	 * @param gender 1为男 2为女
	 */
	public void setRobotGender (long robot, long gender) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetRobotGender");
			put ("Robot", robot);
			put ("Gender", gender);
		}});
	}

	/**
	 * 设置机器人在线状态+附加信息
	 *
	 * @param robot       机器人QQ
	 * @param state       在线状态
	 * @param information 最大255字节
	 */
	public void setRobotState (long robot, long state, String information) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetRobotState");
			put ("Robot", robot);
			put ("State", state);
			put ("Information", information);
		}});
	}

	/**
	 * 设置机器人昵称
	 *
	 * @param robot 机器人QQ
	 * @param name  需要设置的昵称
	 */
	public void setRobotName (long robot, String name) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetRobotName");
			put ("Robot", robot);
			put ("Name", name);
		}});
	}

	/**
	 * 禁言/解禁匿名成员
	 *
	 * @param robot                机器人QQ
	 * @param group                禁言对象所在群.
	 * @param anonymousInformation 收到匿名消息时返回的Flag 例：[AnonyMsg,Name=小遮拦,Fkey=AB4A9698AA5C3D17A173D0F7C89B8675758534099F1477206EDF559D3E3A1DD964EC71B34F9B6B77]
	 * @param duration             单位:秒 最大为1个月. 为零解除对象禁言
	 */
	public boolean setAnonymousMemberBanSpeak (long robot, long group, String anonymousInformation, long duration) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetAnonymousMemberBanSpeak");
			put ("Robot", robot);
			put ("Group", group);
			put ("AnonymousInformation", anonymousInformation);
			put ("Duration", duration);
		}}));
	}

	/**
	 * 禁言群
	 *
	 * @param robot  机器人QQ
	 * @param group  要禁言的群号
	 * @param enable 为真开启禁言. 为假解除
	 */
	public boolean setGroupBanSpeak (long robot, long group, boolean enable) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetGroupBanSpeak");
			put ("Robot", robot);
			put ("Group", group);
			put ("Enable", enable);
		}}));
	}

	/**
	 * 禁言群成员
	 *
	 * @param robot   机器人QQ
	 * @param group   要禁言的群号
	 * @param qq      要禁言的QQ号
	 * @param seconds 单位:秒 最大为1个月. 为零解除
	 */
	public boolean setGroupMemberBanSpeak (long robot, long group, long qq, long seconds) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetGroupMemberBanSpeak");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
			put ("Seconds", seconds);
		}}));
	}

	/**
	 * 修改对象群名片 成功返回真 失败返回假
	 *
	 * @param robot        机器人QQ
	 * @param group        对象所处群号
	 * @param qq           被修改名片人QQ
	 * @param businessCard 需要修改的名片
	 */
	public boolean setGroupMemberBusinessCard (long robot, long group, long qq, String businessCard) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetGroupMemberBusinessCard");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
			put ("BusinessCard", businessCard);
		}}));
	}

	/**
	 * 将对象移除群
	 *
	 * @param robot          机器人QQ
	 * @param group          被执行群号
	 * @param qq             被执行对象
	 * @param noLongerAccept 真为不再接收，假为接收，默认为假
	 */
	public void kickGroupMember (long robot, long group, long qq, boolean noLongerAccept) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "KickGroupMember");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
			put ("NoLongerAccept", noLongerAccept);
		}});
	}

	/**
	 * 设置或取消群管理员 成功返回真 失败返回假
	 *
	 * @param robot  机器人QQ
	 * @param group  指定群号
	 * @param qq     群员QQ号
	 * @param enable 真 为设置管理 假为取消管理
	 */
	public boolean setGroupAdministrator (long robot, long group, long qq, boolean enable) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetGroupAdministrator");
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
			put ("Enable", enable);
		}}));
	}

	/**
	 * 开关群匿名消息发送功能 成功返回真 失败返回假
	 *
	 * @param robot  机器人QQ
	 * @param group  需开关群匿名功能群号
	 * @param enable 真开 假关
	 */
	public boolean setGroupAnonymousEnable (long robot, long group, boolean enable) {
		return Boolean.TRUE.equals (waitSystemGet (boolean.class, new JSONObject () {{
			put ("Type", "SetGroupAnonymousEnable");
			put ("Robot", robot);
			put ("Group", group);
			put ("Enable", enable);
		}}));
	}

	/**
	 * 屏蔽或接收某群消息
	 *
	 * @param robot  机器人QQ
	 * @param group  指定群号
	 * @param enable 真 为屏蔽接收 假为接收并提醒
	 */
	public void setMaskGroupMessage (long robot, long group, boolean enable) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetMaskGroupMessage");
			put ("Robot", robot);
			put ("Group", group);
			put ("Enable", enable);
		}});
	}

	/**
	 * 在框架记录页输出一行信息
	 *
	 * @param content 输出的内容
	 */
	public void log (String content) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "Log");
			put ("Content", content);
		}});
	}

	/**
	 * 置正在输入状态（发送消息后会打断状态）
	 *
	 * @param robot 机器人QQ
	 * @param qq    置正在输入状态接收对象QQ号
	 */
	public void setInputting (long robot, long qq) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetInputting");
			put ("Robot", robot);
			put ("QQ", qq);
		}});
	}

	/**
	 * 将对象移除讨论组
	 *
	 * @param robot   机器人QQ
	 * @param discuss 需执行的讨论组ID
	 * @param qq      被执行对象
	 */
	public void kickDiscussMember (long robot, long discuss, long qq) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "KickDiscussMember");
			put ("Robot", robot);
			put ("Discuss", discuss);
			put ("QQ", qq);
		}});
	}

	/**
	 * 修改讨论组名称
	 *
	 * @param robot   机器人QQ
	 * @param discuss 需执行的讨论组ID
	 * @param name    需修改的名称
	 */
	public void setDiscussName (long robot, long discuss, String name) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetDiscussName");
			put ("Robot", robot);
			put ("Discuss", discuss);
			put ("Name", name);
		}});
	}

	/**
	 * 上传头像（通过读入字节集方式）成功真 失败假
	 *
	 * @param robot  机器人QQ
	 * @param base64 图片base64文本
	 */
	public void setAvatar (long robot, String base64) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", "SetAvatar");
			put ("Robot", robot);
			put ("Data", base64);
		}});
	}

	/**
	 * 上传头像（通过读入字节集方式）成功真 失败假
	 *
	 * @param robot 机器人QQ
	 * @param bytes 图片数据
	 */
	public void setAvatar (long robot, byte[] bytes) {
		setAvatar (robot, Base64.getEncoder ().encodeToString (bytes));
	}

	private void socketClient_onReceived (byte[] bytes) {
		String text = new String (bytes, StandardCharsets.UTF_8);
		try {
			if (onReceived != null) {
				onReceived.invoke (text);
			}
			JSONObject jsonObject = JSONObject.parseObject (text);
			String type = jsonObject.getString ("Type");
			switch (type) {
				default:
					throw new Exception (String.format ("未知的消息：%s", text));
				case "Protocol": {
					String targetProtocolVersion = jsonObject.getString ("Version");
					if (!targetProtocolVersion.equals (protocolVersion)) {
						throw new Exception (String.format ("SDK协议版本：%s 与机器人框架插件的协议版本：%s 不符", protocolVersion, targetProtocolVersion));
					}
					break;
				}
				case "Return":
					waitSystem.set (jsonObject.getLongValue ("ID"), jsonObject.getString ("Result"));
					break;
				case "Message": {
					if (onReceivedMessage != null) {
						onReceivedMessage.invoke (new ChatRobotMessage (
								this,
								ChatRobotAPI.enumParse (ChatRobotMessageType.class, jsonObject.getString ("SubType")),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getString ("Message"),
								jsonObject.getLongValue ("MessageNumber"),
								jsonObject.getLongValue ("MessageID")
						));
					}
					break;
				}
				case "GroupAddRequest":
					if (onReceivedGroupAddRequest != null) {
						onReceivedGroupAddRequest.invoke (
								ChatRobotAPI.enumParse (ChatRobotGroupAddRequestType.class, jsonObject.getString ("SubType")),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getLongValue ("InviterQQ"),
								jsonObject.getLongValue ("Sign"),
								jsonObject.getString ("Message")
						);
					}
					break;
				case "FriendAddResponse":
					if (onReceivedFriendAddResponse != null) {
						onReceivedFriendAddResponse.invoke (
								jsonObject.getBooleanValue ("Agree"),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getString ("Message")
						);
					}
					break;
				case "FriendAddRequest":
					if (onReceivedFriendAddRequest != null) {
						onReceivedFriendAddRequest.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getString ("Message")
						);
					}
					break;
				case "GroupMessageRevoke":
					if (onGroupMessageRevoked != null) {
						onGroupMessageRevoked.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getLongValue ("MessageNumber"),
								jsonObject.getLongValue ("MessageID")
						);
					}
					break;
				case "GroupAnonymousSwitch":
					if (onGroupAnonymousSwitched != null) {
						onGroupAnonymousSwitched.invoke (
								jsonObject.getBooleanValue ("Enable"),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ")
						);
					}
					break;
				case "GroupNameChange":
					if (onGroupNameChanged != null) {
						onGroupNameChanged.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getString ("Name")
						);
					}
					break;
				case "GroupBanSpeak":
					if (onGroupBannedSpeak != null) {
						onGroupBannedSpeak.invoke (
								jsonObject.getBooleanValue ("Enable"),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ")
						);
					}
					break;
				case "GroupAdminChange":
					if (onGroupAdministratorChanged != null) {
						onGroupAdministratorChanged.invoke (
								jsonObject.getBooleanValue ("Enable"),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ")
						);
					}
					break;
				case "GroupMemberBusinessCardChange":
					if (onGroupMemberBusinessCardChanged != null) {
						onGroupMemberBusinessCardChanged.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getString ("BusinessCard")
						);
					}
					break;
				case "GroupMemberLeave":
					if (onGroupMemberLeft != null) {
						onGroupMemberLeft.invoke (
								jsonObject.getBooleanValue ("Kick"),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getLongValue ("OperatorQQ")
						);
					}
					break;
				case "GroupMemberBanSpeak":
					if (onGroupMemberBannedSpeak != null) {
						onGroupMemberBannedSpeak.invoke (
								jsonObject.getBooleanValue ("Enable"),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getLongValue ("OperatorQQ"),
								jsonObject.getIntValue ("Seconds")
						);
					}
					break;
				case "GroupMemberJoin":
					if (onGroupMemberJoined != null) {
						onGroupMemberJoined.invoke (
								ChatRobotAPI.enumParse (ChatRobotGroupMemberJoinType.class, jsonObject.getString ("SubType")),
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getLongValue ("OperatorQQ")
						);
					}
					break;
				case "GroupDisband":
					if (onGroupDisbanded != null) {
						onGroupDisbanded.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("Group"),
								jsonObject.getLongValue ("QQ")
						);
					}
					break;
				case "FriendStateChange":
					if (onFriendStateChanged != null) {
						onFriendStateChanged.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("QQ"),
								jsonObject.getString ("State")
						);
					}
					break;
				case "WasRemoveByFriend":
					if (onWasRemovedByFriend != null) {
						onWasRemovedByFriend.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getLongValue ("QQ")
						);
					}
					break;
				case "OtherEvent":
					if (onReceivedOtherEvent != null) {
						onReceivedOtherEvent.invoke (
								jsonObject.getLongValue ("Robot"),
								jsonObject.getIntValue ("Event"),
								jsonObject.getIntValue ("SubType"),
								jsonObject.getLongValue ("Source"),
								jsonObject.getLongValue ("Active"),
								jsonObject.getLongValue ("Passive"),
								jsonObject.getString ("Message"),
								jsonObject.getLongValue ("MessageNumber"),
								jsonObject.getLongValue ("MessageIDr")
						);
					}
					break;
			}
		} catch (Exception exception) {
			System.out.printf ("%s 处理消息：%s 时出现异常：%s\n", new Date (), text, exception);
			exception.printStackTrace ();
		}
	}

	private void sendGroupMessage (String type, long robot, long group, String message, boolean isAnonymous) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robot);
			put ("Group", group);
			put ("Message", message);
			put ("IsAnonymous", isAnonymous);
		}});
	}

	private void sendFriendMessage (String type, long robot, long qq, String message) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robot);
			put ("QQ", qq);
			put ("Message", message);
		}});
	}

	private void sendGroupTempMessage (String type, long robot, long group, long qq, String message) {
		socketClientSendAsync (new JSONObject () {{
			put ("Type", type);
			put ("Robot", robot);
			put ("Group", group);
			put ("QQ", qq);
			put ("Message", message);
		}});
	}

	private long waitSystemSend (JSONObject jsonObject) {
		long id = waitSystem.getID ();
		jsonObject.put ("ID", id);
		socketClientSendAsync (jsonObject);
		return id;
	}

	private <T> T waitSystemConvert (Class<T> type, String result) {
		if (type.isEnum ()) {
			try {
				return ChatRobotAPI.enumGet (type, Integer.parseInt (result));
			} catch (Exception e) {
				e.printStackTrace ();
				return ChatRobotAPI.enumParse (type, result);
			}
		}
		if (type.equals (boolean.class) || type.equals (Boolean.class)) {
			return (T) (Object) Boolean.parseBoolean (result);
		}
		if (type.equals (int.class) || type.equals (Integer.class)) {
			return (T) (Object) Integer.parseInt (result);
		}
		if (type.equals (long.class) || type.equals (Long.class)) {
			return (T) (Object) Long.parseLong (result);
		}
		if (type.equals (float.class) || type.equals (Float.class)) {
			return (T) (Object) Integer.parseInt (result);
		}
		if (type.equals (String.class)) {
			return (T) result;
		}
		return JSON.parseObject (result, type);
	}

	private <T> T waitSystemGet (Class<T> type, JSONObject jsonObject) {
		try {
			return waitSystemConvert (type, waitSystem.get (waitSystemSend (jsonObject)));
		} catch (TimeoutException | InterruptedException e) {
			e.printStackTrace ();
		}
		return null;
	}

	private <T> T waitSystemGetSafe (Class<T> type, JSONObject jsonObject) throws InterruptedException, TimeoutException {
		return waitSystemConvert (type, waitSystem.get (waitSystemSend (jsonObject)));
	}

	private <Result, T> Pair<Result, T> waitSystemGet (Class<Result> resultType, Class<T> type, JSONObject jsonObject) {
		try {
			JSONArray result = JSONArray.parseArray (waitSystem.get (waitSystemSend (jsonObject)));
			return new Pair<> (waitSystemConvert (resultType, result.getString (0)), waitSystemConvert (type, result.getString (1)));
		} catch (TimeoutException | InterruptedException e) {
			e.printStackTrace ();
		}
		return null;
	}

	private void socketClientSendAsync (JSONObject jsonObject) {
		socketClient.sendAsync (jsonObject.toJSONString ().getBytes (StandardCharsets.UTF_8));
	}

}