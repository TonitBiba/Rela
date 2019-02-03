package md5e4673a3ba90c96183cc06d986fae4af8;


public class FillListViewAdapterViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AndroidRela.Adapters.FillListViewAdapterViewHolder, AndroidRela", FillListViewAdapterViewHolder.class, __md_methods);
	}


	public FillListViewAdapterViewHolder ()
	{
		super ();
		if (getClass () == FillListViewAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("AndroidRela.Adapters.FillListViewAdapterViewHolder, AndroidRela", "", this, new java.lang.Object[] {  });
	}

	public FillListViewAdapterViewHolder (android.view.View p0)
	{
		super ();
		if (getClass () == FillListViewAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("AndroidRela.Adapters.FillListViewAdapterViewHolder, AndroidRela", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

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
