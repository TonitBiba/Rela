package md5d3c1dc0c6c48bae72ca3ed2f4e3a61ec;


public class MainActivity
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer,
		android.support.design.widget.BottomNavigationView.OnNavigationItemSelectedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onNavigationItemSelected:(Landroid/view/MenuItem;)Z:GetOnNavigationItemSelected_Landroid_view_MenuItem_Handler:Android.Support.Design.Widget.BottomNavigationView/IOnNavigationItemSelectedListenerInvoker, Xamarin.Android.Support.Design\n" +
			"";
		mono.android.Runtime.register ("AndroidRela.MainActivity, AndroidRela", MainActivity.class, __md_methods);
	}


	public MainActivity ()
	{
		super ();
		if (getClass () == MainActivity.class)
			mono.android.TypeManager.Activate ("AndroidRela.MainActivity, AndroidRela", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public boolean onNavigationItemSelected (android.view.MenuItem p0)
	{
		return n_onNavigationItemSelected (p0);
	}

	private native boolean n_onNavigationItemSelected (android.view.MenuItem p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
