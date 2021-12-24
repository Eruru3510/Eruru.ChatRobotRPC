package org.eruru.chatrobotrpc.informations;

public class ChatRobotLevelInformation {

private  String vip;
private  int level;
private  int activeDays;
private  int  daysRemainingForUpgrade;

	public String getVip () {
		return vip;
	}

	public void setVip (String vip) {
		this.vip = vip;
	}

	public int getLevel () {
		return level;
	}

	public void setLevel (int level) {
		this.level = level;
	}

	public int getActiveDays () {
		return activeDays;
	}

	public void setActiveDays (int activeDays) {
		this.activeDays = activeDays;
	}

	public int getDaysRemainingForUpgrade () {
		return daysRemainingForUpgrade;
	}

	public void setDaysRemainingForUpgrade (int daysRemainingForUpgrade) {
		this.daysRemainingForUpgrade = daysRemainingForUpgrade;
	}
}
