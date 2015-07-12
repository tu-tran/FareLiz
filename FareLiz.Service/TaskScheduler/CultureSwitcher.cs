using System;

namespace SkyDean.FareLiz.Service.TaskScheduler
{
	internal class CultureSwitcher : IDisposable
	{
	    readonly System.Globalization.CultureInfo cur;
	    readonly System.Globalization.CultureInfo curUI;

	    public CultureSwitcher(System.Globalization.CultureInfo culture)
		{
			cur = System.Threading.Thread.CurrentThread.CurrentCulture;
			curUI = System.Threading.Thread.CurrentThread.CurrentUICulture;
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
		}

		public void Dispose()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = cur;
			System.Threading.Thread.CurrentThread.CurrentUICulture = curUI;
		}
	}
}
