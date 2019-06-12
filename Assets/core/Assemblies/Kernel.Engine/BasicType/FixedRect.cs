
namespace Kernel.Engine
{
	public struct FixedRect
	{
		private Fixed m_XMin;
		private Fixed m_YMin;
		private Fixed m_Width;
		private Fixed m_Height;

		/// <summary>
		///   <para>Shorthand for writing new FixedRect(0,0,0,0).</para>
		/// </summary>
		public static FixedRect zero
		{
			get
			{
				return new FixedRect(0.0f, 0.0f, 0.0f, 0.0f);
			}
		}

		/// <summary>
		///   <para>The X coordinate of the rectangle.</para>
		/// </summary>
		public Fixed x
		{
			get
			{
				return this.m_XMin;
			}
			set
			{
				this.m_XMin = value;
			}
		}

		/// <summary>
		///   <para>The Y coordinate of the rectangle.</para>
		/// </summary>
		public Fixed y
		{
			get
			{
				return this.m_YMin;
			}
			set
			{
				this.m_YMin = value;
			}
		}

		/// <summary>
		///   <para>The X and Y position of the rectangle.</para>
		/// </summary>
		public FixedVector2 position
		{
			get
			{
				return new FixedVector2(this.m_XMin, this.m_YMin);
			}
			set
			{
				this.m_XMin = value.x;
				this.m_YMin = value.y;
			}
		}

		/// <summary>
		///   <para>The position of the center of the rectangle.</para>
		/// </summary>
		public FixedVector2 center
		{
			get
			{
				return new FixedVector2(this.x + this.m_Width / 2f, this.y + this.m_Height / 2f);
			}
			set
			{
				this.m_XMin = value.x - this.m_Width / 2f;
				this.m_YMin = value.y - this.m_Height / 2f;
			}
		}

		/// <summary>
		///   <para>The position of the minimum corner of the rectangle.</para>
		/// </summary>
		public FixedVector2 min
		{
			get
			{
				return new FixedVector2(this.xMin, this.yMin);
			}
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		/// <summary>
		///   <para>The position of the maximum corner of the rectangle.</para>
		/// </summary>
		public FixedVector2 max
		{
			get
			{
				return new FixedVector2(this.xMax, this.yMax);
			}
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		/// <summary>
		///   <para>The width of the rectangle, measured from the X position.</para>
		/// </summary>
		public Fixed width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = value;
			}
		}

		/// <summary>
		///   <para>The height of the rectangle, measured from the Y position.</para>
		/// </summary>
		public Fixed height
		{
			get
			{
				return this.m_Height;
			}
			set
			{
				this.m_Height = value;
			}
		}

		/// <summary>
		///   <para>The width and height of the rectangle.</para>
		/// </summary>
		public FixedVector2 size
		{
			get
			{
				return new FixedVector2(this.m_Width, this.m_Height);
			}
			set
			{
				this.m_Width = value.x;
				this.m_Height = value.y;
			}
		}

		/// <summary>
		///   <para>The minimum X coordinate of the rectangle.</para>
		/// </summary>
		public Fixed xMin
		{
			get
			{
				return this.m_XMin;
			}
			set
			{
				Fixed xMax = this.xMax;
				this.m_XMin = value;
				this.m_Width = xMax - this.m_XMin;
			}
		}

		/// <summary>
		///   <para>The minimum Y coordinate of the rectangle.</para>
		/// </summary>
		public Fixed yMin
		{
			get
			{
				return this.m_YMin;
			}
			set
			{
				Fixed yMax = this.yMax;
				this.m_YMin = value;
				this.m_Height = yMax - this.m_YMin;
			}
		}

		/// <summary>
		///   <para>The maximum X coordinate of the rectangle.</para>
		/// </summary>
		public Fixed xMax
		{
			get
			{
				return this.m_Width + this.m_XMin;
			}
			set
			{
				this.m_Width = value - this.m_XMin;
			}
		}

		/// <summary>
		///   <para>The maximum Y coordinate of the rectangle.</para>
		/// </summary>
		public Fixed yMax
		{
			get
			{
				return this.m_Height + this.m_YMin;
			}
			set
			{
				this.m_Height = value - this.m_YMin;
			}
		}

		/// <summary>
		///   <para>Creates a new rectangle.</para>
		/// </summary>
		/// <param name="x">The X value the rect is measured from.</param>
		/// <param name="y">The Y value the rect is measured from.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public FixedRect(Fixed x, Fixed y, Fixed width, Fixed height)
		{
			this.m_XMin = x;
			this.m_YMin = y;
			this.m_Width = width;
			this.m_Height = height;
		}

