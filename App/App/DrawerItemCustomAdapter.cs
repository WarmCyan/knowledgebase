using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	public class DrawerItemCustomAdapter : ArrayAdapter<string>
	{
		private Context m_context;
		private int m_layoutResourceId;
		private string[] m_data = null;

		public DrawerItemCustomAdapter(Context context, int layoutResourceId, string[] data) : base(context, layoutResourceId, data)
		{
			m_context = context;
			m_layoutResourceId = layoutResourceId;
			m_data = data;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View listItem = convertView;

			LayoutInflater inflater = ((Activity)m_context).LayoutInflater;
			listItem = inflater.Inflate(m_layoutResourceId, parent, false);

			TextView tvLabel = listItem.FindViewById<TextView>(Resource.Id.textViewName);

			string labelText = m_data[position];

			//tvLabel.Text = labelText;
			tvLabel.SetText(labelText, TextView.BufferType.Normal);

			return listItem;

			//return base.GetView(position, convertView, parent);
		}
	}
}