using Microsoft.AspNetCore.Mvc.ModelBinding;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBXplorer.ModelBinders
{
	public class CryptoAddressesModelBinder : IModelBinder
	{
		private readonly char _separator;

		public CryptoAddressesModelBinder(char separator = ',')
		{
			_separator = separator;
		}

		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
				throw new ArgumentNullException(nameof(bindingContext));

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			if (valueProviderResult != ValueProviderResult.None)
			{
				var valueAsString = valueProviderResult.FirstValue;
				try
				{
					var networkProvider = (NBXplorer.NBXplorerNetworkProvider)bindingContext.HttpContext.RequestServices.GetService(typeof(NBXplorer.NBXplorerNetworkProvider));
					var cryptoCode = bindingContext.ValueProvider.GetValue("cryptoCode").FirstValue;
					var network = networkProvider.GetFromCryptoCode(cryptoCode ?? "BTC");

					var result = new List<BitcoinAddress>();
					var values = valueAsString.Split(new[] { _separator }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
					foreach (var value in values)
					{
						var data = BitcoinAddress.Create(value, network.NBitcoinNetwork);
						result.Add(data);
					}

					bindingContext.Result = ModelBindingResult.Success(result);
				}
				catch (System.Exception)
				{
					throw new FormatException("Invalid addresses.");
				}
			}

			return Task.CompletedTask;
		}
	}
}