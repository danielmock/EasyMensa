using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace EasyMensa.ViewModels
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary> 
	public class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
			if (ViewModelBase.IsInDesignModeStatic)
			{
				// Create design time view services and models
			}

			//Register your services used here
			SimpleIoc.Default.Register<INavigationService, NavigationService>();
			SimpleIoc.Default.Register<CanteenCollectionViewModel>();

		}


		// <summary>
		// Gets the StartPage view model.
		// </summary>
		// <value>
		// The StartPage view model.
		// </value>
		public CanteenCollectionViewModel StartPageInstance
		{
			get
			{
				return ServiceLocator.Current.GetInstance<CanteenCollectionViewModel>();
			}
		}

		// <summary>
		// The cleanup.
		// </summary>
		public static void Cleanup()
		{
			// TODO Clear the ViewModels
		}
	}

}