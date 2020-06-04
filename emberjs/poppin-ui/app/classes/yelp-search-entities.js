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
		this.phone = params.phone;
		this.locale = params.locale;
	}
}

