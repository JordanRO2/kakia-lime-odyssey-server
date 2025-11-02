namespace kakia_lime_odyssey_packets.Packets.Models;

public class PosCalc
{
	private static double HalfPI = Math.PI / 2;
	private static double _2PI = Math.PI * 2;
	private static float s_epsilon = 0.0099999998f;

	public bool Initialized { get; set; }
	public uint StartTick { get; set; }
	public float StartAngelRadian { get; set; }
	public float StartVelocity { get; set; }
	public float Accel { get; set; }
	public float AngelRadianSpeed { get; set; }
	public bool Turning { get; set; }
	public FPOS StartPos { get; set; }
	public float Radius { get; set; }
	public FPOS CenterPos { get; set; }
	public FPOS StartAccelVec { get; set; }
	public FPOS AccelVec { get; set; }
	public FPOS StartVelocityVec { get; set; }

	public PosCalc()
	{
		Initialized = false;
		StartTick = 0;
		StartAngelRadian = 0;
		StartVelocity = 0;
		Accel = 0;
		AngelRadianSpeed = 0;
		StartVelocityVec = new FPOS()
		{
			x = 0,
			y = -1,
			z = 0
		};
		AccelVec = new FPOS()
		{
			x = 0,
			y = -1,
			z = 0
		};
		StartAccelVec = new FPOS()
		{
			x = 0,
			y = -1,
			z = 0
		};
		StartPos = new FPOS()
		{
			x = 0,
			y = 0,
			z = 0
		};

		Radius = 0;
		Turning = false;

	}

	public void Start(uint startTick, FPOS startPos, float velocity, float accel, float angleRadian, float angleRadianSpeed) 
	{ 
		FPOS centerVec = new FPOS();		
		FPOS leftVec = new FPOS(); 

		FPOS result;

		Initialized = true;
		StartTick = startTick; 
		StartAngelRadian = angleRadian;
		StartVelocity = velocity;
		Accel = accel;
		AngelRadianSpeed = angleRadianSpeed;

		float oneOverAngleRadianSpeed = 1.0f / AngelRadianSpeed;

		FPOS upVec = new FPOS()
		{
			x = 0.0f,
			y = 0.0f,
			z = 1.0f
		};

		FPOS dirVec = PosCalc.GetDirVector(StartAngelRadian, 0, 0); 
		FPOS.Cross(upVec, dirVec, out leftVec);

		StartPos = startPos;
		Radius = StartVelocity * oneOverAngleRadianSpeed; 
		
		centerVec = leftVec * Radius;
		CenterPos = StartPos + centerVec;
		Turning = false; //angleRadianSpeed != 0.0f; 
		
		if (Turning) 
		{ 
			if (accel != 0.0f) 
			{ 
				throw new InvalidOperationException("0 == accel"); 
			}

			Accel = 0.0f; 
		} 
		else 
		{
			StartVelocityVec = dirVec * velocity;
			AccelVec = dirVec * accel; 
		} 
	}

	public static double GetAngleRadian(float x, float y)
	{
		double asinAngle = Math.Asin(x);
		double asinAngle_2 = HalfPI - (asinAngle - HalfPI);

		if (asinAngle < 0.0)
		{
			asinAngle = asinAngle + _2PI;
		}

		double acosAngle = Math.Acos(-y);
		double acosAngle_2 = Math.PI - (acosAngle - Math.PI);

		if (AlmostSame((float)asinAngle, (float)acosAngle))
		{
			return asinAngle;
		}
		if (AlmostSame((float)asinAngle, (float)acosAngle_2))
		{
			return asinAngle;
		}
		if (AlmostSame((float)asinAngle_2, (float)acosAngle))
		{
			return asinAngle_2;
		}
		if (AlmostSame((float)asinAngle_2, (float)acosAngle_2))
		{
			return asinAngle_2;
		}

		return 0.0;
	}

	// Helper method to check if two values are almost the same
	public static bool AlmostSame(float fval1, float fval2)
	{
		if (fval2 == fval1)
			return true;
		if (fval2 >= fval1)
		{
			if (fval2 < fval1 + s_epsilon)
				return true;
		}
		else if (fval1 < fval2 + s_epsilon)
		{
			return true;
		}
		return false;
	}

	public FPOS GetVelocityVec(uint curTick)
	{
		FPOS result = new FPOS();
		int deltaTick = (int)(curTick - StartTick);
		double deltaSecond = (double)deltaTick * 0.001;
		if (deltaSecond <= 0.0)
		{
			deltaSecond = 0.0;
		}
		FPOS accelComponent = AccelVec * (float)deltaSecond;
		result = StartAccelVec + accelComponent;
		return result;
	}

	public static FPOS GetDirVector(float radianAngle, float x, float y)
	{
		return new FPOS()
		{
			x = (float)Math.Sin(radianAngle),
			y = -(float)Math.Cos(radianAngle),
			z = 0
		};
	}

	public PositionUpdate GetPos(uint curTick) 
	{
		PositionUpdate positionUpdate = new();
		float fScalar; 
		float radianAngle; 
		double deltaSecond; 
		FPOS moveVector = new FPOS(); 
		FPOS c2dVec = new FPOS(); 
		int deltaTick; 
		
		if (this.Initialized) 
		{ 
			deltaTick = (int)(curTick - this.StartTick); 
			deltaSecond = deltaTick * 0.001; 
			
			if (deltaSecond <= 0.0) 
			{ 
				deltaSecond = 0.0; 
			} 
			if (this.Turning) 
			{
				positionUpdate.RadianAngle = this.StartAngelRadian; 
				positionUpdate.Position = this.CenterPos;
				positionUpdate.RadianAngle = this.AngelRadianSpeed * (float)deltaSecond + positionUpdate.RadianAngle; 
				c2dVec.z = 0.0f; 
				radianAngle = positionUpdate.RadianAngle - (float)HalfPI;
				c2dVec = PosCalc.GetDirVector(radianAngle, c2dVec.x, c2dVec.y);
				c2dVec = c2dVec * this.Radius;
				positionUpdate.Position = positionUpdate.Position + c2dVec;
				positionUpdate.Position = new FPOS()
				{
					x = positionUpdate.Position.x,
					y = positionUpdate.Position.y,
					z = this.StartPos.z * (float)deltaSecond + positionUpdate.Position.z
				};
				positionUpdate.Velocity = this.StartVelocity; 
			} 
			else 
			{
				positionUpdate.Position = this.StartPos;
				moveVector = this.AccelVec;
				fScalar = (float)deltaSecond * 0.5f;
				moveVector = moveVector * fScalar;
				moveVector = moveVector + this.StartAccelVec;
				moveVector = moveVector * (float)deltaSecond;
				positionUpdate.Position = positionUpdate.Position + moveVector;
				positionUpdate.RadianAngle = this.StartAngelRadian;
				positionUpdate.Velocity = this.Accel * (float)deltaSecond + this.StartVelocity;
			} 
		}
		else
		{
			positionUpdate.RadianAngle = 0.0f;
			positionUpdate.Velocity = 0.0f;
		}

		return positionUpdate;
	}
}

public class PositionUpdate()
{
	public FPOS Position { get; set; } = new FPOS();
	public float RadianAngle { get; set; }
	public float Velocity { get; set; }
}