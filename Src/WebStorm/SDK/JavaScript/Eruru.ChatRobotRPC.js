if (typeof (Eruru) == "undefined") {
	Eruru = {};
}
Eruru.ChatRobotRPC = {};

Eruru.ChatRobotRPC.ChatRobotEventType = {

	ReceivedOwnMessage: 2099,
	GroupAnnouncementChanged: 2013,
	FriendSignatureChanged: 1004,
	TalkWasCommented: 1005,
	FriendIsTyping: 1006,
	FriendFirstChatToday: 1007,
	WasJitterByFriend: 1008,
	ReceivedTenPayTransfer: 80001,
	AddedNewAccount: 11000,
	QQloggedIn: 11001,
	QQWasOfflineByManual: 11002,
	QQWasOfflineByForce: 11003,
	QQNoResponseForLongTimeOrOffline: 11004

};

Eruru.ChatRobotRPC.ChatRobotFriendAddMethod = {


	AllowAny: 0,
	NeedValidation: 1,
	NeedRightAnswer: 3,
	NeedAnswerQuestion: 4,
	AlreadyFriend: 99

};

Eruru.ChatRobotRPC.ChatRobotGender = {

	Male: 0,
	Female: 1,
	Hide: 255

}

Eruru.ChatRobotRPC.ChatRobotGroupAddRequestType = {

	Request: 1,
	InviteMe: 2,
	MemberInvite: 3

}

Eruru.ChatRobotRPC.ChatRobotGroupMemberJoinType = {

	Approve: 1,
	IJoin: 2,
	Invite: 3

}

Eruru.ChatRobotRPC.ChatRobotMessageType = {

	friend: 1,
	groupTemp: 2,
	discussTemp: 3,
	webpageTemp: 4,
	friendVerificationReply: 5,
	group: 6,
	discuss: 7

};

Eruru.ChatRobotRPC.ChatRobotRequestType = {

	Ignore: 30,
	Agree: 10,
	Refuse: 20

}

Eruru.ChatRobotRPC.ChatRobotSendMessageType = {

	text: 1,
	json: 2,
	xml: 3

};

Eruru.ChatRobotRPC.ChatRobotState = {

	Online: 1,
	QMe: 2,
	Leave: 3,
	Busy: 4,
	DoNotDisturb: 5,
	Invisible: 6

}

