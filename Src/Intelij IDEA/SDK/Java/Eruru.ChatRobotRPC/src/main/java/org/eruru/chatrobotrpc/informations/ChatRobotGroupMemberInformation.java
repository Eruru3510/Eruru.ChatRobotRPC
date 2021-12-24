package org.eruru.chatrobotrpc.informations;

public class ChatRobotGroupMemberInformation {

	private  long qq;
	private  String name;
	private  String businessCard;
	private  int activeIntegral;
	private  int activeLevel;
	private  long joinTimeStamp;
	private  long lastSpeakTimeStamp;
	private  int banSpeakSeconds;
	private  boolean isFriend;

	public long getQq () {
		return qq;
	}

	public void setQq (long qq) {
		this.qq = qq;
	}

	public String getName () {
		return name;
	}

	public void setName (String name) {
		this.name = name;
	}

	public String getBusinessCard () {
		return businessCard;
	}

	public void setBusinessCard (String businessCard) {
		this.businessCard = businessCard;
	}

	public int getActiveIntegral () {
		return activeIntegral;
	}

	public void setActiveIntegral (int activeIntegral) {
		this.activeIntegral = activeIntegral;
	}

	public int getActiveLevel () {
		return activeLevel;
	}

	public void setActiveLevel (int activeLevel) {
		this.activeLevel = activeLevel;
	}

	public long getJoinTimeStamp () {
		return joinTimeStamp;
	}

	public void setJoinTimeStamp (long joinTimeStamp) {
		this.joinTimeStamp = joinTimeStamp;
	}

	public long getLastSpeakTimeStamp () {
		return lastSpeakTimeStamp;
	}

	public void setLastSpeakTimeStamp (long lastSpeakTimeStamp) {
		this.lastSpeakTimeStamp = lastSpeakTimeStamp;
	}

	public int getBanSpeakSeconds () {
		return banSpeakSeconds;
	}

	public void setBanSpeakSeconds (int banSpeakSeconds) {
		this.banSpeakSeconds = banSpeakSeconds;
	}

	public boolean isFriend () {
		return isFriend;
	}

	public void setFriend (boolean friend) {
		isFriend = friend;
	}
}
