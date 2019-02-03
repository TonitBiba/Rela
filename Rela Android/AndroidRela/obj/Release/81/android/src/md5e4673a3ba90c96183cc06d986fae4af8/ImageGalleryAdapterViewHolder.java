package md5e4673a3ba90c96183cc06d986fae4af8;


public class ImageGalleryAdapterViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AndroidRela.Adapters.ImageGalleryAdapterViewHolder, AndroidRela", ImageGalleryAdapterViewHolder.class, __md_methods);
	}


	public ImageGalleryAdapterViewHolder ()
	{
		super ();
		if (getClass () == ImageGalleryAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("AndroidRela.Adapters.ImageGalleryAdapterViewHolder, AndroidRela", "", this, new java.lang.Object[] {  });
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
