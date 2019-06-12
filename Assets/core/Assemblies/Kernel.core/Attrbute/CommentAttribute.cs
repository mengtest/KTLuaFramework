using System;

namespace Kernel.core
{
	[AttributeUsage(AttributeTargets.Field)]
	public class CommentAttribute : System.Attribute
	{
		public string Comment;

		public CommentAttribute(string comment)
		{
			this.Comment = comment;
		}
	}

	[AttributeUsage(AttributeTargets.Enum)]
	public class NoCommentAttribute : System.Attribute
	{

	}
}