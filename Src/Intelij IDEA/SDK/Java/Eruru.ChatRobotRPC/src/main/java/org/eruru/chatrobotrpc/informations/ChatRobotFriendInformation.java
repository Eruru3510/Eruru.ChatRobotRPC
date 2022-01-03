package org.eruru.chatrobotrpc.informations;

import org.eruru.chatrobotrpc.enums.ChatRobotGender;

public class ChatRobotFriendInformation {

	private long qq;
	private String name;
	private ChatRobotGender gender;
	private int age;
	private String country;
	private String province;
	private String city;
	private String avaterURL;

	public long getQQ () {
		return qq;
	}

	public void setQQ (long qq) {
		this.qq = qq;
	}

	public String getName () {
		return name;
	}

	public void setName (String name) {
		this.name = name;
	}

	public ChatRobotGender getGender () {
		return gender;
	}

	public void setGender (ChatRobotGender gender) {
		this.gender = gender;
	}

	public int getAge () {
		return age;
	}

	public void setAge (int age) {
		this.age = age;
	}

	public String getCountry () {
		return country;
	}

	public void setCountry (String country) {
		this.country = country;
	}

	public String getProvince () {
		return province;
	}

	public void setProvince (String province) {
		this.province = province;
	}

	public String getCity () {
		return city;
	}

	public void setCity (String city) {
		this.city = city;
	}

	public String getAvaterURL () {
		return avaterURL;
	}

	public void setAvaterURL (String avaterURL) {
		this.avaterURL = avaterURL;
	}

}