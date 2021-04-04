﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Blazor.Components.Web.Bootstrap.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	public class HxAutosuggest<TItem, TValue> : HxInputBase<TValue>, IInputWithSize
	{
		[Parameter] public AutosuggestDataProviderDelegate<TItem> DataProvider { get; set; }

		/// <summary>
		/// Selects value from item.
		/// Not required when TValueType is same as TItemTime.
		/// </summary>
		[Parameter] public Func<TItem, TValue> ValueSelector { get; set; }

		/// <summary>
		/// Selects text to display from item.
		/// When not set <c>ToString()</c> is used.
		/// </summary>
		[Parameter] public Func<TItem, string> TextSelector { get; set; }

		/// <summary>
		/// Minimal number of characters to start suggesting. Default is <c>2</c>.
		/// </summary>
		[Parameter] public int MinimumLength { get; set; } = 2;

		/// <summary>
		/// Debounce delay in miliseconds. Default is <c>300 ms</c>.
		/// </summary>
		[Parameter] public int Delay { get; set; } = 300;

		/// <summary>
		/// Short hint displayed in the input field before the user enters a value.
		/// </summary>
		[Parameter] public string Placeholder { get; set; }

		/// <inheritdoc />
		[Parameter] public InputSize InputSize { get; set; }

		/// <summary>
		/// Returns corresponding item for (select) Value.
		/// </summary>
		[Parameter] public Func<TValue, Task<TItem>> ItemFromValueResolver { get; set; }

		private protected override string CoreInputCssClass => "form-control";
		private protected override string CoreCssClass => "hx-autosuggest position-relative";

		private HxAutosuggestInternal<TItem, TValue> hxAutosuggestInternalComponent;

		protected override void BuildRenderLabel(RenderTreeBuilder builder)
		{
			// Floating labels renders label after inputs. But HxAutosuggest...
			// HxAutossugest's input is rendered in HxAutosuggestInternal and is followed by (an icon and suggested values).
			// So we pass base.BuildRenderLabel to the component to render this label after HxAutosuggest's input.
			if (!FloatingLabelEffective)
			{
				// Render label only for non-floating form.
				base.BuildRenderLabel(builder);
			}
		}

		protected override void BuildRenderInput(RenderTreeBuilder builder)
		{
			builder.OpenComponent<HxAutosuggestInternal<TItem, TValue>>(1);
			builder.AddAttribute(1000, nameof(HxAutosuggestInternal<TItem, TValue>.Value), Value);
			builder.AddAttribute(1001, nameof(HxAutosuggestInternal<TItem, TValue>.ValueChanged), EventCallback.Factory.Create<TValue>(this, HandleValueChanged));
			builder.AddAttribute(1002, nameof(HxAutosuggestInternal<TItem, TValue>.DataProvider), DataProvider);
			builder.AddAttribute(1003, nameof(HxAutosuggestInternal<TItem, TValue>.ValueSelector), ValueSelector);
			builder.AddAttribute(1004, nameof(HxAutosuggestInternal<TItem, TValue>.TextSelector), TextSelector);
			builder.AddAttribute(1005, nameof(HxAutosuggestInternal<TItem, TValue>.MinimumLength), MinimumLength);
			builder.AddAttribute(1006, nameof(HxAutosuggestInternal<TItem, TValue>.Delay), Delay);
			builder.AddAttribute(1007, nameof(HxAutosuggestInternal<TItem, TValue>.InputId), InputId);
			builder.AddAttribute(1008, nameof(HxAutosuggestInternal<TItem, TValue>.InputCssClass), GetInputCssClassToRender()); // we may render "is-invalid" which has no sense here (there is no invalid-feedback following the element).
			builder.AddAttribute(1009, nameof(HxAutosuggestInternal<TItem, TValue>.EnabledEffective), EnabledEffective);
			builder.AddAttribute(1010, nameof(HxAutosuggestInternal<TItem, TValue>.ItemFromValueResolver), ItemFromValueResolver);
			builder.AddAttribute(1011, nameof(HxAutosuggestInternal<TItem, TValue>.Placeholder), FloatingLabelEffective ? "placeholder" : Placeholder);
			builder.AddAttribute(1012, nameof(HxAutosuggestInternal<TItem, TValue>.FloatingLabelEffective), FloatingLabelEffective);
			builder.AddAttribute(1013, nameof(HxAutosuggestInternal<TItem, TValue>.BuildRenderLabel), (RenderFragment)base.BuildRenderLabel); // base is required
			builder.AddComponentReferenceCapture(1014, component => hxAutosuggestInternalComponent = (HxAutosuggestInternal<TItem, TValue>)component);
			builder.CloseComponent();
		}

		protected override void BuildRenderValidationMessage(RenderTreeBuilder builder)
		{
			if (ShowValidationMessage)
			{
				builder.OpenElement(1, "div");
				builder.AddAttribute(2, "class", IsValueValid() ? InvalidCssClass : null);
				builder.CloseElement();

				builder.OpenRegion(3);
				base.BuildRenderValidationMessage(builder);
				builder.CloseRegion();
			}
		}

		private void HandleValueChanged(TValue newValue)
		{
			CurrentValue = newValue; // setter includes ValueChanged + NotifyFieldChanged
		}

		protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override async ValueTask FocusAsync()
		{
			if (hxAutosuggestInternalComponent == null)
			{
				throw new InvalidOperationException($"Cannot focus {this.GetType()}. The method must be called after first render.");
			}

			await hxAutosuggestInternalComponent.FocusAsync();
		}

		protected override void RenderChipGenerator(RenderTreeBuilder builder)
		{
			if (!String.IsNullOrEmpty(hxAutosuggestInternalComponent?.ChipValue))
			{
				base.RenderChipGenerator(builder);
			}
		}

		protected override void RenderChipValue(RenderTreeBuilder builder)
		{
			builder.AddContent(0, hxAutosuggestInternalComponent.ChipValue);
		}
	}
}
