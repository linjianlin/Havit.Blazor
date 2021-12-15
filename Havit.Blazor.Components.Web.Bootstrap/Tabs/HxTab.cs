﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Havit.Blazor.Components.Web.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	/// <summary>
	/// Single tab in <see cref="HxTabPanel"/>.
	/// </summary>
	public class HxTab : ComponentBase, IRenderNotificationComponent, ICascadeEnabledComponent, IDisposable
	{
		/// <summary>
		/// Cascading parameter to register the tab.
		/// </summary>
		[CascadingParameter(Name = HxTabPanel.TabsRegistrationCascadingValueName)]
		protected CollectionRegistration<HxTab> TabsRegistration { get; set; }

		/// <summary>
		/// ID of the tab (<see cref="HxTabPanel.ActiveTabId"/>).
		/// Autogenerated GUID if not set.
		/// </summary>
		[Parameter] public string Id { get; set; } = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Tab title.
		/// </summary>
		[Parameter] public string Title { get; set; }

		/// <summary>
		/// Tab title template.
		/// </summary>
		[Parameter] public RenderFragment TitleTemplate { get; set; }

		/// <summary>
		/// Content of the tab.
		/// </summary>
		[Parameter] public RenderFragment Content { get; set; }

		/// <summary>
		/// <c>True</c> for visible tab. Set <c>false</c> when tab should not be visible.
		/// </summary>
		[Parameter] public bool Visible { get; set; } = true;

		/// <inheritdoc cref="Web.FormState" />
		[CascadingParameter] protected FormState FormState { get; set; }
		FormState ICascadeEnabledComponent.FormState { get => this.FormState; set => this.FormState = value; }

		/// <summary>
		/// When <c>null</c> (default), the Enabled value is received from cascading <see cref="FormState" />.
		/// When value is <c>false</c>, input is rendered as disabled.
		/// To set multiple controls as disabled use <seealso cref="HxFormState" />.
		/// </summary>
		[Parameter] public bool? Enabled { get; set; }

		RenderedEventHandler IRenderNotificationComponent.Rendered { get; set; }

		/// <summary>
		/// Rised when the tab is activated.
		/// </summary>
		[Parameter] public EventCallback OnTabActivated { get; set; }
		/// <summary>
		/// Triggers the <see cref="OnTabActivated"/> event. Allows interception of the event in derived components.
		/// </summary>
		protected virtual Task InvokeOnTabActivatedAsync() => OnTabActivated.InvokeAsync();

		/// <summary>
		/// Rised when the tab is deactivated (another tab is activates or when <see cref="HxTabPanel"/> is disposed).
		/// </summary>
		[Parameter] public EventCallback OnTabDeactivated { get; set; }
		/// <summary>
		/// Triggers the <see cref="OnTabDeactivated"/> event. Allows interception of the event in derived components.
		/// </summary>
		protected virtual Task InvokeOnTabDeactivatedAsync() => OnTabDeactivated.InvokeAsync();

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			base.OnInitialized();

			Contract.Requires<InvalidOperationException>(TabsRegistration != null, $"{nameof(HxTab)} has to be inside {nameof(HxTabPanel)}.");
			TabsRegistration.Register(this);
		}

		/// <inheritdoc />
		protected override void OnAfterRender(bool firstRender)
		{
			base.OnAfterRender(firstRender);

			((IRenderNotificationComponent)this).Rendered?.Invoke(this, firstRender);
		}

		internal async Task NotifyActivatedAsync()
		{
			await InvokeOnTabActivatedAsync();
		}

		internal async Task NotifyDeactivatedAsync()
		{
			await InvokeOnTabDeactivatedAsync();
		}

		/// <inheritdoc />
		public virtual void Dispose()
		{
			TabsRegistration.Unregister(this);
		}
	}
}
