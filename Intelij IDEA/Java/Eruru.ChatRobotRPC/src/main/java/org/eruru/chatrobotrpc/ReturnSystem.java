package org.eruru.chatrobotrpc;

import java.util.HashMap;
import java.util.Map;

class ReturnSystem {

	private final Map<Long, String> returns = new HashMap<> ();

	public void add (long id, String result) {
		returns.put (id, result);
	}

	public String get (long id) {
		String value = null;
		try {
			while (true) {
				if (returns.containsKey (id)) {
					value = returns.get (id);
					break;
				}
				Thread.sleep (1);
			}
		} catch (Exception exception) {
			exception.printStackTrace ();
		}
		return value;
	}

}