		/// <summary>
		///   <para>Creates a rectangle given a size and position.</para>
		/// </summary>
		/// <param name="position">The position of the minimum corner of the rect.</param>
		/// <param name="size">The width and height of the rect.</param>
		public FixedRect(FixedVector2 position, FixedVector2 size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="source"></param>
		public FixedRect(FixedRect source)
		{
			this.m_XMin = source.m_XMin;
			this.m_YMin = source.m_YMin;
			this.m_Width = source.m_Width;
			this.m_Height = source.m_Height;
		}

		public static bool operator !=(FixedRect lhs, FixedRect rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(FixedRect lhs, FixedRect rhs)
		{
			return (Fixed)lhs.x == (Fixed)rhs.x && (Fixed)lhs.y == (Fixed)rhs.y && (Fixed)lhs.width == (Fixed)rhs.width && (Fixed)lhs.height == (Fixed)rhs.height;
		}

		/// <summary>
		///   <para>Creates a rectangle from min/max coordinate values.</para>
		/// </summary>
		/// <param name="xmin">The minimum X coordinate.</param>
		/// <param name="ymin">The minimum Y coordinate.</param>
		/// <param name="xmax">The maximum X coordinate.</param>
		/// <param name="ymax">The maximum Y coordinate.</param>
		/// <returns>
		///   <para>A rectangle matching the specified coordinates.</para>
		/// </returns>
		public static FixedRect MinMaxRect(Fixed xmin, Fixed ymin, Fixed xmax, Fixed ymax)
		{
			return new FixedRect(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		/// <summary>
		///   <para>Set components of an existing FixedRect.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Set(Fixed x, Fixed y, Fixed width, Fixed height)
		{
			this.m_XMin = x;
			this.m_YMin = y;
			this.m_Width = width;
			this.m_Height = height;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the FixedRect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <param name="allowInverse">Does the test allow the FixedRect's width and height to be negative?</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains(FixedVector2 point)
		{
			return (Fixed)point.x >= (Fixed)this.xMin && (Fixed)point.x < (Fixed)this.xMax && (Fixed)point.y >= (Fixed)this.yMin && (Fixed)point.y < (Fixed)this.yMax;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the FixedRect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <param name="allowInverse">Does the test allow the FixedRect's width and height to be negative?</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains(FixedVector3 point)
		{
			return (Fixed)point.x >= (Fixed)this.xMin && (Fixed)point.x < (Fixed)this.xMax && (Fixed)point.y >= (Fixed)this.yMin && (Fixed)point.y < (Fixed)this.yMax;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the FixedRect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <param name="allowInverse">Does the test allow the FixedRect's width and height to be negative?</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains(FixedVector3 point, bool allowInverse)
		{
			if (!allowInverse)
				return this.Contains(point);
			bool flag = false;
			if ((Fixed)this.width < 0.0 && (Fixed)point.x <= (Fixed)this.xMin && (Fixed)point.x > (Fixed)this.xMax || (Fixed)this.width >= 0.0 && (Fixed)point.x >= (Fixed)this.xMin && (Fixed)point.x < (Fixed)this.xMax)
				flag = true;
			return flag && ((Fixed)this.height < 0.0 && (Fixed)point.y <= (Fixed)this.yMin && (Fixed)point.y > (Fixed)this.yMax || (Fixed)this.height >= 0.0 && (Fixed)point.y >= (Fixed)this.yMin && (Fixed)point.y < (Fixed)this.yMax);
		}

		private static FixedRect OrderMinMax(FixedRect rect)
		{
			if ((Fixed)rect.xMin > (Fixed)rect.xMax)
			{
				Fixed xMin = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = xMin;
			}
			if ((Fixed)rect.yMin > (Fixed)rect.yMax)
			{
				Fixed yMin = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = yMin;
			}
			return rect;
		}

		/// <summary>
		///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		/// <param name="allowInverse">Does the test allow the widths and heights of the Rects to be negative?</param>
		public bool Overlaps(FixedRect other)
		{
			return (Fixed)other.xMax > (Fixed)this.xMin && (Fixed)other.xMin < (Fixed)this.xMax && (Fixed)other.yMax > (Fixed)this.yMin && (Fixed)other.yMin < (Fixed)this.yMax;
		}

		/// <summary>
		///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		/// <param name="allowInverse">Does the test allow the widths and heights of the Rects to be negative?</param>
		public bool Overlaps(FixedRect other, bool allowInverse)
		{
			FixedRect rect = this;
			if (allowInverse)
			{
				rect = FixedRect.OrderMinMax(rect);
				other = FixedRect.OrderMinMax(other);
			}
			return rect.Overlaps(other);
		}

		/// <summary>
		///   <para>Returns a point inside a rectangle, given normalized coordinates.</para>
		/// </summary>
		/// <param name="rectangle">Rectangle to get a point inside.</param>
		/// <param name="normalizedRectCoordinates">Normalized coordinates to get a point for.</param>
		public static FixedVector2 NormalizedToPoint(FixedRect rectangle, FixedVector2 normalizedRectCoordinates)
		{
			return new FixedVector2(FixedMathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), FixedMathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
		}

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        /*/// <summary>
		///   <para>Returns the normalized coordinates cooresponding the the point.</para>
		/// </summary>
		/// <param name="rectangle">Rectangle to get normalized coordinates inside.</param>
		/// <param name="point">A point inside the rectangle to get normalized coordinates for.</param>
		public static FixedVector2 PointToNormalized(FixedRect rectangle, FixedVector2 point)
		{
			return new FixedVector2(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));
		}*/
    }
}
