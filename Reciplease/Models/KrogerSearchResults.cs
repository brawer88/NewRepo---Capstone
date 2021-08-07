using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
	public class AisleLocation {
		public string bayNumber { get; set; }
		public string description { get; set; }
		public string number { get; set; }
		public string numberOfFacings { get; set; }
		public string sequenceNumber { get; set; }
		public string side { get; set; }
		public string shelfNumber { get; set; }
		public string shelfPositionInBay { get; set; }
	}

	public class Fulfillment {
		public bool curbside { get; set; }
		public bool delivery { get; set; }
		public bool instore { get; set; }
		public bool shiptohome { get; set; }
	}

	public class Price {
		public double regular { get; set; }
		public double promo { get; set; }
		public double regularPerUnitEstimate { get; set; }
		public double promoPerUnitEstimate { get; set; }
	}

	public class NationalPrice {
		public double regular { get; set; }
		public double promo { get; set; }
		public double regularPerUnitEstimate { get; set; }
		public double promoPerUnitEstimate { get; set; }
	}

	public class Item {
		public string itemId { get; set; }
		public bool favorite { get; set; }
		public Fulfillment fulfillment { get; set; }
		public Price price { get; set; }
		public NationalPrice nationalPrice { get; set; }
		public string size { get; set; }
		public string soldBy { get; set; }
	}

	public class ItemInformation {
		public string depth { get; set; }
		public string height { get; set; }
		public string width { get; set; }
	}

	public class Temperature {
		public string indicator { get; set; }
		public bool heatSensitive { get; set; }
	}

	public class Size {
		public string id { get; set; }
		public string size { get; set; }
		public string url { get; set; }
	}

	public class Image {
		public string id { get; set; }
		public string perspective { get; set; }
		public bool @default { get; set; }
		public List<Size> sizes { get; set; }
	}

	public class Datum {
		public string productId { get; set; }
		public List<AisleLocation> aisleLocations { get; set; }
		public string brand { get; set; }
		public List<string> categories { get; set; }
		public string countryOrigin { get; set; }
		public string description { get; set; }
		public List<Item> items { get; set; }
		public ItemInformation itemInformation { get; set; }
		public Temperature temperature { get; set; }
		public List<Image> images { get; set; }
		public string upc { get; set; }
	}

	public class Pagination {
		public int total { get; set; }
		public int start { get; set; }
		public int limit { get; set; }
	}

	public class Meta {
		public Pagination pagination { get; set; }
		public List<string> warnings { get; set; }
	}

	public class KrogerSearchResults {
		public List<Datum> data { get; set; }
		public Meta meta { get; set; }
	}
}