Eruru.ChatRobotRPC.ChatRobot = function () {

	this.heartbeatInterval = null;
	this.onConnected = null;
	this.onConnectFailed = null;
	this.onDisconnected = null;
	this.onReceived = null;
	this.onSend = null;
	this.onReceivedMessage = null;
	this.onReceivedGroupAddRequest = null;
	this.onReceivedFriendAddResponse = null;
	this.onReceivedFriendAddRequest = null;
	this.onGroupMessageRevoked = null;
	this.onGroupAnonymousSwitched = null;
	this.onGroupNameChanged = null;
	this.onGroupBannedSpeak = null;
	this.onGroupAdministratorChanged = null;
	this.onGroupMemberBusinessCardChanged = null;
	this.onGroupMemberLeft = null;
	this.onGroupMemberBannedSpeak = null;
	this.onGroupMemberJoined = null;
	this.onGroupDisbanded = null;
	this.onFriendStateChanged = null;
	this.onWasRemovedByFriend = null;
	this.onReceivedOtherEvent = null;

	var socketClient = new SocketClient ();
	var waitSystem = new WaitSystem ();
	var _account;
	var _password;

	Object.defineProperty (this, "heartbeatInterval", {
		get: function () {
			return socketClient.heartbeatInterval;
		},
		set: function (value) {
			socketClient.heartbeatInterval = value;
		}
	});
	socketClient.onConnected = function () {
		waitSystemGet ({
			Type: "Login",
			Account: _account,
			Password: _password
		}, function (success) {
			if (success) {
				if (this.onConnected instanceof Function) {
					this.onConnected ();
				}
				return;
			}
			if (this.onConnectFailed instanceof Function) {
				this.onConnectFailed ();
			}
		}.bind (this));
	}.bind (this);
	socketClient.onReceived = function (message) {
		if (this.onReceived instanceof Function) {
			this.onReceived (message);
		}
		var jsonObject = JSON.parse (message);
		switch (jsonObject["Type"]) {
			default:
				throw "未知的消息：" + message;
			case "Protocol":
				if (Eruru.ChatRobotRPC.ChatRobot.protocolVersion !== jsonObject["Version"]) {
					throw "SDK协议版本：" + Eruru.ChatRobotRPC.ChatRobot.protocolVersion + " 与机器人框架插件的协议版本：" + jsonObject["Version"] + " 不符";
				}
				break;
			case "Return":
				waitSystem.set (jsonObject["ID"], jsonObject["Result"]);
				break;
			case "Message":
				if (this.onReceivedMessage instanceof Function) {
					this.onReceivedMessage (new Eruru.ChatRobotRPC.ChatRobotMessage (
						this,
						Eruru.ChatRobotRPC.ChatRobotAPI.enumParse (Eruru.ChatRobotRPC.ChatRobotMessageType, jsonObject["SubType"]),
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["Message"],
						jsonObject["MessageNumber"],
						jsonObject["MessageID"]
					));
				}
				break;
			case "GroupAddRequest":
				if (this.onReceivedGroupAddRequest != null) {
					this.onReceivedGroupAddRequest (
						Eruru.ChatRobotRPC.ChatRobotAPI.enumParse (Eruru.ChatRobotRPC.ChatRobotGroupAddRequestType, jsonObject["SubType"]),
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["InviterQQ"],
						jsonObject["Sign"],
						jsonObject["Message"]
					);
				}
				break;
			case "FriendAddResponse":
				if (this.onReceivedFriendAddResponse != null) {
					this.onReceivedFriendAddResponse (
						jsonObject["Agree"],
						jsonObject["Robot"],
						jsonObject["QQ"],
						jsonObject["Message"]
					);
				}
				break;
			case "FriendAddRequest":
				if (this.onReceivedFriendAddRequest != null) {
					this.onReceivedFriendAddRequest (
						jsonObject["Robot"],
						jsonObject["QQ"],
						jsonObject["Message"]
					);
				}
				break;
			case "GroupMessageRevoke":
				if (this.onGroupMessageRevoked != null) {
					this.onGroupMessageRevoked (
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["MessageNumber"],
						jsonObject["MessageID"]
					);
				}
				break;
			case "GroupAnonymousSwitch":
				if (this.onGroupAnonymousSwitched != null) {
					this.onGroupAnonymousSwitched (
						jsonObject["Enable"],
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"]
					);
				}
				break;
			case "GroupNameChange":
				if (this.onGroupNameChanged != null) {
					this.onGroupNameChanged (
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["Name"]
					);
				}
				break;
			case "GroupBanSpeak":
				if (this.onGroupBannedSpeak != null) {
					this.onGroupBannedSpeak (
						jsonObject["Enable"],
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"]
					);
				}
				break;
			case "GroupAdminChange":
				if (this.onGroupAdministratorChanged != null) {
					this.onGroupAdministratorChanged (
						jsonObject["Enable"],
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"]
					);
				}
				break;
			case "GroupMemberBusinessCardChange":
				if (this.onGroupMemberBusinessCardChanged != null) {
					this.onGroupMemberBusinessCardChanged (
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["BusinessCard"]
					);
				}
				break;
			case "GroupMemberLeave":
				if (this.onGroupMemberLeft != null) {
					this.onGroupMemberLeft (
						jsonObject["Kick"],
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["OperatorQQ"]
					);
				}
				break;
			case "GroupMemberBanSpeak":
				if (this.onGroupMemberBannedSpeak != null) {
					this.onGroupMemberBannedSpeak (
						jsonObject["Enable"],
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["OperatorQQ"],
						jsonObject["Seconds"]
					);
				}
				break;
			case "GroupMemberJoin":
				if (this.onGroupMemberJoined != null) {
					this.onGroupMemberJoined (
						Eruru.ChatRobotRPC.ChatRobotAPI.enumParse (Eruru.ChatRobotRPC.ChatRobotGroupMemberJoinType, jsonObject["SubType"]),
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"],
						jsonObject["OperatorQQ"]
					);
				}
				break;
			case "GroupDisband":
				if (this.onGroupDisbanded != null) {
					this.onGroupDisbanded (
						jsonObject["Robot"],
						jsonObject["Group"],
						jsonObject["QQ"]
					);
				}
				break;
			case "FriendStateChange":
				if (this.onFriendStateChanged != null) {
					this.onFriendStateChanged (
						jsonObject["Robot"],
						jsonObject["QQ"],
						jsonObject["State"]
					);
				}
				break;
			case "WasRemoveByFriend":
				if (this.onWasRemovedByFriend != null) {
					this.onWasRemovedByFriend (
						jsonObject["Robot"],
						jsonObject["QQ"]
					);
				}
				break;
			case "OtherEvent":
				if (this.onReceivedOtherEvent != null) {
					this.onReceivedOtherEvent (
						jsonObject["Robot"],
						jsonObject["Event"],
						jsonObject["SubType"],
						jsonObject["Source"],
						jsonObject["Active"],
						jsonObject["Passive"],
						jsonObject["Message"],
						jsonObject["MessageNumber"],
						jsonObject["MessageIDr"]
					);
				}
				break;
		}
	}.bind (this);
	socketClient.onSend = function (message) {
		if (this.onSend instanceof Function) {
			this.onSend (message);
		}
	}.bind (this);
	socketClient.onDisconnected = function () {
		if (this.onDisconnected instanceof Function) {
			this.onDisconnected ();
		}
	}.bind (this);
	socketClient.onError = function () {
		throw "WebSocket出错";
	};

	this.connect = function (ip, port, account, password) {
		_account = account;
		_password = password;
		socketClient.connect (ip, port);
	};

	this.disconnect = function () {
		socketClient.disconnect ();
	};

	this.teaEncryption = function (content, key, callback) {
		waitSystemGet ({
			Type: "TEAEncryption",
			Content: content,
			Key: key
		}, callback);
	}

	this.teaDecryption = function (content, key, callback) {
		waitSystemGet ({
			Type: "TEADecryption",
			Content: content,
			Key: key
		}, callback);
	}

	this.queryGroupGiftInformations = function (robot, callback) {
		waitSystemGet ({
			Type: "QueryGroupGiftInformations",
			Robot: robot
		}, callback);
	}

	this.revokeGroupMessage = function (robot, group, messageNumber, messageID, callback) {
		waitSystemGet ({
			Type: "RevokeGroupMessage",
			Robot: robot,
			Group: group,
			MessageNumber: messageNumber,
			MessageID: messageID
		}, callback);
	}

	this.drawGroupGift = function (robot, group, callback) {
		waitSystemGet ({
			Type: "DrawGroupGift",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.handleFriendAddRequest = function (robot, qq, treatmentMethod, information) {
		socketClientSendAsync ({
			Type: "HandleFriendAddRequest",
			Robot: robot,
			QQ: qq,
			TreatmentMethod: treatmentMethod,
			Information: information
		});
	}

	this.handleGroupAddRequest = function (robot, requestType, qq, group, sign, treatmentMethod, information) {
		socketClientSendAsync ({
			Type: "HandleGroupAddRequest",
			Robot: robot,
			RequestType: requestType,
			QQ: qq,
			Group: group,
			Tag: sign,
			TreatmentMethod: treatmentMethod,
			Information: information
		});
	}

	this.createDiscuss = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "CreateDiscuss",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.loginRobot = function (robot) {
		socketClientSendAsync ({
			Type: "LoginRobot",
			Robot: robot
		});
	}

	this.like = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "Like",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.publishGroupAnnouncement = function (robot, group, title, content, callback) {
		waitSystemGet ({
			Type: "PublishGroupAnnouncement",
			Robot: robot,
			Group: group,
			Title: title,
			Content: content
		}, callback);
	}

	this.publishGroupJob = function (robot, group, name, title, content, callback) {
		waitSystemGet ({
			Type: "PublishGroupJob",
			Robot: robot,
			Group: group,
			Name: name,
			Title: title,
			Content: content
		}, callback);
	}

	this.sendFriendJsonMessage = function (robot, qq, message) {
		sendFriendMessage ("SendFriendJsonMessage", robot, qq, message);
	}

	this.sendFriendXmlMessage = function (robot, qq, message) {
		sendFriendMessage ("SendFriendXmlMessage", robot, qq, message);
	}

	this.sendFriendWindowJitter = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "SendFriendWindowJitter",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.sendFriendMessage = function (robot, qq, message) {
		sendFriendMessage ("SendFriendMessage", robot, qq, message);
	}

	this.sendFriendVerificationReplyJsonMessage = function (robot, qq, message) {
		sendFriendMessage ("SendFriendVerificationReplyJsonMessage", robot, qq, message);
	}

	this.sendFriendVerificationReplyXmlMessage = function (robot, qq, message) {
		sendFriendMessage ("SendFriendVerificationReplyXmlMessage", robot, qq, message);
	}

	this.sendFriendVerificationReplyMessage = function (robot, qq, message) {
		sendFriendMessage ("SendFriendVerificationReplyMessage", robot, qq, message);
	}

	this.sendFriendVoice = function (robot, qq, base64, callback) {
		waitSystemGet ({
			Type: "SendFriendVoice",
			Robot: robot,
			QQ: qq,
			Data: base64
		}, callback);
	}

	this.sendGroupJsonMessage = function (robot, group, message, isAnonymous) {
		sendGroupMessage ("SendGroupJsonMessage", robot, group, message, isAnonymous);
	}

	this.sendGroupXmlMessage = function (robot, group, message, isAnonymous) {
		sendGroupMessage ("SendGroupXmlMessage", robot, group, message, isAnonymous);
	}

	this.sendGroupGift = function (robot, group, qq, gift, callback) {
		waitSystemGet ({
			Type: "SendGroupGift",
			Robot: robot,
			Group: group,
			QQ: qq,
			Gift: gift
		}, callback);
	}

	this.sendGroupTempJsonMessage = function (robot, group, qq, message) {
		sendGroupTempMessage ("SendGroupTempJsonMessage", robot, group, qq, message);
	}

	this.sendGroupTempXmlMessage = function (robot, group, qq, message) {
		sendGroupTempMessage ("SendGroupTempXmlMessage", robot, group, qq, message);
	}

	this.sendGroupTempMessage = function (robot, group, qq, message) {
		sendGroupTempMessage ("SendGroupTempMessage", robot, group, qq, message);
	}

	this.sendGroupSignIn = function (robot, group, place, content, callback) {
		waitSystemGet ({
			Type: "SendGroupSignIn",
			Robot: robot,
			Group: group,
			Place: place,
			Content: content
		}, callback);
	}

	this.sendGroupMessage = function (robot, group, message, isAnonymous) {
		sendGroupMessage ("SendGroupMessage", robot, group, message, isAnonymous);
	}

	this.sendData = function (robot, data, callback) {
		waitSystemGet ({
			Type: "SendData",
			Robot: robot,
			Data: data
		}, callback);
	}

	this.sendDiscussTempJsonMessage = function (robot, group, qq, message) {
		sendGroupTempMessage ("SendDiscussTempJsonMessage", robot, group, qq, message);
	}

	this.sendDiscussJsonMessage = function (robot, group, message) {
		sendGroupMessage ("SendDiscussJsonMessage", robot, group, message, false);
	}

	this.sendDiscussTempXmlMessage = function (robot, group, qq, message) {
		sendGroupTempMessage ("SendDiscussTempXmlMessage", robot, group, qq, message);
	}

	this.sendDiscussXmlMessage = function (robot, group, message) {
		sendGroupMessage ("SendDiscussXmlMessage", robot, group, message, false);
	}

	this.sendDiscussTempMessage = function (robot, group, qq, message) {
		sendGroupTempMessage ("SendDiscussTempMessage", robot, group, qq, message);
	}

	this.sendDiscussMessage = function (robot, group, message) {
		sendGroupMessage ("SendDiscussMessage", robot, group, message, false);
	}

	this.sendWebpageTempJsonMessage = function (robot, qq, message) {
		sendFriendMessage ("SendWebpageTempJsonMessage", robot, qq, message);
	}

	this.sendWebpageTempXmlMessage = function (robot, qq, message) {
		sendFriendMessage ("SendWebpageTempXmlMessage", robot, qq, message);
	}

	this.sendWebpageTempMessage = function (robot, qq, message) {
		sendFriendMessage ("SendWebpageTempMessage", robot, qq, message);
	}

	this.friendPictureToGroupPicture = function (robot, picture, callback) {
		waitSystemGet ({
			Type: "FriendPictureToGroupPicture",
			Robot: robot,
			Picture: picture
		}, callback);
	}

	this.joinDiscussByURL = function (robot, url, callback) {
		waitSystemGet ({
			Type: "JoinDiscussByUrl",
			Robot: robot,
			Url: url
		}, callback);
	}

	this.disablePlugin = function () {
		socketClientSendAsync ({
			Type: "DisablePlugin"
		});
	}

	this.getBkn = function (robot, callback) {
		waitSystemGet ({
			Type: "GetBkn",
			Robot: robot
		}, callback);
	}

	this.getClientKey = function (robot, callback) {
		waitSystemGet ({
			Type: "GetClientKey",
			Robot: robot
		}, callback);
	}

	this.getCookies = function (robot, callback) {
		waitSystemGet ({
			Type: "GetCookies",
			Robot: robot
		}, callback);
	}

	this.getPSKey = function (robot, domainName, callback) {
		waitSystemGet ({
			Type: "GetPSKey",
			Robot: robot,
			DomainName: domainName
		}, callback);
	}

	this.getSessionKey = function (robot, callback) {
		waitSystemGet ({
			Type: "GetSessionKey",
			Robot: robot
		}, callback);
	}

	this.getLongBkn = function (robot, callback) {
		waitSystemGet ({
			Type: "GetLongBkn",
			Robot: robot
		}, callback);
	}

	this.getLongClientKey = function (robot, callback) {
		waitSystemGet ({
			Type: "GetLongClientKey",
			Robot: robot
		}, callback);
	}

	this.getLongLdw = function (robot, callback) {
		waitSystemGet ({
			Type: "GetLongLdw",
			Robot: robot
		}, callback);
	}

	this.getMemberGroupChatLevel = function (robot, group, qq, callback) {
		waitSystemGet ({
			Type: "GetMemberGroupChatLevel",
			Robot: robot,
			Group: group,
			QQ: qq
		}, callback);
	}

	this.getCurrentTimeStamp = function (callback) {
		waitSystemGet ({
			Type: "GetCurrentTimeStamp"
		}, callback);
	}

	this.tryGetLevel = function (robot, callback) {
		waitSystemGet ({
			Type: "TryGetLevel",
			Robot: robot
		}, callback);
	}

	this.getFriendQQMasterDays = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendQQMasterDays",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendQAge = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendQAge",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendNotes = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendNotes",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendLevel = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendLevel",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendPersonalDescription = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendPersonalDescription",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendPersonalSignature = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendPersonalSignature",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendListInformations = function (robot, callback) {
		waitSystemGet ({
			Type: "GetFriendListInformations",
			Robot: robot
		}, callback);
	}

	this.getFriendAge = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendAge",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.isFriendOnline = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "IsFriendOnline",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.tryGetFriendInformation = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "TryGetFriendInformation",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendGender = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendGender",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendEmail = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendEmail",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriendOnlineState = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetFriendOnlineState",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getFriends = function (robot, callback) {
		waitSystemGet ({
			Type: "GetFriends",
			Robot: robot
		}, callback);
	}

	this.tryGetRobotStateInformation = function (robot, callback) {
		waitSystemGet ({
			Type: "TryGetRobotStateInformation",
			Robot: robot
		}, callback);
	}

	this.getFrameVersionNumber = function (callback) {
		waitSystemGet ({
			Type: "GetFrameVersionNumber"
		}, callback);
	}

	this.getFrameVersionName = function (callback) {
		waitSystemGet ({
			Type: "GetFrameVersionName"
		}, callback);
	}

	this.getOfflineRobots = function (callback) {
		waitSystemGet ({
			Type: "GetOfflineRobots"
		}, callback);
	}

	this.getRobots = function (callback) {
		waitSystemGet ({
			Type: "GetRobots"
		}, callback);
	}

	this.getFrameLog = function (callback) {
		waitSystemGet ({
			Type: "GetFrameLog"
		}, callback);
	}

	this.getOnlineRobots = function (callback) {
		waitSystemGet ({
			Type: "GetOnlineRobots"
		}, callback);
	}

	this.getTargetFriendAddMethod = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetTargetFriendAddMethod",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.getGroupID = function (group, callback) {
		waitSystemGet ({
			Type: "GetGroupID",
			Group: group
		}, callback);
	}

	this.getGroupMemberListInformation = function (robot, group, callback) {
		waitSystemGet ({
			Type: "GetGroupMemberListInformation",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.getGroupMemberBusinessCard = function (robot, group, qq, callback) {
		waitSystemGet ({
			Type: "GetGroupMemberBusinessCard",
			Robot: robot,
			Group: group,
			QQ: qq
		}, callback);
	}

	this.isGroupMemberBanSpeak = function (robot, group, qq, callback) {
		waitSystemGet ({
			Type: "IsGroupMemberBanSpeak",
			Robot: robot,
			Group: group,
			QQ: qq
		}, callback);
	}

	this.getGroupMembers = function (robot, group, callback) {
		waitSystemGet ({
			Type: "GetGroupMembers",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.getGroupAnnouncement = function (robot, group, callback) {
		waitSystemGet ({
			Type: "GetGroupAnnouncement",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.getGroupAdministrators = function (robot, group, callback) {
		waitSystemGet ({
			Type: "GetGroupAdministrators",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.getGroupQQ = function (id, callback) {
		waitSystemGet ({
			Type: "GetGroupQQ",
			GroupID: id
		}, callback);
	}

	this.getGroups = function (robot, callback) {
		waitSystemGet ({
			Type: "GetGroups",
			Robot: robot
		}, callback);
	}

	this.getGroupInformations = function (robot, callback) {
		waitSystemGet ({
			Type: "GetGroupInformations",
			Robot: robot
		}, callback);
	}

	this.getGroupName = function (robot, group, callback) {
		waitSystemGet ({
			Type: "GetGroupName",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.getGroupMemberNumber = function (robot, group, callback) {
		waitSystemGet ({
			Type: "GetGroupMemberNumber",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.isGroupAnonymousEnabled = function (robot, group, callback) {
		waitSystemGet ({
			Type: "IsGroupAnonymousEnabled",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.getDiscussMembers = function (robot, discuss, callback) {
		waitSystemGet ({
			Type: "GetDiscussMembers",
			Robot: robot,
			Discuss: discuss
		}, callback);
	}

	this.getDiscusss = function (robot, callback) {
		waitSystemGet ({
			Type: "GetDiscusss",
			Robot: robot
		}, callback);
	}

	this.getDiscussURL = function (robot, discuss, callback) {
		waitSystemGet ({
			Type: "GetDiscussURL",
			Robot: robot,
			Discuss: discuss
		}, callback);
	}

	this.getDiscussName = function (robot, discuss, callback) {
		waitSystemGet ({
			Type: "GetDiscussName",
			Robot: robot,
			Discuss: discuss
		}, callback);
	}

	this.getImageURL = function (robot, group, code, callback) {
		waitSystemGet ({
			Type: "GetImageURL",
			Robot: robot,
			Group: group,
			Code: code
		}, callback);
	}

	this.getVoiceURL = function (robot, code, callback) {
		waitSystemGet ({
			Type: "GetVoiceURL",
			Robot: robot,
			Code: code
		}, callback);
	}

	this.getName = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "GetName",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.groupPictureToFriendPicture = function (robot, picture, callback) {
		waitSystemGet ({
			Type: "GroupPictureToFriendPicture",
			Robot: robot,
			Picture: picture
		}, callback);
	}

	this.removeFriend = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "RemoveFriend",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.removeFriendByOneWay = function (robot, qq, operatorType, callback) {
		waitSystemGet ({
			Type: "RemoveFriendByOneWay",
			Robot: robot,
			QQ: qq,
			OperatorType: operatorType
		}, callback);
	}

	this.removeRobot = function (robot) {
		socketClientSendAsync ({
			Type: "RemoveRobot",
			Robot: robot
		});
	}

	this.uploadGroupChatImage = function (robot, group, base64, callback) {
		waitSystemGet ({
			Type: "UploadGroupChatImage",
			Robot: robot,
			Group: group,
			Data: base64
		}, callback);
	}

	this.uploadGroupFile = function (robot, group, filePath, callback) {
		waitSystemGet ({
			Type: "UploadGroupFile",
			Robot: robot,
			Group: group,
			Path: filePath
		}, callback);
	}

	this.uploadGroupChatVoice = function (robot, group, base64, callback) {
		waitSystemGet ({
			Type: "UploadGroupChatVoice",
			Robot: robot,
			Group: group,
			Data: base64
		}, callback);
	}

	this.uploadPrivateChatImage = function (robot, qq, base64, callback) {
		waitSystemGet ({
			Type: "UploadPrivateChatImage",
			Robot: robot,
			QQ: qq,
			Data: base64
		}, callback);
	}

	this.requestAddFriend = function (robot, qq, message, callback) {
		waitSystemGet ({
			Type: "RequestAddFriend",
			Robot: robot,
			QQ: qq,
			Message: message
		}, callback);
	}

	this.requestAddGroup = function (robot, group, message, callback) {
		waitSystemGet ({
			Type: "RequestAddGroup",
			Robot: robot,
			Group: group,
			Message: message
		}, callback);
	}

	this.isMaskSendMessage = function (robot, callback) {
		waitSystemGet ({
			Type: "IsMaskSendMessage",
			Robot: robot
		}, callback);
	}

	this.isEnable = function (callback) {
		waitSystemGet ({
			Type: "IsEnable"
		}, callback);
	}

	this.isFriend = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "IsFriend",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.isAllowGroupTempMessage = function (robot, group, callback) {
		waitSystemGet ({
			Type: "IsAllowGroupTempMessage",
			Robot: robot,
			Group: group
		}, callback);
	}

	this.isAllowWebpageTempMessage = function (robot, qq, callback) {
		waitSystemGet ({
			Type: "IsAllowWebpageTempMessage",
			Robot: robot,
			QQ: qq
		}, callback);
	}

	this.addRobot = function (robot, password, autoLogin, callback) {
		waitSystemGet ({
			Type: "AddRobot",
			Robot: robot,
			Password: password,
			AutoLogin: autoLogin
		}, callback);
	}

	this.removeGroup = function (robot, group) {
		socketClientSendAsync ({
			Type: "RemoveGroup",
			Robot: robot,
			Group: group
		});
	}

	this.removeDiscuss = function (robot, discuss) {
		socketClientSendAsync ({
			Type: "RemoveDiscuss",
			Robot: robot,
			Discuss: discuss
		});
	}

	this.logoutRobot = function (robot) {
		socketClientSendAsync ({
			Type: "LogoutRobot",
			Robot: robot
		});
	}

	this.inviteFriendJoinDiscuss = function (robot, discuss, qq, callback) {
		waitSystemGet ({
			Type: "InviteFriendJoinDiscuss",
			Robot: robot,
			Discuss: discuss,
			QQ: qq
		}, callback);
	}

	this.inviteFriendJoinGroupByAdministrator = function (robot, group, qq) {
		socketClientSendAsync ({
			Type: "InviteFriendJoinGroupByAdministrator",
			Robot: robot,
			Group: group,
			QQ: qq
		});
	}

	this.inviteFriendJoinGroupNonAdministrator = function (robot, group, qq) {
		socketClientSendAsync ({
			Type: "InviteFriendJoinGroupNonAdministrator",
			Robot: robot,
			Group: group,
			QQ: qq
		});
	}

	this.setCover = function (robot, base64, callback) {
		waitSystemGet ({
			Type: "SetCover",
			Robot: robot,
			Data: base64
		}, callback);
	}

	this.setPersonalSignature = function (robot, personalSignature) {
		socketClientSendAsync ({
			Type: "SetPersonalSignature",
			Robot: robot,
			PersonalSignature: personalSignature
		});
	}

	this.setFriendNotes = function (robot, qq, notes) {
		socketClientSendAsync ({
			Type: "SetFriendNotes",
			Robot: robot,
			QQ: qq,
			Notes: notes
		});
	}

	this.setFriendBlacklist = function (robot, qq, enable) {
		socketClientSendAsync ({
			Type: "SetFriendBlacklist",
			Robot: robot,
			QQ: qq,
			Enable: enable
		});
	}

	this.setFriendAuthenticationMethod = function (robot, VerificationMethod, question, answer, callback) {
		waitSystemGet ({
			Type: "SetFriendAuthenticationMethod",
			Robot: robot,
			VerificationMethod: VerificationMethod,
			Question: question,
			Answer: answer
		}, callback);
	}

	this.setRobotGender = function (robot, gender) {
		socketClientSendAsync ({
			Type: "SetRobotGender",
			Robot: robot,
			Gender: gender
		});
	}

	this.setRobotState = function (robot, state, information) {
		socketClientSendAsync ({
			Type: "SetRobotState",
			Robot: robot,
			State: state,
			Information: information
		});
	}

	this.setRobotName = function (robot, name) {
		socketClientSendAsync ({
			Type: "SetRobotName",
			Robot: robot,
			Name: name
		});
	}

	this.setAnonymousMemberBanSpeak = function (robot, group, anonymousInformation, duration, callback) {
		waitSystemGet ({
			Type: "SetAnonymousMemberBanSpeak",
			Robot: robot,
			Group: group,
			AnonymousInformation: anonymousInformation,
			Duration: duration
		}, callback);
	}

	this.setGroupBanSpeak = function (robot, group, enable, callback) {
		waitSystemGet ({
			Type: "SetGroupBanSpeak",
			Robot: robot,
			Group: group,
			Enable: enable
		}, callback);
	}

	this.setGroupMemberBanSpeak = function (robot, group, qq, seconds, callback) {
		waitSystemGet ({
			Type: "SetGroupMemberBanSpeak",
			Robot: robot,
			Group: group,
			QQ: qq,
			Seconds: seconds
		}, callback);
	}

	this.setGroupMemberBusinessCard = function (robot, group, qq, businessCard, callback) {
		waitSystemGet ({
			Type: "SetGroupMemberBusinessCard",
			Robot: robot,
			Group: group,
			QQ: qq,
			BusinessCard: businessCard
		}, callback);
	}

	this.kickGroupMember = function (robot, group, qq, noLongerAccept) {
		socketClientSendAsync ({
			Type: "KickGroupMember",
			Robot: robot,
			Group: group,
			QQ: qq,
			NoLongerAccept: noLongerAccept
		});
	}

	this.setGroupAdministrator = function (robot, group, qq, enable, callback) {
		waitSystemGet ({
			Type: "SetGroupAdministrator",
			Robot: robot,
			Group: group,
			QQ: qq,
			Enable: enable
		}, callback);
	}

	this.setGroupAnonymousEnable = function (robot, group, enable, callback) {
		waitSystemGet ({
			Type: "SetGroupAnonymousEnable",
			Robot: robot,
			Group: group,
			Enable: enable
		}, callback);
	}

	this.setMaskGroupMessage = function (robot, group, enable) {
		socketClientSendAsync ({
			Type: "SetMaskGroupMessage",
			Robot: robot,
			Group: group,
			Enable: enable
		});
	}

	this.log = function (content) {
		socketClientSendAsync ({
			Type: "Log",
			Content: content
		});
	}

	this.setInputting = function (robot, qq) {
		socketClientSendAsync ({
			Type: "SetInputting",
			Robot: robot,
			QQ: qq
		});
	}

	this.kickDiscussMember = function (robot, discuss, qq) {
		socketClientSendAsync ({
			Type: "KickDiscussMember",
			Robot: robot,
			Discuss: discuss,
			QQ: qq
		});
	}

	this.setDiscussName = function (robot, discuss, name) {
		socketClientSendAsync ({
			Type: "SetDiscussName",
			Robot: robot,
			Discuss: discuss,
			Name: name
		});
	}

	this.setAvatar = function (robot, base64) {
		socketClientSendAsync ({
			Type: "SetAvatar",
			Robot: robot,
			Data: base64
		});
	}

	function sendGroupMessage (type, robot, group, message, isAnonymous) {
		socketClientSendAsync ({
			Type: type,
			Robot: robot,
			Group: group,
			Message: message,
			IsAnonymous: isAnonymous
		});
	}

	function sendFriendMessage (type, robot, qq, message) {
		socketClientSendAsync ({
			Type: type,
			Robot: robot,
			QQ: qq,
			Message: message
		});
	}

	function sendGroupTempMessage (type, robot, group, qq, message) {
		socketClientSendAsync ({
			Type: type,
			Robot: robot,
			Group: group,
			QQ: qq,
			Message: message
		});
	}

	function waitSystemSend (jsonObject) {
		var id = waitSystem.getID ();
		jsonObject["ID"] = id;
		socketClientSendAsync (jsonObject);
		return id;
	}

	function waitSystemConvert (result) {
		if (result instanceof Array) {
			for (var i in result) {
				result[i] = waitSystemConvert (result[i]);
			}
			return result;
		}
		return result;
	}

	function waitSystemGet (jsonObject, callback) {
		waitSystem.get (waitSystemSend (jsonObject), function (result) {
			callback (waitSystemConvert (result));
		});
	}

	function socketClientSendAsync (jsonObject) {
		socketClient.sendAsync (JSON.stringify (jsonObject));
	}

	function SocketClient () {

		this.onConnected = null;
		this.onReceived = null
		this.onSend = null;
		this.onDisconnected = null;
		this.onError = null;
		this.heartbeatInterval = 60;

		var webSocket;
		var heartbeatSendTime;
		var heartbeatThread;

		this.connect = function (ip, port) {
			webSocket = new WebSocket ("ws://" + ip + ":" + port);
			webSocket.onopen = function () {
				heartbeatSendTime = new Date ();
				heartbeatThread = setInterval (heartbeat.bind (this), 1000);
				if (this.onConnected instanceof Function) {
					this.onConnected ();
				}
			}.bind (this);
			webSocket.onmessage = function (e) {
				if (this.onReceived instanceof Function) {
					setTimeout (this.onReceived, 0, e.data);
				}
			}.bind (this);
			webSocket.onclose = function () {
				if (this.onDisconnected instanceof Function) {
					this.onDisconnected ();
				}
				clearInterval (heartbeatThread);
			}.bind (this);
			webSocket.onerror = function () {
				if (this.onError instanceof Function) {
					this.onError ();
				}
			}.bind (this);
		};

		this.sendAsync = function (message) {
			heartbeatSendTime = new Date ();
			if (message.length > 0 && this.onSend instanceof Function) {
				this.onSend (message);
			}
			webSocket.send (message);
		};

		this.disconnect = function () {
			webSocket.close ();
		};

		function heartbeat () {
			if (heartbeatSendTime.getTime () <= (new Date ().getTime () - this.heartbeatInterval * 1000)) {
				this.sendAsync ("");
			}
		}

	}

	function WaitSystem () {

		var id = 0;
		var waitPool = [];
		var waits = [];

		this.getID = function () {
			if (id > 10000) {
				id = 0;
			}
			return id++;
		};

		this.set = function (id, result) {
			for (var i = 0; i < waits.length; i++) {
				if (waits[i].id === id) {
					waits[i].callback (result);
					waitPool.push (waits[i]);
					waits.splice (i, 1);
					return;
				}
			}
			throw "没有找到ID为" + id + "的Wait";
		};

		this.get = function (id, callback) {
			var wait;
			if (waitPool.length === 0) {
				wait = new Wait ();
			} else {
				wait = waitPool.pop ();
			}
			wait.id = id;
			wait.callback = callback;
			waits.push (wait);
		};

		function Wait () {

			this.id = null;
			this.callback = null;

		}

	}

};
Eruru.ChatRobotRPC.ChatRobot.version = "1.0.0";
Eruru.ChatRobotRPC.ChatRobot.protocolVersion = "1.0.0.5";

Eruru.ChatRobotRPC.ChatRobotMessage = function (chatRobot, type, robot, group, qq, text, number, id, receivedTime) {

	this.chatRobot = chatRobot;
	this.type = type;
	this.robot = robot;
	this.group = group;
	this.qq = qq;
	this.text = text;
	this.number = number;
	this.id = id;
	this.receivedTime = receivedTime == null ? new Date () : receivedTime;

	this.reply = function (message, isAnonymous) {
		reply (this, message, Eruru.ChatRobotRPC.ChatRobotSendMessageType.text, isAnonymous);
	};

	this.replyJson = function (message, isAnonymous) {
		reply (this, message, Eruru.ChatRobotRPC.ChatRobotSendMessageType.json, isAnonymous);
	};

	this.replyXml = function (message, isAnonymous) {
		reply (this, message, Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml, isAnonymous);
	};

	this.isVoice = function () {
		return Eruru.ChatRobotRPC.ChatRobotAPI.isVoiceMessage (text);
	};

	this.containsPicture = function () {
		return Eruru.ChatRobotRPC.ChatRobotAPI.containsPictureInMessage (text);
	};

	this.containsAt = function () {
		return Eruru.ChatRobotRPC.ChatRobotAPI.containsAtInMessage (text);
	};

	this.isFlashPicture = function () {
		return Eruru.ChatRobotRPC.ChatRobotAPI.isFlashPictureMessage (text);
	};

	function reply (chatRobotMessage, message, type, isAnonymous) {
		Eruru.ChatRobotRPC.ChatRobotMessage.send (
			chatRobotMessage.chatRobot,
			chatRobotMessage.type,
			chatRobotMessage.robot,
			message,
			chatRobotMessage.group,
			chatRobotMessage.qq,
			isAnonymous,
			type
		);
	}

};
Eruru.ChatRobotRPC.ChatRobotMessage.send = function (chatRobot, type, robot, message, group, qq, isAnonymous, sendType) {
	switch (type) {
		default:
			throw Eruru.ChatRobotRPC.ChatRobotAPI.getEnumName (Eruru.ChatRobotRPC.ChatRobotMessageType, type);
		case Eruru.ChatRobotRPC.ChatRobotMessageType.friend:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendFriendMessage (robot, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendFriendJsonMessage (robot, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendFriendXmlMessage (robot, qq, message);
					break;
			}
			break;
		case Eruru.ChatRobotRPC.ChatRobotMessageType.groupTemp:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendGroupTempMessage (robot, group, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendGroupTempJsonMessage (robot, group, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendGroupTempXmlMessage (robot, group, qq, message);
					break;
			}
			break;
		case Eruru.ChatRobotRPC.ChatRobotMessageType.discussTemp:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendDiscussTempMessage (robot, group, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendDiscussTempJsonMessage (robot, group, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendDiscussTempXmlMessage (robot, group, qq, message);
					break;
			}
			break;
		case Eruru.ChatRobotRPC.ChatRobotMessageType.webpageTemp:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendWebpageTempMessage (robot, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendWebpageTempJsonMessage (robot, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendWebpageTempXmlMessage (robot, qq, message);
					break;
			}
			break;
		case Eruru.ChatRobotRPC.ChatRobotMessageType.friendVerificationReply:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendFriendVerificationReplyMessage (robot, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendFriendVerificationReplyJsonMessage (robot, qq, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendFriendVerificationReplyXmlMessage (robot, qq, message);
					break;
			}
			break;
		case Eruru.ChatRobotRPC.ChatRobotMessageType.group:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendGroupMessage (robot, group, message, isAnonymous);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendGroupJsonMessage (robot, group, message, isAnonymous);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendGroupXmlMessage (robot, group, message, isAnonymous);
					break;
			}
			break;
		case Eruru.ChatRobotRPC.ChatRobotMessageType.discuss:
			switch (sendType) {
				default:
					throw error ();
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.text:
					chatRobot.sendDiscussMessage (robot, group, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.json:
					chatRobot.sendDiscussJsonMessage (robot, group, message);
					break;
				case Eruru.ChatRobotRPC.ChatRobotSendMessageType.xml:
					chatRobot.sendDiscussXmlMessage (robot, group, message);
					break;
			}
			break;
	}

	function error () {
		return Eruru.ChatRobotRPC.ChatRobotAPI.getEnumName (Eruru.ChatRobotRPC.ChatRobotMessageType, type) + "." +
			Eruru.ChatRobotRPC.ChatRobotAPI.getEnumName (Eruru.ChatRobotRPC.ChatRobotSendMessageType, sendType);
	}
};
Eruru.ChatRobotRPC.ChatRobotMessage.prototype.toString = function () {
	return this.text;
};

Eruru.ChatRobotRPC.ChatRobotAPI = new function () {

};
Eruru.ChatRobotRPC.ChatRobotAPI.enumParse = function (enumInstance, name) {
	name = name.toLowerCase ();
	for (var key in enumInstance) {
		if (key === name) {
			return enumInstance[key];
		}
	}
	return null;
};
Eruru.ChatRobotRPC.ChatRobotAPI.getEnumName = function (enumInstance, value) {
	for (var key in enumInstance) {
		if (enumInstance[key] === value) {
			return key;
		}
	}
	return null;
};
Eruru.ChatRobotRPC.ChatRobotAPI.isVoiceMessage = function (message) {
	var matchers = /(\[Voi={[0-9A-Za-z-]+?}\..+?])(\[识别结果:([\s\S]+?)])?/.exec (message);
	var result = {
		success: false,
		guid: null,
		identifyResult: null
	};
	if (matchers == null) {
		return result;
	}
	result.success = true;
	result.guid = matchers[1];
	result.identifyResult = matchers[3];
	return result;
};
Eruru.ChatRobotRPC.ChatRobotAPI.containsPictureInMessage = function (message) {
	var pattern = /\[pic={[0-9A-Za-z-]+?}\..+?]/g;
	var matchers = pattern.exec (message);
	var result = {
		success: false,
		guids: null
	};
	if (matchers == null) {
		return result;
	}
	result.success = true;
	result.guids = [];
	do {
		result.guids.push (matchers[0]);
		matchers = pattern.exec (message);
	} while (matchers != null);
	return result;
};
Eruru.ChatRobotRPC.ChatRobotAPI.containsAtInMessage = function (message) {
	var pattern = /\[@([0-9]+?)]/g;
	var matchers = pattern.exec (message);
	var result = {
		success: false,
		qqs: null
	};
	if (matchers == null) {
		return result;
	}
	result.success = true;
	result.qqs = [];
	do {
		result.qqs.push (matchers[1]);
		matchers = pattern.exec (message);
	} while (matchers != null);
	return result;
};
Eruru.ChatRobotRPC.ChatRobotAPI.isFlashPictureMessage = function (message) {
	return /\[FlashPic={[0-9A-Za-z-]+?}\..+?]/.test (message);
};
Eruru.ChatRobotRPC.ChatRobotAPI.flashPictureToPicture = function (flashPictureGUID) {
	return flashPictureGUID.replace ("FlashPic", "pic");
};

Eruru.ChatRobotRPC.ChatRobotCode = new function () {

};
Eruru.ChatRobotRPC.ChatRobotCode.qq = "[QQ]";
Eruru.ChatRobotRPC.ChatRobotCode.split = "[next]";
Eruru.ChatRobotRPC.ChatRobotCode.newLine = "[换行]";
Eruru.ChatRobotRPC.ChatRobotCode.group = "[gname]";
Eruru.ChatRobotRPC.ChatRobotCode.timeInterval = "[TimePer]";
Eruru.ChatRobotRPC.ChatRobotCode.random = "[r]";
Eruru.ChatRobotRPC.ChatRobotCode.at = function (qq, hasSpace) {
	var values = [];
	values.push ("[@");
	if (qq > -1) {
		values.push (qq);
	} else {
		values.push ("all");
	}
	values.push (']');
	if (hasSpace == null || hasSpace) {
		values.push (' ');
	}
	return values.join ("");
};
Eruru.ChatRobotRPC.ChatRobotCode.atAll = function (hasSpace) {
	return Eruru.ChatRobotRPC.ChatRobotAPI.at (-1, hasSpace);
};
Eruru.ChatRobotRPC.ChatRobotCode.emoji = function (id) {
	return "[emoji=" + id + ']';
};
Eruru.ChatRobotRPC.ChatRobotCode.face = function (id) {
	return "[Face" + id + ".gif]";
};
Eruru.ChatRobotRPC.ChatRobotCode.bubble = function (id) {
	return "[气泡" + id + ']';
};
Eruru.ChatRobotRPC.ChatRobotCode.picture = function (id) {
	return "[pic=" + id + ']';
};
Eruru.ChatRobotRPC.ChatRobotCode.font = function (color, size) {
	return "字体[颜色=" + color + ",大小=" + size + ']';
};

Eruru.ChatRobotRPC.ChatRobotParallel = function () {

	var actions = [];

	this.add = function (action, parameters) {
		var targetParameters;
		switch (arguments.length) {
			case 1:
				targetParameters = [];
				break;
			case 2:
				if (parameters instanceof Array) {
					targetParameters = parameters;
				} else {
					targetParameters = [ parameters ];
				}
				break;
			case 3:
				targetParameters = new Array (arguments.length - 1);
				for (var i = 0; i < targetParameters.length; i++) {
					targetParameters[i] = arguments[i + 1];
				}
				break;
		}
		actions.push ({
			action: action,
			parameters: targetParameters
		});
	};

	this.invoke = function (callback) {
		var count = 0;
		for (var i = 0; i < actions.length; i++) {
			actions [i].parameters.push (function (result) {
				this.result = result;
				count++;
				if (count >= actions.length) {
					var results = [];
					for (var i = 0; i < actions.length; i++) {
						results.push (actions[i].result);
					}
					callback.apply (this, results);
				}
			}.bind (actions[i]));
			actions[i].action.apply (this, actions [i].parameters);
		}
	};

};