﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	/// <summary>
	/// Default values for <see cref="HxAutosuggest{TItem, TValue} "/>.
	/// </summary>
	public class HxAutosuggestDefaults
	{
		/// <summary>
		/// Icon displayed in input when no item is selected.
		/// </summary>
		public IconBase SearchIcon { get; set; } = BootstrapIcon.Search;

		/// <summary>
		/// Icon displayed in input on selection clear button when item is selected.
		/// </summary>
		public IconBase ClearIcon { get; set; } = BootstrapIcon.X;

		/// <summary>
		/// Minimal number of characters to start suggesting. Default is <c>2</c>.
		/// </summary>
		public int MinimumLength { get; set; } = 2;

		/// <summary>
		/// Debounce delay in miliseconds. Default is <c>300 ms</c>.
		/// </summary>
		public int Delay { get; set; } = 300;

	}
}
