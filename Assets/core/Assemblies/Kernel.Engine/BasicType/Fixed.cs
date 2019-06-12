using System;
using System.IO;

#if CODE_GEN || UNITY_EDITOR
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endif

namespace Kernel.Engine
{
	/// <summary>
	/// Represents a Q31.32 fixed-point number.
	/// </summary>
	[Serializable]
	public partial struct Fixed : IEquatable<Fixed>, IComparable<Fixed>
#if CODE_GEN || UNITY_EDITOR
		, IXmlSerializable
#endif
	{
		private long raw;

		private const long MAX_VALUE = long.MaxValue;
		private const long MIN_VALUE = long.MinValue;

		public const int NUM_BITS = 64;
		public const int FRACTIONAL_PLACES = 32;

		private const long ONE = 1L << FRACTIONAL_PLACES;
		private const long TEN = 10L << FRACTIONAL_PLACES;
		private const long HALF = 1L << (FRACTIONAL_PLACES - 1);

		private const long PI_TIMES_2 = 0x6487ED511;
		private const long PI = 0x3243F6A88;
		private const long PI_OVER_2 = 0x1921FB544; // pi/2
		private const int LUT_SIZE = (int)(PI_OVER_2 >> 20);

		// Precision of this type is 2^-32, that is 2,3283064365386962890625E-10
		public static readonly decimal Precision = (decimal)(new Fixed(1L));//0.00000000023283064365386962890625m;
		public static readonly Fixed MaxValue = new Fixed(MAX_VALUE - 1);
		public static readonly Fixed MinValue = new Fixed(MIN_VALUE + 2);
		/// <summary>1f</summary>
		public static readonly Fixed One = new Fixed(ONE);
		/// <summary>10f</summary>
		public static readonly Fixed Ten = new Fixed(TEN);
		/// <summary>0.5f</summary>
		public static readonly Fixed Half = new Fixed(HALF);

		public static readonly Fixed Zero = new Fixed();
		public static readonly Fixed PositiveInfinity = new Fixed(MAX_VALUE);
		public static readonly Fixed NegativeInfinity = new Fixed(MIN_VALUE + 1);
		public static readonly Fixed NaN = new Fixed(MIN_VALUE);

		/// <summary>0.1f</summary>
		public static readonly Fixed EN1 = Fixed.One / 10;
		/// <summary>0.01f</summary>
		public static readonly Fixed EN2 = Fixed.One / 100;
		/// <summary>0.001f</summary>
		public static readonly Fixed EN3 = Fixed.One / 1000;
		/// <summary>0.0001f</summary>
		public static readonly Fixed EN4 = Fixed.One / 10000;
		/// <summary>0.00001f</summary>
		public static readonly Fixed EN5 = Fixed.One / 100000;
		/// <summary>0.000001f</summary>
		public static readonly Fixed EN6 = Fixed.One / 1000000;
		/// <summary>0.0000001f</summary>
		public static readonly Fixed EN7 = Fixed.One / 10000000;
		/// <summary>0.00000001f</summary>
		public static readonly Fixed EN8 = Fixed.One / 100000000;

		public static readonly Fixed Epsilon = Fixed.EN3;

		/// <summary>
		/// The value of Pi
		/// </summary>
		public static readonly Fixed Pi = new Fixed(PI);
		public static readonly Fixed PiOver2 = new Fixed(PI_OVER_2);
		public static readonly Fixed PiTimes2 = new Fixed(PI_TIMES_2);
		public static readonly Fixed PiInv = (Fixed)0.3183098861837906715377675267M;
		public static readonly Fixed PiOver2Inv = (Fixed)0.6366197723675813430755350535M;

		public static readonly Fixed Deg2Rad = Pi / new Fixed(180);

		public static readonly Fixed Rad2Deg = new Fixed(180) / Pi;

		public static readonly Fixed LutInterval = (Fixed)(LUT_SIZE - 1) / PiOver2;

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		public static int Sign(Fixed value)
		{
			return
				value.raw < 0 ? -1 :
					value.raw > 0 ? 1 :
						0;
		}


		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
		/// </summary>
		public static Fixed Abs(Fixed value)
		{
			if(value.raw == MIN_VALUE)
			{
				return MaxValue;
			}

			// branchless implementation, see http://www.strchr.com/optimized_abs_function
			var mask = value.raw >> 63;
			return new Fixed((value.raw + mask) ^ mask);
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// FastAbs(Fix64.MinValue) is undefined.
		/// </summary>
		public static Fixed FastAbs(Fixed value)
		{
			// branchless implementation, see http://www.strchr.com/optimized_abs_function
			var mask = value.raw >> 63;
			return new Fixed(~value.raw+1);
		} 


		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		public static Fixed Floor(Fixed value)
		{
			// Just zero out the fractional part
			return new Fixed((long)((ulong)value.raw & 0xFFFFFFFF00000000));
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		public static Fixed Ceiling(Fixed value)
		{
			var hasFractionalPart = (value.raw & 0x00000000FFFFFFFF) != 0;
			return hasFractionalPart ? Floor(value) + One : value;
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		public static Fixed Round(Fixed value)
		{
			var fractionalPart = value.raw & 0x00000000FFFFFFFF;
			var integralPart = Floor(value);
			if(fractionalPart < 0x80000000)
			{
				return integralPart;
			}
			if(fractionalPart > 0x80000000)
			{
				return integralPart + One;
			}
			// if number is halfway between two values, round to the nearest even number
			// this is the method used by System.Math.Round().
			return (integralPart.raw & ONE) == 0
				? integralPart
				: integralPart + One;
		}

		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		public static Fixed operator +(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;
			var sum = xl + yl;
			// if signs of operands are equal and signs of sum and x are different
			if(((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
			{
				return xl > 0 ? MaxValue : MinValue;
			}
			return new Fixed(sum);
		}

		/// <summary>
		/// Adds x and y performing overflow checking. Should be inlined by the CLR.
		/// </summary>
		/*public static Fixed OverflowAdd(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;
			var sum = xl + yl;
			// if signs of operands are equal and signs of sum and x are different
			if(((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
			{
				sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
			}
			return new Fixed(sum);
		}*/

		/// <summary>
		/// Adds x and y witout performing overflow checking. Should be inlined by the CLR.
		/// </summary>
		public static Fixed FastAdd(Fixed x, Fixed y)
		{
			return new Fixed(x.raw + y.raw);
		}

		/// <summary>
		/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		public static Fixed operator -(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;
			var diff = xl - yl;
			// if signs of operands are different and signs of sum and x are different
			if((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
			{
				return xl < 0 ? MinValue : MaxValue;
			}
			return new Fixed(diff);
		}

		/// <summary>
		/// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
		/// </summary>
		/*public static Fixed OverflowSub(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;
			var diff = xl - yl;
			// if signs of operands are different and signs of sum and x are different
			if((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
			{
				diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
			}
			return new Fixed(diff);
		}*/

		/// <summary>
		/// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
		/// </summary>
		public static Fixed FastSub(Fixed x, Fixed y)
		{
			return new Fixed(x.raw - y.raw);
		}

		static long AddOverflowHelper(long x, long y, ref bool overflow)
		{
			var sum = x + y;
			// x + y overflows if sign(x) ^ sign(y) != sign(sum)
			overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
			return sum;
		}

		public static Fixed operator *(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;

			var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
			var xhi = xl >> FRACTIONAL_PLACES;
			var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
			var yhi = yl >> FRACTIONAL_PLACES;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FRACTIONAL_PLACES;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FRACTIONAL_PLACES;

			bool overflow = false;
			var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
			sum = AddOverflowHelper(sum, midResult2, ref overflow);
			sum = AddOverflowHelper(sum, hiResult, ref overflow);

			bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

			// if signs of operands are equal and sign of result is negative,
			// then multiplication overflowed positively
			// the reverse is also true
			if(opSignsEqual)
			{
				if(sum < 0 || (overflow && xl > 0))
				{
					return MaxValue;
				}
			}
			else
			{
				if(sum > 0)
				{
					return MinValue;
				}
			}

			// if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
			// then this means the result overflowed.
			var topCarry = hihi >> FRACTIONAL_PLACES;
			if(topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
			{
				return opSignsEqual ? MaxValue : MinValue;
			}

			// If signs differ, both operands' magnitudes are greater than 1,
			// and the result is greater than the negative operand, then there was negative overflow.
			if(!opSignsEqual)
			{
				long posOp, negOp;
				if(xl > yl)
				{
					posOp = xl;
					negOp = yl;
				}
				else
				{
					posOp = yl;
					negOp = xl;
				}
				if(sum > negOp && negOp < -ONE && posOp > ONE)
				{
					return MinValue;
				}
			}

			return new Fixed(sum);
		}

		/*public static Fixed OverflowMul(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;

			var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
			var xhi = xl >> FRACTIONAL_PLACES;
			var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
			var yhi = yl >> FRACTIONAL_PLACES;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FRACTIONAL_PLACES;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FRACTIONAL_PLACES;

			bool overflow = false;
			var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
			sum = AddOverflowHelper(sum, midResult2, ref overflow);
			sum = AddOverflowHelper(sum, hiResult, ref overflow);

			bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

			// if signs of operands are equal and sign of result is negative,
			// then multiplication overflowed positively
			// the reverse is also true
			if(opSignsEqual)
			{
				if(sum < 0 || (overflow && xl > 0))
				{
					return MaxValue;
				}
			}
			else
			{
				if(sum > 0)
				{
					return MinValue;
				}
			}

			// if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
			// then this means the result overflowed.
			var topCarry = hihi >> FRACTIONAL_PLACES;
			if(topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*)
			{
				return opSignsEqual ? MaxValue : MinValue;
			}

			// If signs differ, both operands' magnitudes are greater than 1,
			// and the result is greater than the negative operand, then there was negative overflow.
			if(!opSignsEqual)
			{
				long posOp, negOp;
				if(xl > yl)
				{
					posOp = xl;
					negOp = yl;
				}
				else
				{
					posOp = yl;
					negOp = xl;
				}
				if(sum > negOp && negOp < -ONE && posOp > ONE)
				{
					return MinValue;
				}
			}

			return new Fixed(sum);
		}*/

		/// <summary>
		/// Performs multiplication without checking for overflow.
		/// Useful for performance-critical code where the values are guaranteed not to cause overflow
		/// </summary>
		public static Fixed FastMul(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;

			var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
			var xhi = xl >> FRACTIONAL_PLACES;
			var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
			var yhi = yl >> FRACTIONAL_PLACES;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FRACTIONAL_PLACES;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FRACTIONAL_PLACES;

			var sum = (long)loResult + midResult1 + midResult2 + hiResult;
			Fixed result;// = default(FP);
			result.raw = sum;
			return result;
		}

		//[MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
		public static int CountLeadingZeroes(ulong x)
		{
			int result = 0;
			while((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
			while((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
			return result;
		}

		public static Fixed operator /(Fixed x, Fixed y)
		{
			var xl = x.raw;
			var yl = y.raw;

			if(yl == 0)
			{
				return MaxValue;
			}

			var remainder = (ulong)(xl >= 0 ? xl : -xl);
			var divider = (ulong)(yl >= 0 ? yl : -yl);
			var quotient = 0UL;
			var bitPos = NUM_BITS / 2 + 1;


			// If the divider is divisible by 2^n, take advantage of it.
			while((divider & 0xF) == 0 && bitPos >= 4)
			{
				divider >>= 4;
				bitPos -= 4;
			}

			while(remainder != 0 && bitPos >= 0)
			{
				int shift = CountLeadingZeroes(remainder);
				if(shift > bitPos)
				{
					shift = bitPos;
				}
				remainder <<= shift;
				bitPos -= shift;

				var div = remainder / divider;
				remainder = remainder % divider;
				quotient += div << bitPos;

				// Detect overflow
				if((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
				{
					return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
				}

				remainder <<= 1;
				--bitPos;
			}

			// rounding
			++quotient;
			var result = (long)(quotient >> 1);
			if(((xl ^ yl) & MIN_VALUE) != 0)
			{
				result = -result;
			}

			return new Fixed(result);
		}

		public static Fixed operator %(Fixed x, Fixed y)
		{
			return new Fixed(
				x.raw == MIN_VALUE && y.raw == -1 ?
					0 :
					x.raw % y.raw);
		}

		/// <summary>
		/// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
		/// Use the operator (%) for a more reliable but slower modulo.
		/// </summary>
		public static Fixed FastMod(Fixed x, Fixed y)
		{
			return new Fixed(x.raw % y.raw);
		}

		public static Fixed operator -(Fixed x)
		{
			return x.raw == MIN_VALUE ? MaxValue : new Fixed(-x.raw);
		}

		public static bool operator ==(Fixed x, Fixed y)
		{
			return x.raw == y.raw;
		}

		public static bool operator !=(Fixed x, Fixed y)
		{
			return x.raw != y.raw;
		}

		public static bool operator >(Fixed x, Fixed y)
		{
			return x.raw > y.raw;
		}

		public static bool operator <(Fixed x, Fixed y)
		{
			return x.raw < y.raw;
		}

		public static bool operator >=(Fixed x, Fixed y)
		{
			return x.raw >= y.raw;
		}

		public static bool operator <=(Fixed x, Fixed y)
		{
			return x.raw <= y.raw;
		}


		/// <summary>
		/// Returns the square root of a specified number.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was negative.
		/// </exception>
		public static Fixed Sqrt(Fixed x)
		{
			var xl = x.raw;
			if(xl < 0)
			{
				// We cannot represent infinities like Single and Double, and Sqrt is
				// mathematically undefined for x < 0. So we just throw an exception.
				throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
			}

			var num = (ulong)xl;
			var result = 0UL;

			// second-to-top bit
			var bit = 1UL << (NUM_BITS - 2);

			while(bit > num)
			{
				bit >>= 2;
			}

			// The main part is executed twice, in order to avoid
			// using 128 bit values in computations.
			for(var i = 0; i < 2; ++i)
			{
				// First we get the top 48 bits of the answer.
				while(bit != 0)
				{
					if(num >= result + bit)
					{
						num -= result + bit;
						result = (result >> 1) + bit;
					}
					else
					{
						result = result >> 1;
					}
					bit >>= 2;
				}

				if(i == 0)
				{
					// Then process it again to get the lowest 16 bits.
					if(num > (1UL << (NUM_BITS / 2)) - 1)
					{
						// The remainder 'num' is too large to be shifted left
						// by 32, so we have to add 1 to result manually and
						// adjust 'num' accordingly.
						// num = a - (result + 0.5)^2
						//       = num + result^2 - (result + 0.5)^2
						//       = num - result - 0.5
						num -= result;
						num = (num << (NUM_BITS / 2)) - 0x80000000UL;
						result = (result << (NUM_BITS / 2)) + 0x80000000UL;
					}
					else
					{
						num <<= (NUM_BITS / 2);
						result <<= (NUM_BITS / 2);
					}

					bit = 1UL << (NUM_BITS / 2 - 2);
				}
			}
			// Finally, if next bit would have been 1, round the result upwards.
			if(num > result)
			{
				++result;
			}
			return new Fixed((long)result);
		}

		/// <summary>
		/// Returns the Sine of x.
		/// This function has about 9 decimals of accuracy for small values of x.
		/// It may lose accuracy as the value of x grows.
		/// Performance: about 25% slower than Math.Sin() in x64, and 200% slower in x86.
		/// </summary>
		public static Fixed Sin(Fixed x)
		{
			bool flipHorizontal, flipVertical;
			var clampedL = ClampSinValue(x.RawValue, out flipHorizontal, out flipVertical);
			var clamped = new Fixed(clampedL);

			// Find the two closest values in the LUT and perform linear interpolation
			// This is what kills the performance of this function on x86 - x64 is fine though
			var rawIndex = FastMul(clamped, LutInterval);
			var roundedIndex = Round(rawIndex);
			var indexError = 0;//FastSub(rawIndex, roundedIndex);

			var nearestValue = new Fixed(SinLut[flipHorizontal ?
				SinLut.Length - 1 - (int)roundedIndex :
				(int)roundedIndex]);
			var secondNearestValue = new Fixed(SinLut[flipHorizontal ?
				SinLut.Length - 1 - (int)roundedIndex - Sign(indexError) :
				(int)roundedIndex + Sign(indexError)]);

			var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue))).RawValue;
			var interpolatedValue = nearestValue.RawValue + (flipHorizontal ? -delta : delta);
			var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
			Fixed a2 = new Fixed(finalValue);
			return a2;
		}

		/// <summary>
		/// Returns a rough approximation of the Sine of x.
		/// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
		/// however its accuracy is limited to 4-5 decimals, for small enough values of x.
		/// </summary>
		public static Fixed FastSin(Fixed x)
		{
			bool flipHorizontal, flipVertical;
			var clampedL = ClampSinValue(x.RawValue, out flipHorizontal, out flipVertical);

			// Here we use the fact that the SinLut table has a number of entries
			// equal to (PI_OVER_2 >> 15) to use the angle to index directly into it
			var rawIndex = (uint)(clampedL >> 15);
			if(rawIndex >= LUT_SIZE)
			{
				rawIndex = LUT_SIZE - 1;
			}
			var nearestValue = SinLut[flipHorizontal ?
				SinLut.Length - 1 - (int)rawIndex :
				(int)rawIndex];
			return new Fixed(flipVertical ? -nearestValue : nearestValue);
		}



		//[MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
		public static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
		{
			// Clamp value to 0 - 2*PI using modulo; this is very slow but there's no better way AFAIK
			var clamped2Pi = angle % PI_TIMES_2;
			if(angle < 0)
			{
				clamped2Pi += PI_TIMES_2;
			}

			// The LUT contains values for 0 - PiOver2; every other value must be obtained by
			// vertical or horizontal mirroring
			flipVertical = clamped2Pi >= PI;
			// obtain (angle % PI) from (angle % 2PI) - much faster than doing another modulo
			var clampedPi = clamped2Pi;
			while(clampedPi >= PI)
			{
				clampedPi -= PI;
			}
			flipHorizontal = clampedPi >= PI_OVER_2;
			// obtain (angle % PI_OVER_2) from (angle % PI) - much faster than doing another modulo
			var clampedPiOver2 = clampedPi;
			if(clampedPiOver2 >= PI_OVER_2)
			{
				clampedPiOver2 -= PI_OVER_2;
			}
			return clampedPiOver2;
		}

		/// <summary>
		/// Returns the cosine of x.
		/// See Sin() for more details.
		/// </summary>
		public static Fixed Cos(Fixed x)
		{
			var xl = x.RawValue;
			var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
			Fixed a2 = Sin(new Fixed(rawAngle));
			return a2;
		}

		/// <summary>
		/// Returns a rough approximation of the cosine of x.
		/// See FastSin for more details.
		/// </summary>
		public static Fixed FastCos(Fixed x)
		{
			var xl = x.RawValue;
			var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
			return FastSin(new Fixed(rawAngle));
		}

		/// <summary>
		/// Returns the tangent of x.
		/// </summary>
		/// <remarks>
		/// This function is not well-tested. It may be wildly inaccurate.
		/// </remarks>
		public static Fixed Tan(Fixed x)
		{
			var clampedPi = x.RawValue % PI;
			var flip = false;
			if(clampedPi < 0)
			{
				clampedPi = -clampedPi;
				flip = true;
			}
			if(clampedPi > PI_OVER_2)
			{
				flip = !flip;
				clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
			}

			var clamped = new Fixed(clampedPi);

			// Find the two closest values in the LUT and perform linear interpolation
			var rawIndex = FastMul(clamped, LutInterval);
			var roundedIndex = Round(rawIndex);
			var indexError = FastSub(rawIndex, roundedIndex);

			var nearestValue = new Fixed(TanLut[(int)roundedIndex]);
			var secondNearestValue = new Fixed(TanLut[(int)roundedIndex + Sign(indexError)]);

			var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue))).RawValue;
			var interpolatedValue = nearestValue.RawValue + delta;
			var finalValue = flip ? -interpolatedValue : interpolatedValue;
			Fixed a2 = new Fixed(finalValue);
			return a2;
		}

		public static Fixed Atan(Fixed y)
		{
			return Atan2(y, 1);
		}

		public static Fixed Atan2(Fixed y, Fixed x)
		{
			var yl = y.raw;
			var xl = x.raw;
			if(xl == 0)
			{
				if(yl > 0)
				{
					return PiOver2;
				}
				if(yl == 0)
				{
					return Zero;
				}
				return -PiOver2;
			}
			Fixed atan;
			var z = y / x;

			Fixed sm = Fixed.EN2 * 28;
			// Deal with overflow
			if(One + sm * z * z == MaxValue)
			{
				return y < Zero ? -PiOver2 : PiOver2;
			}

			if(Abs(z) < One)
			{
				atan = z / (One + sm * z * z);
				if(xl < 0)
				{
					if(yl < 0)
					{
						return atan - Pi;
					}
					return atan + Pi;
				}
			}
			else
			{
				atan = PiOver2 - z / (z * z + sm);
				if(yl < 0)
				{
					return atan - Pi;
				}
			}
			return atan;
		}

		public static Fixed Asin(Fixed value)
		{
			return FastSub(PiOver2, Acos(value));
		}

		public static Fixed Acos(Fixed value)
		{
			if(value == 0)
			{
				return Fixed.PiOver2;
			}

			bool flip = false;
			if(value < 0)
			{
				value = -value;
				flip = true;
			}

			// Find the two closest values in the LUT and perform linear interpolation
			var rawIndex = FastMul(value, LUT_SIZE);
			var roundedIndex = Round(rawIndex);
			if(roundedIndex >= LUT_SIZE)
			{
				roundedIndex = LUT_SIZE - 1;
			}

			var indexError = FastSub(rawIndex, roundedIndex);
			var nearestValue = new Fixed(AcosLut[(int)roundedIndex]);

			var nextIndex = (int)roundedIndex + Sign(indexError);
			if(nextIndex >= LUT_SIZE)
			{
				nextIndex = LUT_SIZE - 1;
			}

			var secondNearestValue = new Fixed(AcosLut[nextIndex]);

			var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue))).RawValue;
			Fixed interpolatedValue = new Fixed(nearestValue.RawValue + delta);
			Fixed finalValue = flip ? (Fixed.Pi - interpolatedValue) : interpolatedValue;

			return finalValue;
		}

		public static implicit operator Fixed(long value)
		{
			var r = value * ONE;
			if(((~(value ^ ONE) & (value ^ r)) & MIN_VALUE) != 0)
			{
				return value > 0 ? MaxValue : MinValue;
			}
			return new Fixed(r);
		}

		public static explicit operator long(Fixed value)
		{
			return value.raw >> FRACTIONAL_PLACES;
		}

		public static implicit operator Fixed(float value)
		{
			if(value >= int.MaxValue)
				return MaxValue;
			if(value <= int.MinValue)
				return MinValue;
			return new Fixed((long)(value * ONE));
		}

		public static explicit operator float(Fixed value)
		{
			return (float)value.raw / ONE;
		}

		public static implicit operator Fixed(double value)
		{
			if(value >= int.MaxValue)
				return MaxValue;
			if(value <= int.MinValue)
				return MinValue;
			return new Fixed((long)(value * ONE));
		}

		public static explicit operator double(Fixed value)
		{
			return (double)value.raw / ONE;
		}

		public static explicit operator Fixed(decimal value)
		{
			return new Fixed((long)(value * ONE));
		}

		public static implicit operator Fixed(int value)
		{
			return new Fixed(value * ONE);
		}

		public static explicit operator decimal(Fixed value)
		{
			return (decimal)value.raw / ONE;
		}

		public float AsFloat()
		{
			return (float)this;
		}

		public int AsInt()
		{
			return (int)this;
		}

		public long AsLong()
		{
			return (long)this;
		}

		public double AsDouble()
		{
			return (double)this;
		}

		public decimal AsDecimal()
		{
			return (decimal)this;
		}

		public static float ToFloat(Fixed value)
		{
			return (float)value;
		}

		public static int ToInt(Fixed value)
		{
			return (int)value;
		}

		public static Fixed FromFloat(float value)
		{
			return (Fixed)value;
		}

		public static bool IsInfinity(Fixed value)
		{
			return value == NegativeInfinity || value == PositiveInfinity;
		}

		public static bool IsNaN(Fixed value)
		{
			return value == NaN;
		}

		public override bool Equals(object obj)
		{
			return obj is Fixed && ((Fixed)obj).raw == raw;
		}

		public override int GetHashCode()
		{
			return raw.GetHashCode();
		}

		public bool Equals(Fixed other)
		{
			return raw == other.raw;
		}

		public int CompareTo(Fixed other)
		{
			return raw.CompareTo(other.raw);
		}

		public override string ToString()
		{
			var d = raw >= 0 ? raw & 0xFFFFFFFFL : (~raw + 1) & 0xFFFFFFFFL;
			// 不直接用AsLong()获取整数部分，因为-1000.1取整是-1001
			var l = raw >= 0 ? raw >> FRACTIONAL_PLACES : -((~raw + 1) >> FRACTIONAL_PLACES);
			var f = (float)d / ONE;

			if(d == 0)
			{
				return l.ToString();
			}
			else if((int)f == 1) //小数部分如果是0.99999999之类的，转float就变成1了
			{
				return (l + 1).ToString();
			}
			else
			{
				var s = f.ToString("F9");
				return l + s.Substring(1).TrimEnd('0', '.');
			}
		}

		public string ToString(string format)
		{
			return AsDouble().ToString(format);
		}

		public static Fixed FromRaw(long rawValue)
		{
			return new Fixed(rawValue);
		}

		public static Fixed Parse(string text)
		{
			if(!string.IsNullOrEmpty(text))
			{
				var dot = text.IndexOf(".", StringComparison.Ordinal);
				if(dot >= 0)
				{
					int part1;
					uint part2;

					string str1 = text.Substring(0, dot);
					string str2 = text.Substring(dot + 1);

					if(int.TryParse(str1, out part1) && uint.TryParse(str2, out part2))
					{
						Fixed dec = 0;
						if(str2.Length == 0)
						{
							dec = 0;
						}
						else
						{
							dec = (Fixed)part2 / Math.Pow(10, str2.Length);
						}
						return FromRaw((long)part1 << FRACTIONAL_PLACES) + dec;
					}
				}
				else
				{
					return int.Parse(text);
				}
			}
			return 0;
		}

		//[UnityEditor.MenuItem("Tools/Acos")]
		internal static void GenerateAcosLut()
		{
			using(var writer = new StreamWriter("Fix64AcosLut.cs"))
			{
				writer.Write(
					@"namespace Kernel.Engine
{
	partial struct Fixed
	{
		public static readonly long[] AcosLut = new[]
		{");
				int lineCounter = 0;
				for(int i = 0; i < LUT_SIZE; ++i)
				{
					var angle = i / ((float)(LUT_SIZE - 1));
					if(lineCounter++ % 8 == 0)
					{
						writer.WriteLine();
						writer.Write("            ");
					}
					var acos = Math.Acos(angle);
					var rawValue = ((Fixed)acos).raw;
					writer.Write($"0x{rawValue:X}L, ");
				}
				writer.Write(
					@"
		};
	}
}");
			}
		}

		//[UnityEditor.MenuItem("Tools/Sin")]
		internal static void GenerateSinLut()
		{
			using(var writer = new StreamWriter("Fix64SinLut.cs"))
			{
				writer.Write(
					@"namespace Kernel.Engine
{
	partial struct Fixed
	{
		public static readonly long[] SinLut = new[]
		{");
				int lineCounter = 0;
				for(int i = 0; i < LUT_SIZE; ++i)
				{
					var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
					if(lineCounter++ % 8 == 0)
					{
						writer.WriteLine();
						writer.Write("            ");
					}
					var sin = Math.Sin(angle);
					var rawValue = ((Fixed)sin).raw;
					writer.Write($"0x{rawValue:X}L, ");
				}
				writer.Write(
					@"
		};
	}
}");
			}
		}

		//[UnityEditor.MenuItem("Tools/Tan")]
		internal static void GenerateTanLut()
		{
			using(var writer = new StreamWriter("Fix64TanLut.cs"))
			{
				writer.Write(
					@"namespace Kernel.Engine
{
	partial struct Fixed
	{
		public static readonly long[] TanLut = new[]
		{");
				int lineCounter = 0;
				for(int i = 0; i < LUT_SIZE; ++i)
				{
					var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
					if(lineCounter++ % 8 == 0)
					{
						writer.WriteLine();
						writer.Write("            ");
					}
					var tan = Math.Tan(angle);
					if(tan > (double)MaxValue || tan < 0.0)
					{
						tan = (double)MaxValue;
					}
					var rawValue = (((decimal)tan > (decimal)MaxValue || tan < 0.0) ? MaxValue : (Fixed)tan).raw;
					writer.Write($"0x{rawValue:X}L, ");
				}
				writer.Write(
					@"
		};
	}
}");
			}
		}

		/// <summary>
		/// The underlying integer representation
		/// </summary>
		public long RawValue { get { return raw; } }

		/// <summary>
		/// This is the constructor from raw value; it can only be used interally.
		/// </summary>
		Fixed(long rawValue)
		{
			raw = rawValue;
		}

		public Fixed(int value)
		{
			raw = value * ONE;
		}

		public Fixed(int numerator, int denominator)
		{
			raw = (numerator << 32) / denominator;
		}

#if CODE_GEN || UNITY_EDITOR
		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			bool isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			if(!isEmptyElement)
			{
				string element = reader.ReadContentAsString();
				if(element != null) this = float.Parse(element);
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteString(AsFloat().ToString());
		}
#endif
	}
}
