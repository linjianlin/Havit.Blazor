﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Hosting;

namespace Havit.Blazor.Components.Web.Bootstrap.Forms
{
	/// <summary>
	/// Numeric input.
	/// </summary>
	/// <typeparam name="TValue">
	/// Supported values: int (Int32), long (Int64), float (Single), double, decimal.
	/// </typeparam>
	public class HxInputNumber<TValue> : HxInputBaseWithInputGroups<TValue>
	{
		private static HashSet<Type> supportedTypes = new HashSet<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) };

		/// <summary>
		/// Gets or sets the error message used when displaying an a parsing error.
		/// Used with String.Format(...), {0} is replaced by Label property, {1} name of bounded property.
		/// </summary>
		[Parameter] public string ParsingErrorMessage { get; set; }

		/// <summary>
		/// Gets or sets the number of decimal points.
		/// Can be used only for floating point types, for integer types throws exception.
		/// When not set 2 decimal points are used.
		/// </summary>
		[Parameter] public int? Decimals
		{
			get
			{
				return _decimals;
			}
			set
			{
				if (IsTValueIntegerType)
				{
					throw new InvalidOperationException($"{nameof(Decimals)} can be set only on floating point types (not on integer types).");
				}
				_decimals = value;
			}
		}
		private int? _decimals;

		/// <summary>
		/// Gets effective value for Decimals (when not set gets 0 for integer types and 2 for floating point types.
		/// </summary>
		protected virtual int DecimalsEffective => Decimals ?? (IsTValueIntegerType ? 0 : 2);

		/// <summary>
		/// Injected host environment.
		/// </summary>
		[Inject] private IHostEnvironment HostEnvironment { get; set; }

		/// <summary>
		/// Returns true for integer types (false for floating point types).
		/// </summary>
		private bool IsTValueIntegerType
		{
			get
			{
				Type undelyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
				return (undelyingType == typeof(int))
					|| (undelyingType == typeof(long));
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public HxInputNumber()
		{
			Type undelyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
			if (!supportedTypes.Contains(undelyingType))
			{
				throw new InvalidOperationException($"Unsupported type {typeof(TValue)}.");
			}
		}

		private bool forceRenderValue = false;
		private int valueSequenceOffset = 0;

		/// <inheritdoc />
		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			if (HostEnvironment.IsDevelopment() && String.IsNullOrEmpty(ParsingErrorMessage))
			{
				throw new InvalidOperationException($"Missing {nameof(ParsingErrorMessage)} property value on {GetType()}.");
			}
		}

		/// <inheritdoc />
		protected override void BuildRenderInput(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "input");
			BuildRenderInput_AddCommonAttributes(builder, "number");

			builder.AddAttribute(1000, "onfocus", "this.select();"); // source: https://stackoverflow.com/questions/4067469/selecting-all-text-in-html-text-input-when-clicked
			builder.AddAttribute(1001, "onchange", EventCallback.Factory.CreateBinder<string>(this, value => CurrentValueAsString = value, CurrentValueAsString));

			// Počítané hodnoty sekvence jsou proti smyslu sekvencí a proti veškerým obecným doporučením.
			// Zde chceme dosáhnout toho, aby při změně uživatelského vstupu, došlo k přerenderování hodnoty, přestože se nezměnila hodnota FormatValueAsString(Value).
			// Důvodem je scénář, kdy se zobrazí hodnota například "1.00", ale uživatel ji změní na "1.0". V takové situaci se nezmění FormatValueAsString(Value),
			// takže atribut není vyrenderován a zůstává uživatelský vstup, tedy "1.0".
			// Jako řešení tedy použijeme hodnotu sequence 2000, 2001, 2002, čímž přimějeme Blazor, aby hodnotu přeci jen vyrenderovat (nezmění se hodnota, ale sequence).
			// Zároveň ale nechceme, aby každý input se pořád znovu a znovu renderoval, takže sequence změníme jen když chceme vynutit vyrenderování hodnoty.
			// (Původně jsem chtěl, aby se použili sequence 1000 a 1001, které se budou přepínat, avšak toto nefunguje - při přechodu z 1001 na 1000 nejspíš Blazor 
			// nejprve (sequence 1000) přijde na to, že má value přidat a poté (sequence 1001), že má value odebrat a tak výsledkem je mizející hodnota z inputu).
			checked
			{
				if (forceRenderValue)
				{
					valueSequenceOffset++;
					forceRenderValue = false;
				}
				builder.AddAttribute(1000 + valueSequenceOffset, "value", FormatValueAsString(Value));
			}

			builder.CloseElement();
		}

		/// <inheritdoc />
		protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
		{
			// omezení počtu desetinných míst
			// pro komplikace s tím, že máme TValue a s ním se dost těžko pracuje se omezíme na řešení a úpravu vstupních dat před konverzí do cílového typu

			if (Decimal.TryParse(value, IsTValueIntegerType ? NumberStyles.Integer : NumberStyles.Float, CultureInfo.InvariantCulture, out decimal parsedValue))
			{
				value = Math.Round(parsedValue, DecimalsEffective, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
			}

			// konverze do cílového typu

			if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.InvariantCulture, out result))
			{
				// pokud došlo jen ke změně bez změny hodnoty (třeba z 5.50 na 5.5), chceme hodnotu převést na korektní formát (5.5 na 5.50).
				// Nestačí však StateHasChange, viz komentář v BuildRenderInput.
				if (FormatValueAsString(result) != value)
				{
					forceRenderValue = true;
				}

				validationErrorMessage = null;
				return true;
			}
			else
			{
				validationErrorMessage = GetParsingErrorMessage();
				return false;
			}
		}

		/// <summary>
		/// Formats the value as a string. Derived classes can override this to determine the formatting used for <c>CurrentValueAsString</c>.
		/// </summary>
		/// <param name="value">The value to format.</param>
		/// <returns>A string representation of the value.</returns>
		protected override string FormatValueAsString(TValue value)
		{
			switch (value)
			{
				case null:
					return null;

				case int @int:
					return BindConverter.FormatValue(@int, CultureInfo.InvariantCulture);

				case long @long:
					return BindConverter.FormatValue(@long, CultureInfo.InvariantCulture);
			}

			string format = "0." + String.Join("", Enumerable.Repeat('0', DecimalsEffective));
			switch (value)
			{ 
				//case short @short:
				//	return BindConverter.FormatValue(@short, CultureInfo.CurrentCulture);

				case float @float:
					return @float.ToString(format, CultureInfo.InvariantCulture);					

				case double @double:
					return @double.ToString(format, CultureInfo.InvariantCulture);					

				case decimal @decimal:
					return @decimal.ToString(format, CultureInfo.InvariantCulture);					
			}

			throw new InvalidOperationException($"Unsupported type {value.GetType()}.");

		}

		private string GetParsingErrorMessage()
		{
			return String.Format(ParsingErrorMessage, Label, FieldIdentifier.FieldName);
		}
	}
}