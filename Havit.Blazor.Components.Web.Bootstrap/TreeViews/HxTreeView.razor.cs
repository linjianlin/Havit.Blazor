namespace Havit.Blazor.Components.Web.Bootstrap
{
	/// <summary>
	/// Component to display hierarchy data structure.
	/// </summary>
	/// <typeparam name="TItem">Type of tree data item.</typeparam>
	public partial class HxTreeView<TItem> : ComponentBase
	{
		/// <summary>
		/// Collection of hierarchy data to display.
		/// </summary>
		[Parameter] public IEnumerable<TItem> Items { get; set; }

		/// <summary>
		/// Selected data item.
		/// </summary>		
		[Parameter] public TItem SelectedItem { get; set; }

		/// <summary>
		/// Event fires when selected data item changes.
		/// </summary>		
		[Parameter] public EventCallback<TItem> SelectedItemChanged { get; set; }
		/// <summary>
		/// Triggers the <see cref="SelectedItemChanged"/> event. Allows interception of the event in derived components.
		/// </summary>
		protected virtual Task InvokeSelectedDataItemChangedAsync(TItem selectedDataItem) => SelectedItemChanged.InvokeAsync(selectedDataItem);

		/// <summary>
		/// Selector to display item title from data item.
		/// </summary>
		[Parameter] public Func<TItem, string> ItemTitleSelector { get; set; }

		/// <summary>
		/// Selector to display icon from data item.
		/// </summary>
		[Parameter] public Func<TItem, IconBase> ItemIconSelector { get; set; }

		/// <summary>
		/// Selector to display children collection for current data item. Children collection should have same type as current item.
		/// </summary>
		[Parameter] public Func<TItem, IEnumerable<TItem>> ItemChildrenSelector { get; set; }

		/// <summary>
		/// Template for the item content.
		/// </summary>
		[Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }

		/// <summary>
		/// Additional CSS class to be applied.
		/// </summary>
		[Parameter] public string CssClass { get; set; }

		private async Task HandleItemSelected(TItem newSelectedItem)
		{
			this.SelectedItem = newSelectedItem;
			await InvokeSelectedDataItemChangedAsync(this.SelectedItem);
		}
	}
}