﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Havit.Blazor.Components.Web
{
	/// <summary>
	/// Edit form wrapper which provides strong type model and model instance update when valid form is submitted.
	/// </summary>
	public class HxModelEditForm<TModel> : ComponentBase
	{
		/// <summary>
		/// Form element id.
		/// </summary>
		[Parameter] public string Id { get; set; }

		/// <summary>
		/// Model.
		/// </summary>
		[Parameter] public TModel Model { get; set; }

		/// <summary>
		/// Model event callback. Invoked when valid form is updated.
		/// </summary>
		[Parameter] public EventCallback<TModel> ModelChanged { get; set; }

		/// <summary>
		/// Child content.
		/// </summary>
		[Parameter] public RenderFragment<TModel> ChildContent { get; set; }

		/// <summary>
		/// Model in edit (clone of Model).
		/// </summary>
		protected TModel ModelInEdit { get; set; }

		private TModel previousModel;

		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			if (!EqualityComparer<TModel>.Default.Equals(previousModel, Model))
			{
				OnModelSet();
				previousModel = Model;
			}
		}

		/// <summary>
		/// Fired when a new model is set from outside (databind, etc).
		/// </summary>
		protected virtual void OnModelSet()
		{
			// we are going to let user edit a clone of the model
			ModelInEdit = CloneModel(Model);
		}

		/// <summary>
		/// Updates Model by current ModelInEdit.
		/// </summary>
		public virtual async Task UpdateModelAsync()
		{
			Model = ModelInEdit;
			previousModel = Model; // to suppress cloning Model in OnParametersSet, must be before ModelChanged is invoked!
			await ModelChanged.InvokeAsync(Model);

			ModelInEdit = CloneModel(ModelInEdit);
			StateHasChanged(); // we are changing the state - ModelInEdit.
		}

		/// <inheritdoc />
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<EditForm>(0);
			builder.AddAttribute(1, nameof(EditForm.Model), ModelInEdit);
			builder.AddAttribute(2, nameof(EditForm.OnValidSubmit), EventCallback.Factory.Create<EditContext>(this, HandleValidSubmit));
			builder.AddAttribute(3, nameof(EditForm.ChildContent), (RenderFragment<EditContext>)((EditContext _) => ChildContent?.Invoke(ModelInEdit)));
			builder.AddAttribute(4, nameof(Id), Id);
			builder.CloseComponent();
		}

		private async Task HandleValidSubmit(EditContext editContext)
		{
			await UpdateModelAsync();
		}

		protected internal static TModel CloneModel(TModel modelToClone)
		{
			return ModelCloner.Clone(modelToClone);
		}
	}
}