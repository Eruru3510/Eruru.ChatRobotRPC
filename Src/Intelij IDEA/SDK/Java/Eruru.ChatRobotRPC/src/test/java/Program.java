import com.alibaba.fastjson.JSON;
import javafx.util.Pair;
import org.eruru.chatrobotrpc.ChatRobot;
import org.eruru.chatrobotrpc.ChatRobotCode;
import org.eruru.chatrobotrpc.informations.ChatRobotGroupMemberInformation;
import org.eruru.chatrobotrpc.informations.ChatRobotGroupMemberListInformation;
import org.junit.Test;

import java.io.IOException;
import java.util.Scanner;

public class Program {

	private static final ChatRobot chatRobot = new ChatRobot ();

	@Test
	public void main () {
		chatRobot.setOnReceived (message -> System.out.printf ("收到消息：%s%n", message));
		chatRobot.setOnSent (message -> System.out.printf ("发送消息：%s%n", message));
		chatRobot.setOnReceivedMessage (message -> {
			String text = message.getText ();
			switch (message.getType ()) {
				case friend:
					break;
				case group:
					if (message.getText ().contains (ChatRobotCode.at (message.getRobot ()))) {
						text = message.getText ().replace (ChatRobotCode.at (message.getRobot ()), "");
						break;
					}
					return;
				default:
					return;
			}
			if (text.startsWith ("测试")) {
				message.reply ("群名：%s 昵称：%s", chatRobot.getGroupName (message.getRobot (), message.getRobot ()), chatRobot.getName (message.getRobot (), message.getQQ ()));
			} else if (text.startsWith ("群人数")) {
				Pair<Integer, Integer> pair = chatRobot.getGroupMemberNumber (message.getRobot (), message.getGroup ());
				message.reply ("%d/%d", pair.getKey (), pair.getValue ());
			} else if (text.startsWith ("群成员列表信息")) {
				Pair<ChatRobotGroupMemberListInformation, ChatRobotGroupMemberInformation[]> pair = chatRobot.getGroupMemberListInformation (message.getRobot (), message.getRobot ());
				message.reply ("%s%s%s", JSON.toJSONString (pair.getKey (), true), ChatRobotCode.split (), JSON.toJSONString (pair.getValue (), true));
			}
		});
		chatRobot.setOnDisconnected (() -> {
			System.out.println ("连接断开");
			connect ();
		});
		connect ();
		Scanner scanner = new Scanner (System.in);
		scanner.nextLine ();
	}

	static void connect () {
		try {
			System.out.println ("开始连接");
			chatRobot.connect ("127.0.0.1", 19730, "root", "root");
			System.out.println ("连接成功");
		} catch (IOException ioException) {
			System.out.println ("连接失败");
			ioException.printStackTrace ();
			connect ();
		}
	}

}