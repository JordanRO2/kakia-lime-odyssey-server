using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential)]
public struct FPOS
{
	private static double HalfPI = Math.PI / 2;
	private static double _2PI = Math.PI * 2;
	private static float s_epsilon = 0.0099999998f;


	public float x;
	public float y;
	public float z;

	public FPOS CalculateDirection(FPOS destination)
	{
		double dx = destination.x - x;
		double dy = destination.y - y;
		var dirVec = new FPOS
		{
			x = (float)dx,
			y = (float)dy,
			z = 0
		};

		return dirVec.Unitize();
	}

	public bool IsNaN()
	{
		return float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z);
	}

	public double CalculateDistance(FPOS destination)
	{
		return Math.Sqrt(
			Math.Pow(destination.x - x, 2) +
			Math.Pow(destination.y - y, 2) +
			Math.Pow(destination.z - z, 2)
		);
	}

	public double CalculateTravelTime(FPOS destination, double velocity) 
	{ 
		double distance = CalculateDistance(destination); 
		return distance / velocity; 
	}

	public FPOS CalculateCurrentPosition(FPOS destination, double velocity, double elapsedTime)
	{
		FPOS direction = CalculateDirection(destination);
		double distanceTraveled = velocity * elapsedTime;

		double currentX = x + direction.x * distanceTraveled;
		double currentY = y + direction.y * distanceTraveled;
		double currentZ = z + direction.z * distanceTraveled;

		// Check if the new position is within epsilon distance of the destination
		if (Math.Abs(currentX - destination.x) < s_epsilon &&
			Math.Abs(currentY - destination.y) < s_epsilon &&
			Math.Abs(currentZ - destination.z) < s_epsilon)
		{
			return destination;
		}

		// Check if the new position exceeds the destination in any direction
		if ((direction.x >= 0 && currentX >= destination.x) || (direction.x < 0 && currentX <= destination.x) && 
			(direction.y >= 0 && currentY >= destination.y) || (direction.y < 0 && currentY <= destination.y)) 
		{ 
			return destination; 
		}

		return new FPOS()
		{
			x = (float)currentX,
			y = (float)currentY,
			z = (float)currentZ
		};
	}

	public FPOS GetRandomPositionWithinRadius(double radius)
	{
		Random random = new Random();

		double u = random.NextDouble();
		double v = random.NextDouble();
		double theta = 2 * Math.PI * u;
		double phi = Math.Acos(2 * v - 1);
		double r = radius * Math.Cbrt(random.NextDouble());
		double x = r * Math.Sin(phi) * Math.Cos(theta);
		double y = r * Math.Sin(phi) * Math.Sin(theta);
		//double z = r * Math.Cos(phi);

		return new FPOS()
		{
			x = (float)(this.x + x),
			y = (float)(this.y + y),
			z = this.z //(float)(this.z + z)
		};
	}

	public bool Compare(FPOS other)
	{		
		return Math.Abs(x - other.x) < s_epsilon && 
			   Math.Abs(y - other.y) < s_epsilon && 
			   Math.Abs(z - other.z) < s_epsilon;
	}

	public FPOS CalculatePositionAtPercentage(FPOS destination, double percentage)
	{
		FPOS direction = CalculateDirection(destination); 
		double distance = CalculateDistance(destination);
		double distanceToTravel = distance * percentage;
		return new FPOS { 
			x = (float)(x + direction.x * distanceToTravel), 
			y = (float)(y + direction.y * distanceToTravel), 
			z = (float)(z + direction.z * distanceToTravel) }; 
	}

	public float GetLength(FPOS pos)
	{
		return (float)Math.Sqrt(z * z + y * y + x * x);
	}

	public FPOS Unitize() 
	{ 
		double fLength = GetLength(this);
		float x = 0;
		float y = 0;
		float z = 0;

		if (fLength > 0.0000009999999974752427) 
		{
			double fRecip = 1.0 / fLength;
			x = (float)(this.x * fRecip);
			y = (float)(this.y * fRecip);
			z = (float)(this.z * fRecip);
		} 

		return new FPOS()
		{
			x = x,
			y = y,
			z = z
		}; 
	}


	public static void Cross(FPOS vector1, FPOS vector2, out FPOS result) 
	{ 
		result = new FPOS 
		{ 
			x = vector1.y * vector2.z - vector1.z * vector2.y, 
			y = vector1.z * vector2.x - vector1.x * vector2.z, 
			z = vector1.x * vector2.y - vector1.y * vector2.x 
		}; 
	}


	public static FPOS operator *(FPOS point, float scalar) 
	{ 
		return new FPOS() 
		{ 
			x = point.x * scalar,
			y = point.y * scalar, 
			z = point.z * scalar 
		}; 
	}

	public static FPOS operator +(FPOS point1, FPOS point2) 
	{ 
		return new FPOS()
		{
			x = point1.x + point2.x,
			y = point1.y + point2.y,
			z = point1.z + point2.z
		}; 
	}
}