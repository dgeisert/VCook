using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_SaveObject : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		SaveObject data = (SaveObject)obj;
		// Add your writer.Write calls here.
		writer.Write(data.phase);
		writer.Write(data.timerStart);
		writer.Write(data.timerDuration);
		writer.Write(data.secondaryTimerStart);
		writer.Write(data.secondaryTimerDuration);
		writer.Write(data.objName);
		writer.Write(data.state);
		writer.Write(data.parentGround);

	}
	
	public override object Read(ES2Reader reader)
	{
		SaveObject data = new SaveObject();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		SaveObject data = (SaveObject)c;
		// Add your reader.Read calls here to read the data into the object.
		data.phase = reader.Read<System.Int32>();
		data.timerStart = reader.Read<System.DateTime>();
		data.timerDuration = reader.Read<System.Single>();
		data.secondaryTimerStart = reader.Read<System.DateTime>();
		data.secondaryTimerDuration = reader.Read<System.Single>();
		data.objName = reader.Read<System.String>();
		data.state = reader.Read<System.String>();
		data.parentGround = reader.Read<UnityEngine.Vector2>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_SaveObject():base(typeof(SaveObject)){}
}