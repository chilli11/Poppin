import { merge } from '@ember/polyfills';

/**
 * @class
 * @prop {String} term generic search term
 * @prop {String} location address, zip code, neighborhood, etc
 * @prop {Float} latitude
 * @prop {Float} longitude
 * @prop {Int} radius in meters (max 40000)
 * @prop {String} categories comma delimited
 * @prop {String} locale
 * @prop {Int} limit page length
 * @prop {Int} offset number to skip
 * @prop {String} sort_by
 * @prop {String} price
 * @prop {Boolean} open_now
 * @prop {Int} open_at
 * @prop {String} attributes comma delimited
 */
export class YelpBusinessSearchParams {
	term;
	location;
	latitude;
	longitude;
	radius;
	categories;
	locale;
	limit;
	offset;
	sort_by;
	price;
	open_now;
	open_at;
	attributes;

	/**
	 * 
	 * @param {Object} params
	 * @param {String} [params.term] generic search term
	 * @param {String} params.location address, zip code, neighborhood, etc
	 * @param {Float} [params.latitude] with `longitude` can replace `location`
	 * @param {Float} [params.longitude] with `latitude` can replace `location`
	 * @param {Int} [params.radius] in meters (max 40000)
	 * @param {String} [params.categories] comma delimited
	 * @param {String} [params.locale]
	 * @param {Int} [params.limit] page length
	 * @param {Int} [params.offset] number to skip
	 * @param {String} [params.sort_by]
	 * @param {String} [params.price]
	 * @param {Boolean} [params.open_now]
	 * @param {Int} [params.open_at]
	 * @param {String} [params.attributes] comma delimited
	 */
	constructor(params) {
		var self = this;
		if (!params.location && !params.longitude) {
			throw '`location` or `longitude` and `latitude` required.'
		}
		self = merge(self, params);
	}
}

/**
 * @class
 * @prop {String} phone
 * @prop {String} locale
 */
export class YelpPhoneSearchParams {
	phone;
	locale;

	/**
	 * 
	 * @param {Object} params
	 * @param {String} params.phone
	 * @param {String} [params.locale]
	 */
	constructor(params) {
		if (!params.phone) {
			throw 'Phone number required.'
		}
		this.phone = params.phone;
		this.locale = params.locale;
	}
}

/**
 * @prop {String} name
 * @prop {String} address1
 * @prop {String} address2
 * @prop {String} address3
 * @prop {String} city
 * @prop {String} state
 * @prop {String} country
 * @prop {Float} latitude
 * @prop {Float} longitude
 * @prop {String} phone
 * @prop {String} zip_code
 * @prop {String} yelp_business_id
 * @prop {String} match_threshold
 */
export class YelpBusinessMatchSearchParams {
	name;
	address1;
	address2;
	address3;
	city;
	state;
	country;
	latitude;
	longitude;
	phone;
	zip_code;
	yelp_business_id;
	match_threshold;

	/**
	 * @prop {Object} params
 	 * @prop {String} params.name
	 * @prop {String} params.address1
	 * @prop {String} [params.address2]
	 * @prop {String} [params.address3]
	 * @prop {String} params.city
	 * @prop {String} params.state
	 * @prop {String} params.country
	 * @prop {Float} [params.latitude]
	 * @prop {Float} [params.longitude]
	 * @prop {String} [params.phone]
	 * @prop {String} [params.zip_code]
	 * @prop {String} [params.yelp_business_id]
	 * @prop {String} [params.match_threshold] 'none' | 'default | 'strict'
	*/
	constructor(params) {
		var self = this;
		const required = ['name', 'address1', 'city', 'state', 'country']
		const missing = required.filter(i => !params[i]);
		if (missing.length) {
			throw 'Missing required parameter(s) ' + missing.join(', ');
		}
		self = merge(self, params);
	}
}

export default {
	YelpBusinessMatchSearchParams,
	YelpBusinessSearchParams,
	YelpPhoneSearchParams
}
