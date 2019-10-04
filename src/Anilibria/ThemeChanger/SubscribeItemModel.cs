using System;
using Windows.UI.Xaml;

namespace Anilibria.ThemeChanger
{
	public class SubscribeItemModel
	{

		public string Name
		{
			get;
			set;
		}

		public DependencyObject Element
		{
			get;
			set;
		}

		public Action<string,DependencyObject> Handler
		{
			get;
			set;
		}

	}
}
