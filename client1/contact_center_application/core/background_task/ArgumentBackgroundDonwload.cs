using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contact_center_application.core.background_task
{
	class ArgumentBackgroundDonwload
	{
		string relativeWay;
		string index;
		public ArgumentBackgroundDonwload(string relativeWay, string index)
		{
			this.relativeWay = relativeWay;
			this.index = index;
		}

		public string getRelativeWay()
		{
			return this.relativeWay;
		}

		public string getIndex()
		{
			return this.index;
		}
	}
}
