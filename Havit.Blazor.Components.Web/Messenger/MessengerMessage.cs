﻿namespace Havit.Blazor.Components.Web
{
	/// <summary>
	/// Messenger message.
	/// </summary>
	public class MessengerMessage
	{
		/// <summary>
		/// Key. Used for component paring during rendering (@key).
		/// </summary>
		public string Key { get; } = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Message icon.
		/// </summary>
		public IconBase Icon { get; set; }

		/// <summary>
		/// CSS class.
		/// </summary>
		public string CssClass { get; set; }

		/// <summary>
		/// Delay in milliseconds to autohide message.
		/// </summary>
		public int? AutohideDelay { get; set; }

		/// <summary>
		/// Message title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Message text (body).
		/// </summary>
		public string Text { get; set; }
	}
}
