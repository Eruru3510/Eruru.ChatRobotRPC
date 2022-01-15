package org.eruru.chatrobotrpc.informations;

/**
 * 群成员信息
 */
public class ChatRobotGroupMemberInformation {

	private long qq;
	private String name;
	private String businessCard;
	private int activeIntegral;
	private int activeLevel;
	private long joinTimeStamp;
	private long lastSpeakTimeStamp;
	private int banSpeakSeconds;
	private boolean isFriend;

	/**
	 * 群成员QQ
	 */
	public long getQQ () {
		return qq;
	}

	/**
	 * 群成员QQ
	 */
	public void setQQ (long qq) {
		this.qq = qq;
	}

	/**
	 * 昵称
	 */
	public String getName () {
		return name;
	}

	/**
	 * 昵称
	 */
	public void setName (String name) {
		this.name = name;
	}

	/**
	 * 群名片
	 */
	public String getBusinessCard () {
		return businessCard;
	}

	/**
	 * 群名片
	 */
	public void setBusinessCard (String businessCard) {
		this.businessCard = businessCard;
	}

	/**
	 * 群活跃积分
	 */
	public int getActiveIntegral () {
		return activeIntegral;
	}

	/**
	 * 群活跃积分
	 */
	public void setActiveIntegral (int activeIntegral) {
		this.activeIntegral = activeIntegral;
	}

	/**
	 * 群活跃等级
	 */
	public int getActiveLevel () {
		return activeLevel;
	}

	/**
	 * 群活跃等级
	 */
	public void setActiveLevel (int activeLevel) {
		this.activeLevel = activeLevel;
	}

	/**
	 * 加群时间戳（10位）
	 */
	public long getJoinTimeStamp () {
		return joinTimeStamp;
	}

	/**
	 * 加群时间戳（10位）
	 */
	public void setJoinTimeStamp (long joinTimeStamp) {
		this.joinTimeStamp = joinTimeStamp;
	}

	/**
	 * 最后发言时间戳（10位）
	 */
	public long getLastSpeakTimeStamp () {
		return lastSpeakTimeStamp;
	}

	/**
	 * 最后发言时间戳（10位）
	 */
	public void setLastSpeakTimeStamp (long lastSpeakTimeStamp) {
		this.lastSpeakTimeStamp = lastSpeakTimeStamp;
	}

	/**
	 * 禁言时间（距禁言结束时间,秒）
	 */
	public int getBanSpeakSeconds () {
		return banSpeakSeconds;
	}

	/**
	 * 禁言时间（距禁言结束时间,秒）
	 */
	public void setBanSpeakSeconds (int banSpeakSeconds) {
		this.banSpeakSeconds = banSpeakSeconds;
	}

	/**
	 * 是否是好友
	 */
	public boolean isFriend () {
		return isFriend;
	}

	/**
	 * 是否是好友
	 */
	public void setFriend (boolean friend) {
		isFriend = friend;
	}

}
